namespace Elysium.RakNet.Protocol;

public static class OfflineIdentifiers
{
    public const int UnconnectedPing = 1;
    public const int PingOpenConnections = 2;
    public const int OpenConnectionRequestFirst = 5;
    public const int OpenConnectionReplyFirst = 6;
    public const int OpenConnectionRequestSecond = 7;
    public const int ConnectionReplySecond = 8;
    public const int RemoteSystemRequiresPublicKey = 10;
    public const int ConnectionAttemptFailed = 17;
    public const int AlreadyConnected = 18;
    public const int ConnectionLost = 22;
    public const int IncompatibleProtocolVersion = 25;
    public const int UnconnectedPong = 28;
}