using System.Net;
using System.Net.Sockets;
using System.Text;

using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
Console.WriteLine("Клиент запущен");

Console.Write("Введите первое сообщение: ");
var message1 = Console.ReadLine() ?? "Первое сообщение";

Console.Write("Введите второе сообщение: ");
var message2 = Console.ReadLine() ?? "Второе сообщение";

var data1 = Encoding.UTF8.GetBytes(message1);
var data2 = Encoding.UTF8.GetBytes(message2);

EndPoint remotePoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);

// Отправляем первое сообщение
int bytes1 = await udpSocket.SendToAsync(data1, remotePoint);
Console.WriteLine($"Отправлено {bytes1} байт: {message1}");

// Небольшая задержка для гарантии доставки
await Task.Delay(100);

// Отправляем второе сообщение
int bytes2 = await udpSocket.SendToAsync(data2, remotePoint);
Console.WriteLine($"Отправлено {bytes2} байт: {message2}");

Console.ReadLine();