using System.Text.Json.Serialization;

namespace MouseClickCounter.Models;

internal class SyncDataRequest
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; } = string.Empty;

    [JsonPropertyName("deviceName")]
    public string DeviceName { get; set; } = string.Empty;

    [JsonPropertyName("leftClicks")]
    public long LeftClicks { get; set; }

    [JsonPropertyName("rightClicks")]
    public long RightClicks { get; set; }
}