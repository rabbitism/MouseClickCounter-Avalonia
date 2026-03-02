using System.Threading.Tasks;
using Avalonia.Controls;
using MouseClickCounter.ViewModels;
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
        this.WindowState = WindowState.Minimized;
        this.ShowInTaskbar = false;
        return Task.FromResult(false);
        (DataContext as MainWindowViewModel)?.Cleanup();
        return base.CanClose();
    }
}