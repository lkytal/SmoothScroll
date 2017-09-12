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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScrollShared;

namespace TestWpf
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window, IPageScroller
	{
		private ScrollController scrollController;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			scrollController = new ScrollController(this, ScrollingDirection.Vertical);

			var fs = new FileStream(@"..\..\..\scrollController\ScrollController.cs", FileMode.Open, FileAccess.Read, FileShare.None);
			var reader = new StreamReader(fs);
			textBox.Text = reader.ReadToEnd();
		}

		public void Scroll(ScrollingDirection direction, double value)
		{
			Action act = () =>
			{
				if (direction == ScrollingDirection.Vertical)
				{
					textBox.UpdateLayout();
					textBox.ScrollToVerticalOffset(textBox.VerticalOffset - value);
				}
				else if (direction == ScrollingDirection.Horizental)
				{
					textBox.ScrollToHorizontalOffset(textBox.HorizontalOffset - value);
				}
			};

			this.Dispatcher.BeginInvoke(act);
		}

		private void textBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;

			scrollController.ScrollView(e.Delta, ScrollingSpeeds.Normal);

		}
	}
}
