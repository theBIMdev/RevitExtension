using System.Windows;
using System.Windows.Controls;

namespace RevitExtension;
public partial class RevitAddinControl : UserControl
{
    public ManifestLocations Locations { get; } = new();
    public ToolTips ToolTips { get; } = new();
    public RevitAddinControl()
    {
        InitializeComponent();
    }

    private void btn_RevitLocation_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
        {
            SelectedPath = Locations.MachinePath
        };
        if (dialog.ShowDialog().GetValueOrDefault())
        {
            Locations.MachinePath = dialog.SelectedPath;
        }
    }

    private void btn_UserLocation_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
        {
            SelectedPath = Locations.UserPath
        };
        if (dialog.ShowDialog().GetValueOrDefault())
        {
            Locations.UserPath = dialog.SelectedPath;
        }
    }
}