using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Ккласс для событий аоказывающий статус отправлялски писем
    /// </summary>
    public class EventSmtpListStatus : EventArgs
    {
        /// <summary>
        /// Событие со статусом
        /// </summary>
        public string Status { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Status">Событие со статусом</param>
        public EventSmtpListStatus(string Status)
        {
            this.Status = Status;
        }
    }
}
