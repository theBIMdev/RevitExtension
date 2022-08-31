using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitExtension.RevitAddinControl;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace RevitExtension;

public class AddinEntry : INotifyPropertyChanged
{
    private ObservableCollection<AddinEntry> _children = new();
    private bool? _onByDefault;
    private bool? _onByDebug;
    private string _filePath;

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
    public bool? OnByDebug
    {
        get => _onByDebug;
        set
        {
            _onByDebug = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(OnByDebug)));
        }
    }
    public string FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(FilePath)));
        }
    }
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

    public void Update(string propertyName, AddinEntry twinEntry)
    {
        IsUpdating = true;

        if (propertyName == nameof(OnByDefault))
        {
            updateFile(DebugState.None);
        }
        else if (propertyName == nameof(OnByDebug))
        {
            //TODO: Update config file
        }

        twinEntry.MatchPropertySilently(propertyName, this);

        if (Parent is AddinEntry parent && !parent.IsUpdating)
        {
            parent.IsUpdating = true;
            parent.VerifyCheckState(propertyName);
            parent.IsUpdating = false;
        }
        IsUpdating = false;
    }

    private void updateFile(DebugState debugger)
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
                throw new NotImplementedException();
            case DebugState.Ending:
                throw new NotImplementedException();
        }
    }

    private void turnOn(string file)
    {
        string newName = file.Replace(".addin.off", ".addin");
        rename(file, newName);
    }
    private void turnOff(string file)
    {
        string newName = file.Replace(".addin", ".addin.off");
        rename(file, newName);
    }
    private void rename(string oldName, string newName)
    {
        try
        {
            File.Move(oldName, newName);
            FilePath = newName;
        }
        catch
        {
            string message = $"Could not rename the manifest for the addin '{AddinName}'.\r\n" +
                $"Please make sure that:\r\n" +
                $"-The file is not in use\r\n" +
                $"-You have access to edit it\r\n" +
                $"-There is not another copy of this manifest in this location\r\n\r\n" +
                $"Current Name: {oldName}\r\n" +
                $"Name Attempted: {newName}";
            VS.MessageBox.ShowError(message);
            throw;
        }
    }

    public void MatchPropertySilently(string propertyName, AddinEntry entry)
    {
        _filePath = entry.FilePath;
        if (propertyName == nameof(OnByDefault))
        {
            _onByDefault = entry.OnByDefault;
        }
        else if (propertyName == nameof(OnByDebug))
        {
            _onByDebug = entry.OnByDebug;
        }
    }
}

