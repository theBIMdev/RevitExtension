using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitExtension.RevitAddinControl;

namespace RevitExtension;

public class AddinEntry : INotifyPropertyChanged
{
    private ObservableCollection<AddinEntry> _children = new();
    private bool? _onByDefault;
    private bool? _onByDebug;

    public AddinEntry() { }

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        handler?.Invoke(this, e);
    }
    public string AddinName { get; set; }
    public string AddinYear { get; set; }
    public string DisplayName { get; set; }
    public bool? OnByDefault
    {
        get => _onByDefault;
        set
        {
            _onByDefault = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(OnByDefault)));
        }
    }
    public string OnByDefaultToolTip { get; set; }
    public bool? OnByDebug
    {
        get => _onByDebug;
        set
        {
            _onByDebug = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(OnByDebug)));
        }
    }
    public string OnByDebugToolTip { get; set; }
    public bool? IsEnabled { get; set; }
    public string FilePath { get; set; }
    public bool IsUpdating { get; set; } = false;

    public AddinEntry Parent { get; set; }
    public string MarginOffset { get => Parent is null ? "0, 0, 0, 0" : "-38, 0, 0, 0"; }
    public string Opacity { get => Parent is null ? "1.0" : "0.75"; }
    public ObservableCollection<AddinEntry> Children
    {
        get => _children;
        set
        {
            _children = value;
            foreach (var child in _children) { child.Parent = this; }
        }
    }

    public void UpdateChildren(string propertyName)
    {
        if (propertyName == nameof(OnByDefault))
        {
            foreach (var child in _children.Where(child => child.OnByDefault != OnByDefault))
            {
                child.OnByDefault = OnByDefault;
            }
        }
        else if (propertyName == nameof(OnByDebug))
        {
            foreach (var child in _children.Where(child => child.OnByDebug != OnByDebug))
            {
                child.OnByDebug = OnByDebug;
            }
        }
    }

    public void VerifyCheckState(string propertyName)
    {
        if (!_children.Any()) { return; }

        if (propertyName == nameof(OnByDefault))
        {
            bool? byDefault = _children.First().OnByDefault;
            foreach (var child in _children)
            {
                if (child.OnByDefault != byDefault)
                { byDefault = null; break; }
            }
            OnByDefault = byDefault;
        }

        else if (propertyName == nameof(OnByDebug))
        {
            bool? byDebug = _children.First().OnByDebug;
            foreach (var child in _children)
            {
                if (child.OnByDebug != byDebug)
                { byDebug = null; break; }
            }
            OnByDebug = byDebug;
        }
    }

    public void MatchPropertySilently(string propertyName, AddinEntry entry)
    {
        FilePath = entry.FilePath;
        if (propertyName == nameof(OnByDefault))
        {
            _onByDefault = entry.OnByDefault;
        }
        else if (propertyName == nameof(OnByDebug))
        {
            _onByDebug = entry.OnByDebug;
        }
    }

    public void UpdateFile(DebugState debugger)
    {
        if (FilePath is null) return;

        switch (debugger)
        {
            case DebugState.None:
                if (!OnByDefault.HasValue) return;
                if (OnByDefault is true) { turnOn(FilePath); }
                else { turnOff(FilePath); }
                break;
            case DebugState.Starting:
                break;
            case DebugState.Ending:
                break;
        }
    }

    private void turnOn(string file)
    {
        string newName = file.Replace(".addin.off", ".addin");
        File.Move(file, newName);
        FilePath = newName;
    }
    private void turnOff(string file)
    {
        string newName = file.Replace(".addin", ".addin.off");
        File.Move(file, newName);
        FilePath = newName;
    }
}

