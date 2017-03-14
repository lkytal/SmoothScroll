using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace SmoothScroll
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class OptionsPage : DialogPage
	{
		private bool extEnable = true;
		private bool smoothEnable = true;
		private bool shiftEnable = true;
		private bool aLtEnable = true;

		private double speedRatio = 1.2;
		private double timeRatio = 1.0;

		[Category("General")]
		[Description("Enable This Extension or not. Take effect after reopen editor (as well as other options)")]
		[DisplayName("Enable Extension")]
		public bool ExtEnable
		{
			get
			{
				return extEnable;
			}
			set
			{
				extEnable = value;
			}
		}

		[Category("General")]
		[Description("Distance Ratio of scrolling, indicates how long to scroll. Take effect after reopen editor.")]
		[DisplayName("Distance Ratio")]
		public double SpeedRatio
		{
			get
			{
				return speedRatio;
			}
			set
			{
				speedRatio = value;
			}
		}

		[Category("General")]
		[Description("Indicates scrolling animation duration. Take effect after reopen editor.")]
		[DisplayName("Animation duration Ratio")]
		public double TimeRatio
		{
			get
			{
				return timeRatio;
			}
			set
			{
				timeRatio = value;
			}
		}

		[Category("Features")]
		[Description("Enable Smooth Scroll or not. Take effect after reopen editor")]
		[DisplayName("Enable Smooth Scroll")]
		public bool SmoothEnable
		{
			get
			{
				return smoothEnable;
			}
			set
			{
				smoothEnable = value;
			}
		}

		[Category("Features")]
		[Description("Hold shift key to scroll horizontally. Take effect after reopen editor.")]
		[DisplayName("Enable Shift Scroll")]
		public bool ShiftEnable
		{
			get
			{
				return shiftEnable;
			}
			set
			{
				shiftEnable = value;
			}
		}

		[Category("Features")]
		[Description("Hold Alt key to scroll text view by one page up/down. Take effect after reopen editor.")]
		[DisplayName("Enable Alt Scroll")]
		public bool AltEnable
		{
			get
			{
				return aLtEnable;
			}
			set
			{
				aLtEnable = value;
			}
		}
	}
}