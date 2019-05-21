using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace SmoothScroll
{
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideService(typeof(SmoothScrollPackage), IsAsyncQueryable = true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
	[Guid(SmoothScrollPackage.PackageGuidString)]
	[ProvideOptionPage(typeof(OptionsPage), "SmoothScroll", "General", 0, 0, true)]

	public sealed class SmoothScrollPackage : AsyncPackage
	{
		public const string PackageGuidString = "6bb22343-df63-4f19-8088-66caeafde0ad";

		public static OptionsPage OptionsPage { get; set; }

		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			await base.InitializeAsync(cancellationToken, progress);

			await JoinableTaskFactory.SwitchToMainThreadAsync();
			OptionsPage = base.GetDialogPage(typeof(OptionsPage)) as OptionsPage;
		}
	}
}
