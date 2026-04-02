using Elysium.Core.Binary;

namespace Elysium.Core.Interfaces;

public interface IPacket
{
    public void Write(RaknetBinaryWriter writer);
    public void Read(RaknetBinaryReader reader);
}