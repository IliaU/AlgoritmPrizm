using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Класс для обработки отправки сообщений на различные емайлы
    /// </summary>
    public static class SmtpFarm
    {
        /// <summary>
        /// Основной класс отвечающий за отправку писем
        /// </summary>
        public static SMTPList SmtpList = SMTPList.GetInstance();
    }
}
