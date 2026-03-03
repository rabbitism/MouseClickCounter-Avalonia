using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

[JsonSerializable(typeof(ProvinceRankingResponse))]
[JsonSerializable(typeof(ProvinceRankingItem))]
[JsonSerializable(typeof(ApiResponse<RankingData>))]
[JsonSerializable(typeof(ApiResponse<ProvinceRankingResponse>))]
[JsonSerializable(typeof(RankingData))]
[JsonSerializable(typeof(ConfigData))]
[JsonSerializable(typeof(SyncDataRequest))]
internal partial class Context : JsonSerializerContext
{
    
}