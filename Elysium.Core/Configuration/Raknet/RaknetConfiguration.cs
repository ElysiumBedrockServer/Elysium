using System.Text.Json.Serialization;

namespace Elysium.Core.Configuration.Raknet;

public class RaknetConfiguration
{
    public string Address { get; set; } = "0.0.0.0";
    public int PortIpv4 { get; set; } = 19132;
    public int PortIpv6 { get; set; } = 19133;
    public int Protocol { get; set; } = 924;
    public Version Version { get; set; } = Version.Parse("1.26.0");
    public string Message { get; set; } = "Elysium";
    public int MaxConnections { get; set; } = 40;
    public int MtuMaxSize { get; set; } = 1492;
    public int MtuMinSize { get; set; } = 400;
    public bool ValidatePort { get; set; }
}