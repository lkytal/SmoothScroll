using System;
using System.Collections.Generic;
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
			textBox.Text = "";

			scrollController = new ScrollController(this, ScrollingDirection.Vertical);
		}

		private void textBox_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			e.Handled = true;

			scrollController.ScrollView(e.Delta, ScrollingSpeeds.Normal);
		}

		public void Scroll(ScrollingDirection direction, double value)
		{
			textBox.ScrollToVerticalOffset(value);
		}
	}
}
