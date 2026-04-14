using Elysium.Core.Interfaces.Packet;

namespace Elysium.Core.Interfaces.Services.Packet;

public interface IPacketFactory
{
    public bool TryCreatePacket(int id, ReadOnlyMemory<byte> data, out Task<IOnlinePacket>? packetTask);
}