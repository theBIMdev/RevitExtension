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

    [Category("Debugging")]
    [DisplayName("Enable during debug")]
    [Description("At debug time, rename each Revit Addin manifest file to add/remove the '.off' extension when the debugger starts/stops.")]
    [DefaultValue(true)]
    public bool RenameOnDebug { get; set; } = true;

    [Category("Debugging")]
    [DisplayName("Always enable developing addin")]
    [Description("At debug time, always turn on the manifest files for the addin currently being developed.")]
    [DefaultValue(true)]
    public bool AlwaysRunDevelopingAddin { get; set; } = true;

    [Category("Debugging")]
    [DisplayName("Only enable relevant year")]
    [Description("At debug time, only turn on the manifest of selected addins that also match the Revit version (year) being launched.")]
    [DefaultValue(true)]
    public bool OnlyEnableRelevantYear { get; set; } = true;

    [Category("Manifest Paths")]
    [DisplayName("Revit's Addin Location")]
    [Description("This where Revit looks for addin manifests that all users use. Typically it should never need to change.")]
    public string MachinePath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _adskPath;

    [Category("Manifest Paths")]
    [DisplayName("User's Addin Location")]
    [Description("This where Revit looks for addin manifests that only the current user uses. Typically it should never need to change.")]
    public string UserPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _adskPath;

}