using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Avalonia.Shared.Contracts;
using MouseClickCounter.Services;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.ViewModels
{
    public partial class AllRankViewModel : ViewModelBase, IDialogContext
    {
        public event EventHandler<object?>? RequestClose;

        private readonly IRankingApiService _rankingApiService;
        private readonly ILogService _logService;

        [ObservableProperty] private string _statsText = "正在加载数据...";

        [ObservableProperty] private ObservableCollection<ProvinceRankingItem> _rankings = [];

        public AllRankViewModel(IRankingApiService rankingApiService, ILogService logService)
        {
            _rankingApiService = rankingApiService;
            _logService = logService;

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                StatsText = "正在加载数据...";

                var data = await _rankingApiService.GetAllProvinceRanking();
                if (data != null)
                {
                    Rankings.Clear();
                    int rank = 1;
                    foreach (var item in data.NationalRankList)
                    {
                        item.Rank = rank++;
                        Rankings.Add(item);
                    }

                    StatsText = $"总参与设备：{data.TotalDevices:N0} | 总点击次数：{data.TotalClicks:N0}";
                    await _logService.WriteInfoAsync($"成功加载 {Rankings.Count} 个省份的排行数据");
                }
                else
                {
                    StatsText = "数据加载失败，请检查网络连接或稍后重试";
                    await _logService.WriteErrorAsync("获取省份排行数据失败");
                }
            }
            catch (Exception ex)
            {
                StatsText = $"数据加载失败：{ex.Message}";
                await _logService.WriteErrorAsync("加载省份排行数据时发生错误", ex);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private void CloseDialog()
        {
            RequestClose?.Invoke(this, null);
        }

        public void Close()
        {
            RequestClose?.Invoke(this, null);
        }
    }
}