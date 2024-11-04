using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using AlgoritmPrizmCom.BLL;
using AlgoritmPrizmCom.Lib;
using AlgoritmPrizmCom.Com;

namespace AlgoritmPrizmCom
{
    /// <summary>
    /// Класс для работы с площадкой ЦРПТ
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Получение конфигурации
        /// </summary>
        /// <returns>Конфигурация настроек</returns>
        public static CdnResponceConfig CdnForIsmpConfig()
        {
            try
            {
                CdnResponceConfig rez = null;

                if (!string.IsNullOrWhiteSpace(Config.Host))
                {
                    // Инициализация лога если этого не произошло
                    Com.Log Lg = new Com.Log();

                    // Строим заголовки которые будем цеплять во все запросы
                    List<HederHttp> HederHttpList = new List<HederHttp>();
                    //HederHttpList.Add(new HederHttp("X-API-KEY", CurTokenForIsmp));

                    // Получаем ответ
                    string resp = GetObject(MethodTyp.GET, string.Format(@"http://{0}:{1}", Config.Host, Config.Port), @"/config", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, null, false, false, true);

                    rez = CdnResponceConfig.DeserializeJson(resp);
                }

                return rez;
            }
            catch (Exception ex)
            {
                Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "CdnForIsmpCheckConfig", ex.Message), "Web", EventEn.Error, true, false);
                throw ex;
            }
        }



        /// <summary>
        /// Проверка матрикс кода на площадке СДН
        /// </summary>
        /// <param name="MatrixCode">Матрикс код который нужно проверить</param>
        /// <returns>Ответ от площадки СДН</returns>
        public static CdnResponce CdnForIsmpCheck(string MatrixCode)
        {
            try
            {
                CdnResponce rez = null;
                string tmpMatrixCode = MatrixCode.Replace(Convert.ToString((char)29), "");

                if (!string.IsNullOrWhiteSpace(Config.Host))
                {
                    // Инициализация лога если этого не произошло
                    Com.Log Lg = new Com.Log();

                    // Строим заголовки которые будем цеплять во все запросы
                    List<HederHttp> HederHttpList = new List<HederHttp>();
                    //HederHttpList.Add(new HederHttp("X-API-KEY", CurTokenForIsmp));

                    // Получаем ответ
                    string resp = GetObject(MethodTyp.POST, string.Format(@"http://{0}:{1}",Config.Host, Config.Port), @"/CdnForIsmpCheckJson", "application/json;charset=UTF-8", HederHttpList, null, true, Encoding.UTF8, tmpMatrixCode, false, false, true);
                    
                    rez = CdnResponce.DeserializeJson(resp);
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
        private static string GetObject(MethodTyp TypRequest, string WebSite, string Folder, string СontentType, List<HederHttp> HederHttpList, string Accept, bool KeepAlive, Encoding EnCod, string JsonQuery, bool lockEventHashExecuting, bool IsNotWriteLog, bool isNotVisibleMessageError)
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
                    turl = Encoding.UTF8.GetString(Encoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(turl)));
                    Uri uri = new Uri(turl);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 10000000;
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
                if (Folder == @"/api/v3/auth/cert/key") Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObject", ex.Message), "Web", EventEn.Error, true, false);
                else Com.Log.EventSave(string.Format(@"Ошибка в методе {0}:""{1}""", "GetObject", ex.Message), "Web", EventEn.Error, true, (isNotVisibleMessageError ? false : true));
                throw ex;
            }
            //return null;
        }
    }
}
