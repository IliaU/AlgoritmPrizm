using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Com.SmtpLib
{
    public class Mail
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public string MessageId { get; private set; }

        /// <summary>
        /// Указывает что идентификатор сообщения назначен по умолчанию
        /// </summary>
        public bool DefaultMessageId { get; private set; }

        /// <summary>
        /// От кого сообщение
        /// </summary>
        public string From { get; private set; }

        /// <summary>
        /// Список кому отправлять сообщения
        /// </summary>
        public string To { get; private set; }

        /// <summary>
        /// Тема сообщения
        /// </summary>
        public string Subject { get; private set; }

        /// <summary>
        /// Тело сообщения
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// Текст сообщения, который показывать в интерфейсе нашему кассиру, если не указан, то будет показан текст изполя Body
        /// </summary>
        public string PrintBody { get; private set; }

        /// <summary>
        /// Кодировка в которой работает сервер
        /// </summary>
        public Encoding CHCP { get; private set; }

        /// <summary>
        /// Запрос, который нужно выполнить после успешной отправки письма.
        /// </summary>
        public string PostSQL { get; private set; }

        /// <summary>
        /// Тег в который можно для промежуточного результата что-то записать
        /// </summary>
        public object Tag;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MessageId">Идентификатор сообщения</param>
        /// <param name="From">От кого сообщение</param>
        /// <param name="To">Список кому отправлять сообщения</param>
        /// <param name="Subject">Тема сообщения</param>
        /// <param name="Body">Тело сообщения</param>
        /// <param name="PrintBody">Текст сообщения, который показывать в интерфейсе нашему кассиру, если не указан, то будет показан текст изполя Body</param>
        /// <param name="CHCP">Кодировка в которой работает сервер</param>
        /// <param name="PostSQL">Запрос, который нужно выполнить после успешной отправки письма.</param>
        public Mail(string MessageId, string From, string To, string Subject, string Body, string PrintBody, Encoding CHCP, string PostSQL)
        {
            if (From != null) this.From = From.Trim();
            if (To != null) this.To = To.Trim();
            if (Subject != null) this.Subject = Subject.Trim();
            this.Body = Body;
            this.PrintBody = PrintBody;
            if (CHCP == null) this.CHCP = Encoding.GetEncoding("koi8-r");
            else this.CHCP = CHCP;
            this.PostSQL = PostSQL;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="From">От кого сообщение</param>
        /// <param name="To">Список кому отправлять сообщения</param>
        /// <param name="Subject">Тема сообщения</param>
        /// <param name="Body">Тело сообщения</param>
        ///  <param name="PrintBody">Текст сообщения, который показывать в интерфейсе нашему кассиру, если не указан, то будет показан текст изполя Body</param>
        /// <param name="CHCP">Кодировка в которой работает сервер</param>
        /// <param name="PostSQL">Запрос, который нужно выполнить после успешной отправки письма.</param>
        public Mail(string From, string To, string Subject, string Body, string PrintBody, Encoding CHCP, string PostSQL)
            : this(Guid.NewGuid().ToString(), From, To, Subject, Body, PrintBody, CHCP, PostSQL)
        {
            this.DefaultMessageId = true;
        }

        /// <summary>
        /// Назначение нового идентификатора сообщения, если оно было дефолтным
        /// </summary>
        /// <param name="MessageId">Идентификатор сообщения</param>
        public void SetMessageId(string MessageId)
        {
            if (this.DefaultMessageId)
            {
                this.MessageId = MessageId;
                this.DefaultMessageId = false;
            }
            else throw new ApplicationException("Идентификатор сообщения уже назначен.");
        }
    }
}
