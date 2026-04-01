using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration;

public class ServerInfoConfiguration
{
    [JsonPropertyName("edition")] public string Edition { get; set; } = "MCBE";
    [JsonPropertyName("name")] public string Name { get; set; } = "MyServer";
    [JsonPropertyName("motd")] public string Motd { get; set; } = "";
    [JsonPropertyName("raknet")] public RaknetConfiguration Raknet { get; set; } = new();
}



