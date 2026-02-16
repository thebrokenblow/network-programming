using Client.Common;
using Core;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client;

public class ProductService
{
    private readonly IPEndPoint _ipEndPoint;
    private const int defaultSizeBuffer = 1024;
    private readonly int _sizeBuffer;

    public ProductService(IPEndPoint ipEndPoint)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;

        _sizeBuffer = defaultSizeBuffer;
    }

    public ProductService(IPEndPoint ipEndPoint, int sizeBuffer)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _sizeBuffer = sizeBuffer;
    }

    public async Task<List<Product>?> GetAllAsync()
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        var products = new List<Product>();

        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);

            var request = new RequestBuilder()
                                    .AddTypeCommand(TypeCommand.Read)
                                    .Build();

            await HandlerSendAsync(clientSocket, request);
            clientSocket.Shutdown(SocketShutdown.Send);

            products = await HandlerReceiveAsync<List<Product>>(clientSocket);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }

        return products;
    }

    public async Task CreateAsync(Product product)
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);

            var request = new RequestBuilder()
                                .AddTypeCommand(TypeCommand.Create)
                                .AddBody(product)
                                .Build();

            await HandlerSendAsync(clientSocket, request);
            clientSocket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task UpdateAsync(Product product)
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);

            var request = new RequestBuilder()
                                .AddTypeCommand(TypeCommand.Update)
                                .AddBody(product)
                                .Build();

            await HandlerSendAsync(clientSocket, request);
            clientSocket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);

            var request = new RequestBuilder()
                                .AddTypeCommand(TypeCommand.Delete)
                                .AddBody(id)
                                .Build();

            await HandlerSendAsync(clientSocket, request);
            clientSocket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    private static async Task HandlerSendAsync(Socket clientSocket, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await clientSocket.SendAsync(messageBytes);
    }

    private async Task<T?> HandlerReceiveAsync<T>(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var responceBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var responceSegment = Encoding.UTF8.GetString(buffer, 0, readBytes);
            responceBuilder.Append(responceSegment);
        }
        while (readBytes > 0);

        clientSocket.Shutdown(SocketShutdown.Receive);
        var responce = JsonSerializer.Deserialize<T>(responceBuilder.ToString());

        return responce;
    }
}