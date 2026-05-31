using System;
using System.Net.Http;
using System.Threading.Tasks;

class HttpClientExample
{
    static async Task Main(string[] args)
    {
        // Адрес сервера (совпадает с префиксом сервера)
        string serverUrl = "http://127.0.0.1:8888/connection/";

        using var client = new HttpClient();

        try
        {
            Console.WriteLine($"Отправка запроса на {serverUrl}...");
            HttpResponseMessage response = await client.GetAsync(serverUrl);

            // Проверяем, успешен ли ответ
            response.EnsureSuccessStatusCode();

            // Получаем содержимое в виде строки
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Ответ сервера:");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Ошибка при запросе: {ex.Message}");
        }

        Console.ReadLine();
    }
}