using System.Net;

namespace Elysium.RakNet.Store;

public class RakNetConnectionContext
{
    private CancellationTokenSource _cts = new();
    private Action<string>? _onAbort;
    private int _abortedFlag = 0;

    public RakNetConnectionContext(EndPoint endPoint, bool onlineMode)
    {
        Connection = new ConnectionContext(endPoint);
        OnlineMode = onlineMode;
        LastSeen = DateTime.UtcNow;
    }

    public ConnectionContext Connection { get; }
    public bool OnlineMode { get; }
    public DateTime LastSeen { get; set; }
    public CancellationToken ConnectionAborted => _cts.Token;
    public void UpdateLastSeen() => LastSeen = DateTime.UtcNow;

    public void SetAborted(Action<string> onAbort) => _onAbort = onAbort;

    public void Abort()
    {
        if (Interlocked.Exchange(ref _abortedFlag, 1) == 0)
        {
            _cts.Cancel();
            _onAbort?.Invoke(Connection.Id);
        }
    }
}