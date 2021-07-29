using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com.ReportLib
{
    /// <summary>
    /// Базовый класс для отчётов
    /// </summary>
    public abstract class BReport:IColbackDocument, IColbackDocumentJournal
    {

        /// <summary>
        /// Обработка фильтра для  принятия решения нужен нам этот документ или нет для отчёта движение товара
        /// </summary>
        /// <param name="Document">Документ который считался (его заголовок)</param>
        /// <returns></returns>
        public virtual List<JsonGetDocumentJournal> ColbackDocument(JsonGetDocumentsData Document)
        {
            try
            {
                // Получаем информацию по строкам документа
                return Web.GetDocumentsJournalRestSharp(Document, null);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.FilterReportDocument", EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Делигат для получения строки и принятия решения о выводе документа на основе фильтрации
        /// </summary>
        /// <param name="journal">Строка которую фильтруем</param>
        /// <returns>Результат фильтрации если True то добавить в ответ, если False</returns>
        public virtual bool ColbackDocumentJournal(JsonGetDocumentJournal journal)
        {
            try
            {
                // Получаем информацию по строкам документа
                return true;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.FilterReportDocument", EventEn.Error);
                throw ae;
            }
        }
    }
}
