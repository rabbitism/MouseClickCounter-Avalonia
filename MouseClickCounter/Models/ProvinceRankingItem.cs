using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

/// <summary>
/// 省份排行数据
/// </summary>
public class ProvinceRankingItem
{
    /// <summary>
    /// 省份名称
    /// </summary>
    [JsonPropertyName("province")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 设备总数
    /// </summary>
    [JsonPropertyName("deviceCount")]
    public int DeviceCount { get; set; }

    /// <summary>
    /// 总点击次数
    /// </summary>
    [JsonIgnore]
    public long TotalClicks { get { return TotalLeftClicks + TotalRightClicks; } }

    /// <summary>
    /// 左键点击次数
    /// </summary>
    [JsonPropertyName("totalLeftClicks")]
    public long TotalLeftClicks { get; set; }

    /// <summary>
    /// 右键点击次数
    /// </summary>
    [JsonPropertyName("totalRightClicks")]
    public long TotalRightClicks { get; set; }

    /// <summary>
    /// 排名
    /// </summary>
    [JsonPropertyName("rank")]
    public int Rank { get; set; }
}