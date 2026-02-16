using Core.Model;
using Core.Responces;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;
using Server.Repositories;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Server;

public class SimpleServer : IDisposable
{
    private readonly IPEndPoint _ipEndPoint;
    private readonly int _backlog;

    private const int DefaultSizeBuffer = 1024;
    private readonly int _sizeBuffer;

    private readonly Socket _serverSocket;

    public SimpleServer(IPEndPoint ipEndPoint, int backlog)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = DefaultSizeBuffer;
        _serverSocket = ConfigurationSocket();
    }

    public SimpleServer(IPEndPoint ipEndPoint, int backlog, int sizeBuffer)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(IPEndPoint), $"Argument {nameof(ipEndPoint)} is null");

        _ipEndPoint = ipEndPoint;
        _backlog = backlog;

        _sizeBuffer = sizeBuffer;

        _serverSocket = ConfigurationSocket();
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
        using var applicationContext = new ApplicationContext();
        using var productRepository = new ProductRepository(applicationContext);

        var (typeCommand, bodyRequest) = await ReadRequestAsync(clientSocket);

        if (typeCommand == TypeCommand.Read)
        {
            await HandleGetAllAsync(clientSocket, productRepository, bodyRequest);
        }
        else if (typeCommand == TypeCommand.Create)
        {
            await HandleCreateAsync(clientSocket, productRepository, bodyRequest);
        }
        else if (typeCommand == TypeCommand.Update)
        {
            await HandleUpdateAsync(clientSocket, productRepository, bodyRequest);
        }
        else if (typeCommand == TypeCommand.Delete)
        {
            await HandleDeleteAsync(clientSocket, productRepository, bodyRequest);
        }
    }

    private static async Task HandleGetAllAsync(Socket clientSocket, ProductRepository productRepository, string bodyRequest)
    {
        try
        {
            var products = await productRepository.GetAllAsync();
            var serializeProducts = JsonSerializer.Serialize(products);

            await HandlerSendAsync(clientSocket, serializeProducts);
        }
        catch (Exception ex)
        {
            var reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.Error,
                ErrorMessage = ex.Message
            };

            var reponce = JsonSerializer.Serialize(reponceDto);
            await HandlerSendAsync(clientSocket, reponce);
        }
    }

    private static async Task HandleCreateAsync(Socket clientSocket, ProductRepository productRepository, string bodyRequest)
    {
        var reponceDto = new ReponceDto()
        {
            StatusReponce = StatusReponce.Ok,
            ErrorMessage = string.Empty,
        };

        try
        {
            var product = JsonSerializer.Deserialize<Product>(bodyRequest)
                ?? throw new SerializationException($"{nameof(Product)} error serialization");

            await productRepository.CreateAsync(product);
        }
        catch (NotFoundException ex)
        {
            reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.NotFound,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.Error,
                ErrorMessage = ex.Message
            };
        }

        var reponce = JsonSerializer.Serialize(reponceDto);
        await HandlerSendAsync(clientSocket, reponce);
    }

    private static async Task HandleUpdateAsync(Socket clientSocket, ProductRepository productRepository, string bodyRequest)
    {
        var reponceDto = new ReponceDto()
        {
            StatusReponce = StatusReponce.Ok,
            ErrorMessage = string.Empty,
        };

        try
        {
            var product = JsonSerializer.Deserialize<Product>(bodyRequest) 
                ?? throw new SerializationException($"{nameof(Product)} error serialization");

            await productRepository.UpdateAsync(product);
        }
        catch (NotFoundException ex)
        {
            reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.NotFound,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
           reponceDto = new ReponceDto()
           {
               StatusReponce = StatusReponce.Error,
               ErrorMessage = ex.Message
           };
        }

        var reponce = JsonSerializer.Serialize(reponceDto);
        await HandlerSendAsync(clientSocket, reponce);
    }

    private static async Task HandleDeleteAsync(Socket clientSocket, ProductRepository productRepository, string bodyRequest)
    {
        var reponceDto = new ReponceDto()
        {
            StatusReponce = StatusReponce.Ok,
            ErrorMessage = string.Empty,
        };

        try
        {
            var id = JsonSerializer.Deserialize<int>(bodyRequest);
            await productRepository.DeleteAsync(id);
        }
        catch (NotFoundException ex)
        {
            reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.NotFound,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            reponceDto = new ReponceDto()
            {
                StatusReponce = StatusReponce.Error,
                ErrorMessage = ex.Message
            };
        }

        var reponce = JsonSerializer.Serialize(reponceDto);
        await HandlerSendAsync(clientSocket, reponce);
    }

    private async Task<(TypeCommand typeCommand, string bodyRequest)> ReadRequestAsync(Socket clientSocket)
    {
        int readBytes;
        var buffer = new byte[_sizeBuffer];
        var requestBuilder = new StringBuilder();
        do
        {
            readBytes = await clientSocket.ReceiveAsync(buffer);
            var request = Encoding.UTF8.GetString(buffer, 0, readBytes);
            requestBuilder.Append(request);
        }
        while (readBytes > 0);

        var strRequest = requestBuilder.ToString();
        var typeCommandAndBody = strRequest.Split(["\n", "\r\n"], StringSplitOptions.RemoveEmptyEntries);
        
        var typeCommand = Enum.Parse<TypeCommand>(typeCommandAndBody.First());
        var bodyRequest = string.Join(" ", typeCommandAndBody[1..]);

        return (typeCommand, bodyRequest);
    }

    private static async Task HandlerSendAsync(Socket clientSocket, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        await clientSocket.SendAsync(messageBytes);
        clientSocket.Shutdown(SocketShutdown.Send);
    }

    private Socket ConfigurationSocket()
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