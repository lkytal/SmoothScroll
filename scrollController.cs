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
		private const int InitTime = 560;
		private const double accelerator = 1.4;
		private readonly double dpiRatio;

		private readonly object Locker = new object();
		private readonly Timer timer;
		private readonly PageScroller pageScroller;
		private readonly ScrollingDirection direction;

		private double total, remain;
		private int totalSteps, round;

		public ScrollController(PageScroller _pageScroller, ScrollingDirection _direction)
		{
			pageScroller = _pageScroller;
			direction = _direction;

			timer = new Timer(ScrollingThread, null, Timeout.Infinite, Interval);

			dpiRatio = SystemParameters.PrimaryScreenHeight / 1080.0;
		}

		private int CalulateTotalSteps(double timeRatio, double totalDistance)
		{
			int maxTotalSteps = (int)(InitTime * timeRatio / Interval);

			double stepsRatio = Math.Sqrt(Math.Abs(totalDistance / dpiRatio) / 720);

			return (int)(maxTotalSteps * Math.Min(stepsRatio, 1));
		}

		public void StartScroll(double distance, double timeRatio)
		{
			lock (Locker)
			{
				if (Math.Sign(distance) != Math.Sign(remain))
				{
					remain = (int) distance;
				}
				else
				{
					remain += (int) (distance * (round == totalSteps ? 1 : accelerator));
				}

				round = 0;
				total = remain;
			}

			totalSteps = CalulateTotalSteps(timeRatio, total);

			timer?.Change(0, Interval);
		}

		private int AmountToScroll()
		{
			double percent = (double) round / totalSteps;

			double stepLength;

			stepLength = (2 * total / totalSteps) * (1 - percent);
			//stepLength = (total / 16.17) * Math.Pow(1 - percent, 2);
			//stepLength = (total / 38.25) * Math.Cos(percent * (2 / Math.PI));

			//if (percent > 0.5)
			//{
			//	stepLength = remain / 10;
			//}
			//else
			//{
			//	double meanSpeed = total / (2 * totalSteps * 0.5 + 10);
			//	stepLength = meanSpeed * (3 - 4.0 * percent);
			//}

			int result = (int)Math.Round(stepLength);

			//Debug.WriteLine($"{remain} ===> {result}");

			return result;
		}

		public void StopScroll()
		{
			timer?.Change(Timeout.Infinite, Interval);

			lock (Locker)
			{
				round = totalSteps;
				total = remain = 0;
			}
		}

		private void ScrollingThread(object obj)
		{
			lock (Locker)
			{
				var stepLength = AmountToScroll();

				if (round == totalSteps || stepLength == 0)
				{
					StopScroll();
					return;
				}

				round += 1;
				remain -= stepLength;

				pageScroller.Scroll(direction, stepLength);
			}
		}
	}

	internal enum ScrollingDirection
	{
		Vertical = 1,
		Horizental = 2
	}
}
