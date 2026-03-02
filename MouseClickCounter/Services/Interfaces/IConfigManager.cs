using System.Threading.Tasks;

namespace MouseClickCounter.Services.Interfaces
{
    public interface IConfigManager
    {
        Task InitializeAsync();
        Task CreateDefaultConfigAsync();
        string GetApiUrl();
        Task SetApiUrlAsync(string apiUrl);
        bool GetJoinRanking();
        Task SetJoinRankingAsync(bool joinRanking);
        bool GetRunOnStartup();
        Task SetRunOnStartupAsync(bool runOnStartup);
        int GetSyncInterval();
        Task SetSyncIntervalAsync(int minutes);
    }
}
