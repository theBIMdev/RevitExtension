namespace RevitExtension;

[Command(PackageIds.MyCommand)]
internal sealed class RevitAddinCommand : BaseCommand<RevitAddinCommand>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        return RevitAddinWindow.ShowAsync();
    }
}
