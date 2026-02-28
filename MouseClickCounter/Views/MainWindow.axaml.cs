using Avalonia.Controls;
using Avalonia.Interactivity;
using MouseClickCounter.ViewModels;
using MouseClickCounter.Services;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MouseClickCounter.Views;

public partial class MainWindow : Window
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
        if (_viewModel != null)
        {
            // Subscribe to PropertyChanged to detect when commands are invoked
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // This is a simple approach - in a real app you'd use a messenger
        // For now, we'll handle button clicks directly in XAML code-behind
    }

    private void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        _viewModel?.Cleanup();
    }

    // These will be called from button clicks in XAML
    private async void OnShowConfigClick(object? sender, RoutedEventArgs e)
    {
        if (App.ServiceProvider == null) return;

        var configViewModel = App.ServiceProvider.GetRequiredService<ConfigViewModel>();
        var configWindow = new ConfigWindow(configViewModel)
        {
            DataContext = configViewModel
        };

        await configWindow.ShowDialog(this);

        // Refresh main window after config changes
        _viewModel?.UpdateSyncTimerInterval();
        _viewModel?.UpdateJoinRanking();
        if (_viewModel != null)
            await _viewModel.RefreshRankingAsync();
    }

    private async void OnShowAllRankClick(object? sender, RoutedEventArgs e)
    {
        if (App.ServiceProvider == null) return;

        var allRankViewModel = App.ServiceProvider.GetRequiredService<AllRankViewModel>();
        var allRankWindow = new AllRankWindow(allRankViewModel)
        {
            DataContext = allRankViewModel
        };

        await allRankWindow.ShowDialog(this);
    }
}