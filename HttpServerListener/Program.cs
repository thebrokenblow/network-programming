using System.Net;
using System.Text;

var server = new HttpListener();
server.Prefixes.Add("http://127.0.0.1:8888/connection/");
server.Start();

var context = await server.GetContextAsync();

var response = context.Response;

string responseText =
    @"<!DOCTYPE html>
    <html>
        <head>
            <meta charset='utf8'>
            <title>METANIT.COM</title>
        </head>
        <body>
            <h2>Hello METANIT.COM</h2>
        </body>
    </html>";

byte[] buffer = Encoding.UTF8.GetBytes(responseText);

response.ContentLength64 = buffer.Length;
using Stream output = response.OutputStream;

await output.WriteAsync(buffer);
await output.FlushAsync();

Console.WriteLine("Запрос обработан");

server.Stop();