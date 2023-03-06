using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Threading;
using System.Windows.Forms;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Класс для ведения логов пробитых чеков
    /// </summary>
    public class FileCheckLog
    {
        private static FileCheckLog obj = null;

        /// <summary>
        /// Количество попыток записи в лог
        /// </summary>
        private static int IOCountPoput = 5;

        /// <summary>
        /// Количество милесекунд мкжду попутками записи
        /// </summary>
        private static int IOWhileInt = 500;

        /// <summary>
        /// Конструктор
        /// </summary>
        public FileCheckLog()
        {
            obj = this;

            Log.EventSave("Запуск модуля логирования чеков", GetType().Name, EventEn.Message);
        }


        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Matrix">Матрикс код</param>
        /// <param name="Sid">Сид документа</param>
        /// <param name="Price">цена</param>
        /// <param name="DocCustTyp">Тип расчёта</param>
        /// <param name="receipt_type">Тип документа</param>
        /// <param name="ItemSid">Сид товара</param>
        /// <param name="IOCountPoput">Количество попыток записи в лог</param>
        public static void EventPrintSave(string Matrix, string Sid, decimal Price, EnFrTyp DocCustTyp, int receipt_type, string ItemSid)
        {
            try
            {
                // Если есть имя файла куда сохранять
                if (!string.IsNullOrWhiteSpace(Config.FileCheckLog))
                {
                    EventPrintSave(Matrix, Sid, Price, DocCustTyp, receipt_type, ItemSid, IOCountPoput);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытки записи в лог с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FileCheckLog.EventPrintSave", EventEn.Error);
            }
        }

        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Matrix">Матрикс код</param>
        /// <param name="Sid">Сид документа</param>
        /// <param name="Price">цена</param>
        /// <param name="DocCustTyp">Тип расчёта</param>
        /// <param name="receipt_type">Тип документа</param>
        /// <param name="ItemSid">Сид товара</param>
        /// <param name="IOCountPoput">Количество попыток записи в лог</param>
        private static void EventPrintSave(string Matrix, string Sid, decimal Price, EnFrTyp DocCustTyp, int receipt_type, string ItemSid, int IOCountPoput)
        {
            try
            {
                lock (obj)
                {
                    // Фиксируем дополнительно в обычном логе
                    Log.EventSave(string.Format("матрикс = {0}", Matrix), "Com.FileCheckLog.EventPrintSave", EventEn.Message);

                    using (StreamWriter SwFileLog = new StreamWriter(Environment.CurrentDirectory + @"\" + Config.FileCheckLog, true))
                    {
                        SwFileLog.WriteLine(DateTime.Now.ToString() 
                            + "\t" + string.Format(", документ={0}", Sid)
                            + "\t" + string.Format(", товар={0}", ItemSid)
                            + "\t" + string.Format(", тип расчёта={0}", DocCustTyp)
                            + "\t" + string.Format(", тип документа={0}", receipt_type)
                            + "\t" + string.Format(", цена = {0}", Price)
                            + "\t" + string.Format(", матрикс = {0}", Matrix));
                    }
                }
            }
            catch (Exception)
            {
                if (IOCountPoput > 0)
                {
                    Thread.Sleep(IOWhileInt);
                    EventPrintSave(Matrix, Sid, Price, DocCustTyp, receipt_type, ItemSid, IOCountPoput - 1);
                }
                else throw;
            }
        }
    }
}
