using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;

namespace RevitExtension;
public static class CustomDialogs
{
    public static string AskUserForFolder(string startFromFolder)
    {
        var dialog = new VistaFolderBrowserDialog
        {
            SelectedPath = startFromFolder
        };
        if (dialog.ShowDialog().GetValueOrDefault())
        {
            return dialog.SelectedPath;
        }
        return null;
    }
}
