using System.Net;

namespace NetworkProgramming.SimpleClient;

public class Program
{
    private const int Port = 8888;
    private readonly static byte[] arrayIPAddress = [127, 0, 0, 1];

    public static async Task Main()
    {
        var ipAddress = new IPAddress(arrayIPAddress);
        var ipEndPoint = new IPEndPoint(ipAddress, Port);

        var client = new Client(ipEndPoint);
        Console.WriteLine("Клиент запущен");

        while (true)
        {
            var value = Console.ReadLine();
            await client.SendAsync(value);
        }
    }
}