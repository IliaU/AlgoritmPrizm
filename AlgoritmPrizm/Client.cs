using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

namespace AlgoritmPrizm
{
    public class Client
    {
        /// <summary>
        /// Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        /// </summary>
        /// <param name="client"></param>
        public Client(TcpClient client)
        {
            // Объявим строку, в которой будет хранится запрос клиента
            string request = "";
            //Буфер для хранения принятых от клиента данных
            byte[] buffer = new byte[1024];
            //Переменная для хранения количества байт, принятых от клиента
            int count;

            //Читаем из потока клиента до тех пор, пока от него поступают данные
            while((count=client.GetStream().Read(buffer,0,buffer.Length))>0)
            {
                // Преобразуем эти данные в строку и добавим ее к переменной Request
                request += Encoding.UTF8.GetString(buffer, 0, count);

                // Запрос должен обрываться последовательностью \r\n\r\n
                // Либо обрываем прием данных сами, если длина строки Request превышает 4 килобайта
                // Нам не нужно получать данные из POST-запроса (и т. п.), а обычный запрос
                // по идее не должен быть больше 4 килобайт

                if (request.IndexOf("\r\n\r\n") >= 0 || request.Length > 4096)
                {
                    break;
                }
            }

            // Парсим строку запроса с использованием регулярных выражений
            // При этом отсекаем все переменные GET-запроса
            Match ReqMatch = Regex.Match(request, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");

            // Если запрос не удался
            if (ReqMatch == Match.Empty)
            {
                // Передаем клиенту ошибку 400 - неверный запрос
                SendError(client, 400);
                return;
            }

            // Получаем строку запроса
            string RequestUri = ReqMatch.Groups[1].Value;

            // Приводим ее к изначальному виду, преобразуя экранированные символы
            // Например, "%20" -> " "
            RequestUri = Uri.UnescapeDataString(RequestUri);

            // Если в строке содержится двоеточие, передадим ошибку 400
            // Это нужно для защиты от URL типа http://example.com/../../file.txt
            if (RequestUri.IndexOf("..") >= 0)
            {
                SendError(client, 400);
                return;
            }

            // Если строка запроса оканчивается на "/", то добавим к ней index.html
            if (RequestUri.EndsWith("/"))
            {
                RequestUri += "index.html";
            }
            


            // Код простой HTML-странички
            string html = "";// "<html><head><meta charset='utf8'></head><body>Привет Мир!</body></html>";
            string str = "HTTP/1.1 200 OK\nContent-type: application/json\nContent-Length:" + html.Length.ToString() + "\n\n" + html;
            byte[] bufferresp = Encoding.UTF8.GetBytes(str);

            // Отправим его клиенту
            client.GetStream().Write(bufferresp, 0, bufferresp.Length);
            client.Close();
        }

        private void SendError(TcpClient client, int code)
        {
            // Получаем строку вида "200 OK"
            // HttpStatusCode хранит в себе все статус-коды HTTP/1.1
            string CodeStr = code.ToString() + " " + ((HttpStatusCode)code).ToString();

            //Код простой HTML - странички
            string html = "<html><head><meta charset='utf8'></head><body><h1>"+ CodeStr + "</h1></body></html>";

            // Необходимые заголовки: ответ сервера, тип и длина содержимого. После двух пустых строк - само содержимое
            string Str = "HTTP/1.1 " + CodeStr + "\nContent-type: text/html\nContent-Length:" + html.Length.ToString() + "\n\n" + html;

            // Приведем строку к виду массива байт
            byte[] Buffer = Encoding.UTF8.GetBytes(Str);
            // Отправим его клиенту
            client.GetStream().Write(Buffer, 0, Buffer.Length);
            // Закроем соединение
            client.Close();
        }

    }
}
