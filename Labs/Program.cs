using HTTPServer;
using Labs;
using System.IO;
using System.Net.Sockets;
using System.Text;
using TCPChat;


//1
/*Console.WriteLine(NativeTest.GetFullMACAdress());

Console.WriteLine(NativeTest.GetNativeMacAdress());

NativeTest.GetNativeWorkgroup();*/

//2
/*ServerObject server = new ServerObject();// создаем сервер
await server.ListenAsync(); // запускаем сервер*/


//3
/*using TcpClient tcpClient = new TcpClient();
Console.WriteLine("Клиент запущен");
await tcpClient.ConnectAsync("127.0.0.1", 1234);

if (tcpClient.Connected)
    Console.WriteLine($"Подключение с {tcpClient.Client.RemoteEndPoint} установлено");
else
    Console.WriteLine("Не удалось подключиться");

byte[] data = new byte[512];
var stream = tcpClient.GetStream();
int bytes = await stream.ReadAsync(data);

string time = Encoding.UTF8.GetString(data, 0, bytes);
Console.WriteLine($"Текущее время: {time}");*/


//4

Server server = new Server("127.0.0.1", 80);
server.Start();


Console.ReadLine();


