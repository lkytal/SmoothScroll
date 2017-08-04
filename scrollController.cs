using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SmoothScroll
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	internal class ScrollController
	{
		private const int Interval = 16;
		private const int Duration = 560;
		private const double accelerator = 1.4;
		private readonly double dpiRatio;

		private readonly object Locker = new object();
		private readonly PageScroller pageScroller;
		private readonly ScrollingDirection direction;

		private double totalDistance, remain;
		private int totalRounds, round;

		private Thread workingThread;

		public ScrollController(PageScroller _pageScroller, ScrollingDirection _direction)
		{
			pageScroller = _pageScroller;
			direction = _direction;

			dpiRatio = SystemParameters.PrimaryScreenHeight / 1080.0;
		}

		private int CalulateTotalRounds(double timeRatio, double requestDistance)
		{
			int maxTotalSteps = (int)(Duration * timeRatio / Interval);

			double stepsRatio = Math.Sqrt(Math.Abs(requestDistance / dpiRatio) / 720);

			return (int)(maxTotalSteps * Math.Min(stepsRatio, 1));
		}

		public void ScrollView(double distance, double intervalRatio)
		{
			lock (Locker)
			{
				if (Math.Sign(distance) != Math.Sign(remain))
				{
					remain = (int)distance;
				}
				else
				{
					remain += (int)(distance * (round < 3 ? 1 : accelerator));
				}

				round = 0;
				totalDistance = remain;
			}

			totalRounds = CalulateTotalRounds(intervalRatio, totalDistance);

			if (!workingThread.IsAlive)
			{
				workingThread = new Thread(ScrollingThread);
			}

			workingThread.Start();
		}

		private int CalculateScrollDistance()
		{
			double percent = (double)round / totalRounds;

			var stepLength = 2 * totalDistance / totalRounds * (1 - percent);

			int result = (int)Math.Round(stepLength);

			//Debug.WriteLine($"{remain} ===> {result}");

			return result;
		}

		public void FinishScroll()
		{
			lock (Locker)
			{
				round = totalRounds;
				totalDistance = remain = 0;
			}

			workingThread?.Abort();
		}

		private void ScrollingThread(object obj)
		{
			while (round <= totalRounds)
			{
				var stepLength = CalculateScrollDistance();

				if (stepLength == 0)
				{
					break;
				}

				lock (Locker)
				{
					round += 1;
					remain -= stepLength;
				}

				pageScroller.Scroll(direction, stepLength);

				Thread.Sleep(Interval);
			}

			lock (Locker)
			{
				round = totalRounds;
				totalDistance = remain = 0;
			}
		}
	}

	internal enum ScrollingDirection
	{
		Vertical = 1,
		Horizental = 2
	}
}
