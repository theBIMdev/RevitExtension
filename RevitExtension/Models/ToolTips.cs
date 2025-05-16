using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitExtension;
public class ToolTips
{
    public string MachinePath { get; } =
        "This where Revit looks for addin manifests that all users use.\r\n" +
        "Typically it should not need to change.";
    public string UserPath { get; } =
        "This where Revit looks for addin manifests that only the current user uses.\r\n" +
        "Typically it should not need to change.";
    public string DefaultAddinState { get; } =
        "This checkbox will immediately append/remove the '.off' extension to the selected manifest file.\r\n" +
        "That will determine which addins are 'on' outside of development.";
    public string DebugAddinState { get; } =
        "This checkbox will mark which manifest files must be renamed with/without the '.off' extension when debugging begins/ends.\r\n" +
        "That will allow you to turn off addins that are still in development when using Revit outside of Visual Studio.";
}
