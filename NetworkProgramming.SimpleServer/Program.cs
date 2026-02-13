using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkProgramming.SimpleServer;

public class Program
{
    private const int port = 8888;
    private const int backlog = 10;

    public async static Task Main()
    {
        var ipPoint = new IPEndPoint(IPAddress.Any, port);
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.Bind(ipPoint);
        socket.Listen(backlog);

        Console.WriteLine("Сервер запущен на порту 8888");

        while (true)
        {
            var clientSocket = await socket.AcceptAsync();
            _ = Task.Run(() =>  HandleClientAsync(clientSocket));
        }
    }

    private static async Task HandleClientAsync(Socket clientSocket)
    {
        try
        {
            int readBytes = 0;
            var buffer = new byte[1024];
            var response = new StringBuilder();

            do
            {
                readBytes = await clientSocket.ReceiveAsync(buffer);
                var message = Encoding.UTF8.GetString(buffer, 0, readBytes);
                response.Append(message);
            } while (readBytes > 0);

            clientSocket.Shutdown(SocketShutdown.Receive);

            Console.WriteLine(response.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
        }
        finally
        {
            clientSocket.Dispose();
        }
    }
}