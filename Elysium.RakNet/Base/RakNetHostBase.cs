using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;

namespace Elysium.RakNet.Base;

public abstract class RakNetHostBase : IHostedService
{
    private CancellationTokenSource _cts = new();
    private Task _executingTask;
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _executingTask = Task.Run(() => ExecuteAsync(_cts.Token));
        
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _cts.Dispose();
        
        await StopAsync();
    }
    
    public abstract Task ExecuteAsync(CancellationToken cancellationToken);
    
    public virtual Task StopAsync()
        => Task.CompletedTask;
}