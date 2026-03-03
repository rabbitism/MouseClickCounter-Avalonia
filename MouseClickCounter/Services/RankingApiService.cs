using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MouseClickCounter.Models;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services;

/// <summary>
/// 排行榜API服务
/// </summary>
public class RankingApiService : IRankingApiService
{
    private readonly IConfigManager _configManager;
    private readonly ILogService _logService;
    private readonly HttpClient _httpClient;

    // 上次同步的点击次数（用于增量同步）
    private long _lastSyncedLeftClicks;
    private long _lastSyncedRightClicks;

    /// <summary>
    /// 创建排行榜API服务实例
    /// </summary>
    public RankingApiService(IConfigManager configManager, ILogService logService, HttpClient httpClient)
    {
        _configManager = configManager;
        _logService = logService;
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// 同步数据到排行榜服务器（增量同步）
    /// </summary>
    public async Task SyncToRankingServer(ClickData clickData)
    {
        try
        {
            // 从配置中获取API地址
            var baseUrl = _configManager.GetApiUrl();
            var apiUrl = $"{baseUrl}/sync-clicks";

            // 计算增量（本次点击次数减去上次同步的点击次数）
            var leftClicksDelta = clickData.LeftClicks - _lastSyncedLeftClicks;
            var rightClicksDelta = clickData.RightClicks - _lastSyncedRightClicks;

            await _logService.WriteInfoAsync($"正在同步数据到排行榜服务器({apiUrl})... 设备：{clickData.DeviceName} (ID: {clickData.DeviceId})");
            await _logService.WriteInfoAsync($"增量数据：左键 {leftClicksDelta} 次，右键 {rightClicksDelta} 次");

            // 检查增量数据，如果为0或小于0，则不同步
            if (leftClicksDelta <= 0 && rightClicksDelta <= 0)
            {
                await _logService.WriteInfoAsync($"增量数据为0或负数，跳过同步");
                return;
            }

            // 构建请求数据（根据API要求，只需要四个参数）
            var requestData = new SyncDataRequest()
            {
                DeviceId = clickData.DeviceId,
                DeviceName = clickData.DeviceName,
                LeftClicks = leftClicksDelta,
                RightClicks = rightClicksDelta
            };

            // 发送HTTP请求
            var json = JsonSerializer.Serialize(requestData, Context.Default.Options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                await _logService.WriteInfoAsync($"服务器响应：{responseContent}");

                // 同步成功后，更新上次同步的点击次数
                _lastSyncedLeftClicks = clickData.LeftClicks;
                _lastSyncedRightClicks = clickData.RightClicks;
                await _logService.WriteInfoAsync($"同步成功，已更新上次同步的点击次数：左键 {_lastSyncedLeftClicks}，右键 {_lastSyncedRightClicks}");
            }
            else
            {
                await _logService.WriteErrorAsync($"服务器返回错误：{response.StatusCode}，内容：{responseContent}");
            }
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync($"同步到服务器失败", ex);
        }
    }

    /// <summary>
    /// 重置上次同步的点击次数（用于程序重启或手动重置）
    /// </summary>
    public void ResetLastSyncedClicks()
    {
        _lastSyncedLeftClicks = 0;
        _lastSyncedRightClicks = 0;
        _ = _logService.WriteInfoAsync($"已重置上次同步的点击次数");
    }

    /// <summary>
    /// 设置上次同步的点击次数（用于加载保存的数据）
    /// </summary>
    public void SetLastSyncedClicks(long leftClicks, long rightClicks)
    {
        _lastSyncedLeftClicks = leftClicks;
        _lastSyncedRightClicks = rightClicks;
        _ = _logService.WriteInfoAsync($"已设置上次同步的点击次数：左键 {leftClicks}，右键 {rightClicks}");
    }

    /// <summary>
    /// 获取设备排名
    /// </summary>
    public async Task<RankingData?> GetDeviceRanking(string deviceId)
    {
        try
        {
            // 从配置中获取API地址，包含当前日期
            var baseUrl = _configManager.GetApiUrl();
            var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            var apiUrl = $"{baseUrl}/device-rank?deviceId={Uri.EscapeDataString(deviceId)}&date={currentDate}";
            await _logService.WriteInfoAsync($"正在获取设备排名... 设备ID: {deviceId}, 日期: {currentDate}");

            // 发送HTTP请求
            var response = await _httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                await _logService.WriteInfoAsync($"服务器响应：{responseContent}");

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<RankingData>>(responseContent, options: Context.Default.Options);
                await _logService.WriteInfoAsync($"获取排名成功，响应代码：{apiResponse?.Code}，消息：{apiResponse?.Message}");
                return apiResponse?.Data;
            }
            else
            {
                await _logService.WriteErrorAsync($"服务器返回错误：{response.StatusCode}，内容：{responseContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync($"获取排行榜数据失败", ex);
            return null;
        }
    }

    /// <summary>
    /// 检查服务器连接
    /// </summary>
    public async Task<bool> CheckServerConnection()
    {
        try
        {
            // 从配置中获取API地址进行检查
            var baseUrl = _configManager.GetApiUrl();
            var apiUrl = $"{baseUrl}/sync-clicks";
            // 发送HEAD请求检查连接（更轻量）
            var request = new HttpRequestMessage(HttpMethod.Head, apiUrl);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取全国省份排行
    /// </summary>
    public async Task<ProvinceRankingResponse?> GetAllProvinceRanking()
    {
        try
        {
            // 从配置中获取API地址
            var baseUrl = _configManager.GetApiUrl();
            var apiUrl = $"{baseUrl}/all-rank";
            await _logService.WriteInfoAsync($"正在获取全国省份排行...");

            // 发送HTTP请求
            var response = await _httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                await _logService.WriteInfoAsync($"服务器响应：{responseContent}");

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProvinceRankingResponse>>(responseContent, Context.Default.Options);
                await _logService.WriteInfoAsync($"获取全国省份排行成功，响应代码：{apiResponse?.Code}，消息：{apiResponse?.Message}");
                return apiResponse?.Data;
            }
            else
            {
                await _logService.WriteErrorAsync($"服务器返回错误：{response.StatusCode}，内容：{responseContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            await _logService.WriteErrorAsync($"获取全国省份排行失败", ex);
            return null;
        }
    }
}