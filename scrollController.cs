using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal class ScrollController
	{
		private const int Interval = 14;

		private readonly object Locker = new object();
		private readonly Timer timer;

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;
		private readonly ScrollingDirection direction;

		private readonly double accelerator = 1.5;
		private readonly double dpiRatio;

		private double total, remain;
		private int totalSteps, round;


		public ScrollController(Dispatcher _DispatcherAgent, IWpfTextView _WpfTextView, ScrollingDirection _direction)
		{
			DispatcherAgent = _DispatcherAgent;
			WpfTextView = _WpfTextView;
			direction = _direction;

			timer = new Timer(ScrollingThread, null, Timeout.Infinite, Interval);

			dpiRatio = SystemParameters.PrimaryScreenHeight / 1080.0;
		}

		public void StartScroll(double distance, int maxTotalSteps)
		{
			lock (Locker)
			{
				if (round == totalSteps)
				{
					total = distance;
					remain = distance;
				}
				else if (Math.Sign(distance) != Math.Sign(remain))
				{
					total = distance;
					remain = distance;
				}
				else
				{
					remain += distance * accelerator;
					total = remain;
				}

				double stepsRatio = Math.Sqrt(Math.Abs(total / dpiRatio) / 400);

				totalSteps = (int)(maxTotalSteps * Math.Min(stepsRatio, 1));

				round = 0;

				timer.Change(0, Interval);
			}
		}

		private void Scroll(double value)
		{
			Action act = () =>
			{
				if (direction == ScrollingDirection.Vertical)
				{
					WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
				}
				else if (direction == ScrollingDirection.Horizental)
				{
					WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(value);
				}
			};

			DispatcherAgent.BeginInvoke(act);
		}

		private int AmountToScroll()
		{
			double rate = (double) round / totalSteps;

			double stepLength;

			//stepLength = (2 * total / totalSteps) * (1 - rate);
			//stepLength = (total / 16.17) * Math.Pow(1 - rate, 2);
			//stepLength = (total / 16.19) * (1 - Math.Sqrt(rate));
			//stepLength = (total / 38.25) * Math.Cos(rate * (2 / Math.PI));

			double meanLength = total / (totalSteps / 2.0 + 10);

			if (rate > 0.333)
			{
				stepLength = remain / 10;
			}
			else
			{
				stepLength = meanLength;
			}

			return (int)Math.Round(stepLength);
		}

		private void StopScroll()
		{
			timer.Change(Timeout.Infinite, Interval);
			round = totalSteps;
			total = remain = 0;
		}

		private void ScrollingThread(object obj)
		{
			int stepLength;

			lock (Locker)
			{
				stepLength = AmountToScroll();

				if (round == totalSteps || stepLength == 0)
				{
					StopScroll();
					return;
				}

				round += 1;
				remain -= stepLength;
			}

			Scroll(stepLength);
		}
	}

	internal enum ScrollingDirection
	{
		Vertical = 1,
		Horizental = 2
	}
}
