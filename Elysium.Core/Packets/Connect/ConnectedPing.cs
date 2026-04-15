using Elysium.Core.Attributes;
using Elysium.Core.Extensions;
using Elysium.Core.Protocol;

namespace Elysium.Core.Packets.Connect;

[RakNetPacket(Packet.ConnectedPing)]
public class ConnectedPing : RakNetPacket
{
    public ConnectedPing()
    {
        Id = Packet.ConnectedPing;
        Size = 1 + sizeof(long);
    }

    public long Time { get; set; }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        Time = buffer.ReadInt64BigEndian(ref Offset);
    }

    public override Span<byte> Serialize()
    {
        var buffer = RawData.Slice(0, Size);
        buffer[0] = Id;
        Offset = 1;

        buffer.WriteBigEndian(ref Offset, Time);

        return buffer;
    }
}