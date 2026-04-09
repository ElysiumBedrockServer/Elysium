using System.Diagnostics;
using System.Net;

namespace Elysium.RakNet.Sockets;

public class RakNetPeer : IPEndPoint
{
    private SocketAddress _cachedSocketAddr;
    
    private int _rtt;
    private int _avgRtt;
    private int _rttCount;
    private float _resendDelay = 27.0f;
    private float _pingSendTimer;
    private float _rttResetTimer;
    private readonly Stopwatch _pingTimer = new Stopwatch();
    private float _timeSinceLastPacket;
    private long _remoteDelta;
    
    internal volatile RakNetPeer? NextPeer;
    internal RakNetPeer? PrevPeer;
    
    public RakNetPeer(long address, int port) : base(address, port)
    {
    }

    public RakNetPeer(IPAddress address, int port) : base(address, port)
    {
    }
    
    public readonly int Id;
    
    public override SocketAddress Serialize() =>
        _cachedSocketAddr;
}