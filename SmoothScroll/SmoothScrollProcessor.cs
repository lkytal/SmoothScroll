using Microsoft.VisualStudio.Text.Editor;
using ScrollShared;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SmoothScroll
{
	internal sealed class SmoothScrollProcessor : MouseProcessorBase
	{
		private const int WM_MOUSEHWHEEL = 0x020E;

		private readonly IWpfTextView wpfTextView;

		private bool ExtEnable => SmoothScrollPackage.OptionsPage?.ExtEnable ?? true;
		private bool ShiftEnable => SmoothScrollPackage.OptionsPage?.ShiftEnable ?? true;
		private bool AltEnable => SmoothScrollPackage.OptionsPage?.AltEnable ?? true;
		private bool SmoothEnable => SmoothScrollPackage.OptionsPage?.SmoothEnable ?? true;
		private double DistanceRatio => SmoothScrollPackage.OptionsPage?.DistanceRatio ?? 1.1;
		private ScrollingSpeeds SpeedLever => SmoothScrollPackage.OptionsPage?.DurationRatio ?? ScrollingSpeeds.Normal;
		private ScrollingFPS FPS => SmoothScrollPackage.OptionsPage?.FPS ?? ScrollingFPS.High;

		private readonly ScrollController verticalController, horizontalController;

		internal SmoothScrollProcessor(IWpfTextView _wpfTextView)
		{
			this.wpfTextView = _wpfTextView;
			var pageScroller = new PageScroller(wpfTextView);
			verticalController = new ScrollController(pageScroller, ScrollingDirection.Vertical, FPS);
			horizontalController = new ScrollController(pageScroller, ScrollingDirection.Horizontal, FPS);

			wpfTextView.VisualElement.Loaded += (_, __) =>
			{
				var source = PresentationSource.FromVisual(wpfTextView.VisualElement) as HwndSource;
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

				wpfTextView.VisualElement.Focus();
				e.Handled = true;
				return;
			}

			if (ShiftEnable && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
			{
				PostScrollRequest(-e.Delta, ScrollingDirection.Horizontal);

				e.Handled = true;
				return;
			}

			PostScrollRequest(e.Delta, ScrollingDirection.Vertical);
			e.Handled = true;
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
				if (SmoothEnable && NativeMethods.IsMouseEvent())
				{
					verticalController.ScrollView(distance * DistanceRatio, SpeedLever);
				}
				else
				{
					wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(distance);
				}
			}
			else
			{
				if (SmoothEnable && NativeMethods.IsMouseEvent())
				{
					horizontalController.ScrollView(distance * DistanceRatio, SpeedLever);
				}
				else
				{
					wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(distance);
				}
			}
		}

		private IntPtr MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WM_MOUSEHWHEEL:
					int delta = (short)NativeMethods.HIWORD(wParam);

					delta = Math.Sign(delta) * -20;
					PostScrollRequest(-delta, ScrollingDirection.Horizontal);

					handled = true;
					return (IntPtr)1;
				default:
					break;
			}

			return IntPtr.Zero;
		}
	}
}
