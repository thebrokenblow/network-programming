using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkProgramming.SimpleClient;

public class Client(IPEndPoint ipEndPoint)
{
    private readonly IPEndPoint _ipEndPoint = ipEndPoint ??
        throw new ArgumentNullException($"{nameof(IPEndPoint)}");

    public async Task SendAsync(string message)
    {
        try
        {
            using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(_ipEndPoint);

            Console.WriteLine($"Подключение к {clientSocket.RemoteEndPoint} установлено");

            var messageBytes = Encoding.UTF8.GetBytes(message);
            await clientSocket.SendAsync(messageBytes);

            clientSocket.Shutdown(SocketShutdown.Both);
            await clientSocket.DisconnectAsync(reuseSocket: true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task SendAsync(string[] messages)
    {
        try
        {
            var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await clientSocket.ConnectAsync(_ipEndPoint);

            Console.WriteLine($"Подключение к {clientSocket.RemoteEndPoint} установлено");

            foreach (var message in messages)
            {
                var messageBytes = Encoding.UTF8.GetBytes(message);
                var length = await clientSocket.SendAsync(messageBytes);

                while (length != messageBytes.Length)
                {
                    messageBytes = Encoding.UTF8.GetBytes(message);
                    length = await clientSocket.SendAsync(messageBytes);
                }
            }

            clientSocket.Shutdown(SocketShutdown.Both);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}