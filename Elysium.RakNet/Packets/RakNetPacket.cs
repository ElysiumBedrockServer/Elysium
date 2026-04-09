namespace Elysium.RakNet.Packets;

internal partial class RakNetPacket
{
    public const int MaxSizeMtu = 1492;
    
    public byte[] RawData { get; set; } = new byte[MaxSizeMtu];
    public int Size { get; set; }
}