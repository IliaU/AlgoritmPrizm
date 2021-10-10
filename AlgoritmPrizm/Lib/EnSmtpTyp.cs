using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Тип запроса
    /// </summary>
    public enum EnSmtpTyp
    {
        /// <summary>
        /// Тип для отправки обычной почты
        /// </summary>
        SMTP,
        /// <summary>
        /// Тип для отправки HTTP запросом 
        /// </summary>
        HTTP,
    }
}
