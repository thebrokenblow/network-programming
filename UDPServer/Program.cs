using System.Net;
using System.Net.Sockets;
using System.Text;

using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555);
udpSocket.Bind(localIP);
Console.WriteLine("UDP-сервер запущен...");

var data = new byte[65536];
EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

// Получаем первое сообщение
var result1 = await udpSocket.ReceiveFromAsync(data, remoteIp);
var message1 = Encoding.UTF8.GetString(data, 0, result1.ReceivedBytes);
Console.WriteLine($"Получено {result1.ReceivedBytes} байт");
Console.WriteLine($"Удаленный адрес: {result1.RemoteEndPoint}");
Console.WriteLine($"Сообщение 1: {message1}");

// Не закрываем сокет после первого получения
// Убираем Shutdown

// Получаем второе сообщение
var result2 = await udpSocket.ReceiveFromAsync(data, remoteIp);
var message2 = Encoding.UTF8.GetString(data, 0, result2.ReceivedBytes);
Console.WriteLine($"Получено {result2.ReceivedBytes} байт");
Console.WriteLine($"Удаленный адрес: {result2.RemoteEndPoint}");
Console.WriteLine($"Сообщение 2: {message2}");

Console.ReadLine();