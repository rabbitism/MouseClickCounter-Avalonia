using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

/// <summary>
/// 省份排行响应
/// </summary>
public class ProvinceRankingResponse
{
    /// <summary>
    /// 省份排行列表
    /// </summary>
    [JsonPropertyName("nationalRankList")]
    public List<ProvinceRankingItem> NationalRankList { get; set; } = [];

    /// <summary>
    /// 总参与设备数
    /// </summary>
    [JsonPropertyName("totalDevices")]
    public int TotalDevices { get; set; }

    /// <summary>
    /// 总点击次数
    /// </summary>
    [JsonPropertyName("totalLeftClicks")]
    public long TotalLeftClicks { get; set; }

    /// <summary>
    /// 总点击次数
    /// </summary>
    [JsonPropertyName("totalRightClicks")]
    public long TotalRightClicks { get; set; }

    /// <summary>
    /// 总点击次数
    /// </summary>
    [JsonIgnore]
    public long TotalClicks { get { return TotalLeftClicks + TotalRightClicks; } }
}