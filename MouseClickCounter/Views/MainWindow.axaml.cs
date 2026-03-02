using Avalonia.Controls;
using Avalonia.Interactivity;
using MouseClickCounter.ViewModels;
using Ursa.Controls;

namespace MouseClickCounter.Views;

public partial class MainWindow : UrsaWindow
{
    private MainWindowViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
        Closing += OnWindowClosing;
    }

    private void OnWindowLoaded(object? sender, RoutedEventArgs e)
    {
        _viewModel = DataContext as MainWindowViewModel;
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        _viewModel?.Cleanup();
    }
}