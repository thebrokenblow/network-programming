using System.Net;
using System.Net.Sockets;
using System.Text;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

var message = "Some text";
var bytes = Encoding.UTF8.GetBytes(message);

var ipAddress = IPAddress.Parse("172.0.0.1");
var ipEndPoint = new IPEndPoint(ipAddress, 8888);

await socket.SendToAsync(bytes, ipEndPoint);
Console.ReadLine();