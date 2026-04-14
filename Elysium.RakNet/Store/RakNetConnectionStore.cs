using System.Net;
using Elysium.RakNet.Sockets;
using Microsoft.Extensions.Logging;

namespace Elysium.RakNet.Store;

public class RakNetConnectionStore : IRakNetConnectionStore
{
    private const int MaxPrimeArrayLength = 0x7FFFFFC3;
    private const int HashPrime = 101;
    private const int Lower31BitMask = 0x7FFFFFFF;

    private static readonly int[] Primes =
    {
        3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
        1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
        17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
        187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
        1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369
    };

    private readonly ILogger<RakNetConnectionStore> _logger;
    private readonly ReaderWriterLockSlim _peersLock = new(LockRecursionPolicy.NoRecursion);

    private int[]? _buckets;
    private int _count;
    private int _freeList = -1;
    private volatile RakNetPeer? _headPeer;
    private int _lastIndex;
    private RakNetPeer[] _peersArray = new RakNetPeer[32];
    private Slot[] _slots;

    public RakNetConnectionStore(ILogger<RakNetConnectionStore> logger)
    {
        _logger = logger;
    }

    public RakNetPeer? GetPeerById(int id)
    {
        return id >= 0 && id < _peersArray.Length ? _peersArray[id] : null;
    }

    public bool TryGetPeerById(int id, out RakNetPeer? peer)
    {
        peer = GetPeerById(id);
        return peer != null;
    }

    public void AddPeer(RakNetPeer? peer)
    {
        if (peer == null)
        {
            _logger.LogError($"Add peer null: {peer}");
            return;
        }

        _peersLock.EnterWriteLock();
        if (_headPeer != null)
        {
            peer.NextPeer = _headPeer;
            _headPeer.PrevPeer = peer;
        }

        _headPeer = peer;
        AddPeerToSet(peer);
        if (peer.Id >= _peersArray.Length)
        {
            var newSize = _peersArray.Length * 2;
            while (peer.Id >= newSize)
                newSize *= 2;
            Array.Resize(ref _peersArray, newSize);
        }

        _peersArray[peer.Id] = peer;
        _peersLock.ExitWriteLock();
    }

    public void RemovePeer(RakNetPeer? peer, bool enableWriteLock)
    {
        if (peer == null)
            return;

        if (enableWriteLock)
            _peersLock.EnterWriteLock();
        if (!RemovePeerFromSet(peer))
        {
            if (enableWriteLock)
                _peersLock.ExitWriteLock();
            return;
        }

        if (peer == _headPeer)
            _headPeer = peer.NextPeer;

        if (peer.PrevPeer != null)
            peer.PrevPeer.NextPeer = peer.NextPeer;
        if (peer.NextPeer != null)
            peer.NextPeer.PrevPeer = peer.PrevPeer;

        peer.PrevPeer = null;

        _peersArray[peer.Id] = null;

        if (enableWriteLock)
            _peersLock.ExitWriteLock();
    }

    public bool TryGetPeer(IPEndPoint endPoint, out RakNetPeer peer)
    {
        return TryGetPeerBase(endPoint, new EndPointComparer(), out peer);
    }

    public bool TryGetPeer(SocketAddress sAddr, out RakNetPeer peer)
    {
        return TryGetPeerBase(sAddr, new SocketAddressComparer(), out peer);
    }

    public RakNetPeer? this[int value]
        => GetPeerById(value);

    private bool RemovePeerFromSet(RakNetPeer? peer)
    {
        if (_buckets == null || peer == null)
            return false;
        var hashCode = peer.GetHashCode() & Lower31BitMask;
        var bucket = hashCode % _buckets.Length;
        var last = -1;
        for (var i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].Next)
            if (_slots[i].HashCode == hashCode && _slots[i].Value.Equals(peer))
            {
                if (last < 0)
                    _buckets[bucket] = _slots[i].Next + 1;
                else
                    _slots[last].Next = _slots[i].Next;
                _slots[i].HashCode = -1;
                _slots[i].Value = null;
                _slots[i].Next = _freeList;

                _count--;
                if (_count == 0)
                {
                    _lastIndex = 0;
                    _freeList = -1;
                }
                else
                {
                    _freeList = i;
                }

                return true;
            }

        return false;
    }

    private bool AddPeerToSet(RakNetPeer value)
    {
        if (_buckets == null)
        {
            var size = HashSetGetPrime(0);
            _buckets = new int[size];
            _slots = new Slot[size];
        }

        var hashCode = value.GetHashCode() & Lower31BitMask;
        var bucket = hashCode % _buckets.Length;

        for (var i = _buckets[bucket] - 1; i >= 0; i = _slots[i].Next)
            if (_slots[i].HashCode == hashCode && _slots[i].Value.Equals(value))
                return false;

        int index;
        if (_freeList >= 0)
        {
            index = _freeList;
            _freeList = _slots[index].Next;
        }
        else
        {
            if (_lastIndex == _slots.Length)
            {
                var newSize = 2 * _count;
                newSize = (uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > _count
                    ? MaxPrimeArrayLength
                    : HashSetGetPrime(newSize);

                var newSlots = new Slot[newSize];
                Array.Copy(_slots, 0, newSlots, 0, _lastIndex);
                _buckets = new int[newSize];
                for (var i = 0; i < _lastIndex; i++)
                {
                    var b = newSlots[i].HashCode % newSize;
                    newSlots[i].Next = _buckets[b] - 1;
                    _buckets[b] = i + 1;
                }

                _slots = newSlots;

                bucket = hashCode % _buckets.Length;
            }

            index = _lastIndex;
            _lastIndex++;
        }

        _slots[index].HashCode = hashCode;
        _slots[index].Value = value;
        _slots[index].Next = _buckets[bucket] - 1;
        _buckets[bucket] = index + 1;
        _count++;

        return true;
    }

    private static int HashSetGetPrime(int min)
    {
        foreach (var prime in Primes)
            if (prime >= min)
                return prime;

        for (var i = min | 1; i < int.MaxValue; i += 2)
            if (IsPrime(i) && (i - 1) % HashPrime != 0)
                return i;
        return min;

        bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                var limit = (int)Math.Sqrt(candidate);
                for (var divisor = 3; divisor <= limit; divisor += 2)
                    if (candidate % divisor == 0)
                        return false;
                return true;
            }

            return candidate == 2;
        }
    }

    private bool TryGetPeerBase<T, TComparer>(T value, TComparer comparer, out RakNetPeer peer)
        where TComparer : struct, IPeerComparer<T>
    {
        if (_buckets != null)
        {
            var hashCode = value!.GetHashCode() & Lower31BitMask;

            _peersLock.EnterReadLock();
            for (var i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].Next)
                if (_slots[i].HashCode == hashCode &&
                    comparer.Equals(_slots[i].Value, value))
                {
                    peer = _slots[i].Value;

                    _peersLock.ExitReadLock();
                    return true;
                }

            _peersLock.ExitReadLock();
        }

        peer = null;
        return false;
    }
}

internal struct Slot
{
    internal int HashCode;
    internal int Next;
    internal RakNetPeer Value;
}

internal interface IPeerComparer<T>
{
    bool Equals(RakNetPeer peer, T value);
}

internal struct EndPointComparer : IPeerComparer<IPEndPoint>
{
    public bool Equals(RakNetPeer peer, IPEndPoint value)
    {
        return peer.Equals(value);
    }
}

internal struct SocketAddressComparer : IPeerComparer<SocketAddress>
{
    public bool Equals(RakNetPeer peer, SocketAddress value)
    {
        return peer.Serialize().Equals(value);
    }
}