using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using Elysium.RakNet.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Elysium.Server.Hosts;

public class ConsoleInputHostedService : RakNetHostBase
{
    private readonly CancellationTokenSource _cts = new();

    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<ConsoleInputHostedService> _logger;
    private Task _executingTask;

    public ConsoleInputHostedService(IHostApplicationLifetime app, ILogger<ConsoleInputHostedService> logger)
    {
        _lifetime = app;
        _logger = logger;
    }

    private async Task HandleCommand(string command)
    {
        var args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        using var client = new UdpClient();

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

            case "test":
                var data = Encoding.UTF8.GetBytes("Hello RakNetServer!");

                var serverIp = "127.0.0.1";
                var serverPort = 19132;

                var sw = Stopwatch.StartNew();

                for (var i = 0; i < 1000; i++) await client.SendAsync(data, data.Length, serverIp, serverPort);

                sw.Stop();
                Console.WriteLine($"Sent 1000 messages in {sw.ElapsedMilliseconds} ms");

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


    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _cts.Dispose();

        return Task.CompletedTask;
    }

    public override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var input = await Console.In.ReadLineAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (!cancellationToken.IsCancellationRequested)
                await HandleCommand(input);
        }
    }
}