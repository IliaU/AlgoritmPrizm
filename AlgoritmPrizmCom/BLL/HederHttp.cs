using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizmCom.BLL
{
    /// <summary>
    /// Вспомогательный клас для заголовков страницы
    /// </summary>
    public class HederHttp
    {
        /// <summary>
        /// Имя в заголовке пакета
        /// </summary>
        public string AtributeName { get; private set; }

        /// <summary>
        /// Значение в заголовке
        /// </summary>
        public string AtributeValue { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="AtributeName">Имя в заголовке пакета</param>
        /// <param name="AtributeValue">Значение в заголовке</param>
        public HederHttp(string AtributeName, string AtributeValue)
        {
            this.AtributeName = AtributeName;
            this.AtributeValue = AtributeValue;
        }
    }
}
