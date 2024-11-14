using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace HTTPServer
{
    struct HTTPHeaders //mb (Dictionary<string,string>)
    {
        public string Method;  //хранит метод, с помощью которого делается запрос
        public string RealPath; //хранит полный путь до файла на нашем сервере(пример: /var/www/index.html)
        public string File; //хранит не полный путь до файла(пример: index.html)
        public static HTTPHeaders Parse(string headers)
        {
            HTTPHeaders result = new HTTPHeaders();
            result.Method = Regex.Match(headers, @"\A\w[a-zA-Z]+", RegexOptions.Multiline).Value; //better to create parser
            result.File = Regex.Match(headers, @"(?<=\w\s)([\Wa-zA-Z0-9]+)(?=\sHTTP)", RegexOptions.Multiline).Value;
            result.RealPath = $"{AppDomain.CurrentDomain.BaseDirectory}{result.File}";
            return result;
        }

        public static string FileExtention(string file)
        {
            return Regex.Match(file, @"(?<=[\W])\w+(?=[\W]{0,}$)").Value;
        }
    }

    public class Client
    {
        Socket _client; // подключенный клиент
        HTTPHeaders Headers; // распарсенные заголовки
        public Client(Socket socket)
        {
            _client = socket;
            byte[] data = new byte[_client.ReceiveBufferSize]; // _client.ReceiveBufferSize - хранит значение полученных данных
            string request = "";
            _client.Receive(data); // считываем входящий запрос и записываем его в наш буфер data
            request = Encoding.UTF8.GetString(data); // преобразуем принятые нами байты с помощью кодировки UTF8 в читабельный вид
            
            if (request == "")
            {
                _client.Close();
                return;
            }

            Headers = HTTPHeaders.Parse(request);
            Console.WriteLine($"[{_client.RemoteEndPoint}]\nFile: {Headers.File}\nDate: {DateTime.Now}");

            if (Headers.RealPath.IndexOf("..") != -1)
            {
                SendError(404);
                _client.Close();
                return;
            }

            if (File.Exists(Headers.RealPath))
            {
                GetSheet();
            }
            else
            {
                SendError(404);
            }
            _client.Close();
        }

        public void GetSheet()
        {
            try
            {
                string contentType = GetContentType();
                FileStream fs = new FileStream(Headers.RealPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                string headers = $"HTTP/1.1 200 OK\nContent-type: {contentType}\nContent-Length: {fs.Length}\n\n";
                // OUTPUT HEADERS
                byte[] data = Encoding.UTF8.GetBytes(headers);
                _client.Send(data, data.Length, SocketFlags.None);
                // OUTPUT CONTENT
                data = new byte[fs.Length];
                int length = fs.Read(data, 0, data.Length);
                _client.Send(data, data.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                // просто выводит ошибку, после появления какой-либо новой ошибки
                // стоит создать новый блок обработки ошибок catch
                Console.WriteLine($"Func: GetSheet()    link: {Headers.RealPath}\nException: {ex}/nMessage: {ex.Message}");
            }
        }

        string GetContentType()
        {
            string result = "";
            string format = HTTPHeaders.FileExtention(Headers.File);
            switch (format)
            {
                //image
                case "gif":
                case "jpeg":
                case "pjpeg":
                case "png":
                case "tiff":
                case "webp":
                    result = $"image/{format}";
                    break;
                case "svg":
                    result = $"image/svg+xml";
                    break;
                case "ico":
                    result = $"image/vnd.microsoft.icon";
                    break;
                case "wbmp":
                    result = $"image/vnd.map.wbmp";
                    break;
                case "jpg":
                    result = $"image/jpeg";
                    break;
                // text
                case "css":
                    result = $"text/css";
                    break;
                case "html":
                    result = $"text/{format}";
                    break;
                case "javascript":
                case "js":
                    result = $"text/javascript";
                    break;
                case "php":
                    result = $"text/html";
                    break;
                case "htm":
                    result = $"text/html";
                    break;
                default:
                    result = "application/unknown";
                    break;
            }
            return result;
        }

        public void SendError(int code)
        {
            string html = $"<html><head><title></title></head><body><h1>Error {code}</h1></body></html>";
            string headers = $"HTTP/1.1 {code} OK\nContent-type: text/html\nContent-Length: {html.Length}\n\n{html}";
            byte[] data = Encoding.UTF8.GetBytes(headers);
            _client.Send(data, data.Length, SocketFlags.None);
            _client.Close();
        }

    }
}
