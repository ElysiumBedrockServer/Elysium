using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration;

public class RaknetConfiguration
{
    [JsonPropertyName("address")] public string Address { get; set; } = "0.0.0.0";
    [JsonPropertyName("portIPV4")] public int PortIpv4 { get; set; } = 19132;
    [JsonPropertyName("portIPV6")] public int PortIpv6 { get; set; } = 19133;
    [JsonPropertyName("protocol")] public int Protocol { get; set; } = 924;
    [JsonPropertyName("version")] public int Version { get; set; } = 818;
    [JsonPropertyName("message")] public string Message { get; set; } = "Elysium";
    [JsonPropertyName("maxConnections")] public int MaxConnections { get; set; } = 40;
    [JsonPropertyName("mtuMaxSize")] public int MtuMaxSize { get; set; } = 1492;
    [JsonPropertyName("mtuMinSize")] public int MtuMinSize { get; set; } = 400;
    [JsonPropertyName("validatePort")] public bool ValidatePort { get; set; }
}