namespace Elysium.Core.Protocol;

public static class Packet
{
    public const byte ConnectedPing = 0x00; // 0
    public const byte UnconnectedPing = 0x01; // 1
    public const byte ConnectedPong = 0x03; // 3
    public const byte OpenConnectionRequest1 = 0x05; // 5
    public const byte OpenConnectionReply1 = 0x06; // 6
    public const byte OpenConnectionRequest2 = 0x07; // 7
    public const byte OpenConnectionReply2 = 0x08; // 8
    public const byte ConnectionRequest = 0x09; // 9
    public const byte ConnectionRequestAccepted = 0x10; // 16
    public const byte NewIncomingConnection = 0x13; // 19
    public const byte Disconnect = 0x15; // 21
    public const byte IncompatibleProtocolVersion = 0x19; // 25
    public const byte UnconnectedPong = 0x1c; // 28
    public const byte FrameSet = 0x80; // 128
    public const byte Nack = 0xa0; // 160
    public const byte Ack = 0xc0; // 192
}