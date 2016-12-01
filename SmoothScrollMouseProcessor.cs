using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
	{
		private readonly object LockerVertical = new object();
		private readonly object LockerHorizontal = new object();

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;

		private double VerticalRemain = 0, HorizontalRemain = 0;
		private int VerticalReScroll = 0, HorizontalReScroll = 0;

		private Task VerticalTask = null, HorizontalTask = null;

		private bool ExtEnable { get; set; }
		private bool ShiftEnable { get; set; }
		private bool AltEnable { get; set; }
		private bool SmoothEnable { get; set; }

		private double SpeedRadio = 1.2;
		private double TimeRadio = 1;
		private int steps = 50;

		internal SmoothScrollMouseProcessor(IWpfTextView wpfTextView)
		{
			this.DispatcherAgent = Dispatcher.CurrentDispatcher;
			this.WpfTextView = wpfTextView;

			if (SmoothScrollPackage.OptionsPage != null)
			{
				ShiftEnable  = SmoothScrollPackage.OptionsPage.ShiftEnable;
				AltEnable    = SmoothScrollPackage.OptionsPage.AltEnable;
				ExtEnable    = SmoothScrollPackage.OptionsPage.ExtEnable;
				SmoothEnable = SmoothScrollPackage.OptionsPage.SmoothEnable;
				SpeedRadio   = SmoothScrollPackage.OptionsPage.SpeedRadio;
				TimeRadio    = SmoothScrollPackage.OptionsPage.TimeRadio;
				steps = (int)(50 * TimeRadio);
			}
		}

		private double AmountToScroll(double remain, int round)
		{
			return remain * 0.1 * 50 / steps;
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (!this.ExtEnable || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return;
			}

			if (this.ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				//this.WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(-e.Delta);
				StartScroll(e.Delta, 2);				

				e.Handled = true;

				return;
			}

			if (this.AltEnable && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
			{
				this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);
				//StartScroll(e.Delta * 4, 1);

				e.Handled = true;

				return;
			}

			if (this.SmoothEnable)
			{
				e.Handled = true;

				StartScroll(e.Delta, 1);
			}
		}

		private void StartScroll(double distance, int direction)
		{
			if (direction == 1)
			{
				lock (LockerVertical)
				{
					VerticalRemain += distance * SpeedRadio;
					VerticalReScroll = 1;
				}

				if (VerticalTask == null || VerticalTask.IsCompleted)
				{
					VerticalTask = new Task(this.VerticallyScrollingThread);
					VerticalTask.Start();
				}
			}
			else
			{
				lock (LockerHorizontal)
				{
					HorizontalRemain += -distance * SpeedRadio;
					HorizontalReScroll = 1;
				}

				if (HorizontalTask == null || HorizontalTask.IsCompleted)
				{
					HorizontalTask = new Task(this.HorizontallyScrollingThread);
					HorizontalTask.Start();
				}
			}
		}

		private void Scroll(double value, int direction)
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

		private void HorizontallyScrollingThread()
		{
			for (int i = 0; i < steps; i++)
			{
				lock (LockerHorizontal)
				{
					double distance = AmountToScroll(HorizontalRemain, i);

					if (Math.Abs(HorizontalRemain) < 15 || Math.Abs(distance) < 1)
					{
						HorizontalRemain = 0;
						break;
					}

					if (HorizontalReScroll == 1)
					{
						HorizontalReScroll = 0;

						i = 0; //Restart
					}

					HorizontalRemain -= distance;
					Scroll(distance, 2);
				}

				Thread.Sleep(15);
			}
		}

		private void VerticallyScrollingThread()
		{
			for (int i = 0; i < steps; i++)
			{
				lock (LockerVertical)
				{
					double distance = AmountToScroll(VerticalRemain, i);

					if (Math.Abs(VerticalRemain) < 10 || Math.Abs(distance) < 1)
					{
						VerticalRemain = 0;
						break;
					}

					if (VerticalReScroll == 1)
					{
						VerticalReScroll = 0;

						i = 0; //Restart
					}

					VerticalRemain -= distance;
					Scroll(distance, 1);
				}

				Thread.Sleep(15);
			}
		}
	}
}
