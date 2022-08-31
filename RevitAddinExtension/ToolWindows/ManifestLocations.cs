using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddinExtension;
public class ManifestLocations : INotifyPropertyChanged
{
    const string _adskPath = "\\Autodesk\\Revit\\Addins";
    private string _machinePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + _adskPath;
    private string _userPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + _adskPath;


    public event PropertyChangedEventHandler? PropertyChanged;
    public virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler? handler = PropertyChanged;
        handler?.Invoke(this, e);
    }

    public string MachinePath
    {
        get => _machinePath;
        set
        {
            _machinePath = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(MachinePath)));
        }
    }
    public string UserPath
    {
        get => _userPath;
        set
        {
            _userPath = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(UserPath)));
        }
    }
}

