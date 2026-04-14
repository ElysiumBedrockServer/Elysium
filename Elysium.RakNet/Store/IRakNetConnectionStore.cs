using System.Net;
using Elysium.RakNet.Sockets;

namespace Elysium.RakNet.Store;

public interface IRakNetConnectionStore
{
    RakNetPeer? GetPeerById(int id);
    bool TryGetPeerById(int id, out RakNetPeer? peer);
    void AddPeer(RakNetPeer? peer);
    void RemovePeer(RakNetPeer? peer, bool enableWriteLock);
    bool TryGetPeer(IPEndPoint endPoint, out RakNetPeer peer);
    bool TryGetPeer(SocketAddress sAddr, out RakNetPeer peer);
    RakNetPeer? this[int value] { get; }
}