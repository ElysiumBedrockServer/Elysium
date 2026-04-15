using System.Net;
using Elysium.Core.Attributes;
using Elysium.Core.Extensions;
using Elysium.Core.Protocol;

namespace Elysium.Core.Packets.Connect;

[RakNetPacket(Packet.ConnectionRequestAccepted)]
public class ConnectionAccepted : RakNetPacket
{
    public short SystemIndex;

    public ConnectionAccepted()
    {
        Id = Packet.ConnectionRequestAccepted;
    }

    public ConnectionAccepted(IPEndPoint client, IList<IPEndPoint> systemAddressIps) : this()
    {
        Client = client;
        SystemAddresses = systemAddressIps;
    }


    public IList<IPEndPoint> SystemAddresses { get; set; } = new List<IPEndPoint>
    {
        new(IPAddress.Any, 19138)
    };

    public IPEndPoint Client { get; set; }
    public long RequestTime { get; set; }
    public long Time { get; set; }

    public override void Deserialize(ReadOnlySpan<byte> buffer)
    {
        Offset = 1;
        Client = Helper.Helper.ReadIpAddress(buffer, ref Offset);
        SystemIndex = buffer.ReadInt16BigEndian(ref Offset);

        SystemAddresses.Clear();
        for (var i = 0; i < 20; i++)
        {
            var ip = Helper.Helper.ReadIpAddress(buffer, ref Offset);

            if (ip == Client)
                continue;

            SystemAddresses.Add(ip);
        }

        RequestTime = buffer.ReadInt64BigEndian(ref Offset);
        Time = buffer.ReadInt64BigEndian(ref Offset);
        
        Size = Offset;
    }

    public override Span<byte> Serialize()
    {
        var buffer = RawData;

        buffer[0] = Id;
        Offset = 1;

        Helper.Helper.WriteIpAdress(buffer, Client, ref Offset);
        buffer.WriteBigEndian(ref Offset, SystemIndex);

        for (var i = 0; i < 20; i++)
        {
            IPEndPoint value;

            if (i < SystemAddresses.Count)
                value = SystemAddresses[i];
            else
                value = Client;

            Helper.Helper.WriteIpAdress(buffer, value, ref Offset);
        }

        buffer.WriteBigEndian(ref Offset, RequestTime);
        buffer.WriteBigEndian(ref Offset, Time);

        return buffer.Slice(0, Offset);
    }
}