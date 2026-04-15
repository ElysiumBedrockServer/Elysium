using System.Net;
using Elysium.Core.Socket;

namespace Elysium.RakNet.Store;

public interface IRakNetConnectionStore
{
    RakNetPeer? this[int value] { get; }
    RakNetPeer? GetPeerById(int id);

    bool TryGetPeerById(int id, out RakNetPeer? peer);
    bool TryGetPeer(IPEndPoint endPoint, out RakNetPeer peer);
    bool TryGetPeer(SocketAddress sAddr, out RakNetPeer peer);

    internal void AddPeer(InternalRakNetPeer? peer);
    internal void RemovePeer(InternalRakNetPeer? peer, bool enableWriteLock);
}