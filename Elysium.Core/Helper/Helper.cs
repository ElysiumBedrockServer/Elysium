using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Elysium.Core.Extensions;

namespace Elysium.Core.Helper;

public static class Helper
{
    public const int MAGIC_LENGTH = 16;

    public static readonly byte[] MAGIC =
        [0x00, 0xff, 0xff, 0x00, 0xfe, 0xfe, 0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0x12, 0x34, 0x56, 0x78];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteUInt24LE(Span<byte> buffer, int value)
    {
        buffer[0] = (byte)value;
        buffer[1] = (byte)(value >> 8);
        buffer[2] = (byte)(value >> 16);
        return 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteUInt24BE(Span<byte> buffer, int value)
    {
        buffer[2] = (byte)value;
        buffer[1] = (byte)(value >> 8);
        buffer[0] = (byte)(value >> 16);
        return 3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadUInt24LE(ReadOnlySpan<byte> buffer)
    {
        var value = 0;
        value |= buffer[0];
        value |= buffer[1] << 8;
        value |= buffer[2] << 16;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadUInt24BE(ReadOnlySpan<byte> buffer)
    {
        var value = 0;
        value |= buffer[2];
        value |= buffer[1] << 8;
        value |= buffer[0] << 16;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyMagicTo(Span<byte> buffer)
    {
        MAGIC.AsSpan().CopyTo(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteString16(Span<byte> buffer, ReadOnlySpan<char> value, Encoding? data = null)
    {
        var writen = (data ?? Encoding.UTF8).GetBytes(value, buffer.Slice(2));
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)writen);
        return writen + 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadString16(ReadOnlySpan<byte> buffer, Encoding? data = null)
    {
        return (data ?? Encoding.UTF8).GetString(buffer.Slice(2, BinaryPrimitives.ReadUInt16BigEndian(buffer)));
    }

    public static IPEndPoint ReadIpAddress(ReadOnlySpan<byte> buffer, ref int offset)
    {
        var version = buffer[offset++];

        IPAddress ip;
        ushort port;

        switch (version)
        {
            case 4:
                ip = new IPAddress(buffer.Slice(4, ref offset));

                return new IPEndPoint(ip, buffer.ReadUInt16BigEndian(ref offset));
            case 6:
                offset += 2;
                port = buffer.ReadUInt16BigEndian(ref offset);
                offset += 4;

                var ipBytes = buffer.Slice(16, ref offset);

                ip = new IPAddress(ipBytes,
                    buffer.ReadUInt32BigEndian(ref offset));

                return new IPEndPoint(ip, port);
            default:
                throw new NotImplementedException("Usupported IP protocol: " + version);
        }
    }

    public static int WriteIpAdress(Span<byte> buffer, IPEndPoint address)
    {
        var ipVersion = (byte)(address.Address.AddressFamily == AddressFamily.InterNetwork ? 4 : 6);
        buffer[0] = ipVersion;
        switch (ipVersion)
        {
            case 4:
                address.Address.GetAddressBytes().AsSpan().CopyTo(buffer.Slice(1));
                BinaryPrimitives.WriteUInt16BigEndian(buffer.Slice(5), (ushort)address.Port);
                break;
            case 6:
                throw new NotImplementedException("IPv6 address type is not supported at the moment.");
            //We dont have enought information about required data such as FlowInfo

            /*WriteUInt16((ushort)AddressFamily.InterNetworkV6, false); //Adress Family but it always should be InterNetworkV6
            WriteUInt16((ushort)address.Port, false);
            Seek(4); //Flow Info idk what is it
            //IPAddress iPAddress = new(ReadBuffer(16), ReadUInt32(false));
            //return new IPEndPoint(iPAddress, port);
            break;*/
        }

        return 7;
    }

    public static void WriteIpAdress(Span<byte> buffer, IPEndPoint address, ref int offset)
    {
        var ipVersion = (byte)(address.AddressFamily == AddressFamily.InterNetwork ? 4 : 6);

        buffer[0] = ipVersion;
        offset++;

        switch (ipVersion)
        {
            case 4:
                address.Address.GetAddressBytes().AsSpan().CopyTo(buffer.Slice(offset));
                buffer.WriteBigEndian(ref offset, (ushort)address.Port);
                break;
        }
    }
}