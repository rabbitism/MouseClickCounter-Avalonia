using System.Threading.Tasks;
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
        (DataContext as MainWindowViewModel)?.Cleanup();
        return base.CanClose();
    }
}