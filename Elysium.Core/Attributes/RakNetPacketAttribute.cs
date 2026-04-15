namespace Elysium.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RakNetPacketAttribute : Attribute
{
    public RakNetPacketAttribute(byte id)
    {
        Id = id;
    }

    public byte Id { get; set; }
}