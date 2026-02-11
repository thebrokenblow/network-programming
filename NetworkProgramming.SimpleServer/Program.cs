using System.Net;

namespace NetworkProgramming.SimpleServer;

public class Program
{
    private const int port = 8888;
    private const int backlog = 10;

    public async static Task Main()
    {
        var ipPoint = new IPEndPoint(IPAddress.Any, port);

        using var server = new Server(ipPoint, backlog);
        await server.RunAsync();
    }
}