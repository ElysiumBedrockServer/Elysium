namespace Elysium.RakNet.Protocol;

public static class OnlineIdentifiers
{
    public const int ConnectedPing = 0;
    public const int ConnectedPong = 3;
    public const int ConnectionRequest = 9;
    public const int ConnectionRequestAccepted = 16;
    public const int NewIncomingConnection = 19;
    public const int DisconnectionNotification = 21;
    public const int Datagram = 132;
    public const int NegativeAcknowledgement = 160;
    public const int Acknowledgement = 192;
}