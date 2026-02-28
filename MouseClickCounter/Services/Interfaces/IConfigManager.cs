namespace MouseClickCounter.Services.Interfaces
{
    public interface IConfigManager
    {
        void CreateDefaultConfig();
        string GetApiUrl();
        void SetApiUrl(string apiUrl);
        bool GetJoinRanking();
        void SetJoinRanking(bool joinRanking);
        bool GetRunOnStartup();
        void SetRunOnStartup(bool runOnStartup);
        int GetSyncInterval();
        void SetSyncInterval(int minutes);
    }
}
