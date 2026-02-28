using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseClickCounter.Services;

namespace MouseClickCounter.ViewModels
{
    public partial class ConfigViewModel : ViewModelBase
    {
        private readonly ConfigManager _configManager;
        private readonly DataStorageService _dataStorageService;
        private readonly RankingApiService _rankingApiService;
        private readonly LogService _logService;

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

        public ConfigViewModel(ConfigManager configManager)
        {
            _configManager = configManager;
            _dataStorageService = new DataStorageService();
            _rankingApiService = new RankingApiService(configManager);
            _logService = new LogService();

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
        private void Save()
        {
            _configManager.SetApiUrl(ApiUrl);
            _configManager.SetJoinRanking(JoinRanking);
            _configManager.SetRunOnStartup(RunOnStartup);
            _configManager.SetSyncInterval(SyncInterval);

            _logService.WriteInfo("配置已保存");
        }

        [RelayCommand]
        private async Task ResetClicks()
        {
            // 重置点击数据
            var clickData = _dataStorageService.LoadClickData();

            if (clickData != null)
            {
                clickData.LeftClicks = 0;
                clickData.RightClicks = 0;
                _dataStorageService.SaveClickData(clickData);

                // 重置排行榜API服务的上次同步数据
                _rankingApiService.ResetLastSyncedClicks();

                _logService.WriteInfo("点击数据已重置为0");
            }

            await Task.CompletedTask;
        }
    }
}
