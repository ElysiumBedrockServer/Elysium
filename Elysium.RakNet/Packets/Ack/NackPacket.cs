using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Elysium.RakNet.Attributes;
using Elysium.RakNet.Protocol;

namespace Elysium.RakNet.Packets.Ack;

[RakNetPacket(Packet.Nack)]
public class NackPacket : RakNetPacket
{
    public IReadOnlyCollection<NackRecord> Records => _records;
    private List<NackRecord> _records { get; } = new();

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        Id = Packet.Nack;

        var count = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(Offset));
        Offset += 2;
        
        _records.Clear();
        _records.Capacity = count;
        
        var localOffSet = 0;

        for (var i = 0; i < count; i++)
        {
            NackRecord record = default;

            record = record.Deserialize(
                buffer.Slice(Offset + localOffSet),
                ref localOffSet
            );
            
            _records.Add(record);
        }

        Offset += localOffSet;
    }

    public override Span<byte> Serialize()
    {
        Array.Clear(RawData, 0, RawData.Length);
        Offset = 0;

        RawData[Offset++] = Id;

        var countOffSet = Offset;
        var recordCount = 0;

        for (int i = 0; i < Records.Count; i++)
        {
            var size = _records[i].Serialize(RawData.AsSpan(Offset));
            Offset += size;
            recordCount++;
        }
        
        if(recordCount > 0)
            BinaryPrimitives.WriteUInt16BigEndian(
                RawData.AsSpan(countOffSet),
                (ushort)recordCount);

        Size = Offset;
        
        return RawData.AsSpan(0, Size);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct NackRecord
{
    public int Low;
    public int High;

    public NackRecord Deserialize(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var isNotRange = buffer[0] != 0;

        Low = RakNet.Helper.Helper.ReadUInt24LE(buffer.Slice(1));
        offset += 4;

        if (!isNotRange)
        {
            High = RakNet.Helper.Helper.ReadUInt24LE(buffer.Slice(4));
            offset += 3;
        }
        else
        {
            High = Low;
        }

        return this;
    }

    public int Serialize(Span<byte> buffer)
    {
        if (Low == High)
        {
            buffer[0] = 1;
            RakNet.Helper.Helper.WriteUInt24LE(buffer.Slice(1), Low);
            return 4;
        }

        buffer[0] = 0;
        RakNet.Helper.Helper.WriteUInt24LE(buffer.Slice(1), Low);
        RakNet.Helper.Helper.WriteUInt24LE(buffer.Slice(4), High);
        return 7;
    }
}