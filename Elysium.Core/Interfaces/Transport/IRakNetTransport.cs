namespace Elysium.Core.Interfaces.Transport;

public interface IRakNetTransport
{
    Task SendAsync(ReadOnlyMemory<byte> data);
}