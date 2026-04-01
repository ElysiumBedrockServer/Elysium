using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration;

public class NetworkConfiguration
{
    [JsonPropertyName("compressionMethod")]
    public int CompressionMethod { get; set; } = 0;
    
    [JsonPropertyName("compressionThreshold")]
    public int CompressionThreshold { get; set; } = 256;
    
    [JsonPropertyName("frameMonitoring")]
    public bool FrameMonitoring { get; set; } = true;
    
    [JsonPropertyName("packetsPerFrame")]
    public int PacketsPerFrame { get; set; } = 64;
}