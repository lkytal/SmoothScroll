using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
	{
		private readonly IWpfTextView WpfTextView;

		private bool ExtEnable { get; set; }
		private bool ShiftEnable { get; set; }
		private bool AltEnable { get; set; }
		private bool SmoothEnable { get; set; }

		private double SpeedRadio = 1.2, TimeRadio = 1;
		private int totalSteps;

		private ScrollController VerticalController, HorizontalController;

		private const int InitSteps = 40;

		internal SmoothScrollMouseProcessor(IWpfTextView _wpfTextView)
		{
			this.WpfTextView = _wpfTextView;

			if (SmoothScrollPackage.OptionsPage != null)
			{
				ReadOption();
			}

			VerticalController = new ScrollController(Dispatcher.CurrentDispatcher, WpfTextView, ScrollingDirection.Vertical);
			HorizontalController = new ScrollController(Dispatcher.CurrentDispatcher, WpfTextView, ScrollingDirection.Horizental);
		}

		private void ReadOption()
		{
			ShiftEnable = SmoothScrollPackage.OptionsPage.ShiftEnable;
			AltEnable = SmoothScrollPackage.OptionsPage.AltEnable;
			ExtEnable = SmoothScrollPackage.OptionsPage.ExtEnable;
			SmoothEnable = SmoothScrollPackage.OptionsPage.SmoothEnable;
			SpeedRadio = SmoothScrollPackage.OptionsPage.SpeedRadio;
			TimeRadio = SmoothScrollPackage.OptionsPage.TimeRadio;
			totalSteps = (int) (InitSteps * TimeRadio);
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (!this.ExtEnable || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return;
			}

			if (this.ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				StartScroll(e.Delta, ScrollingDirection.Horizental);

				e.Handled = true;
				return;
			}

			if (this.AltEnable && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
			{
				WpfTextView.ViewScroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);

				e.Handled = true;
				return;
			}

			if (this.SmoothEnable)
			{
				StartScroll(e.Delta, ScrollingDirection.Vertical);
				e.Handled = true;
			}
		}

		private void StartScroll(double distance, ScrollingDirection direction)
		{
			if (direction == ScrollingDirection.Vertical)
			{
				VerticalController.StartScroll(distance * SpeedRadio, totalSteps);
			}
			else
			{
				HorizontalController.StartScroll(distance * SpeedRadio, totalSteps);
			}
		}
	}
}
