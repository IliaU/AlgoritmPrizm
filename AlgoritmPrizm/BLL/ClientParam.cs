using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Com.SmtpLib;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Параметры для клиента реализующего отрпавку сообщений
    /// </summary>
    public class ClientParam
    {
        /// <summary>
        /// Smtp Сервер
        /// </summary>
        public string SmtpServer { get; private set; }

        /// <summary>
        /// Порт
        /// </summary>
        public int SmtpPort { get; private set; }

        /// <summary>
        /// Пользователь под колторым подключиться
        /// </summary>
        public string SmtpUser { get; private set; }

        /// <summary>
        /// Пароль под которым подключиться
        /// </summary>
        public string SmtpPassword { get; private set; }

        /// <summary>
        /// Использовать при отправке шифрование?
        /// </summary>
        public bool SSL { get; private set; }

        /// <summary>
        /// Метод отправки писем
        /// </summary>
        public string DeliveryMethod { get; private set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public Mail eMail { get; private set; }

        /// <summary>
        /// Информация о запросе к базе двнных из которого получили эти параметры
        /// </summary>
        public ItemSQL itmSql { get; private set; }


        /// <summary>
        /// Ссылка на который идёт отправка например: https://api.infobip.com
        /// </summary>
        public string HttpUri { get; private set; }

        /// <summary>
        /// Тип контекста, например application/json
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Http заголовок Accept, например application/json
        /// </summary>
        public string Accept { get; private set; }

        /// <summary>
        /// Метод, например POST
        /// </summary>
        public EnHttpMethod HttpMethod { get; private set; }

        /// <summary>
        /// Тип авторизации, например BasicToBase64
        /// </summary>
        public EnHttpAuthorizTyp HttpAuthorizTyp { get; private set; }

        /// <summary>
        /// Пользователь под которым залогиниться на сервер
        /// </summary>
        public string AudentUser { get; private set; }

        /// <summary>
        /// Пароль пользователя под котором залогинится на сервер
        /// </summary>
        public string AudentPassword { get; private set; }

        /// <summary>
        /// Параметру которые потом будет использовать клиент для отправки SMTP сообщений
        /// </summary>
        /// <param name="SmtpServer">Smtp Сервер</param>
        /// <param name="SmtpPort">Порт</param>
        /// <param name="SmtpUser">Пользователь под колторым подключиться</param>
        /// <param name="SmtpPassword">Пароль под которым подключиться</param>
        /// <param name="SSL">Использовать при отправке шифрование?</param>
        /// <param name="DeliveryMethod">Метод отправки писем</param>
        /// <param name="eMail">Сообщение</param>
        /// <param name="itmSql">Информация о запросе к базе двнных из которого получили эти параметры</param>
        /// <returns>Результат операции</returns>
        public ClientParam(string SmtpServer, int SmtpPort, string SmtpUser, string SmtpPassword, bool SSL, string DeliveryMethod, Mail eMail, ItemSQL itmSql)
        {
            this.SmtpServer = SmtpServer;
            this.SmtpPort = SmtpPort;
            this.SmtpUser = SmtpUser;
            this.SmtpPassword = SmtpPassword;
            this.SSL = SSL;
            this.DeliveryMethod = DeliveryMethod;
            this.eMail = eMail;
            this.itmSql = itmSql;

        }

        /// <summary>
        /// Параметру которые потом будет использовать клиент для отправки HTTP сообщений
        /// </summary>
        /// <param name="HttpUri">Ссылка на который идёт отправка например: https://api.infobip.com</param>
        /// <param name="ContentType">Тип контекста, например application/json</param>
        /// <param name="Accept">Http заголовок Accept, например application/json</param>
        /// <param name="HttpMethod">Метод, например POST</param>
        /// <param name="HttpAuthorizTyp">Тип авторизации, например BasicToBase64</param>
        /// <param name="AudentUser">Пользователь под которым залогиниться на сервер</param>
        /// <param name="AudentPassword">Пароль пользователя под котором залогинится на сервер</param>
        /// <param name="eMail">Сообщение</param>
        /// <param name="itmSql">Информация о запросе к базе двнных из которого получили эти параметры</param>
        public ClientParam(string HttpUri, string ContentType, string Accept, EnHttpMethod HttpMethod, EnHttpAuthorizTyp HttpAuthorizTyp, string AudentUser, string AudentPassword, Mail eMail, ItemSQL itmSql)
        {
            this.HttpUri = HttpUri;
            this.ContentType = ContentType;
            this.Accept = Accept;
            this.HttpMethod = HttpMethod;
            this.HttpAuthorizTyp = HttpAuthorizTyp;
            this.AudentUser = AudentUser;
            this.AudentPassword = AudentPassword;
            this.eMail = eMail;
            this.itmSql = itmSql;
        }
    }
}
