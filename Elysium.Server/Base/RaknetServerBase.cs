using System.Net;
using System.Net.Sockets;
using Autofac;
using Elysium.Core.Configuration.Raknet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elysium.Server.Base;

public abstract class RaknetServerBase
{
    protected IPEndPoint? ServerAddress { get; set; }
    protected ILogger Logger { get; set; }
    protected IContainer Services { get; private set; }
    protected IConfiguration Configuration { get; private set; }

    public RaknetServerBase(IContainer services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
        Logger = Services.Resolve<ILoggerFactory>()
            .CreateLogger("Raknet Server");
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
    
    public abstract Task RunAsync();
}