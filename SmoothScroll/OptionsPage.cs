using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ScrollShared;

namespace SmoothScroll
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	public class OptionsPage : DialogPage
	{
		[Category("General")]
		[Description("Enable This Extension or not.")]
		[DisplayName("Enable This Extension")]
		public bool ExtEnable { get; set; } = true;

		[Category("General")]
		[Description("Enable Smooth Scroll features or not.")]
		[DisplayName("Enable Smooth Scroll")]
		public bool SmoothEnable { get; set; } = true;

		[Category("General")]
		[Description("Hold shift key to scroll text view horizontally.")]
		[DisplayName("Enable Shift Scroll")]
		public bool ShiftEnable { get; set; } = true;

		[Category("General")]
		[Description("Hold Alt key to scroll text view by one page up/down.")]
		[DisplayName("Enable Alt Scroll")]
		public bool AltEnable { get; set; } = true;

		[Category("Parameter")]
		[Description("Indicates scrolling animation speed.")]
		[DisplayName("Animation speed / duration")]
		public ScrollingSpeeds DurationRatio { get; set; } = ScrollingSpeeds.Normal;

		[Category("Parameter")]
		[Description("Ratio of scrolling distance, indicates how long to scroll per tick.")]
		[DisplayName("Scrolling distance Ratio")]
		public double DistanceRatio { get; set; } = 1.0;

		[Category("Parameter")]
		[Description("FPS of scrolling animation, smoother with higher FPS but will also cost more CPUs.")]
		[DisplayName("Scrolling FPS")]
		public ScrollingFPS FPS { get; set; } = ScrollingFPS.Normal;
	}
}