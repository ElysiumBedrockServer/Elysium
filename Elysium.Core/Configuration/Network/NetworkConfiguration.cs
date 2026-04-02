using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Network;

public class NetworkConfiguration
{
    public int CompressionMethod { get; set; } = 0;
    
    public int CompressionThreshold { get; set; } = 256;
    
    public bool FrameMonitoring { get; set; } = true;
    
    public int PacketsPerFrame { get; set; } = 64;
}