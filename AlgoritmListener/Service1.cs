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
                        Log.SetupLog(string.Format(@"{0}\AlgoritmListener", LogDir), LogFile);
                        flagStardet = true;

                        // Запускаем чтение файла конфигурации
                        Config.SetupConfig(string.Format(@"{0}\AlgoritmListener", LogDir), FileXml);

                        // Для теста того что конфиг запустился и с него можно что-то считать
                        //Log.EventSave(Config.PrizmListener_FileConf, "AlgoritmListener", EventEn.Message);
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
                catch (Exception) { }

                // Пауза между циклами
                await Task.Delay(3000);
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
