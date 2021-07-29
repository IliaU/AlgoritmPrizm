using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;
using AlgoritmPrizm.Com.ReportLib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.Com.Report
{
    /// <summary>
    /// Класс для отрисовки отчёта по движению товара
    /// </summary>
    public class ReportItemsMovement:BReport, IColbackDocument
    {
        /// <summary>
        /// Параметр начала переиода с которого обрабатываем документы
        /// </summary>
        public DateTime StartDt;

        /// <summary>
        /// Спаисок документов этого отчёта
        /// </summary>
        public List<JsonGetDocumentsData> Docs;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="StartDt"></param>
        public ReportItemsMovement(DateTime StartDt)
        {
            this.StartDt = StartDt;
        }

        /// <summary>
        /// Обработка фильтра для  принятия решения нужен нам этот документ или нет для отчёта движение товара
        /// </summary>
        /// <param name="Document">Документ который считался (его заголовок)</param>
        /// <returns></returns>
        public override List<JsonGetDocumentJournal> ColbackDocument(JsonGetDocumentsData Document)
        {
            List<JsonGetDocumentJournal> rez = new List<JsonGetDocumentJournal>();
            try
            {
                // Получаем информацию по строкам документа
                List<JsonGetDocumentJournal> journal = Web.GetDocumentsJournalRestSharp(Document, null);

                rez = journal;

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.FilterReportDocument", EventEn.Error);
                throw ae;
            }
        }



        /// <summary>
        ///  Прорисовка отчёта который передадим пользователю
        /// </summary>
        /// <param name="Docs"></param>
        public string RenderReport()
        {
            // https://html5book.ru/html5-forms/
            string rez = "";
            try
            {
                rez = @"<!DOCYUPE html>";
                rez += @"<html>";
                rez += @"<head>";
                rez += @"</head>";
                rez += @"<body>";
                rez += @"   <form>";
                rez += @"       <fieldset>";
                rez += @"           <legend>Контактная информация</legend>";
                rez += @"           <p><label for=""name"">Имя <em>*</em></label><input type=""text"" id=""name""></p>";
                rez += @"           <p><label for=""email"">E-mail</label><input type=""email"" id=""email""></p>";
                rez += @"     </fieldset>";
                rez += @"    <p><input type=""submit"" value=""Отправить""></p>";
                rez += @"    </form>";
                rez += @"</body>";
                rez += @"</html>";
                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.Web.RenderReportDocument", EventEn.Error);
                throw ae;
            }
        }
    }
}
