using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;

namespace MouseClickCounter.ViewModels;

public partial class ApplicationViewModel: ViewModelBase
{
    [RelayCommand]
    private void Activate()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mainWindow = desktop.MainWindow;
        if (mainWindow is null || mainWindow.IsActive) return;
        if (mainWindow.WindowState is WindowState.Minimized)
        {
            mainWindow.WindowState = WindowState.Normal;
            mainWindow.ShowInTaskbar = true;
        }

        mainWindow.Activate();
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mainContext = desktop.MainWindow?.DataContext as MainWindowViewModel;
        mainContext?.Cleanup();
        desktop.Shutdown();
    }
}