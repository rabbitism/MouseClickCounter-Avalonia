using System;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MouseClickCounter.Models;
using MouseClickCounter.Services;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    // 服务实例
    private readonly IConfigManager _configManager;
    private readonly ILogService _logService;
    private readonly IDeviceInfoService _deviceInfoService;
    private readonly IDataStorageService _dataStorageService;
    private readonly IRankingApiService _rankingApiService;
    private readonly IMouseHookService _mouseHookService;

    // 设备信息
    private DeviceInfoService.DeviceInfo? _deviceInfo;
    private ClickData? _currentClickData;

    // 定时器
    private Timer? _dataSyncTimer;
    private Timer? _localDataTimer;

    [ObservableProperty]
    private long _leftClickCount;

    [ObservableProperty]
    private long _rightClickCount;

    public long TotalClickCount => LeftClickCount + RightClickCount;

    [ObservableProperty]
    private string _rankingText = "全国排名：未参与";

    [ObservableProperty]
    private string _rankingColor = "Gray";

    public MainWindowViewModel(
        IConfigManager configManager,
        ILogService logService,
        IDeviceInfoService deviceInfoService,
        IDataStorageService dataStorageService,
        IRankingApiService rankingApiService,
        IMouseHookService mouseHookService)
    {
        // 初始化服务
        _configManager = configManager;
        _ = _configManager.InitializeAsync();
        _ = _configManager.CreateDefaultConfigAsync();

        _logService = logService;
        _deviceInfoService = deviceInfoService;
        _dataStorageService = dataStorageService;
        _rankingApiService = rankingApiService;
        _mouseHookService = mouseHookService;

        // 初始化设备信息
        InitializeDeviceInfo();

        // 初始化鼠标钩子
        InitializeMouseHook();

        // 初始化定时器
        InitializeTimers();

        // 加载保存的数据
        _ = LoadClickDataAsync();

        _ = _logService.WriteInfoAsync("MainWindowViewModel 初始化成功");
    }

    private void InitializeDeviceInfo()
    {
        _deviceInfo = _deviceInfoService.GetDeviceInfo();
        _ = _logService.WriteInfoAsync($"设备信息初始化完成：名称={_deviceInfo.DeviceName}, ID={_deviceInfo.DeviceId}");
    }

    private void InitializeMouseHook()
    {
        bool success = _mouseHookService.InstallHook();
        if (!success)
        {
            _ = _logService.WriteErrorAsync("安装鼠标钩子失败");
        }
        else
        {
            _ = _logService.WriteInfoAsync("鼠标钩子安装成功");
        }
    }

    private void InitializeTimers()
    {
        // 初始化数据同步定时器（使用配置的刷新时间，仅用于服务器同步）
        int syncIntervalMinutes = _configManager.GetSyncInterval();
        _dataSyncTimer = new Timer(syncIntervalMinutes * 60 * 1000);
        _dataSyncTimer.Elapsed += DataSyncTimer_Elapsed;

        // 初始化本地数据刷新定时器（每6秒刷新一次界面显示）
        _localDataTimer = new Timer(6000);
        _localDataTimer.Elapsed += LocalDataTimer_Elapsed;

        // 程序启动时重置上次同步的点击次数
        _rankingApiService.ResetLastSyncedClicks();

        // 启动定时器
        _dataSyncTimer.Start();
        _localDataTimer.Start();

        _ = _logService.WriteInfoAsync("定时器初始化成功");
    }

    private void LocalDataTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        // 更新界面显示
        UpdateClickCountDisplay();

        // 自动保存数据（防止数据丢失）
        _ = SaveClickDataAsync();
    }

    private async void DataSyncTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        // 如果加入了排行榜，同步到服务器并获取最新排名
        if (_currentClickData != null && _currentClickData.JoinRanking)
        {
            // 同步数据到服务器
            await _rankingApiService.SyncToRankingServer(_currentClickData);
        }

        // 刷新排名数据
        await UpdateRankingDisplay();
    }

    private void UpdateClickCountDisplay()
    {
        LeftClickCount = _mouseHookService.GetLeftClickCount();
        RightClickCount = _mouseHookService.GetRightClickCount();
        OnPropertyChanged(nameof(TotalClickCount));
    }

    private async Task UpdateRankingDisplay()
    {
        try
        {
            bool joinRanking = _configManager.GetJoinRanking();
            if (joinRanking && _deviceInfo != null)
            {
                // 从服务器获取排行榜数据
                await _logService.WriteInfoAsync("正在从服务器获取排行榜数据...");
                var rankingResponse = await _rankingApiService.GetDeviceRanking(_deviceInfo.DeviceId);
                if (rankingResponse != null)
                {
                    RankingText = $"全国排名：第{rankingResponse.Rank}名";
                    RankingColor = "#FF4D4F";
                }
                else
                {
                    RankingText = "全国排名：请求服务器失败...";
                    RankingColor = "#FF0000";
                }
            }
            else
            {
                RankingText = "全国排名：未参与";
                RankingColor = "Gray";
            }
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync("更新排名显示失败", ex);
        }
    }

    private async Task SaveClickDataAsync()
    {
        try
        {
            if (_currentClickData == null)
            {
                _currentClickData = new ClickData();
            }

            if (_deviceInfo != null)
            {
                // 更新数据
                _currentClickData.DeviceId = _deviceInfo.DeviceId;
                _currentClickData.DeviceName = _deviceInfo.DeviceName;
                _currentClickData.LeftClicks = _mouseHookService.GetLeftClickCount();
                _currentClickData.RightClicks = _mouseHookService.GetRightClickCount();
                _currentClickData.JoinRanking = _configManager.GetJoinRanking();
                _currentClickData.Timestamp = DateTime.Now;

                // 保存到文件
                await _dataStorageService.SaveClickDataAsync(_currentClickData);
            }
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync("保存点击数据时发生错误", ex);
        }
    }

    private async Task LoadClickDataAsync()
    {
        try
        {
            _currentClickData = await _dataStorageService.LoadClickDataAsync();
            if (_currentClickData != null)
            {
                // 使用加载的数据
                _mouseHookService.SetLeftClickCount(_currentClickData.LeftClicks);
                _mouseHookService.SetRightClickCount(_currentClickData.RightClicks);
                await _logService.WriteInfoAsync("已加载保存的数据");

                // 设置上次同步的点击次数为当前点击次数
                _rankingApiService.SetLastSyncedClicks(_currentClickData.LeftClicks, _currentClickData.RightClicks);
            }
            else
            {
                // 创建新的数据
                _currentClickData = new ClickData();
                if (_deviceInfo != null)
                {
                    _currentClickData.DeviceId = _deviceInfo.DeviceId;
                    _currentClickData.DeviceName = _deviceInfo.DeviceName;
                    _currentClickData.JoinRanking = _configManager.GetJoinRanking();
                }

                await _logService.WriteInfoAsync("未找到保存的数据，已创建新数据");
            }

            // 更新界面显示
            UpdateClickCountDisplay();

            // 刷新排行榜数据
            _ = UpdateRankingDisplay();
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync("加载数据失败", ex);
        }
    }

    [RelayCommand]
    private async Task ShowAllRank()
    {
        await _logService.WriteInfoAsync("用户点击查看全国排行榜按钮");
        // This will be handled by the view
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ShowConfig()
    {
        await _logService.WriteInfoAsync("用户点击设置按钮");
        // This will be handled by the view
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task RankingClick()
    {
        if (_currentClickData == null || !_currentClickData.JoinRanking)
        {
            // 如果未加入排行榜，打开配置
            await ShowConfig();
        }
    }

    public void UpdateSyncTimerInterval()
    {
        try
        {
            if (_dataSyncTimer != null)
            {
                int syncIntervalMinutes = _configManager.GetSyncInterval();
                _dataSyncTimer.Stop();
                _dataSyncTimer.Interval = syncIntervalMinutes * 60 * 1000;
                _dataSyncTimer.Start();
                _ = _logService.WriteInfoAsync($"已更新数据同步间隔为 {syncIntervalMinutes} 分钟");
            }
        }
        catch (Exception ex)
        {
            _ = _logService.WriteErrorAsync("更新数据同步定时器间隔失败", ex);
        }
    }

    public async Task RefreshRankingAsync()
    {
        await UpdateRankingDisplay();
    }

    public void UpdateJoinRanking()
    {
        if (_currentClickData != null)
        {
            _currentClickData.JoinRanking = _configManager.GetJoinRanking();
        }
    }

    public void Cleanup()
    {
        try
        {
            _mouseHookService.UninstallHook();
            _ = SaveClickDataAsync();
            _ = _logService.WriteInfoAsync("程序关闭，已保存数据");
            _dataSyncTimer?.Stop();
            _localDataTimer?.Stop();
        }
        catch (Exception ex)
        {
            _ = _logService.WriteErrorAsync("清理资源时发生错误", ex);
        }
    }
}
