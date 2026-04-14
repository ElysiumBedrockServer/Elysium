using Elysium.Core.Interfaces.Packet;

namespace Elysium.Core.Interfaces.Services.Parser;

public interface IPacketParser
{
    int PacketId { get; }
}