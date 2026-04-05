using System.Buffers;
using Elysium.Core.Binary;
using Elysium.Core.Interfaces.Packet;

namespace Elysium.RakNet.Base.Packet;

public abstract class PacketBase : IPacket
{
    public abstract int PacketId { get; }
    
    public void Write(RaknetBinaryWriter writer)
    {
        WriteHeader(writer);
        WritePayload(writer);
    }

    public void Read(RaknetBinaryReader reader)
    {
        ReadHeader(reader);
        ReadPayload(reader);
    }

    protected abstract void WritePayload(RaknetBinaryWriter writer);

    protected abstract void ReadPayload(RaknetBinaryReader reader);

    protected abstract void WriteHeader(RaknetBinaryWriter writer);

    protected abstract void ReadHeader(RaknetBinaryReader reader);
    
    
    protected static (T packet, byte[] buffer) Create<T>(Action<T> initialize) where T : IPacket, new() {
        var packet = new T();
        initialize(packet);
        
        var tempBuffer = ArrayPool<byte>.Shared.Rent(1492);
        try {
            var writer = new RaknetBinaryWriter(tempBuffer);
            packet.Write(writer);
            
            var buffer = new byte[writer.Position];
            Array.Copy(tempBuffer, buffer, writer.Position);
            
            return (packet, buffer);
        } finally {
            ArrayPool<byte>.Shared.Return(tempBuffer);
        }
    }
}