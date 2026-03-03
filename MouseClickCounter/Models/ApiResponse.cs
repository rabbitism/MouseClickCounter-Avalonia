using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

/// <summary>
/// API基础响应
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// 响应代码（0表示成功）
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}