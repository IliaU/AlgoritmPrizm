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

            if (string.IsNullOrWhiteSpace(LogDir)) Dir = "";
            else Dir = LogDir;

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
            EventSave(File, Message, Source, evn, IOCountPoput);
        }
        #endregion

        #region Private Method

        /// </summary>
        /// <param name="FileName">Файл в котрый пишем лог</param>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        /// <param name="IOCountPoput">Количество попыток записи в лог</param>
        private static void EventSave(string FileName, string Message, string Source, EventEn evn, int IOCountPoput)
        {
            try
            {
                string newFile = (string.IsNullOrWhiteSpace(FileName) ? File : FileName);

                using (StreamWriter SwFileLog = new StreamWriter(Dir + @"\" + newFile, true))
                {
                    SwFileLog.WriteLine(DateTime.Now.ToString() + "\t" + evn.ToString() + "\t" + Source + "\t" + Message);
                }
                
            }
            catch (Exception)
            {
                if (IOCountPoput > 0)
                {
                    Thread.Sleep(IOWhileInt);
                    EventSave(FileName, Message, Source, evn, IOCountPoput - 1);
                }
                else throw;
            }
        }
        
        #endregion
    }
}
