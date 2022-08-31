using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RevitExtension;
public class RevitAddinWindow : BaseToolWindow<RevitAddinWindow>
{
    public override string GetTitle(int toolWindowId) => "Revit Addin Manager";

    public override Type PaneType => typeof(Pane);

    public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
    {
        return Task.FromResult<FrameworkElement>(new RevitAddinControl());
    }

    [Guid("1304915e-aed9-4d46-a5f1-90721397fb58")]
    internal class Pane : ToolkitToolWindowPane
    {
        public Pane()
        {
            BitmapImageMoniker = KnownMonikers.ToolWindow;
        }
    }
}