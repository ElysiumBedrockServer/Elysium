using Elysium.Core.Binary;
using Elysium.Core.Interfaces.Packet;
using Elysium.Core.Interfaces.Services.Parser;
using Elysium.RakNet.Base.Packet;
using Elysium.RakNet.Protocol;

namespace Elysium.RakNet.Packets.Online;

public class ConnectedPing : OnlinePacket
{
    public override int PacketId => OnlineIdentifiers.ConnectedPing;
    
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
    
    public class ConnectedPingParser : IOnlinePacketParser
    {
        public int PacketId => OnlineIdentifiers.ConnectedPing;

        public Task<IOnlinePacket> ParseAsync(ReadOnlyMemory<byte> data)
        {
            throw new NotImplementedException();
        }
    }
}