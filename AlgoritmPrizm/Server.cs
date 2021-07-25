using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;

//https://habr.com/ru/post/120157/
//https://metanit.com/sharp/net/7.1.php

using System.Web;

// Объект, принимающий TCP-клиентов

namespace AlgoritmPrizm
{
    public class Server
    {
        private static HttpListener listener;
        private TcpListener tcpListener;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Port">Порт который прослушиваем</param>
        public Server(int Port)
        {
            //Task t = Listen(Port);


            //* 
            // Создаём и  запускаем слушателя
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            
            
            // Цыкл для риёма запросов от пользователя
            while(true)
            {
                new Client(tcpListener.AcceptTcpClient());
            }
            
            //*/
        }

        private async static Task Listen(int Port)
        {
            
            listener = new HttpListener();
            //listener.Prefixes.Add(string.Format("http://AlgoritmPrizm:{0}/", Port));
            listener.Prefixes.Add(string.Format("http://chudakov:{0}/", Port));
            //listener.Prefixes.Add(string.Format("http://172.16.1.10:{0}/", Port));
            listener.Start();

            // Цыкл для риёма запросов от пользователя
            while (true)
            {
                //TspListener.AcceptTcpClient();
                HttpListenerContext context = listener.GetContext();
                //Thread.Sleep(200);

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                //Thread.Sleep(200);

                
                  //  ss = request.Headers.GetValues("token").ToString();
                

                
                System.Collections.Specialized.NameValueCollection col = request.Headers;
                String[] arg1 = col.AllKeys;
                for (int i = 0; i < arg1.Length; i++)
                {
                    string Hedername = arg1[i];
                    string[] arg2 = col.GetValues(arg1[i]);
                }


               

                using (var reader = new StreamReader(request.InputStream))
                {
                    string ddd = reader.ReadToEnd();
                }


                string AuthNonce = null;
                string AuthNonceResp = null;

                /*
                HttpWebRequest requestN = (HttpWebRequest)WebRequest.Create(@"http://172.16.1.10/v1/rest/auth");
                try
                {
                    // Делаем запрос и получаем ответ
                    HttpWebResponse responceN = (HttpWebResponse)requestN.GetResponse();

                    System.Collections.Specialized.NameValueCollection colN = responceN.Headers;
                    String[] argN = colN.AllKeys;
                    for (int i = 0; i < argN.Length; i++)
                    {
                        string Hedername = argN[i];
                        string[] arg2 = colN.GetValues(argN[i]);

                        if (Hedername == "Auth-Nonce")
                        {
                            AuthNonce = arg2[0];
                            AuthNonceResp = ((int.Parse(AuthNonce) / 13)%99999).ToString();
                            AuthNonceResp = (int.Parse(AuthNonceResp) * 17).ToString();
                        }
                    }
                }
                catch (WebException ex) {}

                string AuthSession=null;
                HttpWebRequest requestN2 = (HttpWebRequest)WebRequest.Create(@"http://172.16.1.10/v1/rest/auth?usr=sysadmin&pwd=sysadmin");
                requestN2.Headers.Add("Auth-Nonce", AuthNonce);
                requestN2.Headers.Add("Auth-Nonce-Response", AuthNonceResp);
                try
                {
                    // Делаем запрос и получаем ответ
                    HttpWebResponse responceN2 = (HttpWebResponse)requestN2.GetResponse();

                    System.Collections.Specialized.NameValueCollection colN2 = responceN2.Headers;
                    String[] argN2 = colN2.AllKeys;
                    for (int i = 0; i < argN2.Length; i++)
                    {
                        string Hedername = argN2[i];
                        string[] arg2 = colN2.GetValues(argN2[i]);

                        if (Hedername == "Auth-Session")
                        {
                            AuthSession = arg2[0];
                        }
                    }
                }
                catch (WebException ex) { }

                

                HttpWebRequest requestN3 = (HttpWebRequest)WebRequest.Create(@"http://172.16.1.10/v1/rest/sit?ws=webclient");
                requestN2.Headers.Add("Auth-Session", AuthSession);
                try
                {
                    // Делаем запрос и получаем ответ
                    HttpWebResponse responceN2 = (HttpWebResponse)requestN2.GetResponse();

                    System.Collections.Specialized.NameValueCollection colN2 = responceN2.Headers;
                    String[] argN2 = colN2.AllKeys;
                    for (int i = 0; i < argN2.Length; i++)
                    {
                        string Hedername = argN2[i];
                        string[] arg2 = colN2.GetValues(argN2[i]);

                        if (Hedername == "Auth-Session")
                        {
                            AuthSession = arg2[0];
                        }
                    }
                }
                catch (WebException ex) { }

                */


                /*
                long ggg = request.ContentLength64;
                byte[] RequestBuffer = new byte[1024];
                int CountReq;
                string StrRequest="";
                while ((CountReq=request.InputStream.Read(RequestBuffer,0,RequestBuffer.Length))>0)
                {
                    // Преобразуем эти данные в строку и добавим ее к переменной Request
                    StrRequest += Encoding.UTF8.GetString(RequestBuffer, 0, CountReq);
                }
                */

                //Thread.Sleep(200);

                /*
                string responceString = "OK";// "<html><head><meta charset='utf8'></head><body>Привет Мир!</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responceString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
               */

                try
                {
                    String username = "sysadmin";
                    String password = "sysadmin";
                    String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
                    //response.Headers.Add("Authorization", "Basic " + encoded);
                    response.Headers.Add("Accept", @"*/*");
                    response.Headers.Remove("Connection");
                    response.Headers.Remove("Access-Control-Request-Method");
                    response.Headers.Remove("Host");
                    response.Headers.Remove("Access-Control-Request-Headers");
                    //                    response.Headers.Add("Auth-Session", AuthSession);
                    response.ContentType = "application/json";
                    //response.Headers.Add("Origin", "http://172.16.1.10");
                    response.Headers.Add("Access-Control-Allow-Origin", "http://172.16.1.10");
                    response.Headers.Add("Access-Control-Allow-Methods", "POST");
                    response.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.190 Safari/537.36");
                    response.Headers.Add("Sec-Fetch-Mode", "cors");
                    response.Headers.Add("Referer", "http://172.16.1.10/");
                    response.Headers.Add("Accept-Encoding", "gzip, deflate");
                    response.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                    //response.Headers.Add("Access-Control-Allow-Headers", "X-Custom-Header");
                    //response.Headers.Add("Access-Control-Max-Age", "86400");
                    //response.Headers.Add("Vary", "Accept-Encoding, Origin");
                    //response.Headers.Add(HttpRequestHeader.KeepAlive, "timeout=2, max=100");
                    //response.Headers.Add(HttpRequestHeader.Connection, "Keep-Alive");
                    response.KeepAlive = true;
           
                    response.StatusCode = 200;
                    response.Close();
                }
                catch (Exception ex)
                {
                    string ff = ex.Message;
                   
                }
                
            }
        }

        /// <summary>
        /// Деструктор
        /// </summary>
        ~Server()
        {
            if (tcpListener != null)
            {
                tcpListener.Stop();
            }

            if (listener != null)
            {
                listener.Close();
            }
        }
    }
}
