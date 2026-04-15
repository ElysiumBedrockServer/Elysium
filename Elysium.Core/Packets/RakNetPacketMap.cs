using System.Collections.ObjectModel;
using Elysium.Core.Configuration;

namespace Elysium.Core.Packets;

public class RakNetPacketMap
{
    private readonly ReadOnlyDictionary<byte, Type> _packetTypes;

    public RakNetPacketMap(IEnumerable<PacketConfig> configs)
    {
        _packetTypes = new ReadOnlyDictionary<byte, Type>(configs.ToDictionary(x => x.PacketId, y => y.PacketType));
    }

    public Type? GetPacketType(byte id)
    {
        _packetTypes.TryGetValue(id, out var type);

        return type;
    }
}