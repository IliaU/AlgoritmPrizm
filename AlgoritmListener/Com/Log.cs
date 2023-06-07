using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using AlgoritmListener.Lib;

namespace AlgoritmListener.Com
{
    public static class Log
    {
        #region Private Param
        /// <summary>
        /// Количество попыток записи в лог
        /// </summary>
        private static int IOCountPoput = 5;

        /// <summary>
        /// Количество милесекунд мкжду попутками записи
        /// </summary>
        private static int IOWhileInt = 500;

        /// <summary>
        /// Объект для блокировки в одиин поток
        /// </summary>
        private static object lobj = new object();
        #endregion

        #region Public Param
        /// <summary>
        /// Файл в который будем сохранять лог
        /// </summary>
        public static string File { get; private set; }

        /// <summary>
        /// Директория
        /// </summary>
        public static string Dir { get; private set; }

        /// <summary>
        /// Возникновение события в приложении
        /// </summary>
        public static event EventHandler<EventLog> onEventLog;
        #endregion


        #region Public Method

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="LogDir">Путь для файла с логами</param>
        /// <param name="FileLog">Имя файла для лога программы</param>
        public static void SetupLog(string LogDir, string LogFile)
        {
            if (string.IsNullOrWhiteSpace(LogFile)) File = "AlgoritmListener.log";
            else File = LogFile;

            if (string.IsNullOrWhiteSpace(LogDir)) Dir = @"C:\Program Files\AlgoritmListener";
            else Dir = LogDir;

            Thread.Sleep(1000);

            EventSave("Запуск службы", "AlgoritmListener", EventEn.Message);
        }
        
        
        /// <summary>
        /// Событие программы
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public static void EventSave(string Message, string Source, EventEn evn)
        {
            lock (lobj)
            {
                EventSave(Message, Source, evn, IOCountPoput);
            }
        }
        #endregion

        #region Private Method

        /// <summary>
        /// Запись в файл
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="IOCountPoput">Количество попыток записи в лог</param>
        private static void EventSave(string Message, string Source, EventEn evn, int IOCountPoput)
        {
            try
            {
                using (StreamWriter SwFileLog = new StreamWriter(Dir + @"\" + File, true))
                {
                    SwFileLog.WriteLine(DateTime.Now.ToString() + "\t" + evn.ToString() + "\t" + Source + "\t" + Message);
                }
                
            }
            catch (Exception)
            {
                if (IOCountPoput > 0)
                {
                    Thread.Sleep(IOWhileInt);
                    EventSave(Message, Source, evn, IOCountPoput - 1);
                }
                else throw;
            }
        }
        
        #endregion
    }
}
