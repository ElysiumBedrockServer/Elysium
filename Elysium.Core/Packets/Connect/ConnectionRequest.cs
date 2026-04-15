using Elysium.Core.Attributes;
using Elysium.Core.Extensions;
using Elysium.Core.Protocol;

namespace Elysium.Core.Packets.Connect;

[RakNetPacket(Packet.ConnectionRequest)]
public class ConnectionRequest : RakNetPacket
{
    public long ClientGuid;
    public long Time;
    public bool UseSecurity;

    public ConnectionRequest()
    {
        Id = Packet.ConnectionRequest;
        Size = 1 + 2 * sizeof(long) + 1;
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        ClientGuid = buffer.ReadInt64BigEndian(ref Offset);
        Time = buffer.ReadInt64BigEndian(ref Offset);
        UseSecurity = buffer.ReadBool(ref Offset);
    }

    public override Span<byte> Serialize()
    {
        var buffer = RawData;
        Offset = 1;

        buffer.WriteBigEndian(ref Offset, ClientGuid);
        buffer.WriteBigEndian(ref Offset, Time);
        buffer.Write(ref Offset, UseSecurity);

        return buffer;
    }
}