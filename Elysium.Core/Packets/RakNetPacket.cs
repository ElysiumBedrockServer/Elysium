using System.Buffers;
using Elysium.Core.Extensions;

namespace Elysium.Core.Packets;

public class RakNetPacket : IDisposable
{
    public const int MaxSizeMtu = 1492;

    private byte[]? _buffer;
    private bool _disposed;

    protected int Offset;

    public RakNetPacket()
    {
        _buffer = ArrayPool<byte>.Shared.Rent(MaxSizeMtu);
    }

    public byte Id { get; protected set; }

    public Span<byte> RawData
    {
        get
        {
            if (_buffer == null)
                throw new ObjectDisposedException(nameof(RakNetPacket));

            return _buffer;
        }
    }


    public int Size { get; set; }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_buffer != null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
        }
            

        _disposed = true;
    }

    public virtual Span<byte> Serialize()
    {
        return RawData[..Size];
    }

    public virtual void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 0;

        Id = buffer.ReadUInt8(ref Offset);
        Size = buffer.Length;

        buffer.CopyTo(RawData);
    }

    public T Parse<T>() where T : RakNetPacket, new()
    {
        var packet = new T();

        packet.Deserialize(RawData);

        return packet;
    }
}