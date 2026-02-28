using System;
using System.IO;
using System.Text.Json;

namespace MouseClickCounter.Services
{
    public class ConfigManager
    {
        private string _configPath;
        private const string DEFAULT_SERVER_API = "https://exeekfa1x7.sealosbja.site";
        private ConfigData _config = new();

        public ConfigManager()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    _config = JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
                }
                else
                {
                    _config = new ConfigData();
                    SaveConfig();
                }
            }
            catch
            {
                _config = new ConfigData();
            }
        }

        private void SaveConfig()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_config, options);
                File.WriteAllText(_configPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }

        // 创建默认配置
        public void CreateDefaultConfig()
        {
            if (!File.Exists(_configPath))
            {
                _config = new ConfigData();
                SaveConfig();
            }
        }

        // 获取服务器API地址
        public string GetApiUrl()
        {
            return string.IsNullOrEmpty(_config.ApiUrl) ? DEFAULT_SERVER_API : _config.ApiUrl;
        }

        // 设置服务器API地址
        public void SetApiUrl(string apiUrl)
        {
            _config.ApiUrl = apiUrl;
            SaveConfig();
        }

        // 是否加入排行榜
        public bool GetJoinRanking()
        {
            return _config.JoinRanking;
        }

        // 设置是否加入排行榜
        public void SetJoinRanking(bool joinRanking)
        {
            _config.JoinRanking = joinRanking;
            SaveConfig();
        }

        // 获取开机运行设置
        public bool GetRunOnStartup()
        {
            return _config.RunOnStartup;
        }

        // 设置开机运行
        public void SetRunOnStartup(bool runOnStartup)
        {
            _config.RunOnStartup = runOnStartup;
            SaveConfig();
            // Note: Actual startup registration would need to be platform-specific
        }

        // 获取数据刷新时间（分钟）
        public int GetSyncInterval()
        {
            return _config.SyncInterval > 0 ? _config.SyncInterval : 5;
        }

        // 设置数据刷新时间（分钟）
        public void SetSyncInterval(int minutes)
        {
            _config.SyncInterval = minutes;
            SaveConfig();
        }

        private class ConfigData
        {
            public string ApiUrl { get; set; } = DEFAULT_SERVER_API;
            public bool JoinRanking { get; set; } = false;
            public bool RunOnStartup { get; set; } = false;
            public int SyncInterval { get; set; } = 5;
        }
    }
}
