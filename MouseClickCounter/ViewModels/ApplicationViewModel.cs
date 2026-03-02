using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace MouseClickCounter.ViewModels;

public partial class ApplicationViewModel: ViewModelBase
{
    [RelayCommand]
    private void Activate()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var mainWindow = desktop.MainWindow;
        if (mainWindow is not null && !mainWindow.IsActive)
        {
            if (mainWindow.WindowState is WindowState.Minimized)
            {
                mainWindow.WindowState = WindowState.Normal;
                mainWindow.ShowInTaskbar = true;
            }

            mainWindow.Activate();
        }
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainContext = desktop.MainWindow?.DataContext as MainWindowViewModel;
            mainContext?.Cleanup();
            desktop.Shutdown();
        }
    }
}