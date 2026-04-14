using Elysium.RakNet.Extensions;

namespace Elysium.RakNet.Packets;

public class RakNetPacket
{
    public const int MaxSizeMtu = 1492;

    public byte Id { get; protected set; }

    public byte[] RawData { get; set; } = new byte[MaxSizeMtu];

    public int Size { get; set; }

    public virtual Span<byte> Serialize()
    {
        return RawData;
    }

    public virtual void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Id = buffer.ReadUInt8(ref Offset);
        Size = buffer.Length;
    }

    public T Parse<T>() where T : RakNetPacket, new()
    {
        var packet = new T();

        packet.Deserialize(RawData);

        return packet;
    }

    protected int Offset;
}