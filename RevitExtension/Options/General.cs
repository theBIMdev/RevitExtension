using System.ComponentModel;
using System.Runtime.InteropServices;

namespace RevitExtension;
internal partial class OptionsProvider
{
    // Register the options with this attribute on your package class:
    // [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), "RevitExtension", "General", 0, 0, true, SupportsProfiles = true)]
    [ComVisible(true)]
    public class GeneralOptions : BaseOptionPage<General> { }
}

public class General : BaseOptionModel<General>
{
    const string _adskPath = "\\Autodesk\\Revit\\Addins";
    private static readonly string _defaultMachinePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _adskPath;
    private static readonly string _defaultUserPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _adskPath;

    [Category("Debugging")]
    [DisplayName("Enable 'Debug Time' changes")]
    [Description("When the debugger starts/stops, rename each Revit Addin manifest file to add/remove the '.off' extension based on which have the 'Debug' box checked/unchecked.")]
    [DefaultValue(true)]
    public bool RenameOnDebug { get; set; } = true;

    [Category("Debugging")]
    [DisplayName("Enable current addin")]
    [Description("Always treat the addin currently being developed as if its 'Debug' box is checked.")]
    [DefaultValue(true)]
    public bool AlwaysRunDevelopingAddin { get; set; } = true;

    [Category("Debugging")]
    [DisplayName("Selected debug-time addins")]
    [Description("This is a list of addins that have been selected by the user to be turned on when the debugger starts.")]
    public string SelectedDebugAddins { get; set; }

    [Category("Debugging")]
    [DisplayName("Only enable relevant year")]
    [Description("When the debugger starts/stops, only rename the manifest of selected addins that also match the Revit version (year) being launched.")]
    [DefaultValue(true)]
    public bool OnlyEnableRelevantYear { get; set; } = true;

    [Category("Manifest Paths")]
    [DisplayName("Revit's Addin Location")]
    [Description("This where Revit looks for addin manifests that all users use. Typically it should never need to change.")]
    public string MachinePath { get; set; } = _defaultMachinePath;

    [Category("Manifest Paths")]
    [DisplayName("User's Addin Location")]
    [Description("This where Revit looks for addin manifests that only the current user uses. Typically it should never need to change.")]
    public string UserPath { get; set; } = _defaultUserPath;

}