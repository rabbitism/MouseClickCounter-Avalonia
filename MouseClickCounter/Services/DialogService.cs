using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using MouseClickCounter.Services.Interfaces;
using MouseClickCounter.ViewModels;
using MouseClickCounter.Views;
using Ursa.Controls;

namespace MouseClickCounter.Services
{
    public class DialogService : IDialogService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogService _logService;

        public DialogService(IServiceProvider serviceProvider, ILogService logService)
        {
            _serviceProvider = serviceProvider;
            _logService = logService;
        }

        public async Task ShowConfigDialogAsync()
        {
            try
            {
                await _logService.WriteInfoAsync("Opening config dialog");

                var viewModel = _serviceProvider.GetRequiredService<ConfigViewModel>();

                var options = new DialogOptions
                {
                    Title = "设置 - Settings",
                    Button = DialogButton.None,
                    StartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    CanDragMove = true
                };

                // Get the dialog window reference before showing it
                Window? dialogWindow = null;

                // Subscribe to RequestClose event before showing dialog
                void OnRequestClose(object? sender, EventArgs e)
                {
                    viewModel.RequestClose -= OnRequestClose;
                    dialogWindow?.Close();
                }

                viewModel.RequestClose += OnRequestClose;

                // Show the dialog and get the window reference
                var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is
                    Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (mainWindow != null)
                {
                    dialogWindow = new DialogWindow
                    {
                        Content = new ConfigDialog(),
                        DataContext = viewModel,
                        Title = options.Title,
                        CanResize = false,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Icon = mainWindow.Icon,
                        Width = 500,
                        Height = 550
                    };

                    await dialogWindow.ShowDialog(mainWindow);
                }

                await _logService.WriteInfoAsync("Config dialog closed");
            }
            catch (Exception ex)
            {
                await _logService.WriteErrorAsync("Error showing config dialog", ex);
            }
        }

        public async Task ShowAllRankDialogAsync()
        {
            try
            {
                await _logService.WriteInfoAsync("Opening all rank dialog");

                var viewModel = _serviceProvider.GetRequiredService<AllRankViewModel>();

                var options = new DialogOptions
                {
                    Title = "全国省份排行榜",
                    Button = DialogButton.None,
                    StartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner,
                    CanResize = true,
                    CanDragMove = true
                };

                // Get the dialog window reference before showing it
                Window? dialogWindow = null;

                // Subscribe to RequestClose event before showing dialog
                void OnRequestClose(object? sender, EventArgs e)
                {
                    viewModel.RequestClose -= OnRequestClose;
                    dialogWindow?.Close();
                }

                viewModel.RequestClose += OnRequestClose;

                // Show the dialog and get the window reference
                var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is
                    Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                if (mainWindow != null)
                {
                    dialogWindow = new DialogWindow
                    {
                        Content = new AllRankDialog(),
                        DataContext = viewModel,
                        Title = options.Title,
                        CanResize = true,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner,
                        Icon = mainWindow.Icon,
                        Width = 800,
                        Height = 600
                    };

                    await dialogWindow.ShowDialog(mainWindow);
                }

                await _logService.WriteInfoAsync("All rank dialog closed");
            }
            catch (Exception ex)
            {
                await _logService.WriteErrorAsync("Error showing all rank dialog", ex);
            }
        }
    }
}
