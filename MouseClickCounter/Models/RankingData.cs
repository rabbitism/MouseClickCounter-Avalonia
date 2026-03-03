using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

/// <summary>
/// 排行榜数据
/// </summary>
public class RankingData
{
    /// <summary>
    /// 排名（可能是字符串如"10000+"）
    /// </summary>
    [JsonPropertyName("rank")]
    public string Rank { get; set; } = string.Empty;

    /// <summary>
    /// 左键点击次数
    /// </summary>
    [JsonPropertyName("leftClicks")]
    public long LeftClicks { get; set; }

    /// <summary>
    /// 右键点击次数
    /// </summary>
    [JsonPropertyName("rightClicks")]
    public long RightClicks { get; set; }
}