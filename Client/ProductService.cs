using Core.Enums;
using Core.Model;
using Core.Reponces;
using Core.Requests;
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

    public async Task<List<Product>> GetAllAsync()
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var requestDto = new RequestDto
        {
            TypeCommand = TypeCommands.Read,
            Body = string.Empty,
        };

        var request = JsonSerializer.Serialize(requestDto);
        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);
            await HandlerSendAsync(clientSocket, request);
            var products = await HandlerReceiveAsync<List<Product>>(clientSocket);

            return products;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task CreateAsync(Product product)
    {
        using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        var body = JsonSerializer.Serialize(product);
        var requestDto = new RequestDto
        {
            TypeCommand = TypeCommands.Create,
            Body = body,
        };

        var request = JsonSerializer.Serialize(requestDto);
        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);
            await HandlerSendAsync(clientSocket, request);
            var responce = await HandlerReceiveAsync<ReponceDto>(clientSocket);
            Console.WriteLine(responce);
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

        var body = JsonSerializer.Serialize(product);
        var requestDto = new RequestDto
        {
            TypeCommand = TypeCommands.Update,
            Body = body,
        };

        var request = JsonSerializer.Serialize(requestDto);
        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);
            await HandlerSendAsync(clientSocket, request);
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

        var body = JsonSerializer.Serialize(id);
        var requestDto = new RequestDto
        {
            TypeCommand = TypeCommands.Delete,
            Body = body,
        };

        var request = JsonSerializer.Serialize(requestDto);
        try
        {
            await clientSocket.ConnectAsync(_ipEndPoint);
            await HandlerSendAsync(clientSocket, request);
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
        clientSocket.Shutdown(SocketShutdown.Send);
    }

    private async Task<T?> HandlerReceiveAsync<T>(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var responceBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var responce = Encoding.UTF8.GetString(buffer, 0, readBytes);
            responceBuilder.Append(responce);
        }
        while (readBytes > 0);

        clientSocket.Shutdown(SocketShutdown.Receive);

        return JsonSerializer.Deserialize<T>(responceBuilder.ToString());
    }
}
