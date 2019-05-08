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
		private const int Interval = 8;
		private const int AccelerateThreshold = 2;
		private const double Accelerator = 2.0;
		private readonly double dpiRatio;

		private readonly object locker = new object();
		private readonly IPageScroller pageScroller;
		private readonly ScrollingDirection direction;

		private double totalDistance, remain;
		private int totalRounds, round;

		private Task workingThread;

		public ScrollController(IPageScroller _pageScroller, ScrollingDirection _direction)
		{
			pageScroller = _pageScroller;
			direction = _direction;

			dpiRatio = SystemParameters.PrimaryScreenHeight / 720.0;
		}

		private int CalculateTotalRounds(double timeRatio, double requestDistance)
		{
			double distanceRatio = Math.Sqrt(Math.Abs(requestDistance / dpiRatio) / 720);
			double maxTotalSteps = 10 + 25 * Math.Min(distanceRatio, 1.5);

			return (int)(maxTotalSteps * timeRatio);
		}

		public void ScrollView(double distance, ScrollingSpeeds speedLever)
		{
			double intervalRatio = GetSpeedRatio(speedLever);

			lock (locker)
			{
				if (Math.Sign(distance) != Math.Sign(remain))
				{
					remain = (int)distance;
				}
				else
				{
					remain += (int)(distance * (round < AccelerateThreshold ? 1 : Accelerator));
				}

				round = 0;
				totalDistance = remain;
			}

			totalRounds = CalculateTotalRounds(intervalRatio, totalDistance);

			if (workingThread == null)
			{
				workingThread = Task.Run(() => ScrollingThread());
			}
		}

		private double GetSpeedRatio(ScrollingSpeeds speedLever)
		{
			var speedTable = new Dictionary<ScrollingSpeeds, double>()
			{
				{ScrollingSpeeds.Slow, 1.5},
				{ScrollingSpeeds.Normal, 1.0},
				{ScrollingSpeeds.Fast, 0.65}
			};

			return speedTable[speedLever];
		}

		private int CalculateScrollDistance()
		{
			double percent = (double)round / totalRounds;

			var stepLength = 2 * totalDistance / totalRounds * (1 - percent);

			int result = (int)Math.Round(stepLength);

			return result;
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
			while (round <= totalRounds)
			{
				var stepLength = CalculateScrollDistance();

				if (stepLength == 0)
				{
					break;
				}

				lock (locker)
				{
					round += 1;
					remain -= stepLength;
				}

				pageScroller.Scroll(direction, stepLength);

				await Task.Delay(Interval);
			}

			CleanupScroll();
		}
	}
}
