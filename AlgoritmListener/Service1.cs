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
using AlgoritmListener.Lib;
using AlgoritmListener.Com;

namespace AlgoritmListener
{
    public partial class Service1 : ServiceBase
    {
        /// <summary>
        /// Место хранения лога
        /// </summary>
        public static string LogDir = @"C:\Program Files";
        public static string LogFile = "AlgoritmListener.txt";

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
                    if (!Directory.Exists(string.Format(@"{0}\AlgoritmListener", LogDir))) Directory.CreateDirectory(string.Format(@"{0}\AlgoritmListener", LogDir));

                    // Если не писали о том что служба стартанула пишем об этом в лог
                    if (!flagStardet)
                    {
                        Com.Log.SetupLog(string.Format(@"{0}\AlgoritmListener", LogDir), LogFile);
                        flagStardet = true;
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
