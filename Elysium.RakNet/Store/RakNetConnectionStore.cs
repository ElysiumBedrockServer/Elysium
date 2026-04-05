using System.Collections.Concurrent;
using System.Net;

namespace Elysium.RakNet.Store;

public class RakNetConnectionStore
{
    private readonly ConcurrentDictionary<string, RakNetConnectionContext> _connections = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _identifiers = new(StringComparer.Ordinal);
    
    public IEnumerable<RakNetConnectionContext> All => _connections.Values;
    
    public int Count => _connections.Count;
    
    public bool Contains(Guid connectionId)
        => _connections.ContainsKey(connectionId.ToString());

    public bool Contains(EndPoint endPoint)
        => _identifiers.ContainsKey(endPoint.ToString()!);
    
    public RakNetConnectionContext? this[string key]
    {
        get
        {
            if(_connections.TryGetValue(key, out var connection))
                connection.LastSeen = DateTime.UtcNow;
            
            return connection;
        }
    }
    
    public DateTime GetLastConnection(string connectionId)
        => _connections.TryGetValue(connectionId, out var connection) ? connection.LastSeen : DateTime.MinValue;
    
    public void CreateConnection(EndPoint endPoint, bool onlineMode)
    {
        var context = new RakNetConnectionContext(endPoint, onlineMode);
        
        context.SetAborted(x => Remove(x));
        
        _connections.TryAdd(context.Connection.Id, context);
        _identifiers.AddOrUpdate(context.Connection.Ip, context.Connection.Id, (_, __) => context.Connection.Id);
    }

    private bool Remove(string connectionId)
    {
        return _connections.TryRemove(connectionId, out var _) && _identifiers.TryRemove(connectionId, out var _);
    }
    
    public void CleanupInactive(TimeSpan timeout)
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _connections)
        {
            if (now - kvp.Value.LastSeen > timeout)
            {
                if (_connections.TryGetValue(kvp.Key, out var connection))
                    connection.Abort();
            }
        }
    }
}