using System.Net;
using System.Net.Sockets;
using System.Text;

var tcpListener = new TcpListener(IPAddress.Any, 1234);
try
{
    tcpListener.Start();    // запускаем сервер
    Console.WriteLine("Сервер запущен. Ожидание подключений... ");

    while (true)
    {
        // получаем подключение в виде TcpClient
        using var tcpClient = await tcpListener.AcceptTcpClientAsync();
        Console.WriteLine($"Входящее подключение: {tcpClient.Client.RemoteEndPoint}");
        if(tcpClient.Client.Connected)
        {
            var stream = tcpClient.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(DateTime.Now.ToString("dd MMM yy hh:mm:ss zzz"));
            await stream.WriteAsync(data);
            Console.WriteLine($"Клиенту {tcpClient.Client.RemoteEndPoint} отправлена дата");
            return;
        }
    }
}
finally
{
    tcpListener.Stop(); // останавливаем сервер
    Console.ReadLine();
}