using System.Diagnostics;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Input;
using System.Windows.Threading;
using ScrollShared;

namespace SmoothScroll
{
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
	{
		private readonly IWpfTextView wpfTextView;

		private bool ExtEnable => SmoothScrollPackage.OptionsPage?.ExtEnable ?? true;
		private bool ShiftEnable => SmoothScrollPackage.OptionsPage?.ShiftEnable ?? true;
		private bool AltEnable => SmoothScrollPackage.OptionsPage?.AltEnable ?? true;
		private bool SmoothEnable => SmoothScrollPackage.OptionsPage?.SmoothEnable ?? true;
		private double DistanceRatio => SmoothScrollPackage.OptionsPage?.DistanceRatio ?? 1.1;
		private ScrollingSpeeds SpeedLever => SmoothScrollPackage.OptionsPage?.DurationRatio ?? ScrollingSpeeds.Normal;

		private readonly ScrollController verticalController, horizontalController;

		internal SmoothScrollMouseProcessor(IWpfTextView _wpfTextView)
		{
			this.wpfTextView = _wpfTextView;
			var pageScroller = new PageScroller(Dispatcher.CurrentDispatcher, wpfTextView);
			verticalController = new ScrollController(pageScroller, ScrollingDirection.Vertical);
			horizontalController = new ScrollController(pageScroller, ScrollingDirection.Horizental);
		}

		public override void PreprocessMouseWheel(MouseWheelEventArgs e)
		{
			if (!ExtEnable || Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				return;
			}

			if (AltEnable && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)))
			{
				wpfTextView.ViewScroller.ScrollViewportVerticallyByPage(e.Delta < 0 ? ScrollDirection.Down : ScrollDirection.Up);

				e.Handled = true;
				return;
			}

			if (ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				PostScrollRequest(-e.Delta, ScrollingDirection.Horizental);

				e.Handled = true;
				return;
			}

			if (SmoothEnable)
			{
				if (!NativeMethods.IsMouseEvent())
				{
					Trace.WriteLine("Touch");
					return;
				}

				PostScrollRequest(e.Delta, ScrollingDirection.Vertical);
				e.Handled = true;
			}
		}

		public override void PostprocessMouseDown(MouseButtonEventArgs e)
		{
			verticalController.StopScroll();
			horizontalController.StopScroll();
		}

		private void PostScrollRequest(double distance, ScrollingDirection direction)
		{
			if (direction == ScrollingDirection.Vertical)
			{
				verticalController.ScrollView(distance * DistanceRatio, SpeedLever);
			}
			else
			{
				horizontalController.ScrollView(distance * DistanceRatio, SpeedLever);
			}
		}
	}
}
