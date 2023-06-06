using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;
using AlgoritmListener.Lib;
using AlgoritmListener.Com;
using AlgoritmListener.BLL;

namespace AlgoritmListener
{
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Статус последней дату от которой отсчитываем неделю и если старый лог то грохаем его
        /// </summary>
        private static DateTime LastDateLog;

        /// <summary>
        /// Место хранения лога
        /// </summary>
        public static string LogDir = @"C:\Program Files";
        public static string LogFile = "AlgoritmListener.txt";
        public static string FileXml = "AlgoritmListener.xml";

        public static HttpClient IoIodeWebClientIsv = new HttpClient()
        { 
            BaseAddress = new Uri("http://isv:5000"),
        };
        
        public Service1()
        {
            InitializeComponent();
        }

        // Запуск службы
        protected override async void OnStart(string[] args)
        {
            // Флаг пишет что сервис стартанул
            bool flagStardet = false;
            LastDateLog = DateTime.Now.Date;

            // Запускаем службу чтобы работала бесконечно
            while (true)
            {
                // Обработка ошибок чтобы сервис не свалился
                try
                {
                    // Запуск службы и настройка логирования
                    if (!Directory.Exists(string.Format(@"{0}\AlgoritmListener", LogDir))) Directory.CreateDirectory(string.Format(@"{0}\AlgoritmListener", LogDir));
                    //
                    // Если не писали о том что служба стартанула пишем об этом в лог
                    if (!flagStardet)
                    {   
                        // Регистрируем службу логирования
                        Log.SetupLog(string.Format(@"{0}\AlgoritmListener", LogDir), LogFile);
                        flagStardet = true;                      

                        // Запускаем чтение файла конфигурации
                        Config.SetupConfig(string.Format(@"{0}\AlgoritmListener", LogDir), FileXml);

                        // Нужно проверить файл и его грохнуть если он не нужен
                        Log.EventSave("test", "tttt", EventEn.Message);
                        Log.EventSave(LogDir, "tttt - LogDir", EventEn.Message);
                        Log.EventSave(LogFile, "tttt - LogFile", EventEn.Message);
                        if (File.Exists(string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile))) File.Delete(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile));
                        if (File.Exists(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile))) File.Delete(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile));
                    }
                    


                    // Запускаем процесс проверки асинхронный модуля по плагину призма
                    try
                    {
                        PrizmListener.Verif();
                    }
                    catch (Exception ex)
                    {
                        Log.EventSave(string.Format("Не смогли запустить проверку по PrizmListener. Ошибка: {0}", ex.Message), "AlgoritmListener", EventEn.Error);
                    }
                }
                catch (Exception ex) 
                {
                    try
                    {
                        Log.EventSave(string.Format("Ошибка: {0}", ex.Message), "AlgoritmListener", EventEn.Error);
                    }
                    catch (Exception){}
                }

                // Пауза между циклами
                await Task.Delay(Com.Config.TimeOutSec*1000);
            }
        }

        // Остановка службы
        protected override void OnStop()
        {
            try
            {
                Com.Log.EventSave("Cлужба остановлена.", "AlgoritmListener", EventEn.Message);
            }
            catch (Exception){}
        }
    }
}
