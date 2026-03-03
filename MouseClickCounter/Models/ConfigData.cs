using MouseClickCounter.Services;

namespace MouseClickCounter.Models;

internal class ConfigData
{
    public string ApiUrl { get; set; } = ConfigManager.DEFAULT_SERVER_API;
    public bool JoinRanking { get; set; }
    public bool RunOnStartup { get; set; }
    public int SyncInterval { get; set; } = 5;
}