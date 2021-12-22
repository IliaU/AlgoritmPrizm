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
using AlgoritmPrizm.Com.Report;
using System.Net.Http;

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


        /// <summary>
        /// Асинхронный старт головоного процесса
        /// </summary>
        private void AWeb()
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

                        Log.EventSave(string.Format("Успешно запустили Listen (http://{0}:{1})", Host, Port), "Com.Web.AWeb", EventEn.Message);

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
                // Проверка входа
                GetAuthenticationToken();

                // Запрос на изменение номера фискального документа
                string url = string.Format("{0}/v1/rest/document/{1}?filter=row_version,eq,{2}&cols=*", Config.HostPrizmApi, Doc.sid, Doc.row_version);
                RestClient _httpClient = new RestClient(url);
                RestRequest request = new RestRequest { Method = Method.PUT };
                request.AddHeader("Accept", "application/json,version=2");
                request.AddHeader("Auth-Session", AuthSession);
                string payload = string.Format("[{{\"rowversion\": {0},\"{1}\": {2}}}]", Doc.row_version + 1, Config.FieldDocNum.ToString(), NDocNum);
                request.AddJsonBody(payload);
                IRestResponse response = _httpClient.Execute(request);
                string ResContent = response.Content;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.UpdateFiskDocNum", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Запуск литнера синхронно
        /// </summary>
        /// <param name="Host">Сервер</param>
        /// <param name="Port">Порт</param>
        private void  Listen(string Host, int Port)
        {
            try
            {
                // Цыкл для риёма запросов от пользователя
                while (IsRunAsin)
                {
                    //Получаем заголовки
                    HttpListenerContext context = listener.GetContext();

                    // Асинхронный запуск процесса
                    Thread Thr = new Thread(AListen); //Запуск с параметрами   
                    Thr.Name = "AListen";
                    Thr.IsBackground = true;
                    Thr.Start(context);
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
        /// Асинхронная обраотка запроса
        /// </summary>
        /// <param name="obj">Параметр HttpListenerContext</param>
        private void AListen(object obj)
        {
            try
            {
                // Получаеи контекст который в рамках подключения пользователя
                HttpListenerContext context = (HttpListenerContext)obj;

                // Переменная для ответа пользователю
                string responceString = "";
                bool HashFileResponce = false;      // По умолчанию мы не файл отправляем а текс
                List<BLL.JsonWordDotxParams> JsPar = null;  // Параметры если они есть

                //HttpListenerContext context = await listener.GetContextAsync();
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

                        ///AksRepStat?sid=a251c9b3-ca0c-4632-92a8-a6137dd78775
                        string[] RawUrl = request.RawUrl.Split('?');
                        string[] PowUrlParam = null;
                        if (RawUrl.Length > 1)
                        {
                            PowUrlParam = RawUrl[1].Split('&');
                        }

                        // В зависимости от того что хотят выполняем нужные опрации
                        switch (RawUrl[0])
                        {
                            case @"/marking":

                                bool HashProductClass = false;
                                bool Mandatory = Config.MandatoryDefault;

                                if (!Config.GetMatrixAlways)
                                {
                                    JsonDocMarking FineDoc = JsonDocMarking.DeserializeJson(BufPostRequest);
                                    string dcs_code = FineDoc.dcs_code;
                                    foreach (ProdictMatrixClass item in Config.ProdictMatrixClassList)
                                    {
                                        if (string.IsNullOrEmpty(Config.ProductMatrixEndOff.ToString()))
                                        {
                                            if (dcs_code == item.ProductClass)
                                            {
                                                Mandatory = item.Mandatory;
                                                HashProductClass = true;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (dcs_code.Split(Config.ProductMatrixEndOff)[0] == item.ProductClass)
                                            {
                                                Mandatory = item.Mandatory;
                                                HashProductClass = true;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (Config.GetMatrixAlways || HashProductClass)
                                {
                                    string sssstmp = "{" + string.Format(@"""scan_marking"":""True"", ""Mandatory"":""{0}""", Mandatory) + "}";
                                    responceString = sssstmp;
                                }

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
                            case @"/smsgateway":
                                JsonSms Sms = JsonSms.DeserializeJson(BufPostRequest);
                                SmsFarm.AddMessageSms(Sms, null);
                                break;
                            case @"/printfiscdoc":

                                // Десериализуем наш объект
                                JsonPrintFiscDoc Doc = JsonPrintFiscDoc.DeserializeJson(BufPostRequest);

                                // Проверка подключения к базе
                                if (!string.IsNullOrWhiteSpace(Doc.bt_cuid))
                                {
                                    if (Com.ProviderFarm.CurrentPrv != null && Com.ProviderFarm.CurrentPrv.HashConnect)
                                    {
                                        // проверяем что в настройках стоит в качестве поля в котором хранится инфа
                                        string FieldInnTyp;
                                        switch (Config.FieldInnTyp)
                                        {
                                            case FieldDocNumEn.Comment1:
                                                FieldInnTyp = Doc.comment1;
                                                break;
                                            case FieldDocNumEn.Comment2:
                                                FieldInnTyp = Doc.comment2;
                                                break;
                                            default:
                                                FieldInnTyp = null;
                                                break;
                                        }

                                        string[] coment;
                                        if (!string.IsNullOrWhiteSpace(FieldInnTyp))
                                        {
                                            coment = FieldInnTyp.Split(';');

                                            //Если это юрик
                                            if (coment.Length == 2 && coment[0].Trim().ToLower() == "legal")
                                            {
                                                if (string.IsNullOrWhiteSpace(Doc.bt_last_name)) throw new ApplicationException("Не указано наименование у юрлица");
                                                else
                                                {
                                                    // Пробегаем по типу оплаты
                                                    decimal SumDoc = 0;
                                                    foreach (JsonPrintFiscDocTender item in Doc.tenders)
                                                    {
                                                        //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                                                        switch (Doc.receipt_type)
                                                        {
                                                            case 0:
                                                                // Если тип оплаты нал
                                                                if (item.tender_type == Com.Config.TenderTypeCash && item.taken != 0) SumDoc = +(decimal)item.taken;

                                                                break;
                                                            case 1:
                                                                // Если тип оплаты нал
                                                                if (item.tender_type == Com.Config.TenderTypeCash && item.given != 0) SumDoc = +(decimal)item.given * -1;

                                                                break;
                                                            case 2:
                                                                // Депозит

                                                                break;
                                                            default:
                                                                throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                                                        }
                                                    }

                                                    // Сумма за текущий день по юрлицу
                                                    Decimal SumDocOld = Com.ProviderFarm.CurrentPrv.GetTotalCashSum(coment[1].Trim().ToLower(), Doc.created_datetime);

                                                    // Если есть привышение то ругаемся
                                                    if (SumDoc + SumDocOld >= Config.LimitCachForUrik) throw new ApplicationException("Ежедневный лимит по юрлицу исчерпан");

                                                    // Если всё ок то ругаться не нужно просто сохраняем ещё  сумму из текущего чека
                                                    Com.ProviderFarm.CurrentPrv.SetPrizmCustPorog(coment[1].Trim().ToLower(), Doc.sid, Doc.created_datetime, SumDoc);
                                                }
                                            }
                                        }
                                    }
                                    else throw new ApplicationException("Нет подключения к базе данных");
                                }


                                // Отправляем на печать
                                FR.PrintCheck(Doc, 1, "Рога и копыта");
                                break;
                            case @"/AksRepItemHistory":
                                try
                                {
                                    // Выставляем параемтры отчёта
                                    ReportItemsMovement Rep = new ReportItemsMovement(DateTime.Now);

                                    // Получаем данные в отчёт
                                    Rep.Docs = GetDocumentsRestSharp(Rep, true);

                                    // Отрисовываем отчёт
                                    responceString = Rep.RenderReport();
                                    ContentType = "text/html; charset=utf-8";
                                }
                                catch (Exception ex)
                                {
                                    responceString = ex.Message;
                                }
                                break;
                            case @"/AksRepItem":
                                try
                                {
                                    // Выставляем параемтры отчёта
                                    List<BLL.JsonWordDotxParams> JsWdDotxPor = BLL.JsonWordDotxParams.DeserializeJson(BufPostRequest);

                                    // Объект который будем возвращать пользователю
                                    JsonWebMessageResponce resp = new JsonWebMessageResponce();
                                    resp.Message = "Не известная ошибка";

                                    try
                                    {
                                        // Если есть какой нибудь параметр
                                        if (JsWdDotxPor.Count > 0 && !string.IsNullOrWhiteSpace(JsWdDotxPor[0].valueString))
                                        {
                                            switch (JsWdDotxPor[0].valueString)
                                            {
                                                case "ИНВ-3":
                                                    if (JsWdDotxPor.Count > 1 && !string.IsNullOrWhiteSpace(JsWdDotxPor[1].valueString))
                                                        resp.Message = ReportWordDotxFarm.CreateReportInf3(JsWdDotxPor[1].valueString);
                                                    break;
                                                case "ИНВ-8А":
                                                    if (JsWdDotxPor.Count > 1 && !string.IsNullOrWhiteSpace(JsWdDotxPor[1].valueString))
                                                        resp.Message = ReportWordDotxFarm.CreateReportInf8a(JsWdDotxPor[1].valueString);
                                                    break;
                                                case "ИНВ-19":
                                                    if (JsWdDotxPor.Count > 1 && !string.IsNullOrWhiteSpace(JsWdDotxPor[1].valueString))
                                                        resp.Message = ReportWordDotxFarm.CreateReportInf19(JsWdDotxPor[1].valueString);
                                                    break;
                                                case "PL":   // Прайс лист
                                                        resp.Message = ReportWordDotxFarm.CreateReportPl();
                                                    break;
                                                default:
                                                    resp.Message = string.Format("Нет в списке известных нам отчётов шаблона с именем: {0}", JsWdDotxPor[0].valueString);
                                                    break;
                                            }
                                        }
                                        else resp.Message = string.Format("Ни один параметр не задан не знаем как обрабатывать команду");
                                    }
                                    catch (Exception ex)
                                    {
                                        // Если проверка выдала исключение то сообщаем об этом пользователю
                                        resp.Message = ex.Message;
                                    }
                                    // Формируем сообщение для пользователя
                                    responceString = BLL.JsonWebMessageResponce.SerializeJson(resp);
                                }
                                catch (Exception ex)
                                {
                                    responceString = ex.Message;
                                }
                                break;
                            case @"/AksRepStat":
                                try
                                {
                                    if(PowUrlParam!=null && PowUrlParam.Length>0)
                                    {
                                        foreach (string item in PowUrlParam)
                                        {
                                            string[] tmpPar = item.Split('=');
                                            if (tmpPar.Length>1 && tmpPar[0]=="sid")
                                            {
                                                responceString = ReportWordDotxFarm.GetPathReport(tmpPar[1]);
                                                HashFileResponce = true;
                                                break;
                                            }
                                        }
                                        ContentType = "text/html; charset=utf-8";
                                    }
                                    else
                                    {
                                        // Отрисовываем статистику по всем отчётам которые есть в пуле отчёт
                                        try
                                        {
                                            responceString = ReportWordDotxFarm.AksRepStat();
                                        }
                                        catch (Exception ex)
                                        {
                                            responceString = ex.Message;
                                        }
                                       
                                        ContentType = "text/html; charset=utf-8";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    responceString = ex.Message;
                                }
                                break;
                            case @"/CashIncome":

                                // Выставляем параемтры отчёта
                                JsPar = BLL.JsonWordDotxParams.DeserializeJson(BufPostRequest);

                                // Для тестрования
                                // JsPar = new List<JsonWordDotxParams>();
                                // JsPar.Add(new JsonWordDotxParams() { valueDecimal = 100 });

                                try
                                {
                                    // Если есть какой нибудь параметр
                                    if (JsPar!=null && JsPar.Count > 0 && JsPar[0].valueDecimal != null)
                                    {
                                        FR.CashIncome((decimal)JsPar[0].valueDecimal);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Если проверка выдала исключение то сообщаем об этом пользователю
                                    responceString = ex.Message;
                                }

                                break;
                            case @"/CashOutcome":

                                // Выставляем параемтры отчёта
                                JsPar = BLL.JsonWordDotxParams.DeserializeJson(BufPostRequest);

                                // Для тестрования
                                // JsPar = new List<JsonWordDotxParams>();
                                // JsPar.Add(new JsonWordDotxParams() { valueDecimal = 100 });

                                try
                                {
                                    // Если есть какой нибудь параметр
                                    if (JsPar != null && JsPar.Count > 0 && JsPar[0].valueDecimal != null)
                                    {
                                        FR.CashOutcome((decimal)JsPar[0].valueDecimal);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Если проверка выдала исключение то сообщаем об этом пользователю
                                    responceString = ex.Message;
                                }

                                break;
                            case @"/CustomerBonus":

                                // Выставляем параемтры отчёта
                                JsPar = BLL.JsonWordDotxParams.DeserializeJson(BufPostRequest);

                                // Объект который будем возвращать пользователю
                                JsonWebResponceCustomerBonus respCustBon = new JsonWebResponceCustomerBonus();
       
                                try
                                {
                                    // Если есть какой нибудь параметр
                                    if (JsPar != null && JsPar.Count > 0 && JsPar[0].valueDecimal != null)
                                    {
                                        respCustBon.amount = Com.ProviderFarm.CurrentPrv.GetCustBon(JsPar[0].valueString);
                                    }

                                    // Формируем сообщение для пользователя
                                    responceString = BLL.JsonWebResponceCustomerBonus.SerializeJson(respCustBon);
                                }
                                catch (Exception ex)
                                {
                                    // Если проверка выдала исключение то сообщаем об этом пользователю
                                    responceString = ex.Message;
                                }

                                break;
                            case @"/OpenDrawer":
                                FR.OpenDrawer();
                                break;
                            case @"/config":
                                responceString = BLL.JsonConfig.SerializeObject(new BLL.JsonConfig(true));
                                break;
                            default:
                                break;
                        }
                    }

                    // Передаём ответ серверу
                    if (!HashFileResponce)  // По умолчанию текст а нефайл
                    {
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
                    else
                    {
                        try
                        {
                            // Указываем пользователю что это вордовый файл
                            String PathFileName = responceString;
                            switch (Path.GetExtension(PathFileName).ToUpper())
                            {
                                case ".DOC":
                                    response.ContentType = "application/msword";
                                    
                                    response.AddHeader("Content-Disposition", @"attachment; filename=AksReport(" + DateTime.Now.ToString() + ").Doc");
                                    break;
                                case ".XLSX":
                                    response.ContentType = "application/msexcel";
                                    response.AddHeader("Content-Disposition", @"attachment; filename=AksReport(" + DateTime.Now.ToString() + ").xlsx");
                                    break;
                                default:
                                    break;
                            }

                            //string FileName = Path.GetFileName(PathFileName.Replace(".dotx", ".doc"));

                            //string ffftmp = Path.GetFileName(PathFileName).Replace(".dotx", ".doc");
                            //response.AddHeader("Content-Disposition", "attachment; filename=" + "ggggg.doc");
                            //FileName = Encoding.Unicode.GetString(Encoding.Convert(Encoding.Default, Encoding.Unicode, Encoding.Default.GetBytes(FileName)));
                            //response.AddHeader("Content-Disposition", @"attachment; filename=""" + Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(FileName)))+@"""");

                            if (!File.Exists(responceString)) throw new ApplicationException(string.Format("Файл не обнаружен. ({0})", responceString));

                            // Получаем поток для передачи пользователю
                            Stream output = response.OutputStream;

                            // Читаем поток файловый и передаём его пользователю
                            using (FileStream read = new FileStream(PathFileName, FileMode.Open, FileAccess.Read))
                            {
                                byte[] buftmpb = new byte[read.Length];
                                response.ContentLength64 = buftmpb.Length;
                                read.Read(buftmpb, 0, buftmpb.Length);
                                output.Write(buftmpb, 0, buftmpb.Length);
                            }

                            response.Close();
                        }
                        catch (Exception ex)
                        {
                            // Объект который будем возвращать пользователю
                            JsonWebMessageResponce resp = new JsonWebMessageResponce();
                            resp.Message = ex.Message;
                            // Формируем сообщение для пользователя
                            string buferror = BLL.JsonWebMessageResponce.SerializeJson(resp);

                            // Передаём ответ серверу
                            response.ContentType = ContentType;
                            response.Headers.Add("Access-Control-Allow-Origin", origin);
                            response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Origin, Auth-Session, Content-Type");
                            //response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                            response.KeepAlive = true;
                            response.StatusCode = (int)HttpStatusCode.NotFound;
                            //
                            byte[] buffer = Encoding.UTF8.GetBytes(buferror);
                            response.ContentLength64 = buffer.Length;
                            Stream output = response.OutputStream;
                            output.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
                catch (Exception exw)
                {
                    string ErrorMessage = string.Format("Упали при обработке запроса пользователя с ошибкой: {0}", exw.Message);
                    ApplicationException ae = new ApplicationException(ErrorMessage);
                    Log.EventSave(ae.Message, "Com.Web.Listen", EventEn.Warning);

                    // пытаемся передать ошибку но при неудачи падать нельзя
                    try
                    {
                        // Передаём ответ серверу
                        response.ContentType = ContentType;
                        response.Headers.Add("Access-Control-Allow-Origin", origin);
                        response.Headers.Add("Access-Control-Allow-Headers", "Access-Control-Allow-Origin, Auth-Session, Content-Type");
                        //response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                        response.KeepAlive = true;
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        //
                        byte[] buffer = Encoding.UTF8.GetBytes(ErrorMessage);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception exxe) { }
                }
                response.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.AListen", EventEn.Error);
                throw ae;
            }
        }

        /*  список медиа форматов
<?xml version="1.0" encoding="utf-8" ?>
<!--
   Inernet Media Types
   http://en.wikipedia.org/wiki/Internet_media_type
-->
<mediaTypes>
   <mediaType>
      <contentType>application/vnd.ms-excel</contentType>
      <name>Microsoft Excel (tm)</name>
      <refUrl>http://www.iana.org/assignments/media-types/application/vnd.ms-excel</refUrl>
      <fileExtensions>
         <fileExtension>.xls</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>application/msword</contentType>
      <name>Microsoft Word</name>
      <refUrl>http://www.iana.org/assignments/media-types/application/msword</refUrl>
      <fileExtensions>
         <fileExtension>.doc</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>application/vnd.ms-powerpoint</contentType>
      <name>Microsoft Powerpoint (tm)</name>
      <refUrl>http://www.iana.org/assignments/media-types/application/vnd.ms-powerpoint</refUrl>
      <fileExtensions>
         <fileExtension>.ppt</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>application/pdf</contentType>
      <name>Portable Document Format</name>
      <refUrl>http://www.iana.org/assignments/media-types/application/vnd.ms-powerpoint</refUrl>
      <fileExtensions>
         <fileExtension>.pdf</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>image/jpeg</contentType>
      <name>JPEG JFIF image</name>
      <fileExtensions>
         <fileExtension>.jpg</fileExtension>
         <fileExtension>.jpeg</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>image/gif</contentType>
      <name>GIF image</name>
      <fileExtensions>
         <fileExtension>.gif</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>image/vnd.microsoft.icon</contentType>
      <name>ICO image</name>
      <refUrl>http://www.iana.org/assignments/media-types/image/vnd.microsoft.icon</refUrl>
      <fileExtensions>
         <fileExtension>.ico</fileExtension>
      </fileExtensions>
   </mediaType>
   <mediaType>
      <contentType>application/zip</contentType>
      <name>ZIP file</name>
      <refUrl>http://www.iana.org/assignments/media-types/application/zip</refUrl>
      <fileExtensions>
         <fileExtension>.zip</fileExtension>
      </fileExtensions>
   </mediaType>
</mediaTypes>
             
         */


        /// <summary>
        /// Делигат для получения строки и принятия решения о выводе документа на основе фильтрации
        /// </summary>
        /// <param name="journal">Строка которую фильтруем</param>
        /// <returns>Результат фильтрации если True то добавить в ответ, если False</returns>
        public delegate bool ColbackDocumentJournal(JsonGetDocumentJournal journal);

        /// <summary>
        /// Получение списка документов удовлетворяющих фильтрации
        /// </summary>
        /// <param name="ColbackItemFilter">Можно передать делигат для того чтобы фильтрануть данные. || Если указать null то будет считать что без фильтрации</param>
        /// <param name="HashJournal">включить строчки тела документа (journal)</param>
        public static List<JsonGetDocumentsData> GetDocumentsRestSharp(ReportLib.BReport ColbackDocumentFilter, bool HashJournal)
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
                                if (HashJournal)
                                {
                                    item.journal = GetDocumentsJournalRestSharp(item, null);
                                }
                                Rez.Add(item);
                            }
                            else
                            {
                                if (HashJournal)
                                {
                                    // Если успашна проверка то документ тоже добавляем в результат
                                    item.journal = ColbackDocumentFilter.ColbackDocument(item);
                                    if (item.journal.Count>0) Rez.Add(item);
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
        public static List<JsonGetDocumentJournal> GetDocumentsJournalRestSharp(JsonGetDocumentsData doc, ColbackDocumentJournal ColbackDocumentJournalFilter)
        {
            List<JsonGetDocumentJournal> rez = new List<JsonGetDocumentJournal>();
            string ResContent = null;

            try
            {
                int Page = 1;

                // Бесконечный цыкл для того чтобы пробежать по всем страницам
                while (true)
                {
                    // Проверяем наличие токена и если он протух то продлеваем его
                    GetAuthenticationToken();

                    string url = string.Format("{0}/v1/rest/document/{1}/item?cols=*&page_no={2}&page_size=10", Config.HostPrizmApi, doc.sid, Page);
                    RestClient _httpClient = new RestClient(url);
                    RestRequest request = new RestRequest { Method = Method.GET };
                    request.AddHeader("Accept", "application/json,version=2");
                    request.AddHeader("Auth-Session", AuthSession);

                    // Получаем ответ
                    IRestResponse response = _httpClient.Execute(request);
                    ResContent = response.Content;


                    // Получаем предварительный список документов и парсим
                    List<JsonGetDocumentJournal> tmp = JsonGetDocumentJournal.DeserializeJson(ResContent);


                    /* // Тут если надо делам ответ
                    if (response.IsSuccessful && response.ResponseStatus == ResponseStatus.Completed)
                    {

                    }*/

                    // Проверяем есть ли результат
                    if (tmp.Count>0)
                    {
                        if (ColbackDocumentJournalFilter==null)
                        {
                            rez = tmp;
                        }
                        else
                        {
                            foreach (JsonGetDocumentJournal item in tmp)
                            {
                                if (ColbackDocumentJournalFilter(item))
                                {
                                    rez.Add(item);
                                }
                            }
                        }
                    }
                    else break; // Если документов больше нет, то идём на выход

                    Page++;
                }

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
        public static string GetAuthenticationToken()
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
