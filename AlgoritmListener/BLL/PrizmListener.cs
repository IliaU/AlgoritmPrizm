using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.IO;
using System.Xml;
//using System.Threading;
using System.Diagnostics;
using AlgoritmListener.Lib;
using AlgoritmListener.Com;

namespace AlgoritmListener.BLL
{
    /// <summary>
    /// Для работы с инструментами призма и всего что с ним сявзвно
    /// </summary>
    public static class PrizmListener
    {
        #region Private Param

        /// <summary>
        /// Версия XML файла
        /// </summary>
        private static int _Version = 1;

        /// <summary>
        /// Объект для блокировки в одиин поток
        /// </summary>
        private static object lobj = new object();

        #endregion

        #region Public Param

        // Объект для работы с веб мордой и прослушивания всего что нужно для призма
        public static HttpClient IoIodeWebClientIsv;

        /// <summary>
        /// Версия XML файла
        /// </summary>
        public static int Version { get { return _Version; } private set { } }

        #endregion

        /// <summary>
        /// Запкскаем проверку
        /// </summary>
        public static async void Verif()
        {
            try
            {
                lock (lobj)
                {
                    try
                    {
                        XmlDocument Document = new XmlDocument();
                        Document.Load(Config.PrizmListener_FileConf);
                        XmlElement xmlRoot = Document.DocumentElement;

                        ApplicationException appM = new ApplicationException("Неправильный настроечный файл, скорее всего не от этой программы.");
                        ApplicationException appV = new ApplicationException(string.Format("Неправильная версия настроечного яайла, требуется {0} версия", _Version));

                        // Проверяем значения заголовка
                        if (xmlRoot.Name != "AlgoritmPrizm") throw appM;
                        if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) throw appV;

                        // Получаем путь из конфига и если ещё не инициировали объекто то создаём его
                        string XmlUrl = string.Format("http://{0}:{1}", xmlRoot.GetAttribute("Host"), xmlRoot.GetAttribute("Port"));
                        if (IoIodeWebClientIsv == null || IoIodeWebClientIsv.BaseAddress.ToString() != XmlUrl)
                        {
                            Log.EventSave(string.Format("Прочитали с конфига адрес который над проверить. ({0})", XmlUrl), "PrizmListener.Verif", EventEn.Message);
                            IoIodeWebClientIsv = new HttpClient()
                            {
                                BaseAddress = new Uri(XmlUrl),
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.EventSave(string.Format("Обнаружена проблема при чтении файла конфигурации AlgoritmPrizm.xml. Ошибка: {0}", ex.Message), "PrizmListener.Verif", EventEn.Error);
                    }
                }

                // Если объект найден то запускаем проверку объекта в системе
                if (IoIodeWebClientIsv != null)
                {
                    // Флаг который говорит о необходимости запуска нашего процесса
                    bool FlagAlgoritmPrizmExistProcess = false;
                    try
                    {
                        using (HttpResponseMessage resp = await IoIodeWebClientIsv.GetAsync("config"))
                        {
                            // Записывает сведения о запросе в консоль.
                            HttpResponseMessage respMes = resp.EnsureSuccessStatusCode();
                            if (respMes.StatusCode == System.Net.HttpStatusCode.OK)
                            {

                                // Читаем контерн в виде потока данных
                                System.IO.Stream jsonResponseStream = await resp.Content.ReadAsStreamAsync();  // таким способом можно сразу читать из респонса и тормазится поток пока не прочитает

                                // Логируем окончание стения
                                Log.EventSave("Бекенд для призма работает.", "PrizmListener.Verif", EventEn.Message);
                                FlagAlgoritmPrizmExistProcess = true;
                            }
                            else
                            {
                                Log.EventSave("Обнаружено что наш бекенд для призма не запущен.", "PrizmListener.Verif", EventEn.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.EventSave(string.Format("Обнаружено что наш бекенд для призма не запущен. Ошибка: {0}", ex.Message), "PrizmListener.Verif", EventEn.Error);
                    }

                    lock (lobj)
                    {
                        // Если обнаружили что нет процесса то нужно перезапустить наш екзешник
                        if (!FlagAlgoritmPrizmExistProcess)
                        {
                            try
                            {
                                /*
                                // Проверка по процессам, чтобы приложение было в единственном экземпляре.
                                bool oneOnlyProg;
                                Mutex m = new Mutex(true, "AlgoritmPrizm", out oneOnlyProg);
                                if (oneOnlyProg != true) // Если процесс существует надо его убить
                                {
                                }
                                */

                                ProcessStartInfo prc = new ProcessStartInfo();
                                prc.FileName = string.Format(@"{0}\AlgoritmPrizm.exe", Path.GetDirectoryName(Config.PrizmListener_FileConf));
                                prc.UseShellExecute = true;
                                prc.CreateNoWindow = false;
                                prc.WorkingDirectory = Path.GetDirectoryName(Config.PrizmListener_FileConf);
                                Process.Start(prc);
                            }
                            catch (Exception ex)
                            {
                                Log.EventSave(string.Format("Не смогли перезапустить наш процесс. Ошибка: {0}", ex.Message), "PrizmListener.Verif", EventEn.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "PrizmListener.Verif", EventEn.Error);
                throw ae;
            }
        }

    }
}
