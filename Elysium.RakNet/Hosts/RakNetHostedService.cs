using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elysium.RakNet.Hosts;

public class RakNetHostedService : BackgroundService
{
    private readonly RakNetServer _server;
    private readonly ILogger<RakNetHostedService> _logger;

    public RakNetHostedService(RakNetServer server, ILogger<RakNetHostedService> logger)
    {
        _server = server;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Initializing RakNet server...");
        await _server.RunAsync(stoppingToken);
    }

    public override async void Dispose()
    {
        await _server.StopAsync();
        base.Dispose();
    }
}