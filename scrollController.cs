using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal class ScrollController
	{
		private readonly object Locker = new object();

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;

		private double Total = 0, Remain = 0;
		private int direction, steps = 0, round = 0;

		private System.Threading.Timer timer;

		public ScrollController(Dispatcher _DispatcherAgent, IWpfTextView _WpfTextView, int _direction)
		{
			DispatcherAgent = _DispatcherAgent;
			WpfTextView = _WpfTextView;
			direction = _direction;

			timer = new Timer(ScrollingThread, null, Timeout.Infinite, 15);
		}

		~ScrollController()
		{
			timer.Dispose();
		}

		public void StartScroll(double distance, int _steps)
		{
			lock (Locker)
			{
				Remain += distance;
				round = 0;
				steps = _steps;

				timer.Change(0, 15);
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

		private double pos = 0;

		private double AmountToScrollNew(double remain)
		{
			double total = remain + pos;

			double degrees = (double)(round * 90) / steps;

			double last = pos;
			pos = total * Math.Sin(degrees * (Math.PI / 180));

			return pos - last;
		}

		private double AmountToScroll(double remain)
		{
			return remain * 0.1 * steps / 50;
		}

		private void ScrollingThread(object obj)
		{
			if (round == steps)
			{
				timer.Change(Timeout.Infinite, 15);
				return;
			}

			double distance = 0;

			lock (Locker)
			{
				round += 1;

				if (Math.Abs(Remain) < 5)
				{
					round = steps;

					return;
				}

				distance = AmountToScroll(Remain);

				Remain -= distance;
			}

			Scroll(distance);
		}
	}
}
