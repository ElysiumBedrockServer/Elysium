using System.Diagnostics;
using System.Net;
using Elysium.Core.Packets;

namespace Elysium.Core.Socket;

internal enum DisconnectResult
{
    None,
    Reject,
    Disconnect
}

public class RakNetPeer : IPEndPoint
{
    private readonly Stopwatch _pingTimer = new();

    //Common
    private readonly object _shutdownLock = new();
    private readonly object _unreliableChannelLock = new();

    public readonly int Id;
    private int _avgRtt;
    private float _pingSendTimer;
    private long _remoteDelta;

    private float _resendDelay = 27.0f;

    //Ping and RTT
    private int _rtt;
    private int _rttCount;
    private float _rttResetTimer;
    private float _timeSinceLastPacket;
    private RakNetPacket[] _unreliableChannel;
    private int _unreliablePendingCount;

    //Channels
    private RakNetPacket[] _unreliableSecondQueue;

    public RakNetPeer(long address, int port) : base(address, port)
    {
    }

    public RakNetPeer(IPAddress address, int port) : base(address, port)
    {
    }
}