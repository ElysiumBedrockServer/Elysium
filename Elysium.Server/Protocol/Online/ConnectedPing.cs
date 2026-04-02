using Elysium.Core.Binary;
using Elysium.Core.Protocol;
using Elysium.Server.Base.Packet;

namespace Elysium.Server.Protocol.Online;

public class ConnectedPing : OnlinePacket
{
    public override int PacketId => MessageIdentifier.OnlineConnectedPing;
    
    public long Time { get; private set; }
    
    protected override void WritePayload(RaknetBinaryWriter writer)
        => writer.WriteLongBigEndian(Time);
    
    protected override void ReadPayload(RaknetBinaryReader reader)
        => reader.ReadLongBigEndian();

    protected override void WriteHeader(RaknetBinaryWriter writer)
        => writer.WriteByte((byte)PacketId);

    protected override void ReadHeader(RaknetBinaryReader reader)
    {
        var packetId = reader.ReadByte();
        
        if(packetId != (int)PacketId)
            throw new InvalidOperationException(string.Format("Invalid packet id:  {0}", packetId));
    }
    
    public static (ConnectedPing packet, byte[] buffer) Create(long time) {
        
        return OnlinePacket.Create<ConnectedPing>(packet => {
            packet.Time = time;
        });
    }
}