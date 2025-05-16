global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
global using Serilog;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using Microsoft;
using System.Diagnostics;

namespace RevitExtension;
[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideToolWindow(typeof(RevitAddinWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.SolutionExplorer)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.RevitExtensionString)]
[ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "Revit Extension", "General", 0, 0, true, SupportsProfiles = true)]
public sealed class RevitExtensionPackage : ToolkitPackage
{
    private static DTE _dte;
    private static EnvDTE.Events _events;
    private static EnvDTE.DebuggerEvents _debugEvents;

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithMachineName()
            .WriteTo.Debug(Serilog.Events.LogEventLevel.Debug)
            .WriteTo.Seq("http://localhost:5341/")
            .CreateLogger();

        await this.RegisterCommandsAsync();

        this.RegisterToolWindows();






        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        _dte = (EnvDTE.DTE)await this.GetServiceAsync(typeof(EnvDTE.DTE));
        Assumes.Present(_dte);
        _events = _dte.Events;
        _debugEvents = _events.DebuggerEvents;


        //EnvDTE.DebuggerEvents eventsDebuggerEvents = _events.DebuggerEvents;

        //debugEvents.OnEnterBreakMode +=
        //    new _dispDebuggerEvents_OnEnterBreakModeEventHandler(DebugEvents_OnEnterBreakMode);

        //debugEvents.OnContextChanged +=
        //    new _dispDebuggerEvents_OnContextChangedEventHandler(ContextHandler);

        //debugEvents.OnEnterDesignMode += DebugEvents_OnEnterDesignMode;
        _debugEvents.OnEnterRunMode += DebugEvents_OnEnterRunMode;
    }

    private void DebugEvents_OnEnterRunMode(dbgEventReason Reason)
    {
        Debug.WriteLine("DebugEvents_OnEnterRunMode");
        Log.Information("Debugger event triggered");
    }

    //private void DebugEvents_OnEnterDesignMode(dbgEventReason Reason)
    //{
    //    Debug.WriteLine("DebugEvents_OnEnterDesignMode");
    //}

    //private void DebugEvents_OnEnterBreakMode(dbgEventReason Reason, ref dbgExecutionAction ExecutionAction)
    //{
    //    Debug.WriteLine("DebugEvents_OnEnterBreakMode");
    //}

    //private void ContextHandler(EnvDTE.Process NewProcess, Program NewProgram, EnvDTE.Thread NewThread, EnvDTE.StackFrame NewStackFrame)
    //{
    //    Debug.WriteLine("ContextHandler");
    //}
}