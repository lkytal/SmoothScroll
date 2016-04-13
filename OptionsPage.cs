using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmoothScroll
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class OptionsPage : DialogPage
	{
		private bool shiftEnable = true;

		private bool aLtEnable = true;

		[Category("Design")]
		[Description("Hold shift key to Scroll horizontally.")]
		[DisplayName("ShiftScroll")]
		public bool ShiftEnable
		{
			get
			{
				return this.shiftEnable;
			}
			set
			{
				this.shiftEnable = value;
			}
		}

		[Category("Design")]
		[Description("Hold Alt key to scroll text view by one page up/down.")]
		[DisplayName("AltScroll")]
		public bool AltEnable
		{
			get
			{
				return this.aLtEnable;
			}
			set
			{
				this.aLtEnable = value;
			}
		}

		public OptionsPage()
		{
		}
	}
}