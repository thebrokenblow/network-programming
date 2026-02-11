using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkProgramming.SimpleServer;

public class Server : IDisposable
{
    private readonly IPEndPoint _ipEndPoint;
    private readonly int _backlog;
    private readonly Socket _serverSocket;

    public Server(IPEndPoint ipEndPoint, int backlog)
    {
        _ipEndPoint = ipEndPoint ?? throw new ArgumentNullException($"{nameof(IPEndPoint)} parametr is null");
        _backlog = backlog;

        _serverSocket = ConfigurationSocket();
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Сервер запущен");
        try
        {
            while (true)
            {
                using var clientSocket = await _serverSocket.AcceptAsync();
                Console.WriteLine($"Адрес подключенного клиента: {clientSocket.RemoteEndPoint}");
                await HandleRequest(clientSocket);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static async Task HandleRequest(Socket client)
    {
        var buffer = new byte[1024];

        int readBytes;
        var responce = new StringBuilder();

        do
        {
            readBytes = await client.ReceiveAsync(buffer);
            var responsePart = Encoding.UTF8.GetString(buffer, 0, readBytes);

            if (!string.IsNullOrEmpty(responsePart))
            {
                responce.Append(responsePart);
            }

        } while (readBytes > 0);

        Console.WriteLine(responce.ToString());
    }

    private Socket ConfigurationSocket()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(_ipEndPoint);
        socket.Listen(_backlog);

        return socket;
    }

    public void Dispose()
    {
        _serverSocket.Dispose();
    }
}