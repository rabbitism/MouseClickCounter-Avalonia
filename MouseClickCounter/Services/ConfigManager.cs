using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services;

public class ConfigManager : IConfigManager
{
    private string _configPath;
    internal const string DEFAULT_SERVER_API = "https://exeekfa1x7.sealosbja.site";
    private ConfigData _config = new();
    private readonly object _lock = new object();

    public ConfigManager()
    {
        _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
    }

    public async Task InitializeAsync()
    {
        await LoadConfigAsync();
    }

    private async Task LoadConfigAsync()
    {
        try
        {
            if (File.Exists(_configPath))
            {
                string json = await File.ReadAllTextAsync(_configPath);
                var config = JsonSerializer.Deserialize<ConfigData>(json, Context.Default.Options);
                if (config != null)
                {
                    lock (_lock)
                    {
                        _config = config;
                    }
                }
            }
            else
            {
                lock (_lock)
                {
                    _config = new ConfigData();
                }
                await SaveConfigAsync();
            }
        }
        catch
        {
            lock (_lock)
            {
                _config = new ConfigData();
            }
        }
    }

    private async Task SaveConfigAsync()
    {
        try
        {
            ConfigData configToSave;
            lock (_lock)
            {
                configToSave = _config;
            }

            var options = Context.Default.Options; // new JsonSerializerOptions { WriteIndented = true };
            options.WriteIndented = true;
            string json = JsonSerializer.Serialize(configToSave, options);
            await File.WriteAllTextAsync(_configPath, json);
        }
        catch
        {
            // Ignore save errors
        }
    }

    // 创建默认配置
    public async Task CreateDefaultConfigAsync()
    {
        if (!File.Exists(_configPath))
        {
            lock (_lock)
            {
                _config = new ConfigData();
            }
            await SaveConfigAsync();
        }
    }

    // 获取服务器API地址
    public string GetApiUrl()
    {
        lock (_lock)
        {
            return string.IsNullOrEmpty(_config.ApiUrl) ? DEFAULT_SERVER_API : _config.ApiUrl;
        }
    }

    // 设置服务器API地址
    public async Task SetApiUrlAsync(string apiUrl)
    {
        lock (_lock)
        {
            _config.ApiUrl = apiUrl;
        }
        await SaveConfigAsync();
    }

    // 是否加入排行榜
    public bool GetJoinRanking()
    {
        lock (_lock)
        {
            return _config.JoinRanking;
        }
    }

    // 设置是否加入排行榜
    public async Task SetJoinRankingAsync(bool joinRanking)
    {
        lock (_lock)
        {
            _config.JoinRanking = joinRanking;
        }
        await SaveConfigAsync();
    }

    // 获取开机运行设置
    public bool GetRunOnStartup()
    {
        lock (_lock)
        {
            return _config.RunOnStartup;
        }
    }

    // 设置开机运行
    public async Task SetRunOnStartupAsync(bool runOnStartup)
    {
        lock (_lock)
        {
            _config.RunOnStartup = runOnStartup;
        }
        await SaveConfigAsync();
        // Note: Actual startup registration would need to be platform-specific
    }

    // 获取数据刷新时间（分钟）
    public int GetSyncInterval()
    {
        lock (_lock)
        {
            return _config.SyncInterval > 0 ? _config.SyncInterval : 5;
        }
    }

    // 设置数据刷新时间（分钟）
    public async Task SetSyncIntervalAsync(int minutes)
    {
        lock (_lock)
        {
            _config.SyncInterval = minutes;
        }
        await SaveConfigAsync();
    }
}

internal class ConfigData
{
    public string ApiUrl { get; set; } = ConfigManager.DEFAULT_SERVER_API;
    public bool JoinRanking { get; set; }
    public bool RunOnStartup { get; set; }
    public int SyncInterval { get; set; } = 5;
}