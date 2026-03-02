using System.Threading.Tasks;
using Avalonia.Controls;
using Ursa.Controls;

namespace MouseClickCounter.Views;

public partial class MainWindow : UrsaWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override Task<bool> CanClose()
    {
        WindowState = WindowState.Minimized;
        ShowInTaskbar = false;
        return Task.FromResult(false);
    }
}