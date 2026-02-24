using System.Net;
using System.Net.Sockets;
using System.Text;

var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

var ipAddress = IPAddress.Parse("172.0.0.1");
var ipEndPoint = new IPEndPoint(ipAddress, 5555);
socket.Bind(ipEndPoint);

var buffer = new byte[1024];
var ipRemoveClient = new IPEndPoint(IPAddress.Any, 0);

var result = await socket.ReceiveFromAsync(buffer, ipRemoveClient);
var message = Encoding.UTF8.GetString(buffer, 0, result.ReceivedBytes);

Console.WriteLine(message);