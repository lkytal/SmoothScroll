using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SmoothScroll
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class OptionsPage : DialogPage
	{
		private bool extEnable = true;

		private bool smoothEnable = true;

		private bool shiftEnable = true;

		private bool aLtEnable = true;

		private double speedRadio = 1.2;

		[Category("General")]
		[Description("Enable This Extension or not. Take effect after reopen editor (as well as other options)")]
		[DisplayName("Enable Extension")]
		public bool ExtEnable
		{
			get
			{
				return this.extEnable;
			}
			set
			{
				this.extEnable = value;
			}
		}

		[Category("General")]
		[Description("Radio of scrolling speed, set to 1 means original speed. Take effect after reopen editor.")]
		[DisplayName("Speed Radio")]
		public double SpeedRadio
		{
			get
			{
				return this.speedRadio;
			}
			set
			{
				this.speedRadio = value;
			}
		}

		[Category("Features")]
		[Description("Enable SmoothScroll or not. Take effect after reopen editor")]
		[DisplayName("Enable SmoothScroll")]
		public bool SmoothEnable
		{
			get
			{
				return this.smoothEnable;
			}
			set
			{
				this.smoothEnable = value;
			}
		}

		[Category("Features")]
		[Description("Hold shift key to Scroll horizontally. Take effect after reopen editor.")]
		[DisplayName("Enable ShiftScroll")]
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

		[Category("Features")]
		[Description("Hold Alt key to scroll text view by one page up/down. Take effect after reopen editor.")]
		[DisplayName("Enable AltScroll")]
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
			if (SmoothScrollPackage.OptionsPage == null)
			{
				SmoothScrollPackage.OptionsPage = this;
			}
		}
	}
}