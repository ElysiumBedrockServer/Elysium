using System.Text.Json.Serialization;
using Elysium.Core.Configuration.Raknet;
using Elysium.Core.Configuration.WorldDefaultSettings;

namespace Elysium.Core.Configuration;

public class ServerInfoConfiguration
{
    public string Edition { get; set; } = "MCBE";
    public string Name { get; set; } = "MyServer";
    public string Motd { get; set; } = "";
    
    public RaknetConfiguration Raknet { get; set; } = new();

    public WorldDefaultSettingsConfiguration WorldDefaultSettings { get; set; } = new();
}