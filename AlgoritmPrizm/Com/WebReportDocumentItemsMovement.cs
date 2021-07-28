using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com
{
    // Для отчёта товаро движения
    public partial class Web
    {
        /// <summary>
        ///  Прорисовка отчёта который передадим пользователю
        /// </summary>
        /// <param name="Docs"></param>
        private static string RenderReportDocumentItemsMovement(List<JsonGetDocumentsData> Docs)
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
