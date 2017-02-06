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
		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;

		private bool ExtEnable { get; set; }
		private bool ShiftEnable { get; set; }
		private bool AltEnable { get; set; }
		private bool SmoothEnable { get; set; }

		private double SpeedRadio = 1.2;
		private double TimeRadio = 1;
		private int steps = 50;

		private ScrollController VerticalController, HorizontalController;

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

			VerticalController = new ScrollController(DispatcherAgent, wpfTextView, 1);
			HorizontalController = new ScrollController(DispatcherAgent, wpfTextView, 2);
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
				VerticalController.StartScroll(distance * SpeedRadio, steps);
			}
			else
			{
				HorizontalController.StartScroll(distance * SpeedRadio, steps);
			}
		}
	}
}
