using System.Net;
using System.Net.Sockets;
using Elysium.Core.Configuration;
using Elysium.Core.Configuration.Raknet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elysium.RakNet.Base;

public abstract class RakNetServerBase
{
    protected IPEndPoint? ServerAddress { get; set; }
    protected ServerInfoConfiguration Config { get; private set; }
    protected ILogger<RakNetServer> Logger { get; set; }

    public RakNetServerBase(ILogger<RakNetServer> logger, IOptions<ServerInfoConfiguration> config)
    {
        Logger = logger;
        Config = config.Value;
    }

    public void ApplyConfiguration(RaknetConfiguration config)
    {
        ServerAddress = ConvertAddress(config.Address, config.PortIpv4, config.PortIpv6);
    }

    private IPEndPoint ConvertAddress(string address, int ipv4, int ipv6)
    {
        if (!IPAddress.TryParse(address, out var value))
            value = IPAddress.Any;
        
        var port = value.AddressFamily == AddressFamily.InterNetwork ?
            ipv4 : ipv6;
        
        return new IPEndPoint(value, port);
    }
    
    public abstract Task RunAsync(CancellationToken ctx);
}