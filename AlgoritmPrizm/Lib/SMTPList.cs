using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using AlgoritmPrizm.Com.SmtpLib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Класс показывающий список всех клиентов
    /// </summary>
    public class SMTPList
    {
        private Thread Thr;
        private static volatile SMTPList MyObj = null;
        private static List<SMTPClient> SmtpList = new List<SMTPClient>();
        private static List<HTTPClient> HttpList = new List<HTTPClient>();


        /// <summary>
        /// Событие отпраки сообщения
        /// </summary>C:\Users\user\Documents\Visual Studio 2010\Projects\AlgoritmSMTP\AlgoritmSMTP\Lib\ParamList.cs
        public event EventHandler<EventSmtpClient> onEventSendMail;

        /// <summary>
        /// Событие изменения статуса
        /// </summary>
        public event EventHandler<EventSmtpListStatus> onEventStatus;

        /// <summary>
        /// Загрузка синглетон объекта
        /// </summary>
        /// <returns>SMTPList</returns>
        public static SMTPList GetInstance()
        {
            if (MyObj == null) MyObj = new SMTPList();
            return MyObj;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        private SMTPList()
        {

        }

        /// <summary>
        /// Добавление сервера на который будем отправлять сообщения
        /// </summary>
        /// <param name="CliPar">Параметры клиента</param>
        public void AddClient(ClientParam CliPar)
        {
            try
            {
                EventSmtpListStatus myArg = new EventSmtpListStatus("Running");
                if (onEventStatus != null)
                {
                    onEventStatus(this, myArg);
                }

                switch (CliPar.itmSql.SmtpTyp)
                {
                    case EnSmtpTyp.SMTP:
                        this.AddSmtpClientSMTP(CliPar);
                        break;
                    case EnSmtpTyp.HTTP:
                        this.AddSmtpClientHTTP(CliPar);
                        break;
                    default:
                        throw new ApplicationException(string.Format("Данный тип рассылки ({0}) ещё не реализован в методе SMTPList.AddSmtpClient.", CliPar.itmSql.SmtpTyp));
                }
            }
            catch (Exception ex)
            {
                // Обработка события
                Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}", ex.Message), "SMTPList.AddSmtpClient", EventEn.Error);
            }
        }

        /// <summary>
        /// Добавление сервера на который будем отправлять сообщения
        /// </summary>
        /// <param name="CliPar">Параметры клиента</param>
        public void AddSmtpClientSMTP(ClientParam CliPar)
        {
            try
            {
                SMTPClient CurSmtpClient = null;

                bool FlagAdd = true;
                SMTPClient nSMTPClient = new SMTPClient(CliPar.SmtpServer, CliPar.SmtpPort, CliPar.SmtpUser, CliPar.SmtpPassword, CliPar.SSL, CliPar.DeliveryMethod);
                foreach (SMTPClient item in SmtpList)
                {
                    if (item.SmtpServer == nSMTPClient.SmtpServer
                        && item.SmtpPort == nSMTPClient.SmtpPort
                        && item.SmtpPassword == nSMTPClient.SmtpPassword
                        && item.SmtpUser == nSMTPClient.SmtpUser
                        && item.SSL == nSMTPClient.SSL)
                    {
                        FlagAdd = false;
                        CurSmtpClient = item;
                        break;
                    }
                }

                // Если этого сервера в нашем списке нет, то добавлем его
                if (FlagAdd)
                {
                    SmtpList.Add(nSMTPClient);
                    CurSmtpClient = nSMTPClient;
                    nSMTPClient.onEventSendMail += new EventHandler<EventSmtpClient>(nSMTPClient_onEventSendMail);
                }

                // Найден клиент который должен отправить сообщение
                if (CurSmtpClient != null)
                {
                    // Добавляем сообщение
                    CurSmtpClient.AddEmail(CliPar.eMail);

                    // Если процесс отправки не запущен запускаем его
                    if (!CurSmtpClient.HashRun) CurSmtpClient.StartSend();
                }

            }
            catch (Exception ex)
            {
                // Обработка события
                Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}", ex.Message), "SMTPList.AddSmtpClientSMTP", EventEn.Error);
            }
        }


        /// <summary>
        /// Добавление сервера на который будем отправлять сообщения
        /// </summary>
        /// <param name="CliPar">Параметры клиента</param>
        public void AddSmtpClientHTTP(ClientParam CliPar)
        {
            try
            {
                HTTPClient CurHttpClient = null;

                bool FlagAdd = true;
                HTTPClient nHttpClient = new HTTPClient(CliPar.HttpUri, CliPar.ContentType, CliPar.Accept, CliPar.HttpMethod, CliPar.HttpAuthorizTyp, CliPar.AudentUser, CliPar.AudentPassword);
                foreach (HTTPClient item in HttpList)
                {
                    if (item.HttpUri == nHttpClient.HttpUri)
                    {
                        FlagAdd = false;
                        CurHttpClient = item;
                        break;
                    }
                }

                // Если этого сервера в нашем списке нет, то добавлем его
                if (FlagAdd)
                {
                    HttpList.Add(nHttpClient);
                    CurHttpClient = nHttpClient;
                    nHttpClient.onEventSendMail += new EventHandler<EventSmtpClient>(nSMTPClient_onEventSendMail);
                }

                // Найден клиент который должен отправить сообщение
                if (CurHttpClient != null)
                {
                    // Добавляем сообщение
                    CurHttpClient.AddEmail(CliPar.eMail);

                    // Если процесс отправки не запущен запускаем его
                    if (!CurHttpClient.HashRun) CurHttpClient.StartSend();
                }

            }
            catch (Exception ex)
            {
                // Обработка события
                Com.Log.EventSave(string.Format("Произошло падение при отправке сообшения пользоваелю: {0}", ex.Message), "SMTPList.AddSmtpClientSMTP", EventEn.Error);
            }
        }

        // Событие отправки сообщения
        void nSMTPClient_onEventSendMail(object sender, EventSmtpClient e)
        {
            if (onEventSendMail != null)
            {
                onEventSendMail.Invoke(this, e);
            }
        }

        /// <summary>
        /// Остановка отпраки сообщений, говорит о том, что сообщения отправляться более не будут. Отправятся только те что сейчас находятся в очереди
        /// </summary>
        /// <param name="Sinhonize">режим в котором запустить остановку сервиса. Если True, то будет дожидаться окончания работы всех потоков</param>
        public void StopSend(bool Sinhonize)
        {

            if (Sinhonize) this.StopSendRun();
            else
            {
                // асинхронный запуск нашего метода
                //Thread Thrtmp = new Thread(new ThreadStart(TaskThread.Run));
                this.Thr = new Thread(StopSendRun);
                Thr.Name = "SMTPList";
                Thr.IsBackground = true;
                Thr.Start();
            }
        }
        /// <summary>
        /// Остановка отпраки сообщений, говорит о том, что сообщения отправляться более не будут. Отправятся только те что сейчас находятся в очереди
        /// </summary>
        public void StopSend()
        {
            this.StopSend(false);
        }

        /// <summary>
        /// Асинхронная остановка процесса отправки сообщений
        /// </summary>
        public void StopSendRun()
        {
            int SendSuccessMes = 0;
            int SendErrorMes = 0;

            for (int i = 0; i < SmtpList.Count; i++)
            {
                SmtpList[i].StopSend();
                SendSuccessMes += SmtpList[i].SendSuccessMes;
                SendErrorMes += SmtpList[i].SendErrorMes;
            }

            for (int i = 0; i < HttpList.Count; i++)
            {
                HttpList[i].StopSend();
                SendSuccessMes += HttpList[i].SendSuccessMes;
                SendErrorMes += HttpList[i].SendErrorMes;
            }

            EventSmtpListStatus myArg = new EventSmtpListStatus("Stop");
            if (onEventStatus != null)
            {
                onEventStatus(this, myArg);
            }

            // Обработка события
            Com.Log.EventSave(string.Format("Отправка всех сообщений завершена. Успешно отправлено {0} сообщений c ошибкой {1}", SendSuccessMes, SendErrorMes), "SMTPList.StopSendRun", (SendErrorMes == 0 ? EventEn.Message : EventEn.Error));
        }
    }
}
