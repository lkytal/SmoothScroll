using System;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text.Editor;
using ScrollShared;

namespace SmoothScroll
{
	internal class PageScroller : IPageScroller
	{
		private readonly Dispatcher DispatcherAgent;
		private readonly IWpfTextView WpfTextView;

		public PageScroller(Dispatcher dispatcherAgent, IWpfTextView wpfTextView)
		{
			DispatcherAgent = dispatcherAgent;
			WpfTextView = wpfTextView;
		}

		public void Scroll(ScrollingDirection direction, double value)
		{
			Action act = () =>
			{
				if (direction == ScrollingDirection.Vertical)
				{
					WpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
				}
				else if (direction == ScrollingDirection.Horizental)
				{
					WpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(value);
				}
			};

			DispatcherAgent.BeginInvoke(act);
		}
	}
}
