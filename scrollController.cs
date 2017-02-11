using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal class ScrollController
	{
		private readonly object Locker = new object();
		private readonly Timer timer;

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;
		private readonly int direction;

		private double Total, Remain;
		private int steps, round;

		private const int Interval = 15;

		public ScrollController(Dispatcher _DispatcherAgent, IWpfTextView _WpfTextView, int _direction)
		{
			DispatcherAgent = _DispatcherAgent;
			WpfTextView = _WpfTextView;
			direction = _direction;

			timer = new Timer(ScrollingThread, null, Timeout.Infinite, Interval);
		}

		public void StartScroll(double distance, int _steps)
		{
			lock (Locker)
			{
				if (round == steps)
				{
					Total = distance;
					Remain = distance;
				}
				else
				{
					Remain += distance;
					Total = Remain;
				}

				round = 0;
				steps = _steps;

				timer.Change(0, Interval);
			}
		}

		private void Scroll(double value)
		{
			Action act = () =>
			{
				if (direction == 1)
				{
					this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
				}
				else if (direction == 2)
				{
					this.WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(value);
				}
			};

			this.DispatcherAgent.BeginInvoke(act);
		}

		private double AmountToScroll()
		{
			double pos = (2 * Total / steps) * (steps - round) / steps;

			return pos;
		}

		private void ScrollingThread(object obj)
		{
			if (round == steps)
			{
				timer.Change(Timeout.Infinite, Interval);
				return;
			}

			double distance;

			lock (Locker)
			{
				distance = AmountToScroll();
				double dis = Math.Abs(distance);

				if (0.3 < dis && dis < 0.9)
				{
					distance = distance > 0 ? 1 : -1;
				}
				else if (0.3 > dis)
				{
					round = steps;
					return;
				}

				round += 1;
				Remain -= distance;
			}

			Scroll(distance);
		}
	}
}
