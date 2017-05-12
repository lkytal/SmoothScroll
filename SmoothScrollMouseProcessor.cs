using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
	{
		private readonly IWpfTextView WpfTextView;

		private bool ExtEnable => SmoothScrollPackage.OptionsPage?.ExtEnable ?? true;

		private bool ShiftEnable => SmoothScrollPackage.OptionsPage?.ShiftEnable ?? true;
		private bool AltEnable => SmoothScrollPackage.OptionsPage?.AltEnable ?? true;
		private bool SmoothEnable => SmoothScrollPackage.OptionsPage?.SmoothEnable ?? true;

		private double SpeedRatio => SmoothScrollPackage.OptionsPage?.SpeedRatio ?? 1.1;
		private double TimeRatio => SmoothScrollPackage.OptionsPage?.TimeRatio ?? 1;

		private readonly ScrollController VerticalController, HorizontalController;

		internal SmoothScrollMouseProcessor(IWpfTextView _wpfTextView)
		{
			this.WpfTextView = _wpfTextView;

			VerticalController = new ScrollController(Dispatcher.CurrentDispatcher, WpfTextView, ScrollingDirection.Vertical);
			HorizontalController = new ScrollController(Dispatcher.CurrentDispatcher, WpfTextView, ScrollingDirection.Horizental);
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (!ExtEnable || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return;
			}

			if (AltEnable && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
			{
				WpfTextView.ViewScroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);

				e.Handled = true;
				return;
			}

			if (ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				StartScroll(-e.Delta, ScrollingDirection.Horizental);

				e.Handled = true;
				return;
			}

			if (SmoothEnable)
			{
				StartScroll(e.Delta, ScrollingDirection.Vertical);
				e.Handled = true;
			}
		}

		public override void PostprocessMouseDown(MouseButtonEventArgs e)
		{
			VerticalController.StopScroll();
			HorizontalController.StopScroll();
		}

		private void StartScroll(double distance, ScrollingDirection direction)
		{
			if (direction == ScrollingDirection.Vertical)
			{
				VerticalController.StartScroll(distance * SpeedRatio, TimeRatio);
			}
			else
			{
				HorizontalController.StartScroll(distance * SpeedRatio, TimeRatio);
			}
		}
	}
}
