using System.Buffers.Binary;
using System.Net;
using System.Text;
using Elysium.Core.Protocol;

namespace Elysium.Core.Binary;

public class RaknetBinaryWriter(byte[] buffer) {

    public byte[] Buffer { get; } = buffer;
    public int Position { get; set; }
    public byte[] ToArray() => Buffer[..Position];

    public void WriteByte(byte value) => Buffer[Position++] = value;
    public void WriteBoolean(bool value) => Buffer[Position++] = (byte)(value ? 1 : 0);
    
    public void Write(byte[] bytes) {
        
        if (Position + bytes.Length > Buffer.Length)
            throw new InvalidOperationException($"Not enough space in buffer to write {bytes.Length} bytes.");
        
        Array.Copy(bytes, 0, Buffer, Position, bytes.Length);
        Position += bytes.Length;
    }
    
    public void WriteShortLittleEndian(short value) {
        
        BinaryPrimitives.WriteInt16LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(short))], value);
        Position += sizeof(short);
    }

    public void WriteShortBigEndian(short value) {
        
        BinaryPrimitives.WriteInt16BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(short))], value);
        Position += sizeof(short);
    }

    public void WriteUnsignedShortLittleEndian(ushort value) {
        
        BinaryPrimitives.WriteUInt16LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(short))], value);
        Position += sizeof(short);
    }

    public void WriteUnsignedShortBigEndian(ushort value) {
        
        BinaryPrimitives.WriteUInt16BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(short))], value);
        Position += sizeof(short);
    }

    public void WriteFloatLittleEndian(float value) {
        
        BinaryPrimitives.WriteSingleLittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(float))], value);
        Position += sizeof(float);
    }
    
    public void WriteFloatBigEndian(float value) {
        
        BinaryPrimitives.WriteSingleBigEndian(Buffer.AsSpan()[Position..(Position + sizeof(float))], value);
        Position += sizeof(float);
    }

    public void WriteIntLittleEndian(int value) {
        
        BinaryPrimitives.WriteInt32LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(int))], value);
        Position += sizeof(int);
    }

    public void WriteIntBigEndian(int value) {
        
        BinaryPrimitives.WriteInt32BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(int))], value);
        Position += sizeof(int);
    }

    public void WriteUnsignedIntLittleEndian(uint value) {
        
        BinaryPrimitives.WriteUInt32LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(int))], value);
        Position += sizeof(int);
    }

    public void WriteUnsignedIntBigEndian(uint value) {
        
        BinaryPrimitives.WriteUInt32BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(int))], value);
        Position += sizeof(int);
    }

    public void WriteLongLittleEndian(long value) {
        
        BinaryPrimitives.WriteInt64LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(long))], value);
        Position += sizeof(long);
    }

    public void WriteLongBigEndian(long value) {
        
        BinaryPrimitives.WriteInt64BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(long))], value);
        Position += sizeof(long);
    }

    public void WriteUnsignedLongLittleEndian(ulong value) {
        
        BinaryPrimitives.WriteUInt64LittleEndian(Buffer.AsSpan()[Position..(Position + sizeof(long))], value);
        Position += sizeof(long);
    }

    public void WriteUnsignedLongBigEndian(ulong value) {
        
        BinaryPrimitives.WriteUInt64BigEndian(Buffer.AsSpan()[Position..(Position + sizeof(long))], value);
        Position += sizeof(long);
    }
    
    public void WriteTriadLittleEndian(int value) {
        
        if (value is < 0 or > 0xFFFFFF)
            throw new ArgumentOutOfRangeException(nameof(value), "Triad must be between 0 and 16777215 (0xFFFFFF).");

        if (Position + 3 > Buffer.Length)
            throw new InvalidOperationException("Write exceeds buffer size.");

        Buffer[Position++] = (byte)(value & 0xFF);
        Buffer[Position++] = (byte)((value >> 8) & 0xFF);
        Buffer[Position++] = (byte)((value >> 16) & 0xFF);
    }

    public void WriteString(string value) {
        
        var encoded = Encoding.UTF8.GetBytes(value);
        
        if (encoded.Length > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value), $"String too long (max {ushort.MaxValue} bytes).");
        
        WriteUnsignedShortBigEndian((ushort)encoded.Length);

        if (Position + encoded.Length > Buffer.Length)
            throw new InvalidOperationException("Not enough space in buffer to write string.");

        
        Array.Copy(encoded, 0, Buffer, Position, encoded.Length);
        Position += encoded.Length;
    }

    public void WriteMagic() {
        if (Position + MessageIdentifier.Magic.Length > Buffer.Length)
            throw new InvalidOperationException("Not enough space to write Magic.");
        
        Array.Copy(MessageIdentifier.Magic, 0, Buffer, Position, MessageIdentifier.Magic.Length);
        Position += MessageIdentifier.Magic.Length;
    }
    
    public void WriteIpEndPoint(IPEndPoint? endPoint) {
        
        switch (endPoint?.Address.AddressFamily) {
            
            case System.Net.Sockets.AddressFamily.InterNetwork: {
                
                WriteByte(4);
                var addressBytes = endPoint.Address.GetAddressBytes();
                
                foreach (var b in addressBytes) {
                    WriteByte((byte)~b);
                }

                WriteUnsignedShortBigEndian((ushort)endPoint.Port);
                break;
            }
            
            case System.Net.Sockets.AddressFamily.InterNetworkV6: {
                
                WriteByte(6); 
                WriteUnsignedShortBigEndian(23);
                
                WriteUnsignedShortBigEndian((ushort)endPoint.Port);
                WriteUnsignedLongBigEndian(0);

                var addressBytes = endPoint.Address.GetAddressBytes();
                if (addressBytes.Length != 16)
                    throw new ArgumentException("IPv6 address must be 16 bytes long");

                Write(addressBytes);
                break;
            }
            
            default:
                throw new InvalidOperationException("Unsupported address family");
        }
    }
    
    public void WriteVarUInt(ulong value) {
    
        while ((value & 0xFFFFFF80) != 0) {
            WriteByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }

        WriteByte((byte)(value & 0x7F));
    }

    public void WriteVarInt(int value) {
        
        do {
            
            var temp = (byte)(value & 0x7F);
            value >>= 7;

            if (value != 0) {
                temp |= 0x80;
            }

            buffer[Position++] = temp;
        } while (value != 0);
    }
    
    public void WriteVarUInt(long value) {
        
        while ((value & 0xFFFFFF80) != 0) {
            WriteByte((byte)((value & 0x7F) | 0x80));
            value >>= 7;
        }

        WriteByte((byte)(value & 0x7F));
    }
}