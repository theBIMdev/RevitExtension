global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
global using Serilog;
using System.Runtime.InteropServices;
using System.Threading;

namespace RevitExtension;
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideToolWindow(typeof(RevitAddinWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.SolutionExplorer)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.RevitExtensionString)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "Revit Extension", "General", 0, 0, true, SupportsProfiles = true)]
public sealed class RevitExtensionPackage : ToolkitPackage
{
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .WriteTo.Debug(Serilog.Events.LogEventLevel.Debug)
            .CreateLogger();

        await this.RegisterCommandsAsync();

        this.RegisterToolWindows();
    }
}