using System;

namespace MouseClickCounter.Models;

/// <summary>
/// 鼠标点击数据模型
/// </summary>
public class ClickData
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 左键点击次数
    /// </summary>
    public long LeftClicks { get; set; }

    /// <summary>
    /// 右键点击次数
    /// </summary>
    public long RightClicks { get; set; }

    /// <summary>
    /// 是否加入排行榜
    /// </summary>
    public bool JoinRanking { get; set; }

    /// <summary>
    /// 数据时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 创建新的点击数据实例
    /// </summary>
    public ClickData()
    {
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// 转换为字符串表示
    /// </summary>
    public override string ToString()
    {
        return $"DeviceId:{DeviceId}\r\n" +
               $"DeviceName:{DeviceName}\r\n" +
               $"TotalLeftClicks:{LeftClicks}\r\n" +
               $"TotalRightClicks:{RightClicks}\r\n" +
               $"JoinRanking:{JoinRanking}\r\n" +
               $"Timestamp:{Timestamp:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// 从字符串解析点击数据
    /// </summary>
    public static ClickData FromString(string data)
    {
        var clickData = new ClickData();
        var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("DeviceId:"))
            {
                clickData.DeviceId = line.Substring("DeviceId:".Length).Trim();
            }
            else if (line.StartsWith("DeviceName:"))
            {
                clickData.DeviceName = line.Substring("DeviceName:".Length).Trim();
            }
            else if (line.StartsWith("TotalLeftClicks:"))
            {
                long.TryParse(line.Substring("TotalLeftClicks:".Length).Trim(), out var leftClicks);
                clickData.LeftClicks = leftClicks;
            }
            else if (line.StartsWith("TotalRightClicks:"))
            {
                long.TryParse(line.Substring("TotalRightClicks:".Length).Trim(), out var rightClicks);
                clickData.RightClicks = rightClicks;
            }
            else if (line.StartsWith("JoinRanking:"))
            {
                bool.TryParse(line.Substring("JoinRanking:".Length).Trim(), out var joinRanking);
                clickData.JoinRanking = joinRanking;
            }
            else if (line.StartsWith("Timestamp:"))
            {
                if (DateTime.TryParse(line.Substring("Timestamp:".Length).Trim(), out var timestamp))
                {
                    clickData.Timestamp = timestamp;
                }
            }
        }

        return clickData;
    }
}