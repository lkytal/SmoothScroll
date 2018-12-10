using System;
using System.Diagnostics;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using ScrollShared;

namespace SmoothScroll
{
	internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
    {
        private const int WM_MOUSEHWHEEL = 0x020E;

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
			var pageScroller = new PageScroller(wpfTextView);
			verticalController = new ScrollController(pageScroller, ScrollingDirection.Vertical);
			horizontalController = new ScrollController(pageScroller, ScrollingDirection.Horizontal);

            wpfTextView.VisualElement.Loaded += (_, __) =>
            {
                HwndSource source = PresentationSource.FromVisual(wpfTextView.VisualElement) as HwndSource;
                source?.AddHook(MessageHook);
            };
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
				if (SmoothEnable)
				{
					PostScrollRequest(-e.Delta, ScrollingDirection.Horizontal);
				}
				else
				{
					wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(-e.Delta);
				}

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

        public void ProcessMouseHWheel(int delta)
        {
            delta = Math.Sign(delta) * -20;
            if (SmoothEnable)
            {
                PostScrollRequest(-delta, ScrollingDirection.Horizontal);
            }
            else
            {
                wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(-delta);
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

        private IntPtr MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_MOUSEHWHEEL:
                    int delta = (short)HIWORD(wParam);
                    ProcessMouseHWheel(delta);
                    handled = true;
                    return (IntPtr)1;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets high bits values of the pointer.
        /// </summary>
        private static int HIWORD(IntPtr ptr)
        {
            var val32 = ptr.ToInt32();
            return ((val32 >> 16) & 0xFFFF);
        }
    }
}
