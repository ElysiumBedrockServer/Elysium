namespace Elysium.Core.Configuration;

public class PacketConfig
{
    public PacketConfig(Type packetType, byte packetId)
    {
        PacketType = packetType;
        PacketId = packetId;
    }

    public byte PacketId { get; set; }
    public Type PacketType { get; private set; }
}