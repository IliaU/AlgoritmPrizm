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
            // фиксируем дату
            LastDateLog = DateTime.Now.Date;

            // Запуск службы и настройка логирования
            if (!Directory.Exists(string.Format(@"{0}\AlgoritmListener", LogDir))) Directory.CreateDirectory(string.Format(@"{0}\AlgoritmListener", LogDir));

            // Чистка старого лога
            if (File.Exists(string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile))) File.Delete(string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile));
            if (File.Exists(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile))) File.Move(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile), string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile));

            // Запуск логирования
            Log.SetupLog(string.Format(@"{0}\AlgoritmListener", LogDir), LogFile);
                        
            // Запускаем чтение файла конфигурации
            Config.SetupConfig(string.Format(@"{0}\AlgoritmListener", LogDir), FileXml);

            // Запускаем службу чтобы работала бесконечно
            while (true)
            {
                // Обработка ошибок чтобы сервис не свалился
                try
                {
                    // Проверяем дату и выполняем чистку логов 
                    if(LastDateLog.AddDays(Com.Config.ClearLogDay)<DateTime.Now)
                    {
                        // Чистка старого лога
                        if (File.Exists(string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile))) File.Delete(string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile));
                        if (File.Exists(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile))) File.Move(string.Format(@"{0}\AlgoritmListener\{1}", LogDir, LogFile), string.Format(@"{0}\AlgoritmListener\Old{1}", LogDir, LogFile));

                        // фиксируем дату
                        LastDateLog = DateTime.Now.Date;
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
