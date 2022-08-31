using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RevitExtension;
public partial class RevitAddinControl : UserControl
{
    public enum DebugState { None, Starting, Ending }
    public ObservableCollection<AddinEntry> EntriesByAddin { get; set; } = new();
    public ObservableCollection<AddinEntry> EntriesByYear { get; set; } = new();
    public ManifestLocations Locations { get; } = new();
    public ToolTips ToolTips { get; } = new();
    
    private const string _changeGroupingToYear = "Group by Revit Year";
    private const string _changeGroupingToName = "Group by Addin Name";

    public RevitAddinControl()
    {
        InitializeComponent();
        Locations.PropertyChanged += new PropertyChangedEventHandler(location_PropertyChanged);
        refreshList(Locations.MachinePath, Locations.UserPath);
    }

    private void refreshList(string machinePath, string userPath)
    {
        getAddinEntries(machinePath, userPath);

        trvMenu.Items.Clear();
        if (groupingToggle.Content is null || groupingToggle.Content.ToString() == _changeGroupingToYear)
        { foreach (var entry in EntriesByAddin) { trvMenu.Items.Add(entry); } }
        else
        { foreach (var entry in EntriesByYear) { trvMenu.Items.Add(entry); } }
    }

    private void getAddinEntries(string machinePath, string userPath)
    {
        SortedDictionary<string, ObservableCollection<AddinEntry>> byYear = new();
        SortedDictionary<string, ObservableCollection<AddinEntry>> byAddin = new();

        var directories = Directory.GetDirectories(machinePath).ToList();
        directories.AddRange(Directory.GetDirectories(userPath));

        foreach (string path in directories)
        {
            string year = path.Substring(path.Length - 4);
            if (!byYear.ContainsKey(year)) { byYear.Add(year, new()); }

            var files = Directory.GetFiles(path).Where(p => p.EndsWith(".addin") || p.EndsWith(".off"));
            foreach (string filePath in files)
            {
                int offset = filePath.EndsWith(".off") ? 10 : 6;
                string addinName = Path.GetFileName(filePath);
                addinName = addinName.Substring(0, addinName.Length - offset);
                if (!byAddin.ContainsKey(addinName)) { byAddin.Add(addinName, new()); }

                bool onByDefault = filePath.EndsWith(".addin");
                bool onByDebug = false; // TODO: Use a config file.

                var yearEntry = new AddinEntry()
                {
                    DisplayName = addinName,
                    AddinName = addinName,
                    AddinYear = year,
                    OnByDefault = onByDefault,
                    OnByDebug = onByDebug,
                    FilePath = filePath
                };
                yearEntry.PropertyChanged += new PropertyChangedEventHandler(childEntry_PropertyChanged);
                byYear[year].Add(yearEntry);

                var nameEntry = new AddinEntry()
                {
                    DisplayName = year,
                    AddinName = addinName,
                    AddinYear = year,
                    OnByDefault = onByDefault,
                    OnByDebug = onByDebug,
                    FilePath = filePath
                };
                nameEntry.PropertyChanged += new PropertyChangedEventHandler(childEntry_PropertyChanged);
                byAddin[addinName].Add(nameEntry);
            }
        }

        EntriesByYear.Clear();
        foreach (var kvp in byYear)
        {
            var parent = new AddinEntry()
            {
                DisplayName = kvp.Key,
                Children = kvp.Value
            };
            parent.VerifyCheckState(nameof(parent.OnByDefault));
            parent.VerifyCheckState(nameof(parent.OnByDebug));
            parent.PropertyChanged += new PropertyChangedEventHandler(parentEntry_PropertyChanged);
            EntriesByYear.Add(parent);
        }

        EntriesByAddin.Clear();
        foreach (var kvp in byAddin)
        {
            var parent = new AddinEntry()
            {
                DisplayName = kvp.Key,
                Children = kvp.Value
            };
            parent.VerifyCheckState(nameof(parent.OnByDefault));
            parent.VerifyCheckState(nameof(parent.OnByDebug));
            parent.PropertyChanged += new PropertyChangedEventHandler(parentEntry_PropertyChanged);
            EntriesByAddin.Add(parent);
        }
    }

    private void parentEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is AddinEntry parent && !parent.IsUpdating)
        {
            parent.IsUpdating = true;
            parent.UpdateChildren(e.PropertyName);
            parent.IsUpdating = false;
        }
    }
    private void childEntry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is AddinEntry child && !child.IsUpdating)
        {
            child.IsUpdating = true;
            var entryByYear = EntriesByYear.FirstOrDefault(item => item.DisplayName == child.AddinYear)?
                .Children.FirstOrDefault(item => item.DisplayName == child.AddinName);
            var entryByAddin = EntriesByAddin.FirstOrDefault(item => item.DisplayName == child.AddinName)?
                .Children.FirstOrDefault(item => item.DisplayName == child.AddinYear);

            entryByYear?.MatchPropertySilently(e.PropertyName, child);
            entryByAddin?.MatchPropertySilently(e.PropertyName, child);

            if (child.Parent is AddinEntry parent && !parent.IsUpdating)
            {
                parent.PropertyChanged -= new PropertyChangedEventHandler(parentEntry_PropertyChanged);
                parent.VerifyCheckState(e.PropertyName);
                parent.PropertyChanged += new PropertyChangedEventHandler(parentEntry_PropertyChanged);
            }

            if (e.PropertyName == nameof(child.OnByDefault))
            {
                child.UpdateFile(DebugState.None);
            }
            else if (e.PropertyName == nameof(child.OnByDebug))
            {
                //TODO: Update config file
            }
            child.IsUpdating = false;
        }

    }
    private void location_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (sender is ManifestLocations locations)
        {
            refreshList(locations.MachinePath, locations.UserPath);
        }
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

    private void groupingToggle_Click(object sender, RoutedEventArgs e)
    {
        if (trvMenu is not null)
        {
            trvMenu.Items.Clear();
            if (groupingToggle.Content.ToString() == _changeGroupingToName)
            {
                groupingToggle.Content = _changeGroupingToYear;
                foreach (var entry in EntriesByAddin) { trvMenu.Items.Add(entry); } 
            }
            else
            {
                groupingToggle.Content = _changeGroupingToName;
                foreach (var entry in EntriesByYear) { trvMenu.Items.Add(entry); } 
            }
        }
    }

    private void textBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {

        if (e.ClickCount == 2 && 
            sender is TextBlock textBlock && 
            textBlock.ToolTip is string path &&
            File.Exists(path))
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}