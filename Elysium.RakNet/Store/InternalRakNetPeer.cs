using System.Net;
using Elysium.Core.Socket;

namespace Elysium.RakNet.Store;

public class InternalRakNetPeer : RakNetPeer
{
    internal volatile InternalRakNetPeer? NextPeer;
    internal InternalRakNetPeer? PrevPeer;

    public InternalRakNetPeer(long address, int port) : base(address, port)
    {
    }

    public InternalRakNetPeer(IPAddress address, int port) : base(address, port)
    {
    }
}