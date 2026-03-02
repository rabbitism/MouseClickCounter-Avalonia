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

                await Dialog.ShowModal<ConfigDialog, ConfigViewModel>(viewModel, options: options);
                
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
                
                await Dialog.ShowModal<AllRankDialog, AllRankViewModel>(viewModel, options: options);

                await _logService.WriteInfoAsync("All rank dialog closed");
            }
            catch (Exception ex)
            {
                await _logService.WriteErrorAsync("Error showing all rank dialog", ex);
            }
        }
    }
}
