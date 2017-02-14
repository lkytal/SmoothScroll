using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal class ScrollController
	{
		private readonly object Locker = new object();
		private readonly Timer timer;

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;
		private readonly ScrollingDirection direction;

		private double total, remain;
		private int totalSteps, round;

		private const int Interval = 15;

		public ScrollController(Dispatcher _DispatcherAgent, IWpfTextView _WpfTextView, ScrollingDirection _direction)
		{
			DispatcherAgent = _DispatcherAgent;
			WpfTextView = _WpfTextView;
			direction = _direction;

			timer = new Timer(ScrollingThread, null, Timeout.Infinite, Interval);
		}

		public void StartScroll(double distance, int _totalSteps)
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
					remain += distance;
					total = remain;
				}

				round = 0;
				totalSteps = _totalSteps;

				timer.Change(0, Interval);
			}
		}

		private void Scroll(double value)
		{
			Action act = () =>
			{
				if (direction == ScrollingDirection.Vertical)
				{
					this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
				}
				else if (direction == ScrollingDirection.Horizental)
				{
					this.WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(value);
				}
			};

			this.DispatcherAgent.BeginInvoke(act);
		}

		private double AmountToScroll()
		{
			//double stepLength = (Total / 12) * Math.Pow((1 - (double)round / steps), 3);

			double stepLength = remain / 10;

			//if (Math.Abs(stepLength) < 1)
			//{
			//	stepLength = stepLength > 0 ? 1 : -1;
			//}

			return stepLength;
		}

		private void StopScroll()
		{
			timer.Change(Timeout.Infinite, Interval);
			round = totalSteps;
			total = remain = 0;
		}

		private void ScrollingThread(object obj)
		{
			double stepLength;

			lock (Locker)
			{
				stepLength = AmountToScroll();

				if (round == totalSteps || Math.Abs(stepLength) < 0.1)
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
