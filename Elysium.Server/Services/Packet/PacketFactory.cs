using System.Collections.Concurrent;
using Elysium.Core.Interfaces.Packet;
using Elysium.Core.Interfaces.Services.Packet;
using Elysium.Core.Interfaces.Services.Parser;

namespace Elysium.Server.Services.Packet;

public class PacketFactory : IPacketFactory
{
    private readonly ConcurrentDictionary<int, IOnlinePacketParser> _online;
    private readonly ConcurrentDictionary<int, IOfflinePacketParser> _offline;

    public PacketFactory(IEnumerable<IOnlinePacketParser> online, IEnumerable<IOfflinePacketParser> offline)
    {
        _online = new ConcurrentDictionary<int, IOnlinePacketParser>(
            online.ToDictionary(x => x.PacketId, y => y));
        
        _offline = new ConcurrentDictionary<int, IOfflinePacketParser>(
            offline.ToDictionary(x => x.PacketId, y => y));
    }
    
    public bool TryCreateOnlinePacket(int id, ReadOnlyMemory<byte> data, out Task<IOnlinePacket>? packetTask)
    {
        if (_online.TryGetValue(id, out var parser))
        {
            packetTask = parser.ParseAsync(data);
            return true;
        }

        packetTask = null;
        return false;
    }

    public bool TryCreateOfflinePacket(int id, ReadOnlyMemory<byte> data, out Task<IOfflinePacket>? packetTask)
    {
        if (_offline.TryGetValue(id, out var parser))
        {
            packetTask = parser.ParseAsync(data);
            return true;
        }

        packetTask = null;
        return false;
    }
}