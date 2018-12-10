using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Input;

namespace SmoothScroll
{
	[Export(typeof(IMouseProcessorProvider))]
	[ContentType("text")]
	[TextViewRole(PredefinedTextViewRoles.Interactive)]
	[Name("Smooth Scroll Mouse Processor")]
	internal sealed class SmoothScrollMouseProcessorProvider : IMouseProcessorProvider
	{
		IMouseProcessor IMouseProcessorProvider.GetAssociatedProcessor(IWpfTextView wpfTextView)
		{
			return new SmoothScrollProcessor(wpfTextView);
		}
	}
}
