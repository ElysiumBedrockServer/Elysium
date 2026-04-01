using System.Net.Sockets;
using Autofac;
using Elysium.Core.Configuration;
using Microsoft.Extensions.Configuration;

namespace Elysium.Server.Server;

public class Raknet
{
    private readonly IContainer _container;
    private readonly string[] _args;

    public IConfiguration Configuration { get; private set; }
    
   
    internal Socket Socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

    public Raknet(IContainer container, IConfiguration configuration, string[] args)
    {
        Configuration = configuration;
        _container = container;
        _args = args;
    }

    public async Task RunAsync()
    {
        var config = Configuration.GetValue<ServerInfoConfiguration>("Server");
        
        
    }
}