﻿using System;
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
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Текущий токен
        /// </summary>
        private static string CurToken;

        /// <summary>
        /// Последняя дата получения токена
        /// </summary>
        private static DateTime? LastGetCurToken;

        /// <summary>
        /// Токен который мы используем для работы с документами
        /// </summary>
        private static string CurTokenForIsmp;

        /// <summary>
        /// CDN  который мы используем для работы с документами
        /// </summary>
        private static string CurCdnForIsmp;

        /// <summary>
        /// Для блокировки запросов к принтеру чтобы шли в один поток
        /// </summary>
        private static object LockFG = new object();

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
                //При запуске приложения указываем SSL протокол который будет использовать наша программа
                ServicePointManager.SecurityProtocol = Config.WebSecurityProtocolType;

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

                // Флаг о необходимости отката изменений сделанных при печати чека
                bool Rollbackprintfiscdoc = false;
                JsonPrintFiscDoc RollbackDoc = null;

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

                            if (Com.Config.Trace) Com.Log.EventSave(string.Format("{0}\r\n\t{1}", request.RawUrl, BufPostRequest), "Com.Web.Listen", EventEn.Trace);
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

                                JsonDocMarking FineDoc = JsonDocMarking.DeserializeJson(BufPostRequest);
                                string login = Com.ProviderFarm.CurrentPrv.GetLoginFromEmplName(FineDoc.created_by);
                                if (string.IsNullOrWhiteSpace(login)) throw new ApplicationException(string.Format("Пользователь с логином {0} в базе не найден.", FineDoc.created_by));

                                // Проверка что это кассир который имеет право пробивать чеки
                                bool FAccessSale = false;
                                foreach (Custumer item in Config.customers)
                                {
                                    if (item.login.ToUpper() == login.Trim().ToUpper()
                                        && !string.IsNullOrWhiteSpace(item.inn))
                                    {
                                        FAccessSale = true;
                                        break;
                                    }
                                }
                                if (!FAccessSale) throw new ApplicationException(string.Format("указанный Вами сотрудник {0} не является кассиром. Смените сотрудника.", login));

                                if (!Config.GetMatrixAlways)
                                {
                                    //JsonDocMarking FineDoc = JsonDocMarking.DeserializeJson(BufPostRequest);
                                    string dcs_code = EnProductMatrixClassType.dcs_code.ToString();
                                    switch (Config.ProductMatrixClassType)
                                    {
                                        case EnProductMatrixClassType.dcs_code:
                                            dcs_code = FineDoc.dcs_code;
                                            break;
                                        case EnProductMatrixClassType.udf_string01:
                                            dcs_code = FineDoc.udf_string01;
                                            break;
                                        case EnProductMatrixClassType.udf_string02:
                                            dcs_code = FineDoc.udf_string02;
                                            break;
                                        case EnProductMatrixClassType.udf_string03:
                                            dcs_code = FineDoc.udf_string03;
                                            break;
                                        case EnProductMatrixClassType.udf_string04:
                                            dcs_code = FineDoc.udf_string04;
                                            break;
                                        case EnProductMatrixClassType.udf_string05:
                                            dcs_code = FineDoc.udf_string05;
                                            break;
                                        case EnProductMatrixClassType.udf_string06:
                                            dcs_code = FineDoc.udf_string06;
                                            break;
                                        case EnProductMatrixClassType.udf_string07:
                                            dcs_code = FineDoc.udf_string07;
                                            break;
                                        default:
                                            throw new ApplicationException(string.Format("Не можем обработать параметр который указан в конфиге: {0}", Config.ProductMatrixClassType));
                                    }
                                    
                                    foreach (ProdictMatrixClass item in Config.ProdictMatrixClassList)
                                    {
                                        // смотрим какой тип парсинга выбран пользователем
                                        switch (Config.MatrixParceTyp)
                                        {
                                            // Выбрано полное сравнение с значением в базе
                                            case EnMatrixParceTyp.Normal:
                                                if (dcs_code == item.ProductClass)
                                                {
                                                    Mandatory = item.Mandatory;
                                                    HashProductClass = true;
                                                    break;
                                                }
                                                break;
                                            // Выбрано сравнение на основе сепоратора
                                            case EnMatrixParceTyp.Seporate:
                                                if (dcs_code.Split(Config.ProductMatrixEndOff)[0] == item.ProductClass)
                                                {
                                                    Mandatory = item.Mandatory;
                                                    HashProductClass = true;
                                                    break;
                                                }
                                                break;
                                            // выбран парсинг на основе регулярного выражения например @"Туз(\w*)" для поиска всего что начинается с Туз
                                            // \w  означает любой алфавитно цыфровой символ
                                            // *   означает любое колисество
                                            case EnMatrixParceTyp.Regular:
                                                // Создаём регулярное выражение
                                                Regex regex = new Regex(item.ProductClass);
                                                // Применяем регулярное выражение ктексту
                                                MatchCollection mathes = regex.Matches(dcs_code);
                                                // Если найдено хоть одно значение то применяем правило
                                                if (mathes.Count>0)
                                                {
                                                    Mandatory = item.Mandatory;
                                                    HashProductClass = true;
                                                }
                                                break;
                                            default:
                                                break;
                                        }

                                        /*if (string.IsNullOrEmpty(Config.ProductMatrixEndOff.ToString()))
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
                                        }*/
                                    }
                                }

                                // Отображение информации на дисплей покупателя
                                if (FineDoc != null && Com.DisplayFarm.CurDisplay != null)
                                {
                                    //Описание товара
                                    InvnSbsItemText TmpTextInfo = ProviderFarm.CurrentPrv.GetInvnSbsItemText(FineDoc.invn_sbs_item_sid);
                                    string TmpDisplayOut = null;
                                    switch (Config.DisplayFieldItem)
                                    {
                                        case FieldItemEn.Description1:
                                            TmpDisplayOut = FineDoc.item_description1;
                                            break;
                                        case FieldItemEn.Description2:
                                            TmpDisplayOut = FineDoc.item_description2;
                                            break;
                                        case FieldItemEn.InvnSbsItemNo:
                                            TmpDisplayOut = FineDoc.item_pos.ToString();
                                            break;
                                        case FieldItemEn.Attribute:
                                            TmpDisplayOut = FineDoc.attribute;
                                            break;
                                        case FieldItemEn.ItemSize:
                                            TmpDisplayOut = FineDoc.item_size;
                                            break;
                                        case FieldItemEn.ScanUpc:
                                            TmpDisplayOut = FineDoc.scan_upc;
                                            break;
                                        case FieldItemEn.Text1:
                                            TmpDisplayOut = TmpTextInfo.Text1;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text2:
                                            TmpDisplayOut = TmpTextInfo.Text2;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text3:
                                            TmpDisplayOut = TmpTextInfo.Text3;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text4:
                                            TmpDisplayOut = TmpTextInfo.Text4;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text5:
                                            TmpDisplayOut = TmpTextInfo.Text5;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text6:
                                            TmpDisplayOut = TmpTextInfo.Text6;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text7:
                                            TmpDisplayOut = TmpTextInfo.Text7;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text8:
                                            TmpDisplayOut = TmpTextInfo.Text8;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text9:
                                            TmpDisplayOut = TmpTextInfo.Text9;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        case FieldItemEn.Text10:
                                            TmpDisplayOut = TmpTextInfo.Text10;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                        default:
                                            TmpDisplayOut = FineDoc.item_description1;
                                            if (TmpDisplayOut != null) TmpDisplayOut = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(TmpDisplayOut)));
                                            break;
                                    }
                                    Com.DisplayFarm.CurDisplay.ShowText(string.Format("{0} {1} руб.", TmpDisplayOut, FineDoc.price));
                                }

                                if (Config.GetMatrixAlways || HashProductClass)
                                {
                                    string sssstmp = "{" + string.Format(@"""scan_marking"":""True"", ""Mandatory"":""{0}""", Mandatory) + "}";
                                    responceString = sssstmp;
                                }

                                break;
                            case @"/CdnForIsmpCheckJson":
                            case @"/CdnForIsmpCheck":

                                JsonCndResponce rezcdn = new JsonCndResponce();
                                if (!string.IsNullOrWhiteSpace(BufPostRequest))
                                {
                                    // Пробуем сделать запрос через площадку CDN
                                    JsonCdnForIsmpResponce resp = null;
                                    
                                    try
                                    {
                                        //resp = Com.Web.CdnForIsmpCheck(BufPostRequest);
                                        WebCdnForIsmpCheckAsinh webrespAsinCdnCheck = new WebCdnForIsmpCheckAsinh(BufPostRequest);
                                        resp = webrespAsinCdnCheck.rez;
                                    }
                                    catch (Exception exCdn)
                                    {
                                        ApplicationException aeCdn = new ApplicationException(string.Format("Упали при оращении к порталу честный знак с ошибкой: {0} \r\nПереключились на локальный модуль.", exCdn.Message));
                                        Log.EventSave(aeCdn.Message, string.Format("{0}.AListen", GetType().Name), EventEn.Error, true, false);

                                        try
                                        {
                                            // Строим заголовки которые будем цеплять во все запросы
                                            List<HederHttp> HederHttpList = new List<HederHttp>();
                                            HederHttpList.Add(new HederHttp("Authorization", String.Format("Basic {0}", Convert.ToBase64String(Encoding.Default.GetBytes(string.Format(@"{0}:{1}", Config.EniseyLogin, Config.EniseyPassword))))));
                                            HederHttpList.Add(new Web.HederHttp("X-ClientId", Config.FrSerialNumber));

                                            //Построение запроса к базе Енисей
                                            JsonEniseyRequest reqEnisey = new JsonEniseyRequest();
                                            int GetObjectEnisey29tmp = BufPostRequest.IndexOf(string.Format("\u001d"));
                                            if (GetObjectEnisey29tmp <= 0) GetObjectEnisey29tmp = 31; // 25;
                                            reqEnisey.cis_list.Add(BufPostRequest.Substring(0, GetObjectEnisey29tmp));
                                            //JsonEniseyRequest reqEnisey = JsonEniseyRequest.DeserializeJson(JsonEniseyRequest.SampleTest);

                                            // Получение даных от Енисея
                                            string respEniseyTmp = Web.GetObjectEnisey(Web.MethodTyp.POST, @"/api/v1/cis/outCheck", "application/json", HederHttpList, null, true, Encoding.UTF8, JsonEniseyRequest.SerializeObject(reqEnisey), false, false);
                                            JsonEniseyResponce respEnisey = JsonEniseyResponce.DeserializeJson(respEniseyTmp);

                                            // Проверяем ответ от енисея
                                            if (respEnisey != null)
                                            {
                                                resp = new JsonCdnForIsmpResponce();
                                                resp.codes.Add(new JsonCdnForIsmpResponceItem());
                                                resp.codes[0].errorCode = respEnisey.results[0].code;
                                                resp.reqTimestamp = (long)respEnisey.results[0].reqTimestamp;
                                                resp.description = respEnisey.results[0].description;
                                                //
                                                if (respEnisey.results[0].codes[0].isBlocked == false
                                                    || respEnisey.results[0].codes[0].isGreyGtin == false)
                                                {
                                                    resp.codes[0].valid = true;
                                                    resp.codes[0].realizable = (respEnisey.results[0].description == "ok" ? true : false);
                                                    resp.codes[0].isBlocked = respEnisey.results[0].codes[0].isBlocked;
                                                    resp.codes[0].cis = respEnisey.results[0].codes[0].cis;
                                                    resp.reqId = respEnisey.results[0].reqId;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке с ошибкой: {0}", ex.Message));
                                            Log.EventSave(ae.Message, string.Format("{0}.AListen", GetType().Name), EventEn.Error, true, false);
                                            throw ae;
                                        }
                                    }
                                    
                                    
                                    // Если есть ответ
                                    if (resp != null)
                                    {
                                        rezcdn.Rezult = true;
                                        rezcdn.reqId = resp.reqId;
                                        rezcdn.reqTimestamp = resp.reqTimestamp;
                                    }

                                    // Проверка ответа КМ
                                    if (resp!=null && resp.codes.Count == 1 && resp.codes[0].valid && resp.codes[0].errorCode == 0 && resp.codes[0].realizable && !resp.codes[0].isBlocked)
                                    {
                                        Com.JsonCdnFarm.BufferAdd(resp);

                                        if (RawUrl[0] != @"/CdnForIsmpCheck") responceString = JsonCndResponce.SerializeObject(rezcdn);
                                    }
                                    else
                                    {
                                        string ErrorMesssage = string.Format("Матрикс код ({0}) не прошёл проверку - ошибка ! Удалите товар продажа не возможна!", BufPostRequest);

                                        if (RawUrl[0] == @"/CdnForIsmpCheck") throw new ApplicationException(ErrorMesssage);
                                        else
                                        {
                                            rezcdn.Rezult = false;
                                            rezcdn.Message = ErrorMesssage;
                                            responceString = JsonCndResponce.SerializeObject(rezcdn);
                                        }
                                    }
                                }
                                else
                                {
                                    string ErrorMesssage = "Не удалось проверить матрикс код так как он не задан";

                                    if (RawUrl[0] == @"/CdnForIsmpCheck") throw new ApplicationException(ErrorMesssage);
                                    else
                                    {
                                        rezcdn.Rezult = false;
                                        rezcdn.Message = ErrorMesssage;
                                        responceString = JsonCndResponce.SerializeObject(rezcdn);
                                    }
                                }

                                break;
                            case @"/xreport":
                                lock (LockFG)
                                {
                                    FR.XREport();
                                }
                                break;
                            case @"/zreport":
                                lock (LockFG)
                                {
                                    FR.ZREport();
                                }
                                break;
                            case @"/openshift":
                                lock (LockFG)
                                {
                                    FR.OpenShift();
                                }
                                break;
                            case @"/sale":
                                lock (LockFG)
                                {
                                    // FR.PrintCheck(BLL.JsonPrintFiscDoc.DeserializeJson(BufPostRequest), 1, "Рога и копыта");
                                }
                                break;
                            case @"/smsgateway":
                                JsonSms Sms = JsonSms.DeserializeJson(BufPostRequest);
                                SmsFarm.AddMessageSms(Sms, null);
                                break;
                            case @"/printfiscdoc":

                                // Десериализуем наш объект
                                JsonPrintFiscDoc Doc = JsonPrintFiscDoc.DeserializeJson(BufPostRequest);
                                RollbackDoc = Doc;

                                JsonPrintFiscDocReturn rezPrintCheck = null;
                                lock (LockFG)
                                {
                                    try
                                    {
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
                                                    case FieldDocNumEn.bt_address_line3:
                                                        FieldInnTyp = Doc.bt_address_line3;
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
                                                    if (
                                                            (coment.Length == 2 && coment[0].Trim().ToLower() == "legal")
                                                            ||(coment.Length == 1 && coment[0].Trim().Length == 10)
                                                       )
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

                                                            if (coment.Length == 2) FieldInnTyp = coment[1];

                                                            // Сумма за текущий день по юрлицу
                                                            Decimal SumDocOld = 0;
                                                            if (Config.CalculatedDaySumForUrik) SumDocOld = Com.ProviderFarm.CurrentPrv.GetTotalCashSum(FieldInnTyp.Trim().ToLower(), Doc.created_datetime);

                                                            // Если есть привышение то ругаемся
                                                            if (SumDoc + SumDocOld >= Config.LimitCachForUrik) throw new ApplicationException("Ежедневный лимит по юрлицу исчерпан");

                                                            // Если всё ок то ругаться не нужно просто сохраняем ещё  сумму из текущего чека
                                                            Com.ProviderFarm.CurrentPrv.SetPrizmCustPorog(FieldInnTyp.Trim().ToLower(), Doc.sid, Doc.created_datetime, SumDoc);
                                                        }
                                                    }
                                                }
                                            }
                                            else throw new ApplicationException("Нет подключения к базе данных");
                                        }


                                        // Обогощаем документ строками при ошибке они исчезают !!!!!!!
                                        if (RollbackDoc.items.Count(t => string.IsNullOrWhiteSpace(t.invn_sbs_item_sid) &&
                                                !string.IsNullOrWhiteSpace(t.link)) > 0)
                                        {
                                            List<BLL.JsonPrintFiscDocItem> DocTmp = Com.ProviderFarm.CurrentPrv.GetItemsForReturnOrder(RollbackDoc.sid);
                                            for (int i = 0; i < RollbackDoc.items.Count(); i++)
                                            {
                                                DocTmp[i].link = RollbackDoc.items[i].link;
                                                RollbackDoc.items[i] = DocTmp[i];
                                            }
                                        }

                                        //throw new ApplicationException("Наша тестовая ошибка");

                                        // Отправляем на печать
                                        rezPrintCheck = FR.PrintCheck(Doc, 1, "Рога и копыта");

                                    }
                                    catch (Exception ex)
                                    {
                                        Rollbackprintfiscdoc = true;

                                        throw ex;
                                    }
                                }

                                // Формируем сообщение для пользователя
                                responceString = BLL.JsonPrintFiscDocReturn.SerializeObject(rezPrintCheck);

                                break;
                            case @"/printdoccopy":
                                // Десериализуем наш объект
                                JsonPrintFiscDoc DocCopy = JsonPrintFiscDoc.DeserializeJson(BufPostRequest);

                                // Отправляем на печать
                                lock (LockFG)
                                {
                                    FR.PrintCheck(DocCopy, 1, "Рога и копыта", true);
                                }
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
                                                case "UPD":   // УПД
                                                    if (JsWdDotxPor.Count > 1 && !string.IsNullOrWhiteSpace(JsWdDotxPor[1].valueString))
                                                        resp.Message = ReportWordDotxFarm.CreateReportUdp(JsWdDotxPor[1].valueString);
                                                    break;
                                                case "Returnblank":
                                                    if (JsWdDotxPor.Count > 1 && !string.IsNullOrWhiteSpace(JsWdDotxPor[1].valueString))
                                                        resp.Message = ReportWordDotxFarm.CreateReportReturnBlankWrd(JsWdDotxPor[1].valueString);
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
                                        lock (LockFG)
                                        {
                                            FR.CashIncome((decimal)JsPar[0].valueDecimal);
                                        }
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
                                        lock (LockFG)
                                        {
                                            FR.CashOutcome((decimal)JsPar[0].valueDecimal);
                                        }
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
                                lock (LockFG)
                                {
                                    FR.OpenDrawer();
                                }
                                break;
                            case @"/display":
                                JsonDisplayParams DispPar = BLL.JsonDisplayParams.DeserializeJson(BufPostRequest);
                                if (DisplayFarm.CurDisplay != null) DisplayFarm.CurDisplay.ShowText(DispPar.text);
                                break;
                            case @"/config":
                                responceString = BLL.JsonConfig.SerializeObject(new BLL.JsonConfig(true));
                                break;
                            case @"/imei":

                                BLL.JsonImei rez = new BLL.JsonImei();
                                // Пробегаем по всем зарегистрированным IMEI
                                foreach (Com.LicLib.onLicEventKey item in Com.Lic._LicImeiKey)
                                {
                                    // Если текущая дата больше чем та до которой валидна лицензия
                                    if (item.ValidToYYYYMMDD > int.Parse(((DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + DateTime.Now.Day).ToString()))
                                    {
                                        // Пробегаем по списку IMEI и добавляем в результат
                                        foreach (string IMEI in item.ScnFullNameList)
                                        {
                                            rez.imei.Add(IMEI);
                                        }
                                    }
                                }

                                responceString = BLL.JsonImei.SerializeObject(rez);
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
                    // Проверяем наличие ошибки на принтере и если она есть то делаем откат заголовка до того как передать ошибку в браузер
                    if (Config.IsHeldForDocements && Rollbackprintfiscdoc && RollbackDoc != null)
                    {
                        // Устанавливаем признак отложенного документа
                        Com.ProviderFarm.CurrentPrv.SetIsHeldForDocements(RollbackDoc.sid, 1);

                        // Устанавливаем дату и признак отложенного документа если есть ошибка
                        Com.ProviderFarm.CurrentPrv.SetIsHelpRollbackDoc(RollbackDoc.sid);
                    }

                    string ErrorMessage = string.Format("Упали при обработке запроса пользователя с ошибкой: {0}", exw.Message);
                    ApplicationException ae = new ApplicationException(ErrorMessage);
                    Log.EventSave(ae.Message, "Com.Web.AListen", EventEn.Warning);

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

                // Если есть ошибка при печати чека нужно спустя время откатить количество назад
                if (Config.IsHeldForDocements && Rollbackprintfiscdoc && RollbackDoc!=null)
                {
                    try
                    {
                        // Пауза во времени
                        Thread.Sleep(Config.HeldForDocementsTimeout * 1000);

                        //Установка признака отложенности докумнета
                        Com.ProviderFarm.CurrentPrv.SetIsHelpRollbackDoc(RollbackDoc.sid);

                        // Пробегаем по позициям в чеке
                        foreach (JsonPrintFiscDocItem RollbackItem in RollbackDoc.items)
                        {
                            //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                            switch (RollbackDoc.receipt_type)
                            {
                                case 0:
                                case 1:
                                    Com.ProviderFarm.CurrentPrv.SetQtyRollbackItem(RollbackItem.invn_sbs_item_sid, RollbackItem.quantity);
                                    break;
                                case 2:
                                case 3:
                                    Com.ProviderFarm.CurrentPrv.SetQtyRollbackItem(RollbackItem.invn_sbs_item_sid, RollbackItem.quantity * -1);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception RollbackEx)
                    {
                        ApplicationException Rollbackae = new ApplicationException(string.Format("Упали с ошибкой: {0}", RollbackEx.Message));
                        Log.EventSave(Rollbackae.Message, "Com.Web.AListen (Rollback)", EventEn.Error);
                    }
                }
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
        /// Получаем списко строк которые относятся к документу
        /// </summary>
        /// <param name="doc">Документ к которому мы хотим получить строки</param>
        /// <returns>Список строк в документе</returns>
        public static List<JsonPrintFiscDocItem> GetCopyDocumentsJournalRestSharp(string Link)
        {
            List<JsonPrintFiscDocItem> rez = new List<JsonPrintFiscDocItem>();
            string ResContent = null;

            try
            {
                // Проверяем наличие токена и если он протух то продлеваем его
                GetAuthenticationToken();

                string url = string.Format("{0}{1}?cols=*", Config.HostPrizmApi, Link);
                RestClient _httpClient = new RestClient(url);
                RestRequest request = new RestRequest { Method = Method.GET };
                request.AddHeader("Accept", "application/json,version=2");
                request.AddHeader("Auth-Session", AuthSession);

                // Получаем ответ
                IRestResponse response = _httpClient.Execute(request);
                ResContent = response.Content;


                // Получаем предварительный список документов и парсим
                rez = JsonPrintFiscDocItem.DeserializeJson(ResContent);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0} при парсинге следующего текста /r/n{1}", ex.Message, ResContent));
                Log.EventSave(ae.Message, "Com.Web.GetDocumentsJournalRestSharp", EventEn.Error);
                //throw ae;
            }

            return rez;
        }

        /// <summary>
        /// Получаем списко строк которые относятся к документу
        /// </summary>
        /// <param name="doc">Документ к которому мы хотим получить строки</param>
        /// <returns>Список строк в документе</returns>
        public static List<JsonPrintFiscDocTender> GetCopyDocumentsTenderRestSharp(string Link)
        {
            List<JsonPrintFiscDocTender> rez = new List<JsonPrintFiscDocTender>();
            string ResContent = null;

            try
            {
                // Проверяем наличие токена и если он протух то продлеваем его
                GetAuthenticationToken();

                string url = string.Format("{0}{1}?cols=*", Config.HostPrizmApi, Link);
                RestClient _httpClient = new RestClient(url);
                RestRequest request = new RestRequest { Method = Method.GET };
                request.AddHeader("Accept", "application/json,version=2");
                request.AddHeader("Auth-Session", AuthSession);

                // Получаем ответ
                IRestResponse response = _httpClient.Execute(request);
                ResContent = response.Content;


                // Получаем предварительный список документов и парсим
                rez = JsonPrintFiscDocTender.DeserializeJson(ResContent);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0} при парсинге следующего текста /r/n{1}", ex.Message, ResContent));
                Log.EventSave(ae.Message, "Com.Web.GetDocumentsJournalRestSharp", EventEn.Error);
                //throw ae;
            }

            return rez;
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
        /// Проверка токена перед отправкой любого запроса если он протух то нужно его получить заново чтобы потом вставлять в заголовки запросов
        /// </summary>
        public static void ValidToken()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Config.DefaultTokenEcp))
                {
                    // Проверяем протухшесть токена если он протух то получаем по новой
                    if (LastGetCurToken == null || ((DateTime)LastGetCurToken).AddMinutes(Config.DefaultActivMinToken) < DateTime.Now)
                    {
                        // Com.RepositoryFarm.CurrentRep.WepQueuLoock(LastGetCurToken, CurToken)  делать не нужно этот метод запускается только там где эта проверка прошла

                        // Получаем массив данных который нужно подписать
                        string resp1 = GetObjectCDN(MethodTyp.GET, Com.Config.WebSiteForIsmp, @"/api/v3/auth/cert/key", "application/json;charset=UTF-8", null, null, true, Encoding.UTF8, null, false, false, true);

                        if (resp1.IndexOf(@"""uuid""") == -1 || resp1.IndexOf(@"""data""") == -1) throw new ApplicationException("Не удаётся получить данные которые необходимо подписать при авторизации.");

                        // Парсим чтобы получить тот массив который нужно подписать
                        string uuid = resp1.Substring(resp1.IndexOf(@"""uuid""") + 8);
                        uuid = uuid.Substring(0, uuid.IndexOf(@""""));
                        string data = resp1.Substring(resp1.IndexOf(@"""data""") + 8);
                        data = data.Substring(0, data.IndexOf(@""""));

                        // Получаем подпись
                        byte[] PodPytKey = Crypto.CreateSignature(data);

                        // Строим запрос на токен
                        string PodStrKey = Convert.ToBase64String(PodPytKey);
                        //string request2 = @"{""uuid"":""" + uuid + @""",""data"":""" + PodStrKey + @"""}";
                        string request2 = @"{""data"":""" + PodStrKey + @"""}";

                        // Строим заголовки которые будем цеплять во все запросы
                        List<HederHttp> HederHttpList = new List<HederHttp>();
                        //HederHttpList.Add(new HederHttp("Authorization", "Bearer <token>"));

                        // Получаем токен
                        string resp2 = GetObjectCDN(MethodTyp.POST, Com.Config.WebSiteForIsmp, @"/api/v3/true-api/auth/permissive-access", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, request2, false, false, true);

                        // Парсим токен
                        string token = resp2.Substring(resp2.IndexOf(@"<access_token>") + 14);
                        token = token.Substring(0, token.IndexOf(@"</access_token>"));

                        // Сохраняем актуальный токен 
                        LastGetCurToken = DateTime.Now;
                        CurTokenForIsmp = token;
                        //Com.RepositoryFarm.CurrentRep.WepQueuLoock(LastGetCurToken, CurToken, LastGetCurTokenForMarkingCode, CurTokenForMarkingCode);
                    }
                }
                else CurTokenForIsmp = Config.DefaultTokenEcp;

                // Если токен есть и не протух
                if (!string.IsNullOrEmpty(CurTokenForIsmp))
                {
                    // Строим заголовки которые будем цеплять во все запросы
                    List<HederHttp> HederHttpList = new List<HederHttp>();
                    HederHttpList.Add(new HederHttp("X-API-KEY", CurTokenForIsmp));

                    // Получаем токен
                    string resp3 = GetObjectCDN(MethodTyp.GET, Com.Config.WebSiteForIsmp, @"/api/v4/true-api/cdn/info", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, null, false, false, true);
                    if (Config.Trace) Com.Log.EventSave(string.Format(@"Получили список площадок {0}", resp3), "Web.ValidToken", EventEn.Message, true, false);

                    // Получаем информацию по Cdn площадкам
                    JsonCdnForIsmp Cdns = JsonCdnForIsmp.DeserializeJson(resp3);
                    //
                    foreach (JsonCdnForIsmpHost item in Cdns.hosts)
                    {
                        try
                        {
                            // делаем проверку производительности и доступности площадки
                            string resp4 = GetObjectCDN(MethodTyp.GET, item.host, @"/api/v4/true-api/cdn/health/check", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, null, false, false, true);
                            if (Config.Trace) Com.Log.EventSave(string.Format(@"Результат проверки по площадоке {0}: {1}", item.host, resp4), "Web.ValidToken", EventEn.Message, true, false);
                            JsonCdnCheck CdnsCheck = JsonCdnCheck.DeserializeJson(resp4);

                            //Если площадка доступна сохраняем отклик
                            if (CdnsCheck.code == 0) item.png = CdnsCheck.avgTimeMs;
                        }
                        catch (Exception){}
                    }


                    // Поиск самой доступной и быстрой площадки
                    int MinJsonCdnForIsmpHost = -1;
                    for (int i = 0; i < Cdns.hosts.Count; i++)
                    {
                        if (Cdns.hosts[i].png != null)
                        {
                            if (MinJsonCdnForIsmpHost == -1) MinJsonCdnForIsmpHost = i;
                            else
                            {
                                if ((int)Cdns.hosts[i].png < (int)Cdns.hosts[MinJsonCdnForIsmpHost].png) MinJsonCdnForIsmpHost = i;
                            }
                        }
                    }

                    //Проверка и сохранение доступной площадки
                    if (MinJsonCdnForIsmpHost == -1) throw new ApplicationException("Нет доступных площадок");
                    else CurCdnForIsmp = Cdns.hosts[MinJsonCdnForIsmpHost].host;
                    if (Config.Trace) Com.Log.EventSave(string.Format(@"Выбрана целевой площадка: {0}", CurCdnForIsmp), "Web.ValidToken", EventEn.Message, true, false);
                }
                
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "ValidToken", ex.Message), "Web.ValidToken", EventEn.Error, true, false);
                throw ex;
            }
        }

        /// <summary>
        /// Проверка матрикс кода на площадке СДН
        /// </summary>
        /// <param name="MatrixCode">Матрикс код который нужно проверить</param>
        /// <returns>Ответ от площадки СДН</returns>
        public static JsonCdnForIsmpResponce CdnForIsmpCheck(string MatrixCode)
        {
            try
            {
                JsonCdnForIsmpResponce rez = null;

                // Стандартная проверка через площадку CDN
                ValidToken();
                if (!string.IsNullOrWhiteSpace(CurCdnForIsmp) && MatrixCode.Length>37)
                {
                    // Строим заголовки которые будем цеплять во все запросы
                    List<HederHttp> HederHttpList = new List<HederHttp>();
                    HederHttpList.Add(new HederHttp("X-API-KEY", CurTokenForIsmp));
                                        
                    string tmpMatrixCode = string.Format(@"{0}\u001d{1}\u001d{2}", MatrixCode.Substring(0, 31), MatrixCode.Substring(31,6), MatrixCode.Substring(37));



                    //
                    /*
                    
                    Вот исходный код:
                    010460620309799921yEM1!K9        8005269000      93CbP6      240FA083231.05
                    вот что надо посылать в запросе:
                    010460620309799921yEM1!K9  \u001d8005269000\u001d93CbP6\u001d240FA083231.05


                    (char)29
                      
                    tmpMatrixCode = tmpMatrixCode.Replace(Convert.ToString((char)29), "\u001d");
                    */

                    // Замечательное экронирование
                    tmpMatrixCode = tmpMatrixCode.Replace(@"""", @"\""");
                    /*
                    tmpMatrixCode = tmpMatrixCode.Replace(@"%", "%25");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"""", "%22");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"-", "%2D");
                    tmpMatrixCode = tmpMatrixCode.Replace(@".", "%2E");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"<", "%3C");
                    tmpMatrixCode = tmpMatrixCode.Replace(@">", "%3E");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"\", "%5C");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"^", "%5E");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"_", "%5F");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"`", "%60");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"{", "%7B");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"|", "%7C");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"}", "%7D");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"~", "%7E");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"!", "%21");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"#", "%23");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"$", "%24");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"&", "%26");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"'", "%27");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"(", "%28");
                    tmpMatrixCode = tmpMatrixCode.Replace(@")", "%29");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"*", "%2A");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"+", "%2B");
                    tmpMatrixCode = tmpMatrixCode.Replace(@",", "%2C");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"/", "%2F");
                    tmpMatrixCode = tmpMatrixCode.Replace(@":", "%3A");
                    tmpMatrixCode = tmpMatrixCode.Replace(@";", "%3B");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"=", "%3D");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"?", "%3F");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"@", "%40");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"[", "%5B");
                    tmpMatrixCode = tmpMatrixCode.Replace(@"]", "%5D");
                    */

                    string request2 = @"{""codes"":[""" + tmpMatrixCode + @"""]}";

                    // Получаем токен
                    string resp2 = GetObjectCDN(MethodTyp.POST, CurCdnForIsmp, @"/api/v4/true-api/codes/check", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, request2, false, false, true);
                    Com.Log.EventSave(string.Format(@"Отправка кода ({0}) на площадку ЦДН ({1}) и получили ответ:""{2}""", tmpMatrixCode, CurCdnForIsmp, resp2), "WebClient", EventEn.Message,true, false);

                    rez = JsonCdnForIsmpResponce.DeserializeJson(resp2);

                    if (rez.codes.Count==1) rez.codes[0].requestMatrixCode = MatrixCode;
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "CdnForIsmpCheck", ex.Message), "Web", EventEn.Error, true, false);
                throw ex;
            }
        }
           

        /// <summary>
        /// Получаем данные
        /// </summary>
        /// <param name="TypRequest">Тип POST или GET</param>
        /// <param name="WebSite">Websait на который делаем запрос если не указан, то берётся из конфига</param>
        /// <param name="Folder">Путь после сайта</param>
        /// <param name="СontentType">Чото указать в типе контекста в заголовке</param>
        /// <param name="HederHttpList">Список параметров который воткнуть в заголовок</param>
        /// <param name="KeepAlive">держать конект или нет</param>
        /// <param name="EnCod">Кодировка в которой делать запрос мы использоватли Encoding.UTF8</param>
        /// <param name="JsonQuery">Данные которые передаём методом Post</param>
        /// <param name="lockEventHashExecuting">Блокировать генерацию собятия HashExecuting</param>
        /// <param name="IsNotWriteLog">Не писать в лог</param>
        /// <param name="isNotVisibleMessageError">Не отображать ошибки пользователю</param>
        /// <returns>результат запроса который возвратил сервер</returns>
        private static string GetObjectCDN(MethodTyp TypRequest, string WebSite, string Folder, string СontentType, List<HederHttp> HederHttpList, string Accept, bool KeepAlive, Encoding EnCod, string JsonQuery, bool lockEventHashExecuting, bool IsNotWriteLog, bool isNotVisibleMessageError)
        {   // https://docs.microsoft.com/ru-ru/dotnet/framework/network-programming/how-to-send-data-using-the-webrequest-class
            try
            {
                string rez = null;
                //
                string tmpFolder = null;
                // Правим путь который будет использоваться в запросе на формат, надо чтобы начиналось с /
                if (!string.IsNullOrWhiteSpace(Folder)) tmpFolder = (@"/" + Folder).Replace(@"//", @"/");

                try
                {
                    // Проверяем на наличие адреса к серверу
                    if (string.IsNullOrWhiteSpace(WebSite)) throw new ApplicationException("Не указан сайт с которым работаем");

                    // Переменная для массива байт в нужной нам кодировке
                    byte[] MessOut = null;

                    // Создаём подключение к серверу
                    string turl = WebSite + tmpFolder;
                    if (Config.Trace) Com.Log.EventSave(string.Format(@"Построен адрес запроса {0}", turl), "Web.GetObject", EventEn.Message, true, false);
                    turl = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(turl)));
                    Uri uri = new Uri(turl);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Timeout = Config.CdnRequestTimeout;
                    // Устанавливаем заголовк
                    if (HederHttpList != null)
                    {
                        WebHeaderCollection myWebHeaderCollection = null;
                        foreach (HederHttp item in HederHttpList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.AtributeName) && !string.IsNullOrWhiteSpace(item.AtributeName))
                            {
                                if (myWebHeaderCollection == null) myWebHeaderCollection = request.Headers;
                                myWebHeaderCollection.Set(item.AtributeName, item.AtributeValue);
                            }
                        }
                    }

                    // Если указан null то берём по умолчанию
                    request.Accept = Accept ?? @"text/html, application/xhtml+xml, */*";
                    request.ContentType = (string.IsNullOrWhiteSpace(СontentType) ? @"application/json;charset=UTF-8" : СontentType);
                    //request.KeepAlive = KeepAlive;

                    if (!string.IsNullOrWhiteSpace(JsonQuery) && TypRequest == MethodTyp.POST)
                    {
                        request.Method = TypRequest.ToString();

                        // Получаем массив в байтах
                        MessOut = Encoding.Convert(Encoding.Default, EnCod, Encoding.Default.GetBytes(JsonQuery));

                        request.ContentLength = MessOut.Length;

                        // Закачиваем то что нам нужно передать на сервер
                        using (Stream reqStream = request.GetRequestStream())
                        {
                            reqStream.Write(MessOut, 0, MessOut.Length);
                        }
                    }
                    else
                    {
                        if (TypRequest == MethodTyp.GET) request.Method = TypRequest.ToString();
                        else request.Method = TypRequest.ToString();
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
                                //rez = respReader.ReadToEnd();
                                rez = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(respReader.ReadToEnd())));
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        if (!IsNotWriteLog) Log.EventSave("Message: " + ex.Message + "\r\nResponse: " + ex.Response, @"Web.GetObject WebException", Lib.EventEn.Error);
                        throw new ApplicationException(ex.Message);
                    }

                    // В настройка включена трасировка всех запросов и ответов к web серверу
                    if (Config.Trace)
                    {
                        if (!IsNotWriteLog) Log.EventSave("\r\nrequest=" + JsonQuery, @"Web.GetObject", Lib.EventEn.Trace);
                        if (!IsNotWriteLog) Log.EventSave("\r\nresponse=" + rez, @"Web.GetObject", Lib.EventEn.Trace);
                    }
                }
                catch (Exception ex)
                {
                    if (!IsNotWriteLog) Log.EventSave(ex.Message, @"Web.GetObject", Lib.EventEn.Error);
                    if (!IsNotWriteLog) Log.EventSave("\r\nfolder=" + tmpFolder + "\r\n" + JsonQuery, @"Web.GetObject", Lib.EventEn.Dump);
                    throw ex;
                }
                
                return rez;
            }
            catch (Exception ex)
            {
                // Если это тест подклюыения к нету то сообщать пользователю новым окном не будем
                if (Folder == @"/api/v3/auth/cert/key") Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObject", ex.Message), "Web.GetObject", EventEn.Error, true, false);
                else Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObject", ex.Message), "Web.GetObject", EventEn.Error, true, (isNotVisibleMessageError ? false : true));
                throw ex;
            }
            //return null;
        }
        //
        /// <summary>
        /// Получаем данные
        /// </summary>
        /// <param name="TypRequest">Тип POST или GET</param>
        /// <param name="Folder">Путь после сайта</param>
        /// <param name="СontentType">Чото указать в типе контекста в заголовке</param>
        /// <param name="HederHttpList">Список параметров который воткнуть в заголовок</param>
        /// <param name="KeepAlive">держать конект или нет</param>
        /// <param name="EnCod">Кодировка в которой делать запрос мы использоватли Encoding.UTF8</param>
        /// <param name="JsonQuery">Данные которые передаём методом Post</param>
        /// <param name="IsNotWriteLog">Не писать в лог</param>
        /// <param name="isNotVisibleMessageError">Не отображать ошибки пользователю</param>
        /// <returns>результат запроса который возвратил сервер</returns>
        public static string GetObjectEnisey(MethodTyp TypRequest, string Folder, string СontentType, List<HederHttp> HederHttpList, string Accept, bool KeepAlive, Encoding EnCod, string JsonQuery, bool IsNotWriteLog, bool isNotVisibleMessageError)
        {   // https://docs.microsoft.com/ru-ru/dotnet/framework/network-programming/how-to-send-data-using-the-webrequest-class
            try
            {
                string rez = null;
                //
                string tmpFolder = null;
                // Правим путь который будет использоваться в запросе на формат, надо чтобы начиналось с /
                if (!string.IsNullOrWhiteSpace(Folder)) tmpFolder = (@"/" + Folder).Replace(@"//", @"/");

                try
                {
                    // Проверяем на наличие адреса к серверу
                    if (string.IsNullOrWhiteSpace(Config.EniseyHost)) throw new ApplicationException("Не указан сайт Enisey");
                    if (Config.EniseyPort==0) throw new ApplicationException("Не указан порт Enisey");

                    // Переменная для массива байт в нужной нам кодировке
                    byte[] MessOut = null;

                    // Создаём подключение к серверу
                    string turl = string.Format(@"http://{0}:{1}{2}", Config.EniseyHost, Config.EniseyPort, tmpFolder);
                    if (Config.Trace) Com.Log.EventSave(string.Format(@"Построен адрес запроса {0}", turl), "Web.GetObjectEnisey", EventEn.Message, true, false);
                    turl = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(turl)));
                    Uri uri = new Uri(turl);

                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                    request.Timeout = Config.CdnRequestTimeout*10;
                    // Устанавливаем заголовк
                    if (HederHttpList != null)
                    {
                        WebHeaderCollection myWebHeaderCollection = null;
                        foreach (HederHttp item in HederHttpList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.AtributeName) && !string.IsNullOrWhiteSpace(item.AtributeName))
                            {
                                if (myWebHeaderCollection == null) myWebHeaderCollection = request.Headers;
                                myWebHeaderCollection.Set(item.AtributeName, item.AtributeValue);
                            }
                        }
                    }

                    // Если указан null то берём по умолчанию
                    request.Accept = Accept ?? @"text/html, application/xhtml+xml, */*";
                    request.ContentType = (string.IsNullOrWhiteSpace(СontentType) ? @"application/json;charset=UTF-8" : СontentType);
                    //request.KeepAlive = KeepAlive;

                    if (!string.IsNullOrWhiteSpace(JsonQuery) && TypRequest == MethodTyp.POST)
                    {
                        request.Method = TypRequest.ToString();

                        // Получаем массив в байтах
                        MessOut = Encoding.Convert(Encoding.Default, EnCod, Encoding.Default.GetBytes(JsonQuery));

                        request.ContentLength = MessOut.Length;

                        // Закачиваем то что нам нужно передать на сервер
                        using (Stream reqStream = request.GetRequestStream())
                        {
                            reqStream.Write(MessOut, 0, MessOut.Length);
                        }
                    }
                    else
                    {
                        if (TypRequest == MethodTyp.GET) request.Method = TypRequest.ToString();
                        else request.Method = TypRequest.ToString();
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
                                //rez = respReader.ReadToEnd();
                                rez = Encoding.Default.GetString(Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(respReader.ReadToEnd())));
                            }
                        }
                    }
                    catch (WebException ex)
                    {
                        if (!IsNotWriteLog) Log.EventSave("Message: " + ex.Message + "\r\nResponse: " + ex.Response, @"Web.GetObjectEnisey WebException", Lib.EventEn.Error);
                        throw new ApplicationException(ex.Message);
                    }

                    // В настройка включена трасировка всех запросов и ответов к web серверу
                    //if (Config.Trace)
                    //{
                        if (!IsNotWriteLog) Log.EventSave("\r\nrequest=" + JsonQuery, @"Web.GetObjectEnisey", Lib.EventEn.Trace);
                        if (!IsNotWriteLog) Log.EventSave("\r\nresponse=" + rez, @"Web.GetObjectEnisey", Lib.EventEn.Trace);
                    //}
                }
                catch (Exception ex)
                {
                    if (!IsNotWriteLog) Log.EventSave(ex.Message, @"Web.GetObjectEnisey", Lib.EventEn.Error);
                    if (!IsNotWriteLog) Log.EventSave("\r\nfolder=" + tmpFolder + "\r\n" + JsonQuery, @"Web.GetObjectEnisey", Lib.EventEn.Dump);
                    throw ex;
                }

                return rez;
            }
            catch (Exception ex)
            {
                // Если это тест подклюыения к нету то сообщать пользователю новым окном не будем
                if (Folder == @"/api/v3/auth/cert/key") Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObjectEnisey", ex.Message), "Web.GetObjectEnisey", EventEn.Error, true, false);
                else Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObjectEnisey", ex.Message), "Web.GetObject", EventEn.Error, true, (isNotVisibleMessageError ? false : true));
                throw ex;
            }
            //return null;
        }
        //
        /// <summary>
        /// Инициализация базы данных Енисей
        /// </summary>
        /// <returns>Ответ веб сервиса</returns>
        public static string GetInitEnisey()
        {
            try
            {
                // Строим заголовки которые будем цеплять во все запросы
                List<HederHttp> HederHttpList = new List<HederHttp>();
                HederHttpList.Add(new HederHttp("Authorization", String.Format("Basic {0}", Convert.ToBase64String(Encoding.Default.GetBytes(string.Format(@"{0}:{1}", Config.EniseyLogin, Config.EniseyPassword))))));
                //HederHttpList.Add(new Web.HederHttp("X-ClientId", Config.FrSerialNumber));

                //Построение запроса к базе Енисей
                string reqtmp = string.Format(@"{{""token"" : ""{0}""}}", Config.DefaultTokenEcp);

                // Получение даных от Енисея
                return Web.GetObjectEnisey(Web.MethodTyp.POST, @"/api/v1/init", "text/plain", HederHttpList, null, false, Encoding.UTF8, reqtmp, false, false);

            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetInitEnisey", ex.Message), "Web", EventEn.Error, true, false);
                throw ex;
            }
        }
        //
        /// <summary>
        /// Статус базы данных Енисей
        /// </summary>
        /// <returns>Ответ веб сервиса</returns>
        public static string GetStatusEnisey()
        {
            try
            {
                // Строим заголовки которые будем цеплять во все запросы
                List<HederHttp> HederHttpList = new List<HederHttp>();
                HederHttpList.Add(new HederHttp("Authorization", String.Format("Basic {0}", Convert.ToBase64String(Encoding.Default.GetBytes(string.Format(@"{0}:{1}", Config.EniseyLogin, Config.EniseyPassword))))));

                // Получение даных от Енисея
                return Web.GetObjectEnisey(Web.MethodTyp.GET, @"/api/v1/status", "application/json", HederHttpList, null, true, Encoding.UTF8, null, false, false);
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetStatusEnisey", ex.Message), "Web", EventEn.Error, true, false);
                throw ex;
            }
        }
        //
        /// <summary>
        /// Типы запросов
        /// </summary>
        public enum MethodTyp
        {
            POST, GET
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

        /// <summary>
        /// Вспомогательный клас для заголовков страницы
        /// </summary>
        public class HederHttp
        {
            /// <summary>
            /// Имя в заголовке пакета
            /// </summary>
            public string AtributeName { get; private set; }

            /// <summary>
            /// Значение в заголовке
            /// </summary>
            public string AtributeValue { get; private set; }

            /// <summary>
            /// Конструктор
            /// </summary>
            /// <param name="AtributeName">Имя в заголовке пакета</param>
            /// <param name="AtributeValue">Значение в заголовке</param>
            public HederHttp(string AtributeName, string AtributeValue)
            {
                this.AtributeName = AtributeName;
                this.AtributeValue = AtributeValue;
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
