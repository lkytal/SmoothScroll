using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Threading;
using ScrollShared;
using System;

namespace SmoothScroll
{
	internal class PageScroller : IPageScroller
	{
		private readonly IWpfTextView wpfTextView;

		public PageScroller(IWpfTextView wpfTextView)
		{
			this.wpfTextView = wpfTextView;
		}

		public void Scroll(ScrollingDirection direction, double value)
		{
			ThreadHelper.JoinableTaskFactory.Run(async delegate
			{
				await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

				if (direction == ScrollingDirection.Vertical)
				{
					wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
				}
				else if (direction == ScrollingDirection.Horizontal)
				{
					wpfTextView.ViewScroller.ScrollViewportHorizontallyByPixels(value);
				}
			});
		}
	}
}
