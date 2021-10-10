using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AlgoritmPrizm.Lib;


namespace AlgoritmPrizm.Com.SmtpLib
{
    /// <summary>
    /// Класс реализующий HTTP клиента
    /// </summary>
    public class HTTPClient
    {
        #region Private Param
        /// <summary>
        /// Поток, который отправляет сообщения на указанный Uri
        /// </summary>
        private Thread Thr;

        /// <summary>
        /// Поток, который проверяет сообщение надоставку клиенту
        /// </summary>
        private Thread ThrVerif;

        /// <summary>
        /// Oчередь в которой будут сообщения, которые нужно отправить
        /// </summary>
        private Queue<Mail> queue = new Queue<Mail>();

        /// <summary>
        /// Oчередь в которой будут сообщения, которые нужно проверить на доставку клиенту
        /// </summary>
        private Queue<Mail> queueVerif = new Queue<Mail>();
        #endregion

        #region Puplic Param

        /// <summary>
        /// Количество успешно отправленных сообщений
        /// </summary>
        public int SendSuccessMes { get; private set; }

        /// <summary>
        /// Количество не отправленных сообщений
        /// </summary>
        public int SendErrorMes { get; private set; }

        /// <summary>
        /// Статус асинхронного процесса
        /// </summary>
        public bool HashRun { get; private set; }

        /// <summary>
        /// Ссылка на который идёт отправка например: https://api.infobip.com
        /// </summary>
        public string HttpUri { get; private set; }

        /// <summary>
        /// Тип контекста, например application/json
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Http заголовок Accept, например application/json
        /// </summary>
        public string Accept { get; private set; }

        /// <summary>
        /// Метод, например POST
        /// </summary>
        public EnHttpMethod HttpMethod { get; private set; }

        /// <summary>
        /// Тип авторизации, например BasicToBase64
        /// </summary>
        public EnHttpAuthorizTyp HttpAuthorizTyp { get; private set; }

        /// <summary>
        /// Пользователь под которым залогиниться на сервер
        /// </summary>
        public string AudentUser { get; private set; }

        /// <summary>
        /// Пароль пользователя под котором залогинится на сервер
        /// </summary>
        public string AudentPassword { get; private set; }


        /// <summary>
        /// Событие отпраки сообщения
        /// </summary>
        public event EventHandler<EventSmtpClient> onEventSendMail;
        #endregion

        #region Puplic metod
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="HttpUri">Ссылка на который идёт отправка например: https://api.infobip.com</param>
        /// <param name="ContentType">Тип контекста, например application/json</param>
        /// <param name="Accept">Http заголовок Accept, например application/json</param>
        /// <param name="HttpMethod">Метод, например POST</param>
        /// <param name="HttpAuthorizTyp">Тип авторизации, например BasicToBase64</param>
        /// <param name="AudentUser">Пользователь под которым залогиниться на сервер</param>
        /// <param name="AudentPassword">Пароль пользователя под котором залогинится на сервер</param>
        public HTTPClient(string HttpUri, string ContentType, string Accept, EnHttpMethod HttpMethod, EnHttpAuthorizTyp HttpAuthorizTyp, string AudentUser, string AudentPassword)
        {
            this.HttpUri = HttpUri;
            this.ContentType = ContentType;
            this.Accept = Accept;
            this.HttpMethod = HttpMethod;
            this.HttpAuthorizTyp = HttpAuthorizTyp;
            this.AudentUser = AudentUser;
            this.AudentPassword = AudentPassword;




            /*

                        // Создаём подключение к серверу
                        Uri uri = new Uri(@"https://api.infobip.com/sms/1/text/single");
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                        request.Timeout = 100000000;
                        request.ContentType = @"application/json";
                        request.Accept = @"application/json";
                        request.Method = "POST";
                        request.KeepAlive = true;  // Держать подключение для отправки многих писем
                        string credentials = string.Format("{0}:{1}", @"fp_rus", @"g$3Tc7A7&1*");
                        request.Headers["Authorization"] = string.Format("Basic {0}", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(credentials)));
                        //request.Credentials = new NetworkCredential(@"fp_rus", @"g$3Tc7A7&1*");

                        if (true) // post 
                        {
                            // Получаем массив в байтах
                            byte[] MessOut = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(@"
            {  
               ""from"":""InfoSMS"",
               ""to"":""+79163253757"",
               ""text"":""My first первое Infobip SMS""
            }
            "));

                            request.ContentLength = MessOut.Length;

                            // Закачиваем то что нам нужно передать на сервер
                            using (Stream reqStream = request.GetRequestStream())
                            {
                                reqStream.Write(MessOut, 0, MessOut.Length);
                            }
                        }
                        else
                        {
                            //if (Method == Lib.MethodEn.POST_GET) request.Method = "GET";
                            //else request.Method = Method.ToString();
                        }

                        try
                        {
                            // Делаем запрос и получаем ответ
                            HttpWebResponse responce = (HttpWebResponse)request.GetResponse();

                            // Создаём потоки и читаем нужную инфу с сервера
                            using (Stream respStream = responce.GetResponseStream())
                            {
                                using (StreamReader respReader = new StreamReader(respStream, Encoding.UTF8))
                                {
                                    string rez = respReader.ReadToEnd();
            //                        string rez = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(respReader.ReadToEnd())));



            //                         {"messages":[{"to":"79163253757","status":{"groupId":1,"groupName":"PENDING","id":7,"name":"PENDING_ENROUTE","description":"Message sent to next instance"},"smsCount":1,"messageId":"2327841630171630995"}]}
            //                         {"messages":[{"to":"79161705674","status":{"groupId":1,"groupName":"PENDING","id":7,"name":"PENDING_ENROUTE","description":"Message sent to next instance"},"smsCount":1,"messageId":"2327886703771631815"}]}


                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            string err = ex.Message;
                        }
            */
        }

        /// <summary>
        /// Добавление в очередь сообщения для отправки
        /// </summary>
        /// <param name="eMail">Сообщение которое нужно отправить</param>
        public void AddEmail(Mail eMail)
        {
            lock (this.queue)
            {
                this.queue.Enqueue(eMail);
            }
        }

        /// <summary>
        /// Запуск процесса отправки сообщений
        /// </summary>
        public void StartSend()
        {
            if (!this.HashRun)
            {
                this.HashRun = true;
                this.SendSuccessMes = 0;
                this.SendErrorMes = 0;

                // асинхронный запуск нашего метода по отправке сообщений
                //Thread Thrtmp = new Thread(new ThreadStart(TaskThread.Run));
                this.Thr = new Thread(StartSendRun);
                this.Thr.Name = "HTTPClient";
                this.Thr.IsBackground = true;
                this.Thr.Start();

                //Запускаем поток который будет заниматься проверкой о доставке сообщения
                this.ThrVerif = new Thread(StartSendVerifRun);
                this.ThrVerif.Name = "HTTPClientVerif";
                this.ThrVerif.IsBackground = true;
                this.ThrVerif.Start();
            }
        }

        /// <summary>
        /// Остановка асинхронного отправления писем
        /// </summary>
        public void StopSend()
        {
            this.HashRun = false;
            if (this.Thr != null)
            {
                this.Thr.Join();
                this.Thr = null;
            }
            if (this.ThrVerif != null) this.ThrVerif.Join();
        }
        #endregion

        #region Private metod
        /// <summary>
        /// Асинхронная отправка писем
        /// </summary>
        private void StartSendRun()
        {
            // На всякий пожарный пауза на создание подключения к серверу, если оно ещё не создалось
            Thread.Sleep(2000);
            int QCount = 0;
            lock (this.queue)
            {
                QCount = this.queue.Count;
            }

            // Если процесс не завершился то асинхронная работа не заканчивается до завершения отправки всех писем
            while (this.HashRun || QCount > 0)
            {
                // Если в очереди ничего нет, нужно подождать может пользователь ещё закинет сообщений
                if (this.queue.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // Извлекаем сообщение
                Mail eMail = null;
                lock (this.queue)
                {
                    eMail = this.queue.Dequeue();
                    QCount = this.queue.Count;
                }
                if (eMail == null) continue;

                try
                {
                    Uri uri = new Uri(this.HttpUri);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 100000000;
                    request.ContentType = this.ContentType;
                    request.Accept = this.Accept;
                    request.KeepAlive = false;  // Держать подключение для отправки многих писем

                    // Добовляем заголовок для базовой авторизации
                    switch (this.HttpAuthorizTyp)
                    {
                        case EnHttpAuthorizTyp.BasicToBase64:
                            string credentials = string.Format("{0}:{1}", this.AudentUser, this.AudentPassword);
                            request.Headers["Authorization"] = string.Format("Basic {0}", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(credentials)));
                            break;
                        case EnHttpAuthorizTyp.Empty:
                        default:
                            break;
                    }

                    // Проверяем метод отправки
                    switch (this.HttpMethod)
                    {
                        case EnHttpMethod.POST:
                            request.Method = "POST";

                            // Получаем массив в байтах
                            byte[] MessOut = Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(eMail.Body));
                            request.ContentLength = MessOut.Length;

                            // Закачиваем то что нам нужно передать на сервер
                            using (Stream reqStream = request.GetRequestStream())
                            {
                                reqStream.Write(MessOut, 0, MessOut.Length);
                                reqStream.Close();
                            }
                            break;
                        case EnHttpMethod.GET:
                            request.Method = "GET";
                            break;
                        default:
                            request.Method = this.HttpMethod.ToString();
                            break;
                    }

                    bool AsincFlagVerif = false;
                    try
                    {
                        // Делаем запрос и получаем ответ
                        HttpWebResponse responce = (HttpWebResponse)request.GetResponse();

                        // Создаём потоки и читаем нужную инфу с сервера
                        using (Stream respStream = responce.GetResponseStream())
                        {
                            using (StreamReader respReader = new StreamReader(respStream, Encoding.UTF8))
                            {
                                string rez = respReader.ReadToEnd();
                                respReader.Close();

                                // Осуществляем дополнительную логику которая присуща каким-то конкретным провайдерам этой услуги
                                try
                                {
                                    // Если провайдером является infobip.com, то мы умеем получать идентификатор письма
                                    if (this.HttpUri.IndexOf(@"://api.infobip.com") > -1)
                                    {
                                        AsincFlagVerif = true;
                                        try
                                        {
                                            string template = @"""messageId"":""";
                                            int indStart = rez.IndexOf(template);
                                            int indEnd = rez.IndexOf(@"""}]}", indStart);
                                            eMail.SetMessageId(rez.Substring(indStart + template.Length, indEnd - indStart - template.Length));
                                        }
                                        catch (Exception ex)
                                        {
                                            // Обработка события
                                            Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}. Не смогли извлечь MessageId из ответа {1} для верификации отправки({2}).", eMail.To, rez, ex.Message), "StartSendRun", EventEn.Error);
                                            EventSmtpClient myArgErr = new EventSmtpClient(this, eMail, ex.Message);
                                            if (onEventSendMail != null)
                                            {
                                                this.SendErrorMes++;
                                                onEventSendMail.Invoke(this, myArgErr);
                                            }
                                            continue;
                                        }

                                    }
                                    /*else if (1 == 1)
                                    {
                                    }*/
                                    else
                                    {
                                        Com.Log.EventSave(string.Format("Мы не умеем получать идентифткатор письма от провайдера: ({0}) будет присвоен идентификатор по умолчанию, наш собственный.", eMail.To), "StartSendRun", EventEn.Message);
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                    // Если асинхронной проверки о доставке сообщения нет, то обрабатываем успешную отправку прямо в этом потоке
                    if (!AsincFlagVerif)
                    {
                        // Если есть запрос который нужно выполнить после отправки письма, то передаём его провайдеру
                        if (!string.IsNullOrWhiteSpace(eMail.PostSQL))
                        {
//                            Com.ProviderFarm.CurrentPrv.SetData(eMail.PostSQL);
                        }

                        // Собственно обработка события
                        EventSmtpClient myArg = new EventSmtpClient(this, eMail, "Success");
                        if (onEventSendMail != null)
                        {
                            this.SendSuccessMes++;
                            onEventSendMail.Invoke(this, myArg);
                        }

                        // Логируем в лог факт отправки письма
                        Com.Log.EventSave(string.Format("Сообщение успешно отправлено на адресс {0} с текстом сообщения {1}", eMail.To, eMail.Body), "StartSendRun", EventEn.Error);
                    }
                    else
                    {
                        // Добавляеминформацию по сообщению, чтобы очередь верификации знала о них и могда их читать
                        eMail.Tag = new DataTag(DateTime.Now, 10, 7);
                        lock (this.queueVerif)
                        {
                            this.queueVerif.Enqueue(eMail);
                        }
                    }

                }
                catch (HttpListenerException ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0} ({1})", eMail.To, ex.Message), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        this.SendErrorMes++;
                        onEventSendMail.Invoke(this, myArg);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0} ({1})", eMail.To, ex.Message), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        this.SendErrorMes++;
                        onEventSendMail.Invoke(this, myArg);
                    }
                }
            }
        }

        /// <summary>
        /// Асинхронная отправка писем
        /// </summary>
        private void StartSendVerifRun()
        {
            // На всякий пожарный пауза на создание подключения к серверу, если оно ещё не создалось
            Thread.Sleep(2000);
            int QCount = 0;
            lock (this.queueVerif)
            {
                QCount = this.queueVerif.Count;
            }

            // Если процесс не завершился то асинхронная работа не заканчивается до завершения проверки всех сообщений               
            while (this.HashRun || QCount > 0 || this.Thr != null)
            {
                // Если в очереди ничего нет, нужно подождать может пользователь ещё закинет сообщений
                if (this.queueVerif.Count == 0)
                {
                    Thread.Sleep(1000);
                    lock (this.queueVerif)
                    {
                        QCount = this.queueVerif.Count;
                    }
                    continue;
                }

                // Извлекаем сообщение
                Mail eMail = null;
                lock (this.queueVerif)
                {
                    eMail = this.queueVerif.Dequeue();
                    QCount = this.queueVerif.Count;
                }
                if (eMail == null) continue;

                try
                {
                    // Если письмо было отправлено провайдеру только только, то нужно немного подождать чтобы тать провайдеру отправить сообщение клиенту
                    if (eMail.Tag != null && ((DataTag)eMail.Tag).SendToProvider.AddSeconds(((DataTag)eMail.Tag).TimeWaitForRepiat) < DateTime.Now) Thread.Sleep(((DataTag)eMail.Tag).TimeWaitForRepiat * 1000);

                    Uri uri = null;
                    if (this.HttpUri.IndexOf(@"://api.infobip.com") > -1)
                    {
                        uri = new Uri(string.Format(@"https://api.infobip.com/sms/1/reports?messageId={0}", eMail.MessageId));
                    }

                    // Если адресс не найден, значит мы не умеем обрабатывать этот провайдер
                    if (uri == null)
                    {
                        // Обработка события
                        string myArgErrUriMes = string.Format("Произошло падение при отправке сообшения пользоваелю: {0}. Мы не умеем проверять доставку писем для провайдера: {0}", eMail.To, this.HttpUri);
                        Com.Log.EventSave(myArgErrUriMes, "StartSendVerifRun", EventEn.Error);
                        EventSmtpClient myArgErrUri = new EventSmtpClient(this, eMail, myArgErrUriMes);
                        if (onEventSendMail != null)
                        {
                            onEventSendMail.Invoke(this, myArgErrUri);
                        }
                        continue;
                    }

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 100000000;
                    request.ContentType = this.ContentType;
                    request.Accept = this.Accept;
                    request.KeepAlive = false;  // Держать подключение для отправки многих писем

                    // Добовляем заголовок для базовой авторизации
                    switch (this.HttpAuthorizTyp)
                    {
                        case EnHttpAuthorizTyp.BasicToBase64:
                            string credentials = string.Format("{0}:{1}", this.AudentUser, this.AudentPassword);
                            request.Headers["Authorization"] = string.Format("Basic {0}", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(credentials)));
                            break;
                        case EnHttpAuthorizTyp.Empty:
                        default:
                            break;
                    }

                    // Проверяем метод отправки
                    switch (this.HttpMethod)
                    {
                        case EnHttpMethod.POST:
                            if (this.HttpUri.IndexOf(@"://api.infobip.com") > -1) request.Method = "GET";
                            break;
                        case EnHttpMethod.GET:
                            request.Method = "GET";
                            break;
                        default:
                            request.Method = this.HttpMethod.ToString();
                            break;
                    }

                    try
                    {
                        // Делаем запрос и получаем ответ
                        HttpWebResponse responce = (HttpWebResponse)request.GetResponse();

                        // Создаём потоки и читаем нужную инфу с сервера
                        using (Stream respStream = responce.GetResponseStream())
                        {
                            using (StreamReader respReader = new StreamReader(respStream, Encoding.UTF8))
                            {
                                string rez = respReader.ReadToEnd();
                                respReader.Close();
                                //{"results":[{"messageId":"2329800842443535765","to":"79163253757","from":"InfoSMS","sentAt":"2018-07-30T19:48:04.247+0000","doneAt":"2018-07-30T19:48:05.928+0000","smsCount":1,"mccMnc":"null","price":{"pricePerMessage":0.0100000000,"currency":"RUB"},"status":{"groupId":3,"groupName":"DELIVERED","id":5,"name":"DELIVERED_TO_HANDSET","description":"Message delivered to handset"},"error":{"groupId":0,"groupName":"OK","id":0,"name":"NO_ERROR","description":"No Error","permanent":false}}]}

                                // Осуществляем дополнительную логику которая присуща каким-то конкретным провайдерам этой услуги
                                try
                                {
                                    // Если провайдером является infobip.com, то мы умеем получать идентификатор письма
                                    if (this.HttpUri.IndexOf(@"://api.infobip.com") > -1)
                                    {
                                        try
                                        {
                                            string status = null;
                                            if (rez == @"{""results"":[]}")
                                            {
                                                if (eMail.Tag != null) status = ((DataTag)eMail.Tag).TmpStatus;
                                            }
                                            else
                                            {
                                                string template = @"""description"":""";
                                                int indStart = rez.LastIndexOf(template);
                                                int indEnd = rez.IndexOf(@""",", indStart);
                                                status = rez.Substring(indStart + template.Length, indEnd - indStart - template.Length);
                                            }

                                            if (eMail.Tag != null && status != "No Error")
                                            {
                                                // Если не получилост, то нужно немного подождать чтобы было ещё несколько попыток
                                                if (((DataTag)eMail.Tag).CountRepeat > 0)
                                                {
                                                    ((DataTag)eMail.Tag).CountRepeat--;
                                                    ((DataTag)eMail.Tag).TmpStatus = status;
                                                    lock (this.queueVerif)
                                                    {
                                                        this.queueVerif.Enqueue(eMail);
                                                        QCount = this.queueVerif.Count;
                                                    }
                                                    Com.Log.EventSave(string.Format(@"Есть ошибка при отправке клиенту: {0}, но мы предпримем ещё {1} попыток.", status, ((DataTag)eMail.Tag).CountRepeat), "StartSendVerifRun", EventEn.Warning);
                                                    Thread.Sleep(((DataTag)eMail.Tag).TimeWaitForRepiat * 1000);
                                                    continue;
                                                }
                                                else
                                                {
                                                    // Обработка события
                                                    string ErrMessaStat = string.Format("Проверка статуса сообщения выдала ошибку: {0}", status);
                                                    Com.Log.EventSave(ErrMessaStat, "StartSendVerifRun", EventEn.Error);
                                                    EventSmtpClient myArgErr = new EventSmtpClient(this, eMail, ErrMessaStat);
                                                    if (onEventSendMail != null)
                                                    {
                                                        onEventSendMail.Invoke(this, myArgErr);
                                                    }
                                                    this.SendErrorMes++;
                                                    continue;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            // Обработка события
                                            Com.Log.EventSave(string.Format("Прроверка статуса сообщения выдала ошибку: {0}. Не смогли извлечь суть ошибки из ответа {1}).", eMail.To, rez, ex.Message), "StartSendVerifRun", EventEn.Error);
                                            EventSmtpClient myArgErr = new EventSmtpClient(this, eMail, ex.Message);
                                            if (onEventSendMail != null)
                                            {
                                                this.SendErrorMes++;
                                                onEventSendMail.Invoke(this, myArgErr);
                                            }
                                            continue;
                                        }

                                    }
                                    /*else if (1 == 1)
                                    {
                                    }*/
                                    else
                                    {
                                        Com.Log.EventSave(string.Format("Мы не умеем получать статус доставки сообщения от провайдера: ({0}).", eMail.To), "StartSendVerifRun", EventEn.Message);
                                        continue;
                                    }
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string err = ex.Message;
                    }

                    // Если есть запрос который нужно выполнить после отправки письма, то передаём его провайдеру
                    if (!string.IsNullOrWhiteSpace(eMail.PostSQL))
                    {
//                        Com.ProviderFarm.CurrentPrv.SetData(eMail.PostSQL);
                    }

                    // Собственно обработка события
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, "Success");
                    if (onEventSendMail != null)
                    {
                        this.SendSuccessMes++;
                        onEventSendMail.Invoke(this, myArg);
                    }

                    // Логируем в лог факт отправки письма
                    Com.Log.EventSave(string.Format("Сообщение успешно отправлено на адресс {0} с текстом сообщения {1}", eMail.To, eMail.Body), "StartSendRun", EventEn.Message);
                }
                catch (HttpListenerException ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0} ({1})", eMail.To, ex.Message), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        this.SendErrorMes++;
                        onEventSendMail.Invoke(this, myArg);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0} ({1})", eMail.To, ex.Message), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        this.SendErrorMes++;
                        onEventSendMail.Invoke(this, myArg);
                    }
                }


            }
        }


        /// <summary>
        /// Конвертация в нужную кодировку
        /// </summary>
        /// <param name="EnSource">Кодировка источника</param>
        /// <param name="enTarget">Кодировка приёмника</param>
        /// <param name="Data">Данные которые нужно перекодировать</param>
        /// <returns>Перекодированные данные</returns>
        private string ConvertEncoding(Encoding EnSource, Encoding enTarget, string Data)
        {
            byte[] tmpSource = EnSource.GetBytes(Data);
            byte[] tmpTarget = Encoding.Convert(EnSource, enTarget, tmpSource);
            return enTarget.GetString(tmpTarget);
        }
        #endregion

        /// <summary>
        /// промежуточный класс в котором будем хранить доп инфу которая принадлежит именно этому сообщению
        /// </summary>
        private class DataTag
        {
            /// <summary>
            /// Время в которое было отправлено данное сообщение провайдеру
            /// </summary>
            public DateTime SendToProvider;

            /// <summary>
            /// Количество повторов операции если не получилось отправить сообщение
            /// </summary>
            public int CountRepeat;

            /// <summary>
            /// Количество секунд между операциями повтора
            /// </summary>
            public int TimeWaitForRepiat;

            /// <summary>
            /// Промежуточный статус сообщения
            /// </summary>
            public string TmpStatus;

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="SendToProvider">Время в которое было отправлено данное сообщение провайдеру</param>
            public DataTag(DateTime SendToProvider, int CountRepeat, int TimeWaitForRepiat)
            {
                this.SendToProvider = SendToProvider;
                this.CountRepeat = CountRepeat;
                this.TimeWaitForRepiat = TimeWaitForRepiat;
            }
        }
    }
}
