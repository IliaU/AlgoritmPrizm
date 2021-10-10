using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Класс представляющий из себя параметры, которые нужно передать в запрос которй будет выполняься в базе данных
    /// </summary>
    public class Param
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name;

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Name">Имя параметра</param>
        /// <param name="Value">Значение параметра</param>
        public Param(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
    }
}
