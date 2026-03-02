using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MouseClickCounter.ViewModels;
using MouseClickCounter.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using MouseClickCounter.Services;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = new ApplicationViewModel();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Configure dependency injection
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            // Resolve MainWindowViewModel from DI container
            var mainWindowViewModel = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register services
        services.AddSingleton<IConfigManager, ConfigManager>();
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<IDeviceInfoService, DeviceInfoService>();
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IDataStorageService, DataStorageService>();
        services.AddSingleton<IMouseHookService, MouseHookService>();
        services.AddSingleton<IDialogService, DialogService>();

        // Register HttpClient for RankingApiService
        services.AddSingleton<HttpClient>();
        services.AddSingleton<IRankingApiService, RankingApiService>();

        // Register ViewModels
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<ConfigViewModel>();
        services.AddTransient<AllRankViewModel>();
    }
}