using System.Threading.Tasks;
using MouseClickCounter.Models;

namespace MouseClickCounter.Services.Interfaces;

public interface IRankingApiService
{
    Task SyncToRankingServer(ClickData clickData);
    void ResetLastSyncedClicks();
    void SetLastSyncedClicks(long leftClicks, long rightClicks);
    Task<RankingData?> GetDeviceRanking(string deviceId);
    Task<bool> CheckServerConnection();
    Task<ProvinceRankingResponse?> GetAllProvinceRanking();
}