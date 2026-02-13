using System.Net.Sockets;
using System.Text;

namespace NetworkProgramming.SimpleClient;

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Клиент запущен");

        while (true)
        {
            using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync("127.0.0.1", 8888);

            var message = Console.ReadLine();

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await clientSocket.SendAsync(messageBytes);

            clientSocket.Shutdown(SocketShutdown.Both);
            await clientSocket.DisconnectAsync(reuseSocket: true);
        }
    }
}