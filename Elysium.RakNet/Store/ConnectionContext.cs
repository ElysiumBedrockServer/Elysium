using System.Net;

namespace Elysium.RakNet.Store;

public class ConnectionContext
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public EndPoint? EndPoint { get; set; }
    public string Ip { get; set; } = "";

    public ConnectionContext(EndPoint endPoint)
    {
        EndPoint = endPoint;
        Ip = endPoint.ToString()!;
    }
}