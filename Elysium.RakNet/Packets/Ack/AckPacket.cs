using System.Buffers.Binary;
using System.Runtime.InteropServices;
using Elysium.RakNet.Attributes;
using Elysium.RakNet.Protocol;

namespace Elysium.RakNet.Packets.Ack;

[RakNetPacket(Packet.Ack)]
public class AckPacket : RakNetPacket
{
    public AckPacket(ReadOnlySpan<int> values)
    {
        SetRecords(values);
    }

    public IReadOnlyCollection<AckRecord> Records => _records.AsReadOnly();

    public List<AckRecord> _records { get; } = new();

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        Id = Packet.Ack;
        
        var count = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(Offset));
        Offset += 2;

        _records.Clear();
        _records.Capacity = count;

        var localOffset = 0;

        for (var i = 0; i < count; i++)
        {
            AckRecord record = default;

            record = record.Deserialize(
                buffer.Slice(Offset + localOffset),
                ref localOffset
            );

            _records.Add(record);
        }

        Offset += localOffset;
    }

    public override Span<byte> Serialize()
    {
        Array.Clear(RawData, 0, RawData.Length);

        Offset = 0;
        RawData[Offset++] = Id;

        var countOffset = Offset;
        Offset += 2;

        var recordCount = 0;

        for (var i = 0; i < Records.Count; i++)
        {
            var size = _records[i].Serialize(RawData.AsSpan(Offset));
            Offset += size;
            recordCount++;
        }

        if (recordCount > 0)
            BinaryPrimitives.WriteUInt16BigEndian(
                RawData.AsSpan(countOffset),
                (ushort)recordCount
            );

        Size = Offset;
        return RawData.AsSpan(0, Size);
    }

    public void SetRecords(ReadOnlySpan<int> values)
    {
        var sorted = values.ToArray();
        Array.Sort(sorted);

        var start = sorted[0];
        var last = start;

        for (var i = 1; i < sorted.Length; i++)
        {
            var current = sorted[i];

            if (current == last + 1)
            {
                last = current;
            }
            else
            {
                _records.Add(new AckRecord
                {
                    Low = start,
                    High = last
                });

                start = last = current;
            }
        }

        Size = Records.Count;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct AckRecord
{
    public int Low;
    public int High;

    public AckRecord Deserialize(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var isNotRange = buffer[0] != 0;

        Low = Helper.Helper.ReadUInt24LE(buffer.Slice(1));
        offset += 4;

        if (!isNotRange)
        {
            High = Helper.Helper.ReadUInt24LE(buffer.Slice(4));
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
            Helper.Helper.WriteUInt24LE(buffer.Slice(1), Low);
            return 4;
        }

        buffer[0] = 0;
        Helper.Helper.WriteUInt24LE(buffer.Slice(1), Low);
        Helper.Helper.WriteUInt24LE(buffer.Slice(4), High);
        return 7;
    }
}