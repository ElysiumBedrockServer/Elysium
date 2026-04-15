using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Elysium.Core.Configuration.Raknet;
using Elysium.Core.Packets;
using Elysium.RakNet.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Elysium.RakNet.Sockets;

public interface IRakNetSocket
{
    int V4Port { get; }
    int V6Port { get; }

    CancellationToken? Token { get; }

    bool Start();

    bool Stop();
}

public class RakNetSocket : IRakNetSocket
{
    private readonly RaknetConfiguration? _config;
    private readonly IRakNetConnectionStore _connectionStore;
    private readonly ILogger<RakNetSocket> _logger;

    private readonly bool _v6Support = Socket.OSSupportsIPv6;

    private CancellationTokenSource? _cts;

    private volatile bool _isRunning;

    private int _lastPeerId;
    private readonly ConcurrentQueue<int> _peerIds = new();

    private Thread? _receiveThread;

    private Socket? _updSocketv4;
    private Socket? _updSocketv6;

    public RakNetSocket(ILogger<RakNetSocket> logger, IOptions<RaknetConfiguration> config,
        IRakNetConnectionStore connectionStore)
    {
        _logger = logger;
        _config = config.Value;
        _connectionStore = connectionStore;

        V4Port = _config.PortIpv4;
        V6Port = _config.PortIpv6;
    }

    public int ReceivePollingTime { get; } = 50000;


    public CancellationToken? Token => _cts?.Token;
    public int V4Port { get; }
    public int V6Port { get; }

    public bool Start()
    {
        if (_isRunning)
            return false;

        _updSocketv4 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        if (!BindSocket(_updSocketv4, new IPEndPoint(IPAddress.Any, V4Port)))
        {
            _updSocketv4.Dispose();
            _updSocketv4 = null;

            return false;
        }


        _isRunning = true;

        if (_v6Support)
        {
            _updSocketv6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);

            if (!BindSocket(_updSocketv6, new IPEndPoint(IPAddress.IPv6Any, V6Port)))
            {
                _updSocketv6.Dispose();
                _updSocketv6 = null;
            }
        }

        _cts = new CancellationTokenSource();

        ThreadStart ts = ReceiveLogic;

        _receiveThread = new Thread(ts)
        {
            Name = "ReceiveThread",
            IsBackground = true
        };

        _receiveThread.Start();


        return true;
    }

    public bool Stop()
    {
        try
        {
            if (!_isRunning)
                return false;

            _cts?.Cancel();
            _updSocketv4?.Dispose();
            _updSocketv6?.Dispose();
        }
        finally
        {
            _isRunning = false;
        }

        return true;
    }

    private void ReceiveLogic()
    {
        EndPoint bufferEndPoint4 = new IPEndPoint(IPAddress.Any, 0);
        EndPoint bufferEndPoint6 = new IPEndPoint(IPAddress.IPv6Any, 0);

        var selectReadList = new List<Socket>(2);

        var socketV4 = _updSocketv4!;
        var socketV6 = _updSocketv6;

        while (_isRunning)
            try
            {
                selectReadList.Clear();

                if (socketV4 != null)
                    selectReadList.Add(socketV4);

                if (socketV6 != null)
                    selectReadList.Add(socketV6);

                Socket.Select(selectReadList, null, null, ReceivePollingTime);

                foreach (var socket in selectReadList)
                    if (socket == socketV4)
                        ReceiveFrom(socketV4, ref bufferEndPoint4);
                    else if (socket == socketV6)
                        ReceiveFrom(socketV6, ref bufferEndPoint6);
            }
            catch (SocketException e)
            {
                var result = ProcessError(e);
                _logger.LogError(e, "SocketReceiveThread error: {Message}", e.Message);

                if (!result)
                    return;
            }
            catch (ObjectDisposedException e)
            {
                return;
            }
            catch (ThreadAbortException e)
            {
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SocketReceiveThread error: {Message}", e.Message);
            }
    }

    private int ReceiveFrom(Socket socket, ref EndPoint endPoint)
    {
        var packet = new RakNetPacket();

        var sender = socket.AddressFamily == AddressFamily.InterNetwork
            ? new IPEndPoint(IPAddress.Any, 0).Serialize()
            : new IPEndPoint(IPAddress.IPv6Any, 0).Serialize();

        packet.Size = socket.ReceiveFrom(
            packet.RawData,
            SocketFlags.None,
            sender);

        endPoint = _connectionStore.TryGetPeer(sender, out var value)
            ? value
            : (IPEndPoint)endPoint.Create(sender);

        return packet.Size;
    }

    private bool BindSocket(Socket socket, IPEndPoint endPoint)
    {
        try
        {
            socket.ReceiveTimeout = 500;
            socket.SendTimeout = 500;
            socket.ReceiveBufferSize = 1048576;
            socket.SendBufferSize = 1048576;
            socket.Blocking = true;

            socket.Bind(endPoint);

            if (endPoint.AddressFamily == AddressFamily.InterNetwork)
                socket.EnableBroadcast = true;

            var ipVersion = endPoint.AddressFamily switch
            {
                AddressFamily.InterNetwork => "IPv4",
                AddressFamily.InterNetworkV6 => "IPv6",
                _ => "Unknown"
            };

            var isDualMode = socket.AddressFamily == AddressFamily.InterNetworkV6 && socket.DualMode;

            _logger.LogInformation(
                "Server bound successfully | Endpoint: {EndPoint} | IP Version: {IPVersion} | DualMode: {DualMode} | Blocking: {Blocking} | RcvBuf: {ReceiveBuffer} | SndBuf: {SendBuffer}",
                endPoint,
                ipVersion,
                isDualMode,
                socket.Blocking,
                socket.ReceiveBufferSize,
                socket.SendBufferSize
            );

            return true;
        }
        catch (SocketException se)
        {
            _logger.LogError(se,
                "BindSocket failed (SocketException) | Endpoint: {EndPoint} | Code: {ErrorCode} | Message: {Message}",
                endPoint,
                se.SocketErrorCode,
                se.Message
            );
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "BindSocket failed (Unexpected) | Endpoint: {EndPoint} | Message: {Message}",
                endPoint,
                e.Message
            );
            return false;
        }
    }

    private bool ProcessError(SocketException ex)
    {
        switch (ex.SocketErrorCode)
        {
            case SocketError.NotConnected:
                return true;
            case SocketError.Interrupted:
            case SocketError.NotSocket:
            case SocketError.OperationAborted:
                return true;
            case SocketError.ConnectionReset:
            case SocketError.MessageSize:
            case SocketError.TimedOut:
            case SocketError.NetworkReset:
            case SocketError.WouldBlock:
                //NetDebug.Write($"[R]Ignored error: {(int)ex.SocketErrorCode} - {ex}");
                break;
            default:
                _logger.LogError($"[R]Error code: {(int)ex.SocketErrorCode} - {ex}");
                break;
        }

        return false;
    }

    private int GetNextPeerId()
    {
        return _peerIds.TryDequeue(out var id) ? id : _lastPeerId++;
    }
}