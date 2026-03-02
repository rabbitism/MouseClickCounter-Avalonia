using System.Text.Json.Serialization;

namespace MouseClickCounter.Services;

[JsonSerializable(typeof(ProvinceRankingResponse))]
[JsonSerializable(typeof(ProvinceRankingItem))]
[JsonSerializable(typeof(ApiResponse<RankingData>))]
[JsonSerializable(typeof(ApiResponse<ProvinceRankingResponse>))]
[JsonSerializable(typeof(RankingData))]
[JsonSerializable(typeof(ConfigData))]
public partial class Context : JsonSerializerContext
{
    
}