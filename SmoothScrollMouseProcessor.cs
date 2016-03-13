using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;

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

		internal SmoothScrollMouseProcessor(IWpfTextView wpfTextView)
		{
			this.DispatcherAgent = Dispatcher.CurrentDispatcher;
			this.WpfTextView = wpfTextView;			
		}

		private static float AmountToScroll(float a, float b, float amount)
		{
			return a + (b - a) * amount;
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return;
			}

			lock(Locker)
			{
				Remain += e.Delta * 1.2;
				ReScroll = 1;
			}

			if (ScrollTask == null || ScrollTask.IsCompleted)
			{
				ScrollTask = new Task(() => this.SmoothScroll());
				ScrollTask.Start();
			}		

			e.Handled = true;
		}

		private void Scroll(double value)
		{
			Action Act = () => {
				this.WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
			};

			this.DispatcherAgent.BeginInvoke(Act);
		}

		private void SmoothScroll()
		{
			double Radio = 0.1;

			for (int i = 0; i < 45; i++ )
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

					double Step = Remain * Radio;
					Remain -= Step;
					Scroll(Step);
				}
				
				Thread.Sleep(12);
			}
		}
	}
}
