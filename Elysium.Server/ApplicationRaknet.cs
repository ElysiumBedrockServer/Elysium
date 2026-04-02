using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Autofac;
using Elysium.Core.Configuration;
using Elysium.Core.Configuration.Raknet;
using Elysium.Server.Base;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elysium.Server;

public class ApplicationRaknet : RaknetServerBase
{
    private readonly string[] _args;
    private Socket Socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private CancellationTokenSource _cts = new();
    private ServerInfoConfiguration? ServerConfig { get; set; }
    
    
    public ApplicationRaknet(IContainer container, IConfiguration configuration, string[] args) : base(container, configuration)
    {
        _args = args;
        ServerConfig = Configuration
            .GetSection("Server")
            .Get<ServerInfoConfiguration>();
    }

    public async Task InitializeAsync()
    {
        ApplyConfiguration(ServerConfig?.Raknet ?? new RaknetConfiguration());
        Logger.LogInformation(
            "Binding RakNet on {Address}:{Port} (IPv{Version})",
            ServerAddress?.Address,
            ServerAddress?.Port,
            ServerAddress?.Address.AddressFamily == AddressFamily.InterNetwork ? "4" : "6"
        );
    }

    public override async Task RunAsync()
    {
        Logger.LogDebug("Initializing RakNet server...");
        await InitializeAsync();
        Logger.LogInformation("Server started successfully");
        
        Socket.EnableBroadcast = true;
        Socket.Bind(ServerAddress!);
        
        Logger.LogInformation("Starting Server - {Name}", ServerConfig.Name);

        while (!_cts.Token.IsCancellationRequested)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(8192);

            try
            {
                var sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEndPoint = sender;

                var received = await Socket.ReceiveFromAsync(
                    buffer,
                    SocketFlags.None,
                    remoteEndPoint,
                    _cts.Token
                );

                
                var length = received.ReceivedBytes;
                var clientIp = (IPEndPoint)received.RemoteEndPoint;
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);                
            }
        }
    }

    public Task StopAsync()
    {
        Logger.LogInformation("Stopping Server - {Name}", ServerConfig.Name);
        return Task.CompletedTask;
    }
}