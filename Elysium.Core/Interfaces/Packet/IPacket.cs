using Elysium.Core.Binary;

namespace Elysium.Core.Interfaces.Packet;

public interface IPacket
{
    public int PacketId { get; }
    
    public void Write(RaknetBinaryWriter writer);
    public void Read(RaknetBinaryReader reader);
}