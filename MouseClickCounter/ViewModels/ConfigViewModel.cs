using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseClickCounter.Services;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.ViewModels
{
    public partial class ConfigViewModel : ViewModelBase
    {
        private readonly IConfigManager _configManager;
        private readonly IDataStorageService _dataStorageService;
        private readonly IRankingApiService _rankingApiService;
        private readonly ILogService _logService;

        public event EventHandler? RequestClose;

        [ObservableProperty]
        private string _apiUrl = string.Empty;

        [ObservableProperty]
        private bool _joinRanking;

        [ObservableProperty]
        private bool _runOnStartup;

        [ObservableProperty]
        private int _syncInterval = 5;

        public int SyncIntervalIndex
        {
            get
            {
                return SyncInterval switch
                {
                    1 => 0,
                    3 => 1,
                    5 => 2,
                    10 => 3,
                    _ => 2 // Default to 5 minutes
                };
            }
            set
            {
                SyncInterval = value switch
                {
                    0 => 1,
                    1 => 3,
                    2 => 5,
                    3 => 10,
                    _ => 5
                };
                OnPropertyChanged();
            }
        }

        public ConfigViewModel(
            IConfigManager configManager,
            IDataStorageService dataStorageService,
            IRankingApiService rankingApiService,
            ILogService logService)
        {
            _configManager = configManager;
            _dataStorageService = dataStorageService;
            _rankingApiService = rankingApiService;
            _logService = logService;

            LoadConfig();
        }

        private void LoadConfig()
        {
            ApiUrl = _configManager.GetApiUrl();
            JoinRanking = _configManager.GetJoinRanking();
            RunOnStartup = _configManager.GetRunOnStartup();
            SyncInterval = _configManager.GetSyncInterval();
        }

        [RelayCommand]
        private async Task Save()
        {
            await _configManager.SetApiUrlAsync(ApiUrl);
            await _configManager.SetJoinRankingAsync(JoinRanking);
            await _configManager.SetRunOnStartupAsync(RunOnStartup);
            await _configManager.SetSyncIntervalAsync(SyncInterval);

            await _logService.WriteInfoAsync("配置已保存");
        }

        [RelayCommand]
        private async Task ResetClicks()
        {
            // 重置点击数据
            var clickData = await _dataStorageService.LoadClickDataAsync();

            if (clickData != null)
            {
                clickData.LeftClicks = 0;
                clickData.RightClicks = 0;
                await _dataStorageService.SaveClickDataAsync(clickData);

                // 重置排行榜API服务的上次同步数据
                _rankingApiService.ResetLastSyncedClicks();

                await _logService.WriteInfoAsync("点击数据已重置为0");
            }

            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task SaveAndClose()
        {
            await Save();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void CloseDialog()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
