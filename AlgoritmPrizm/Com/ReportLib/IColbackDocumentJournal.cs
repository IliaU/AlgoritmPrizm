using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;


namespace AlgoritmPrizm.Com.ReportLib
{
    public interface IColbackDocumentJournal
    {
        /// <summary>
        /// Делигат для получения строки и принятия решения о выводе документа на основе фильтрации
        /// </summary>
        /// <param name="journal">Строка которую фильтруем</param>
        /// <returns>Результат фильтрации если True то добавить в ответ, если False</returns>
        bool ColbackDocumentJournal(JsonGetDocumentJournal journal);
    }
}
