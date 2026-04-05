using Elysium.Core.Interfaces.Packet;

namespace Elysium.Core.Interfaces.Services.Packet;

public interface IPacketFactory
{
    public bool TryCreateOnlinePacket(int id, ReadOnlyMemory<byte> data, out Task<IOnlinePacket>? packetTask);
    public bool TryCreateOfflinePacket(int id, ReadOnlyMemory<byte> data, out Task<IOfflinePacket>? packetTask);
}