using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using RestSharp;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;
using System.Threading;

//https://habr.com/ru/post/120157/
//https://metanit.com/sharp/net/7.1.php

namespace AlgoritmPrizm.Com
{
    public partial class Web
    {
        private static Thread ThrWeb;

        private static HttpListener listener;
        private TcpListener tcpListener;
        private static string AuthSession;
        private static DateTime GetLastAuthSession;


        public static bool IsRunAsin;
        public static string Host { get; private set; }
        public static int Port { get; private set; }



        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Port">Порт который прослушиваем</param>
        public Web(string TmpHost, int TmpPort)
        {
            try
            {
                GetLastAuthSession = DateTime.Now;
                Host = TmpHost;
                Port = TmpPort;
                IsRunAsin = true;

                // Асинхронный запуск процесса
                ThrWeb = new Thread(AWeb);
                //ThrWeb = new Thread(new ParameterizedThreadStart(Run)); //Запуск с параметрами   
                ThrWeb.Name = "A_Thr_Web";
                ThrWeb.IsBackground = true;
                ThrWeb.Start();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке Web модуля с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.FatalError);
                throw ae;
            }
        }

        private static void AWeb()
        {
            try
            {
                while (IsRunAsin)
                {
                    try
                    {
                        listener = new HttpListener();
                        listener.Prefixes.Add(string.Format("http://{0}:{1}/", Host, Port));
                        listener.Start();

                        Listen(Host, Port);
                    }
                    catch (Exception ex)
                    {
                        ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке AWeb модуля с ошибкой: {0}", ex.Message));
                        Log.EventSave(ae.Message, "Com.Web.AWeb", EventEn.Error);
                        Thread.Sleep(10000);
                    }
                }
            }
            catch (Exception exf)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке AWeb модуля с ошибкой: {0}", exf.Message));
                Log.EventSave(ae.Message, "Com.Web.AWeb", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Стопим web модуль
        /// </summary>
        public static void StopAWeb()
        {
            try
            {
                IsRunAsin = false;
                ThrWeb.Abort();
                //ThrWeb.Join();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке остановке с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.AWeb.StopAWeb", EventEn.FatalError);
                throw ae;
            }
        }


        /// <summary>
        /// Обновление  документа в базе Prizm записываем фискальный номер документа
        /// </summary>
        /// <param name="Doc">Документ с которым работаем</param>
        /// <param name="NDocNum">Номер фискального документа</param>
        public static void UpdateFiskDocNum(JsonPrintFiscDoc Doc, int NDocNum)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.UpdateFiskDocNum", EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Запуск литнера
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Port"></param>
        private static void Listen(string Host, int Port)
        {
            try
            {
                // Цыкл для риёма запросов от пользователя
                while (IsRunAsin)
                {
                    string responceString = "";

                    //Получаем заголовки
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    string origin = request.Headers["Origin"];
                    string SecFetchMode = request.Headers["Sec-Fetch-Mode"];
                    string ContentType = "application/json";

                    try
                    {
                        // усли равен cors значит это предварительный запрос
                        if (string.IsNullOrWhiteSpace(SecFetchMode) || SecFetchMode != "cors")
                        {
                            // Читаем что передало приложение
                            string BufPostRequest = null;
                            using (var reader = new StreamReader(request.InputStream))
                            {
                                BufPostRequest = reader.ReadToEnd();
                            }


                            // В зависимости от того что хотят выполняем нужные опрации
                            switch (request.RawUrl)
                            {
                                case @"/marking":
                                    responceString = @"{""scan_marking"":""True""}";
                                   // responceString = @"{""error_data"":""True""}";
                                    break;
                                case @"/xreport":
                                    FR.XREport();
                                    break;
                                case @"/zreport":
                                    FR.ZREport();
                                    break;
                                case @"/openshift":
                                    FR.OpenShift();
                                    break;
                                case @"/sale":
                                    // FR.PrintCheck(BLL.JsonPrintFiscDoc.DeserializeJson(BufPostRequest), 1, "Рога и копыта");
                                    break;
                                case @"/printfiscdoc":
                                    FR.PrintCheck(BLL.JsonPrintFiscDoc.DeserializeJson(BufPostRequest), 1, "Рога и копыта");
                                    break;
                                case @"/AksRepItemHistory":
                                    try
                                    {
                                        // Получаем токен
                                        //string AuthSession = GetAuthenticationToken();
                                        ColbackDocument fil = FilterReportDocument;
                                        // Запрос данных
                                        List<JsonGetDocumentsData> Docs = GetDocumentsRestSharp(fil, true);
                                        // Возврат страницы пользователю
                                        responceString = RenderReportDocumentItemsMovement(Docs);
                                        ContentType = "text/html; charset=utf-8";
                                    }
                                    catch (Exception ex)
                                    {
                                        responceString = ex.Message;
                                    }
                                    
                                    break;
                                default:
                                    break;
                            }
                        }

                        // Передаём ответ серверу
                        response.ContentType = ContentType;
                        response.Headers.Add("Access-Control-Allow-Origin", origin);
                        response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Origin, Auth-Session, Content-Type");
                        //response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                        response.KeepAlive = true;
                        response.StatusCode = 200;
                        //
                        byte[] buffer = Encoding.UTF8.GetBytes(responceString);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception exw)
                    {
                        ApplicationException ae = new ApplicationException(string.Format("Упали при обработке запроса пользователя с ошибкой: {0}", exw.Message));
                        Log.EventSave(ae.Message, "Com.Web.Listen", EventEn.Warning);
                    }


                    
                    response.Close();

                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.Listen", EventEn.Error);
                throw ae;
            }
        }




        /// <summary>
        /// Делигат для получения документов построчно и принятия решения о выводе документа на основе фильтрации
        /// </summary>
        /// <param name="Document">Документ который возвращает призм и который можно филтрануть</param>
        /// <returns>Результат фильтрации если True то добавить в ответ, если False</returns>
        private delegate List<JsonGetDocumentJournal> ColbackDocument(JsonGetDocumentsData Document);

        /// <summary>
        /// Обработка фильтра для  принятия решения нужен нам этот документ или нет для отчёта движение товара
        /// </summary>
        /// <param name="Document">Документ который считался (его заголовок)</param>
        /// <returns></returns>
        private static List<JsonGetDocumentJournal> FilterReportDocument(JsonGetDocumentsData Document)
        {
            List<JsonGetDocumentJournal> rez = new List<JsonGetDocumentJournal>();
            try
            {
                // Получаем информацию по строкам документа
                List<JsonGetDocumentJournal> journal = GetDocumentsJournalRestSharp(Document);

                rez = journal;

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.FilterReportDocument", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получение списка документов удовлетворяющих фильтрации
        /// </summary>
        /// <param name="ColbackItemFilter">Можно передать делигат для того чтобы фильтрануть данные. || Если указать null то будет считать что без фильтрации</param>
        /// <param name="HashJournal">включить строчки тела документа (journal)</param>
        private static List<JsonGetDocumentsData> GetDocumentsRestSharp(ColbackDocument ColbackDocumentFilter, bool HashJournal)
        {
            List<JsonGetDocumentsData> Rez = new List<JsonGetDocumentsData>();
            string ResContent = null;

            try
            {
                int Page = 1;

                // Бесконечный цыкл для того чтобы пробежать по всем страницам
                while (true)
                {
                    // Проверяем наличие токена и если он протух то продлеваем его
                    GetAuthenticationToken();
                    ResContent = null;

                    // Обращаемся к серверу за списком документов
                    //Пример фильтрации http://172.16.1.102/api/backoffice/document?filter=sid,eq,603647062000327839&page_no=1&page_size=10
                    string url = string .Format("{0}/api/backoffice/document?cols=*&page_no={1}&page_size=10", Config.HostPrizmApi, Page);
                    RestClient _httpClient = new RestClient(url);
                    RestRequest request = new RestRequest { Method = Method.GET };
                    request.AddHeader("Accept", "application/json,version=2");
                    request.AddHeader("Auth-Session", AuthSession);

                    // Получаем ответ
                    IRestResponse response = _httpClient.Execute(request);
                    ResContent = response.Content;

                    // Получаем предварительный список документов и парсим
                    JsonGetDocuments doc = JsonGetDocuments.DeserializeJson(ResContent);

                    /* // Тут если надо делам ответ
                    if (response.IsSuccessful && response.ResponseStatus == ResponseStatus.Completed)
                    {

                    }*/

                    // Проверяем есть ли документы
                    if (doc.data.Count > 0)
                    {
                        // Пробегаем по догументам
                        foreach (JsonGetDocumentsData item in doc.data)
                        {
                            // Проверяем наличие фильтра если его нет то фильтрацию пропускаем и добавляем документ в список
                            if (ColbackDocumentFilter == null)
                            {
                                item.journal = GetDocumentsJournalRestSharp(item);
                                Rez.Add(item);
                            }
                            else
                            {
                                if (HashJournal)
                                {
                                    // Если успашна проверка то документ тоже добавляем в результат
                                    item.journal = ColbackDocumentFilter(item);
                                    Rez.Add(item);
                                }
                            }
                        }

                        Page++;
                    }
                    else break; // Если документов больше нет, то идём на выход
                }

                return Rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0} при парсинге следующего текста /r/n{1}", ex.Message, ResContent));
                Log.EventSave(ae.Message, "Com.Web.GetDocumentsRestSharp", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получаем списко строк которые относятся к документу
        /// </summary>
        /// <param name="doc">Документ к которому мы хотим получить строки</param>
        /// <returns>Список строк в документе</returns>
        private static List<JsonGetDocumentJournal> GetDocumentsJournalRestSharp(JsonGetDocumentsData doc)
        {
            List<JsonGetDocumentJournal> rez = new List<JsonGetDocumentJournal>();
            string ResContent = null;

            try
            {
                // Проверяем наличие токена и если он протух то продлеваем его
                GetAuthenticationToken();
                
                string url = string.Format("{0}/v1/rest/document/{1}/item?cols=*&page_no=1&page_size=10", Config.HostPrizmApi, doc.sid);
                RestClient _httpClient = new RestClient(url);
                RestRequest request = new RestRequest { Method = Method.GET };
                request.AddHeader("Accept", "application/json,version=2");
                request.AddHeader("Auth-Session", AuthSession);

                // Получаем ответ
                IRestResponse response = _httpClient.Execute(request);
                ResContent = response.Content;
                

                // Получаем предварительный список документов и парсим
                rez = JsonGetDocumentJournal.DeserializeJson(ResContent);

                /* // Тут если надо делам ответ
                if (response.IsSuccessful && response.ResponseStatus == ResponseStatus.Completed)
                {

                }*/

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0} при парсинге следующего текста /r/n{1}", ex.Message, ResContent));
                Log.EventSave(ae.Message, "Com.Web.GetDocumentsJournalRestSharp", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получаем список товаров
        /// </summary>
        /// <returns>Список строк в документе</returns>
        private static List<JsonGetInventory> GetInventoryRestSharp()
        {
            List<JsonGetInventory> rez = new List<JsonGetInventory>();
            string ResContent = null;

            try
            {
                // Проверяем наличие токена и если он протух то продлеваем его

                GetAuthenticationToken();

                string url = string.Format("{0}/api/backoffice/inventory?cols=*&page_no=1&page_size=10", Config.HostPrizmApi);
                RestClient _httpClient = new RestClient(url);
                RestRequest request = new RestRequest { Method = Method.GET };
                request.AddHeader("Accept", "application/json,version=2");
                request.AddHeader("Content-Type", "application/json,version=2");
                request.AddHeader("Auth-Session", AuthSession); // 

                // Получаем ответ
                IRestResponse response = _httpClient.Execute(request);
                ResContent = response.Content;

                //List<JsonRestError> Er = JsonRestError.DeserializeJson(ResContent);

                // Получаем предварительный список документов и парсим
                rez = JsonGetInventory.DeserializeJson(ResContent);

                /* // Тут если надо делам ответ
                if (response.IsSuccessful && response.ResponseStatus == ResponseStatus.Completed)
                {

                }*/

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0} при парсинге следующего текста /r/n{1}", ex.Message, ResContent));
                Log.EventSave(ae.Message, "Com.Web.GetDocumentsJournalRestSharp", EventEn.Error);
                throw ae;
            }
        }

        
        /// <summary>
        /// получаем токен для работы через API
        /// </summary>
        private static string GetAuthenticationToken()
        {
            try
            {
                //  Проверяем нужно ли получать токен заново или подойдёт существующий
                if (GetLastAuthSession.AddMinutes(Config.PrizmApiTimeLiveTockenMinute) < DateTime.Now || AuthSession==null)
                {
                    // Делаем запрос1 для получения  числа
                    string url = string.Format("{0}/v1/rest/auth", Config.HostPrizmApi);
                    WebRequest req1 = HttpWebRequest.Create(url);
                    req1.Method = "GET";
                    req1.ContentType = "application/json; charset=UTF-8";
                    req1.Headers.Add(HttpRequestHeader.ContentEncoding, "utf-8");
                    //req.Headers.Add(HttpRequestHeader.ContentLength, "-1");
                    req1.Headers.Add("ContentRangeEnd", "-1");
                    req1.Headers.Add("ContentRangeStart", "-1");
                    req1.Headers.Add("ContentRangeInstanceLength", "-1");
                    req1.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                    req1.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
                    //req.Headers.Add(HttpRequestHeader.Accept, "ACCEPT");


                    WebResponse resp = req1.GetResponse();
                    string AuthNoce = resp.Headers["Auth-Nonce"].ToString();

                    /*
                    using (Stream str = resp.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(str))
                        {
                            string resp1 =reader.ReadToEnd();
                        }
                    }*/


                    // Запрос 2 для получения токена
                    url = string.Format("{0}/v1/rest/auth?usr={1}&pwd={2}", Config.HostPrizmApi, Config.PrizmApiSystemLogon, Config.PrizmApiSystemPassord);
                    WebRequest req2 = HttpWebRequest.Create(url);
                    req2.Method = "GET";
                    req2.ContentType = "application/json; charset=UTF-8";
                    req2.Headers.Add(HttpRequestHeader.ContentEncoding, "utf-8");
                    req2.Headers.Add("ContentRangeEnd", "-1");
                    req2.Headers.Add("ContentRangeStart", "-1");
                    req2.Headers.Add("ContentRangeInstanceLength", "-1");
                    req2.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                    req2.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
                    req2.Headers.Add("Auth-Nonce", AuthNoce);
                    req2.Headers.Add("Auth-Nonce-Response", (int.Parse(AuthNoce) / 13 % 99999 * 17).ToString());


                    WebResponse resp2 = req2.GetResponse();

                    // получить токен
                    AuthSession = resp2.Headers["Auth-Session"].ToString();


                    // Фиксируем вход в Prizm https://{SERVERNAME}/v1/rest/sit?ws={WORKSTATIONNAME}
                    url = string.Format("{0}/v1/rest/sit?ws={1}", Config.HostPrizmApi, Environment.MachineName);
                    WebRequest req3 = HttpWebRequest.Create(url);
                    req3.Method = "GET";
                    req3.ContentType = "application/json; charset=UTF-8";
                    req3.Headers.Add(HttpRequestHeader.ContentEncoding, "utf-8");
                    req3.Headers.Add("ContentRangeEnd", "-1");
                    req3.Headers.Add("ContentRangeStart", "-1");
                    req3.Headers.Add("ContentRangeInstanceLength", "-1");
                    req3.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                    req3.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
                    req3.Headers.Add("Auth-Session", AuthSession);


                    WebResponse resp3 = req3.GetResponse();



                    // запоминаем время когда получили токен
                    GetLastAuthSession = DateTime.Now;
                }

                // возвращаем токен
                return AuthSession;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.GetAuthenticationToken", EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Деструктор
        /// </summary>
        ~Web()
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



/*
        private static void GetDocuments(string AuthSession)
        {

            try
            {
                // Делаем запрос1 для получения  числа
                HttpWebRequest req1 = (HttpWebRequest)HttpWebRequest.Create("http://172.16.1.102/api/backoffice/document?cols=*&page_no=1&page_size=10");
                req1.Method = "GET";
                req1.ContentType = "application/json,version=2";
                req1.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                req1.Headers.Add(HttpRequestHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                req1.Headers.Add("Auth-Session", AuthSession);
                req1.KeepAlive = true;
                req1.Host = "172.16.1.102";
                req1.Referer = "http://172.16.1.102/api-explorer/";
                req1.UserAgent="Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.164 Safari/537.36";

                WebResponse resp1 = req1.GetResponse();
                //string AuthNoce = resp.Headers["Auth-Nonce"].ToString();

                
                using (Stream str = resp1.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(str))
                    {
                        string tmpresp1 =reader.ReadToEnd();
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
*/
