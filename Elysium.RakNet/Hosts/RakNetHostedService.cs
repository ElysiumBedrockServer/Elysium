using Elysium.RakNet.Base;
using Elysium.RakNet.Sockets;
using Microsoft.Extensions.Logging;

namespace Elysium.RakNet.Hosts;

public class RakNetHostedService : RakNetHostBase
{
    private Task _executingTask;
    
    private readonly ILogger<RakNetHostedService> _logger;
    private readonly IRakNetSocket _socket;

    public RakNetHostedService(ILogger<RakNetHostedService> logger, IRakNetSocket socket)
    {
        _logger = logger;
        _socket = socket;
    }

    public override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Initializing RakNet server...");

        if (_socket.Start())
            while (!_socket.Token?.IsCancellationRequested ?? false || !cancellationToken.IsCancellationRequested);
        else
            _logger.LogError("RakNet server failed to start.");

        return Task.CompletedTask;
    }

    public override Task StopAsync()
    {
        _socket.Stop();
        return Task.CompletedTask;
    }
}