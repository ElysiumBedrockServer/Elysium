using Elysium.Core.Interfaces.Packet;

namespace Elysium.Core.Interfaces.Services.Parser;

public interface IPacketParser
{
    int PacketId { get; }
}

public interface IOnlinePacketParser : IPacketParser
{
    Task<IOnlinePacket> ParseAsync(ReadOnlyMemory<byte> data);
}

public interface IOfflinePacketParser : IPacketParser
{ 
    Task<IOfflinePacket> ParseAsync(ReadOnlyMemory<byte> data);
}