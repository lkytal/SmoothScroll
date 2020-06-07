using ScrollShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestWpf
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window, IPageScroller, IParameters
	{
		public ScrollingSpeeds SpeedLever { get; set; }
		public ScrollingFPS FPS { get; set; }

		private const int WM_MOUSEHWHEEL = 0x020E;
		private ScrollController vscrollController;
		private ScrollController hscrollController;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			vscrollController = new ScrollController(this, this, ScrollingDirection.Vertical);
			hscrollController = new ScrollController(this, this, ScrollingDirection.Horizontal);
			textBox.PreviewMouseWheel += textBox_PreviewMouseWheel;
			HwndSource source = PresentationSource.FromVisual(textBox) as HwndSource;
			source?.AddHook(textBox_Hook);

			var fs = new FileStream(@"..\..\..\scrollController\ScrollController.cs", FileMode.Open, FileAccess.Read, FileShare.None);
			var reader = new StreamReader(fs);
			var text = reader.ReadToEnd();
			reader.Close();
			fs.Close();

			textBox.Text = text + text + text;
			textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
		}

		public void Scroll(ScrollingDirection direction, double value)
		{
			Action act = () =>
			{
				if (direction == ScrollingDirection.Vertical)
				{
					textBox.ScrollToVerticalOffset(textBox.VerticalOffset - value);
				}
				else if (direction == ScrollingDirection.Horizontal)
				{
					textBox.ScrollToHorizontalOffset(textBox.HorizontalOffset - value);
				}
			};

			this.Dispatcher.BeginInvoke(act);
		}

		private void Scroll(int delta, ScrollController controller)
		{
			if (radioSlow.IsChecked ?? false)
			{
				SpeedLever = ScrollingSpeeds.Slow;
			}
			else if (radioFast.IsChecked ?? false)
			{
				SpeedLever = ScrollingSpeeds.Fast;
			}
			else
			{
				SpeedLever = ScrollingSpeeds.Normal;
			}

			if (fvslow.IsChecked ?? false)
			{
				FPS = ScrollingFPS.Very_Low;
			}
			else if (fslow.IsChecked ?? false)
			{
				FPS = ScrollingFPS.Low;
			}
			else if (fnormal.IsChecked ?? false)
			{
				FPS = ScrollingFPS.Normal;
			}
			else if (ffast.IsChecked ?? false)
			{
				FPS = ScrollingFPS.High;
			}
			else if (fvfast.IsChecked ?? false)
			{
				FPS = ScrollingFPS.Very_High;
			}

			controller.ScrollView(delta);
		}

		private void textBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;
			if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
			{
				Scroll(e.Delta, hscrollController);
			}
			else
			{
				Scroll(e.Delta, vscrollController);
			}
		}

		private void textBox_OnMouseHWheel(int delta)
		{
			Scroll(Math.Sign(delta) * -20, hscrollController);
		}

		private IntPtr textBox_Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case WM_MOUSEHWHEEL:
					int delta = (short)NativeMethods.HIWORD(wParam);
					textBox_OnMouseHWheel(delta);
					handled = true;
					return (IntPtr)1;
				default:
					break;
			}
			return IntPtr.Zero;
		}
	}
}
