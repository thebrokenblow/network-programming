using Core.Model;
using System.Net;

namespace Client;

public class Program
{
    private const int port = 8888;
    private readonly static byte[] arrayIpAddress = [127, 0, 0, 1];

    public static async Task Main()
    {
        Console.WriteLine("Клиент запущен");

        var ipAddress = new IPAddress(arrayIpAddress);
        var ipEndPoint = new IPEndPoint(ipAddress, port);
        var simpleClient = new ProductService(ipEndPoint);

        while (true)
        {
            var message = Console.ReadLine();

            if (string.IsNullOrEmpty(message))
            {
                continue;
            }

            if (message == "1")
            {
                var products = await simpleClient.GetAllAsync();

                foreach (var product in products)
                {
                    Console.WriteLine(product);
                }
            }
            else if (message == "2")
            {
                Console.WriteLine("Введите наименование товара");
                var name = Console.ReadLine();

                Console.WriteLine("Введите Описание товара");
                var description = Console.ReadLine();

                Console.WriteLine("Введите цену товара");
                var price = decimal.Parse(Console.ReadLine());

                var product = new Product
                {
                    Name = name,
                    Description = description,
                    Price = price
                };

                await simpleClient.CreateAsync(product);
            }
            else if (message == "3")
            {
                Console.WriteLine("Введите номер товара");
                var id = int.Parse(Console.ReadLine());

                Console.WriteLine("Введите наименование товара");
                var name = Console.ReadLine();

                Console.WriteLine("Введите Описание товара");
                var description = Console.ReadLine();

                Console.WriteLine("Введите цену товара");
                var price = decimal.Parse(Console.ReadLine());

                var product = new Product
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    Price = price
                };

                await simpleClient.UpdateAsync(product);
            }
            else if (message == "4")
            {
                Console.WriteLine("Введите номер товара");
                var id = int.Parse(Console.ReadLine());

                await simpleClient.DeleteAsync(id);
            }
        }
    }
}