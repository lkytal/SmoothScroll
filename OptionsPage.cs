using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace SmoothScroll
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class OptionsPage : DialogPage
	{
		[Category("General")]
		[Description("Enable This Extension or not. Take effect after reopen editor (as well as other options)")]
		[DisplayName("Enable Extension")]
		public bool ExtEnable { get; set; } = true;

		[Category("General")]
		[Description("Distance Ratio of scrolling, indicates how long to scroll. Take effect after reopen editor.")]
		[DisplayName("Distance Ratio")]
		public double SpeedRatio { get; set; } = 1.2;

		[Category("General")]
		[Description("Indicates scrolling animation duration. Take effect after reopen editor.")]
		[DisplayName("Animation duration Ratio")]
		public double TimeRatio { get; set; } = 1.0;

		[Category("Features")]
		[Description("Enable Smooth Scroll or not. Take effect after reopen editor")]
		[DisplayName("Enable Smooth Scroll")]
		public bool SmoothEnable { get; set; } = true;

		[Category("Features")]
		[Description("Hold shift key to scroll horizontally. Take effect after reopen editor.")]
		[DisplayName("Enable Shift Scroll")]
		public bool ShiftEnable { get; set; } = true;

		[Category("Features")]
		[Description("Hold Alt key to scroll text view by one page up/down. Take effect after reopen editor.")]
		[DisplayName("Enable Alt Scroll")]
		public bool AltEnable { get; set; } = true;
	}
}