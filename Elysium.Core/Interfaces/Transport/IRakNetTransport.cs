using System.Net;
using Microsoft.Extensions.Hosting;

namespace Elysium.Core.Interfaces.Transport;

public interface IRakNetTransport : IHostedService, IDisposable
{
    Task SendConversionAsync(EndPoint ip, ReadOnlyMemory<byte> data, bool onlineMode);
}