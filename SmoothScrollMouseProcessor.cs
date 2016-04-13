using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
	{
		private readonly object Locker = new object();

		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;

		private double Remain = 0;
		private int ReScroll = 0;

		private Task ScrollTask = null;

		private bool ShiftEnable { get; set; }
		private bool AltEnable { get; set; }

		internal SmoothScrollMouseProcessor(IWpfTextView wpfTextView)
		{
			this.DispatcherAgent = Dispatcher.CurrentDispatcher;
			this.WpfTextView = wpfTextView;

			if (SmoothScrollPackage.OptionsPage != null)
			{
				ShiftEnable = SmoothScrollPackage.OptionsPage.ShiftEnable;
				AltEnable = SmoothScrollPackage.OptionsPage.AltEnable;
			}
		}

		private static double AmountToScroll(double remain)
		{
			double Radio = 0.1;

			return remain * Radio;
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return; //Only UnHandled Event
			}

			e.Handled = true;

			if (this.AltEnable && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
			{
				this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);

				return;
			}

			if (this.ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				this.WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels((double)(-e.Delta));
				return;
			}

			lock (Locker)
			{
				Remain += e.Delta * 1.2;
				ReScroll = 1;
			}

			if (ScrollTask == null || ScrollTask.IsCompleted)
			{
				ScrollTask = new Task(() => this.SmoothScroll());
				ScrollTask.Start();
			}
		}

		private void Scroll(double value)
		{
			Action Act = () =>
			{
				this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
			};

			this.DispatcherAgent.BeginInvoke(Act);
		}

		private void SmoothScroll()
		{
			for (int i = 0; i < 45; i++)
			{
				if (Math.Abs(Remain) < 1)
				{
					break;
				}

				lock (Locker)
				{
					if (ReScroll == 1)
					{
						ReScroll = 0;

						i = 0; //Restart
					}

					double Step = AmountToScroll(Remain);
					Remain -= Step;
					Scroll(Step);
				}

				Thread.Sleep(12);
			}
		}
	}
}