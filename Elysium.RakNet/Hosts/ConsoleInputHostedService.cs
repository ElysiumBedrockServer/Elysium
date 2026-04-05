using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elysium.RakNet.Hosts;

public class ConsoleInputHostedService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<ConsoleInputHostedService> _logger;
    private readonly CancellationTokenSource _cts = new();

    public ConsoleInputHostedService(IHostApplicationLifetime app, ILogger<ConsoleInputHostedService> logger)
    {
        _lifetime = app;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, _cts.Token);
        var token = linkedCts.Token;
        
        while (!token.IsCancellationRequested)
        {
            var input = await Console.In.ReadLineAsync(cancellationToken: token);

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if(!token.IsCancellationRequested)
                await HandleCommand(input);
        }
    }
    
    private async Task HandleCommand(string command)
    {
        var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        switch (args[0].ToLower())
        {
            case "stop":
                _logger.LogDebug("Stopping server...");
                _lifetime.StopApplication();
                _cts.Cancel();
                break;

            case "ping":
                _logger.LogInformation("pong");
                break;

            case "say":
                var message = string.Join(' ', args.Skip(1));
                _logger.LogInformation($"Broadcast: {message}");
                break;

            default:
                _logger.LogInformation($"❓ Unknown command: {command}");
                break;
        }
    }
}