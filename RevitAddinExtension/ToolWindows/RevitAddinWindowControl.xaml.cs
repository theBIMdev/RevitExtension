using System.Windows;
using System.Windows.Controls;

namespace RevitAddinExtension;
public partial class RevitAddinWindowControl : UserControl
{
    public RevitAddinWindowControl()
    {
        InitializeComponent();
    }

    private void button1_Click(object sender, RoutedEventArgs e)
    {
        VS.MessageBox.Show("RevitAddinExtension", "Button clicked");
    }
}