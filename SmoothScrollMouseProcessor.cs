using System.Diagnostics;
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
		private ScrollingSpeeds SpeedLever => SmoothScrollPackage.OptionsPage?.DurationRatio ?? ScrollingSpeeds.Normal;

		private readonly ScrollController VerticalController, HorizontalController;

		internal SmoothScrollMouseProcessor(IWpfTextView _wpfTextView)
		{
			this.WpfTextView = _wpfTextView;
			var pageScroller = new PageScroller(Dispatcher.CurrentDispatcher, WpfTextView);
			VerticalController = new ScrollController(pageScroller, ScrollingDirection.Vertical);
			HorizontalController = new ScrollController(pageScroller, ScrollingDirection.Horizental);
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
				postScrollRequest(-e.Delta, ScrollingDirection.Horizental);

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

				postScrollRequest(e.Delta, ScrollingDirection.Vertical);
				e.Handled = true;
			}
		}

		public override void PostprocessMouseDown(MouseButtonEventArgs e)
		{
			VerticalController.StopScroll();
			HorizontalController.StopScroll();
		}

		private void postScrollRequest(double distance, ScrollingDirection direction)
		{
			if (direction == ScrollingDirection.Vertical)
			{
				VerticalController.ScrollView(distance * SpeedRatio, SpeedLever);
			}
			else
			{
				HorizontalController.ScrollView(distance * SpeedRatio, SpeedLever);
			}
		}
	}
}
