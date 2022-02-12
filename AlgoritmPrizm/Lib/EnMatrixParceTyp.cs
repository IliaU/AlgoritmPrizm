using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    ///  Тип парсинга который используем во время поиска нужно спрашивать матрикс код или нет
    /// </summary>
    public enum EnMatrixParceTyp
    {
        /// <summary>
        /// Стандартное с полным совпадением строки
        /// </summary>
        Normal,
        /// <summary>
        /// С поиском до разделителя
        /// </summary>
        Seporate,
        /// <summary>
        /// С использованием регулярного выражения
        /// </summary>
        Regular
    }
}
