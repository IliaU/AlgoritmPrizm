using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com.SmtpLib
{
    /// <summary>
    /// Класс реализующий SMTP клиента
    /// </summary>
    public class SMTPClient
    {
        #region Private Param
        /// <summary>
        /// Почтовый клиент
        /// </summary>
        private SmtpClient smtp = null;

        /// <summary>
        /// Поток который отправляет сообщения
        /// </summary>
        private Thread Thr;

        /// <summary>
        /// Очередь сообщений которую нужно обработать
        /// </summary>
        private Queue<Mail> queue = new Queue<Mail>();
        #endregion

        #region Puplic Param
        /// <summary>
        /// Количество успешно отправленных сообщений
        /// </summary>
        public int SendSuccessMes { get; private set; }

        /// <summary>
        /// Количество не отправленных сообщений
        /// </summary>
        public int SendErrorMes { get; private set; }

        /// <summary>
        /// Статус асинхронного процесса
        /// </summary>
        public bool HashRun { get; private set; }

        /// <summary>
        /// Сервер на который идёт отправка например: smtp.mail.ru
        /// </summary>
        public string SmtpServer { get; private set; }

        /// <summary>
        /// Порт на котором работает почтовый сервер по умолчанию 25
        /// </summary>
        public int SmtpPort { get; private set; }

        /// <summary>
        /// Пользователь под которым залогиниться на сервер
        /// </summary>
        public string SmtpUser { get; private set; }

        /// <summary>
        /// Пароль пользователя под котором залогинится на сервер
        /// </summary>
        public string SmtpPassword { get; private set; }

        /// <summary>
        /// Использовать при отправке шифрование?<
        /// </summary>
        public bool SSL { get; private set; }

        /// <summary>
        /// Событие отпраки сообщения
        /// </summary>
        public event EventHandler<EventSmtpClient> onEventSendMail;
        #endregion

        #region Puplic metod
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="SmtpServer">Сервер на который идёт отправка например: smtp.mail.ru</param>
        /// <param name="SmtpPort">Порт на котором работает почтовый сервер по умолчанию 25</param>
        /// <param name="SmtpUser">Пользователь под которым залогиниться на сервер</param>
        /// <param name="SmtpPassword">Пароль пользователя под котором залогинится на сервер</param>
        /// <param name="SSL">Использовать при отправке шифрование?</param>
        /// <param name="DeliveryMethod">Метод отправки писем</param>
        public SMTPClient(string SmtpServer, int SmtpPort, string SmtpUser, string SmtpPassword, bool SSL, string DeliveryMethod)
        {
            this.SmtpServer = SmtpServer;
            if (SmtpPort > 0) this.SmtpPort = SmtpPort;
            else this.SmtpPort = 25;

            this.SmtpUser = SmtpUser;
            this.SmtpPassword = SmtpPassword;

            this.SSL = SSL;

            // Создаём клиента для отправки сообщений
            smtp = new SmtpClient(this.SmtpServer, this.SmtpPort);
            if (!string.IsNullOrWhiteSpace(this.SmtpUser) && !string.IsNullOrWhiteSpace(this.SmtpPassword))
            {
                smtp.Credentials = new System.Net.NetworkCredential(SmtpUser, SmtpPassword);
            }
            smtp.EnableSsl = this.SSL;

            switch (DeliveryMethod)
            {
                case "Network":
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    break;
                case "PickupDirectoryFromIis":
                    smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                    break;
                case "SpecifiedPickupDirectory":
                    smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Добавление в очередь сообщения для отправки
        /// </summary>
        /// <param name="eMail">Сообщение которое нужно отправить</param>
        public void AddEmail(Mail eMail)
        {
            this.queue.Enqueue(eMail);
        }

        /// <summary>
        /// Запуск процесса отправки сообщений
        /// </summary>
        public void StartSend()
        {
            if (!this.HashRun)
            {
                HashRun = true;
                this.SendSuccessMes = 0;
                this.SendErrorMes = 0;

                // асинхронный запуск нашего метода
                //Thread Thrtmp = new Thread(new ThreadStart(TaskThread.Run));
                this.Thr = new Thread(StartSendRun);
                Thr.Name = this.SmtpServer;
                Thr.IsBackground = true;
                Thr.Start();
            }
        }

        /// <summary>
        /// Остановка асинхронного отправления писем
        /// </summary>
        public void StopSend()
        {
            this.HashRun = false;
            this.Thr.Join();
        }
        #endregion

        #region Private metod

        /// <summary>
        /// Асинхронная отправка писем
        /// </summary>
        private void StartSendRun()
        {
            // На всякий пожарный пауза на создание подключения к серверу, если оно ещё не создалось
            Thread.Sleep(2000);

            // Если процесс не завершился то асинхронная работа не заканчивается до завершения отправки всех писем
            while (this.HashRun || this.queue.Count > 0)
            {
                // Если в очереди ничего нет, нужно подождать может пользователь ещё закинет сообщений
                if (this.queue.Count == 0)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // Извлекаем сообщение
                Mail eMail = this.queue.Dequeue();

                try
                {
                    MailMessage mail = new MailMessage();

                    mail.HeadersEncoding = eMail.CHCP;
                    mail.From = new MailAddress(ConvertEncoding(Encoding.Default, eMail.CHCP, eMail.From));

                    foreach (string item in eMail.To.Split(','))
                    {
                        mail.To.Add(new MailAddress(ConvertEncoding(Encoding.Default, eMail.CHCP, item.Trim())));
                    }

                    mail.SubjectEncoding = eMail.CHCP;
                    mail.Subject = ConvertEncoding(Encoding.Default, eMail.CHCP, eMail.Subject);

                    mail.BodyEncoding = eMail.CHCP;
                    mail.Body = ConvertEncoding(Encoding.Default, eMail.CHCP, eMail.Body);
                    smtp.Send(mail);

                    // Если есть запрос который нужно выполнить после отправки письма, то передаём его провайдеру
                    if (!string.IsNullOrWhiteSpace(eMail.PostSQL))
                    {
//                        Com.ProviderFarm.CurrentPrv.SetData(eMail.PostSQL);
                    }

                    // Собственно обработка события
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, "Success");
                    if (onEventSendMail != null)
                    {
                        onEventSendMail.Invoke(this, myArg);
                    }

                    // Логируем в лог факт отправки письма
                    Com.Log.EventSave(string.Format("Сообщение успешно отправлено на адресс {0} с текстом сообщения {1}", eMail.To, eMail.Body), "StartSendRun", EventEn.Message);
                }
                catch (SmtpException ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}", eMail.To), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        onEventSendMail.Invoke(this, myArg);
                    }
                }
                catch (Exception ex)
                {
                    // Обработка события
                    Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}", eMail.To), "StartSendRun", EventEn.Error);
                    EventSmtpClient myArg = new EventSmtpClient(this, eMail, ex.Message);
                    if (onEventSendMail != null)
                    {
                        onEventSendMail.Invoke(this, myArg);
                    }
                }


            }
        }

        /// <summary>
        /// Конвертация в нужную кодировку
        /// </summary>
        /// <param name="EnSource">Кодировка источника</param>
        /// <param name="enTarget">Кодировка приёмника</param>
        /// <param name="Data">Данные которые нужно перекодировать</param>
        /// <returns>Перекодированные данные</returns>
        private string ConvertEncoding(Encoding EnSource, Encoding enTarget, string Data)
        {
            byte[] tmpSource = EnSource.GetBytes(Data);
            byte[] tmpTarget = Encoding.Convert(EnSource, enTarget, tmpSource);
            return enTarget.GetString(tmpTarget);
        }
        #endregion
    }
}
