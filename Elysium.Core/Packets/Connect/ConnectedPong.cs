using Elysium.Core.Attributes;
using Elysium.Core.Extensions;
using Elysium.Core.Protocol;

namespace Elysium.Core.Packets.Connect;

[RakNetPacket(Packet.ConnectedPong)]
public class ConnectedPong : RakNetPacket
{
    public ConnectedPong()
    {
        Id = Packet.ConnectedPong;
        Size = 1 + 2 * sizeof(long);
    }

    public long PingTime { get; set; }
    public long PongTime { get; set; }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        PingTime = buffer.ReadInt64BigEndian(ref Offset);
        PongTime = buffer.ReadInt64BigEndian(ref Offset);
    }

    public override Span<byte> Serialize()
    {
        var buffer = RawData;

        buffer[0] = Id;
        Offset = 1;

        buffer.WriteBigEndian(ref Offset, PingTime);
        buffer.WriteBigEndian(ref Offset, PongTime);

        return buffer.Slice(0, Size);
    }
}