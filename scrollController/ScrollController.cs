using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ScrollShared
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class ScrollController
	{
		private const int AccelerateThreshold = 2;
		private const float Accelerator = 2.0f;
		private readonly double dpiRatio;

		private readonly object locker = new object();
		private readonly IPageScroller pageScroller;
		private readonly IParameters parameters;
		private readonly ScrollingDirection direction;

		private int Interval = 12;

		private int totalDistance, remain;
		private int totalRounds, round;

		private Task workingThread = null;

		public ScrollController(IPageScroller _pageScroller, IParameters _parameters, ScrollingDirection _direction)
		{
			pageScroller = _pageScroller;
			parameters = _parameters;
			direction = _direction;

			dpiRatio = SystemParameters.PrimaryScreenHeight / 720.0;
		}

		private int CalculateTotalRounds(double speedRatio, double requestDistance)
		{
			var distanceRatio = Math.Sqrt(Math.Abs(requestDistance / dpiRatio) / 720);
			var maxTotalSteps = 10 + 25 * Math.Min(distanceRatio, 1.5);

			return (int)(maxTotalSteps * speedRatio);
		}

		public void ScrollView(double distance)
		{
			double intervalRatio = GetSpeedRatio(parameters.SpeedLever);
			SetFPS(parameters.FPS);

			int new_remain = 0;

			if (Math.Sign(distance) != Math.Sign(remain))
			{
				new_remain = (int)distance;
			}
			else
			{
				new_remain = remain + (int)(distance * (round < AccelerateThreshold ? 1 : Accelerator));
			}

			lock (locker)
			{
				round = 0;
				remain = totalDistance = new_remain;
			}

			// no competing writes, so needs no lock
			totalRounds = CalculateTotalRounds(intervalRatio, totalDistance);

			if (workingThread == null)
			{
				workingThread = Task.Run(ScrollingThread);
			}
		}

		private double GetSpeedRatio(ScrollingSpeeds speedLever)
		{
			var speedTable = new Dictionary<ScrollingSpeeds, double>()
			{
				{ScrollingSpeeds.Slow, 1.5},
				{ScrollingSpeeds.Normal, 1.1},
				{ScrollingSpeeds.Fast, 0.7},
				{ScrollingSpeeds.Very_Fast, 0.5}
			};

			return speedTable[speedLever];
		}

		private void SetFPS(ScrollingFPS _fps)
		{
			switch (_fps)
			{
				case ScrollingFPS.Very_Low:
					Interval = 25;
					break;

				case ScrollingFPS.Low:
					Interval = 16;
					break;

				case ScrollingFPS.Normal:
					Interval = 12;
					break;

				case ScrollingFPS.Very_High:
					Interval = 5;
					break;

				default: // fall though
				case ScrollingFPS.High:
					Interval = 8;
					break;
			}
		}

		private int CalculateScrollDistance()
		{
			float percent = (float)round / totalRounds;

			var stepLength = 2 * totalDistance / totalRounds * (1 - percent);

			return (int)Math.Round(stepLength);
		}

		public void StopScroll()
		{
			workingThread?.Wait(100);
			CleanupScroll();
		}

		private void CleanupScroll()
		{
			lock (locker)
			{
				round = totalRounds;
				totalDistance = remain = 0;

				workingThread = null;
			}
		}

		private async Task ScrollingThread()
		{
			//int start = Environment.TickCount & Int32.MaxValue;

			while (round <= totalRounds)
			{
				//Console.WriteLine("Round {0}: {1}", round, Environment.TickCount & Int32.MaxValue - start);
				//start = Environment.TickCount & Int32.MaxValue;

				var stepLength = CalculateScrollDistance();

				if (stepLength == 0)
				{
					break;
				}

				Interlocked.Exchange(ref remain, remain - stepLength);
				Interlocked.Increment(ref round);

				pageScroller.Scroll(direction, stepLength);

				//await Task.Delay(Interval);
				Thread.Sleep(Interval);
			}

			CleanupScroll();
		}
	}
}
