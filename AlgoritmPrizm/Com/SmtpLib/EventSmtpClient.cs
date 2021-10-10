using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Com.SmtpLib
{
    /// <summary>
    /// Для события отправки письма
    /// </summary>
    public class EventSmtpClient : EventArgs
    {
        /// <summary>
        /// Сообщение
        /// </summary>
        public Mail email;

        /// <summary>
        /// Клинет который обслуживает данный процесс
        /// </summary>
        public SMTPClient smtpClient;

        /// <summary>
        /// Клиент которы обслуживает данный процесс
        /// </summary>
        public HTTPClient httpClient;

        /// <summary>
        /// Статус отправвки
        /// </summary>
        public string Status;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="smtpClient"></param>
        /// <param name="email"></param>
        public EventSmtpClient(SMTPClient smtpClient, Mail email, string Status)
        {
            this.smtpClient = smtpClient;
            this.email = email;
            this.Status = Status;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="smtpClient"></param>
        /// <param name="email"></param>
        public EventSmtpClient(HTTPClient httpClient, Mail email, string Status)
        {
            this.httpClient = httpClient;
            this.email = email;
            this.Status = Status;
        }
    }
}
