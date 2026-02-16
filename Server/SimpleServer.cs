using Azure;
using Core.Enums;
using Core.Model;
using Core.Reponces;
using Core.Requests;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Repositories;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server;

public class SimpleServer : IDisposable
{
    private readonly IPEndPoint _ipEndPoint;
    private readonly int _backlog;

    private const int defaultSizeBuffer = 1024;
    private readonly int _sizeBuffer;

    private readonly Socket _serverSocket;

    public SimpleServer(IPEndPoint ipEndPoint, int backlog)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = defaultSizeBuffer;

        _serverSocket = Configuration();
    }

    public SimpleServer(IPEndPoint ipEndPoint, int backlog, int sizeBuffer)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = sizeBuffer;

        _serverSocket = Configuration();
    }

    public async Task RunAsync()
    {
        Socket? clientSocket = null;

        try
        {
            while (true)
            {
                clientSocket = await _serverSocket.AcceptAsync();
                _ = Task.Run(async () => await HandleRequestAsync(clientSocket));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            clientSocket?.Dispose();
        }
    }

    private async Task HandleRequestAsync(Socket clientSocket)
    {
        var context = new ApplicationContext();
        var productRepository = new ProductRepository(context);

        var requestDto = await HandlerReceiveAsync(clientSocket);

        if (requestDto.TypeCommand == TypeCommands.Read)
        {
            await HandleGetAllAsync(clientSocket, productRepository, requestDto.Body);
        }
        else if (requestDto.TypeCommand == TypeCommands.Create)
        {
            await HandleCreateAsync(clientSocket, productRepository, requestDto.Body);
        }
        else if (requestDto.TypeCommand == TypeCommands.Update)
        {
            await HandleUpdateAsync(productRepository, requestDto.Body);
        }
        else if (requestDto.TypeCommand == TypeCommands.Delete)
        {
            await HandleDeleteAsync(productRepository, requestDto.Body);
        }
    }

    private static async Task HandleGetAllAsync(Socket clientSocket, ProductRepository productRepository, string requestBody)
    {
        var products = await productRepository.GetAllAsync();
        var responce = JsonSerializer.Serialize(products);

        await HandlerSendAsync(clientSocket, responce);
    }

    private static async Task HandleCreateAsync(Socket clientSocket, ProductRepository productRepository, string requestBody)
    {
        var reponceDto = new ReponceDto
        {
            TypeResponce = TypeResponces.OK,
            Error = string.Empty,
        };

        try
        {
            var product = JsonSerializer.Deserialize<Product>(requestBody);
            await productRepository.CreateAsync(product);
        }
        catch (Exception ex)
        {
            reponceDto = new ReponceDto
            {
                TypeResponce = TypeResponces.Error,
                Error = ex.ToString(),
            };
        }

        var reponce = JsonSerializer.Serialize(reponceDto);
        await HandlerSendAsync(clientSocket, reponce);
    }

    private static async Task HandleUpdateAsync(ProductRepository productRepository, string requestBody)
    {
        var product = JsonSerializer.Deserialize<Product>(requestBody);
        await productRepository.UpdateAsync(product);
    }

    private static async Task HandleDeleteAsync(ProductRepository productRepository, string requestBody)
    {
        var id = JsonSerializer.Deserialize<int>(requestBody);
        await productRepository.DeleteAsync(id);
    }

    private async Task<RequestDto> HandlerReceiveAsync(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var requestBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var requestSegment = Encoding.UTF8.GetString(buffer, 0, readBytes);
            requestBuilder.Append(requestSegment);
        }
        while (readBytes > 0);

        clientSocket.Shutdown(SocketShutdown.Receive);
        var request = requestBuilder.ToString();
        var requestDto = JsonSerializer.Deserialize<RequestDto>(request);

        return requestDto;
    }

    private static async Task HandlerSendAsync(Socket clientSocket, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        await clientSocket.SendAsync(messageBytes);
        clientSocket.Shutdown(SocketShutdown.Send);
    }

    private Socket Configuration()
    {
        var serverSoket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSoket.Bind(_ipEndPoint);
        serverSoket.Listen(_backlog);

        return serverSoket;
    }

    public void Dispose()
    {
        _serverSocket.Dispose();
    }
}