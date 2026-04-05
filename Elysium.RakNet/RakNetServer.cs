using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Elysium.Core.Configuration;
using Elysium.Core.Configuration.Raknet;
using Elysium.RakNet.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elysium.RakNet;

public class RakNetServer : RakNetServerBase
{
    private Socket Socket { get; set; } = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private CancellationTokenSource _cts = new();
    
    public RakNetServer(ILogger<RakNetServer> logger, IOptions<ServerInfoConfiguration> config) : base(logger, config)
    {
       
    }

    public async Task InitializeAsync()
    {
        ApplyConfiguration(Config?.Raknet ?? new RaknetConfiguration());
        Logger.LogInformation(
            "Binding RakNet on {Address}:{Port} (IPv{Version})",
            ServerAddress?.Address,
            ServerAddress?.Port,
            ServerAddress?.Address.AddressFamily == AddressFamily.InterNetwork ? "4" : "6"
        );
    }

    public override async Task RunAsync(CancellationToken ctx)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ctx, _cts.Token);
        var token = linkedCts.Token;
        
        await InitializeAsync();
        Logger.LogDebug("Server started successfully");
        
        Socket.EnableBroadcast = true;
        Socket.Bind(ServerAddress!);
        
        Logger.LogDebug("Starting Server - {Name}", Config.Name);

        while (!token.IsCancellationRequested)
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
                    token
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

    

    public async Task StopAsync()
    {
        await _cts.CancelAsync();

        try
        {
            Socket.Close();
            Socket.Dispose();
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error while closing socket");
        }
    }
}