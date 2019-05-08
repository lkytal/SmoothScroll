using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics.CodeAnalysis;
using Task = System.Threading.Tasks.Task;

namespace SmoothScroll
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideService(typeof(SmoothScrollPackage), IsAsyncQueryable = true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
	[Guid(SmoothScrollPackage.PackageGuidString)]
	[ProvideOptionPage(typeof(OptionsPage), "SmoothScroll", "General", 0, 0, true)]

	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class SmoothScrollPackage : AsyncPackage
	{
		public const string PackageGuidString = "6bb22343-df63-4f19-8088-66caeafde0ad";

		public static OptionsPage OptionsPage;

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>

		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await Task.Run(() => OptionsPage = base.GetDialogPage(typeof(OptionsPage)) as OptionsPage);
		}
	}
}
