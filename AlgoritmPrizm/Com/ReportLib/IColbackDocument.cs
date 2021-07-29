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
    /// Интерфейс для перенаправления при фильтрации документа
    /// </summary>
    public interface IColbackDocument
    {
        /// <summary>
        /// Интерфейс для получения документов построчно и принятия решения о выводе документа на основе фильтрации
        /// </summary>
        /// <param name="Document">Документ который возвращает призм и который можно филтрануть</param>
        /// <returns>Результат фильтрации если возвращаем хоть одну строку то документ будет включён в список если не возвращаем ничего то документ не возвращается в список</returns>
        List<JsonGetDocumentJournal> ColbackDocument(JsonGetDocumentsData Document);

    }
}
