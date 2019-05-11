using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SmoothScroll
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(SmoothScrollPackage.PackageGuidString)]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	[ProvideOptionPage(typeof(OptionsPage), "SmoothScroll", "General", 0, 0, true)]

	public sealed class SmoothScrollPackage : Package
	{
		public const string PackageGuidString = "6bb22343-df63-4f19-8088-66caeafde0ad";

		public static OptionsPage OptionsPage;

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			OptionsPage = base.GetDialogPage(typeof(OptionsPage)) as OptionsPage;
		}
	}
}
