namespace Elysium.Core.Protocol;

public static class MessageIdentifier
{
    public static readonly byte[] Magic = [0x00, 0xFF, 0xFF, 0x00, 0xFE, 0xFE, 0xFE, 0xFE, 0xFD, 0xFD, 0xFD, 0xFD, 0x12, 0x34, 0x56, 0x78];
    public const byte RakNetVersion = 11;
    
    public const int OfflineUnconnectedPing = 1;
    public const int OfflinePingOpenConnections = 2;
    public const int OfflineOpenConnectionRequestFirst = 5;
    public const int OfflineOpenConnectionReplyFirst = 6;
    public const int OfflineOpenConnectionRequestSecond = 7;
    public const int OfflineConnectionReplySecond = 8;
    public const int OfflineRemoteSystemRequiresPublicKey = 10;
    public const int OfflineConnectionAttemptFailed = 17;
    public const int OfflineAlreadyConnected = 18;
    public const int OfflineConnectionLost = 22;
    public const int OfflineIncompatibleProtocolVersion = 25;
    public const int OfflineUnconnectedPong = 28;

    public const int OnlineConnectedPing = 0;
    public const int OnlineConnectedPong = 3;
    public const int OnlineConnectionRequest = 9;
    public const int OnlineConnectionRequestAccepted = 16;
    public const int OnlineNewIncomingConnection = 19;
    public const int OnlineDisconnectionNotification = 21;
    public const int OnlineDatagram = 132;
    public const int OnlineNegativeAcknowledgement = 160;
    public const int OnlineAcknowledgement = 192;
}