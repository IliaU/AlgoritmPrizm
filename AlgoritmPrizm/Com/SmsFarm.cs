using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Реализовывает отправку SMS
    /// </summary>
    public static class SmsFarm
    {
        /// <summary>
        /// Класс который реализует отправку с помощью SMTP шлюза
        /// </summary>
        private static SmtpLib.SMTPClient SmtpCli = null;

        /// <summary>
        /// Класс который реализует отправку с помощью HTTP шлюза
        /// </summary>
        private static SmtpLib.HTTPClient HttpCli = null;

        /// <summary>
        /// Запуск модуля отправки сообщений
        /// </summary>
        public static void SmsStart()
        {
            try
            {
                Log.EventSave(string.Format("Загрузка модуля отправки сообщений в режиме: {0}", Com.Config.SmsTypGateway), "Com.SmsFarm.SmsStart", EventEn.Message);

                switch (Com.Config.SmsTypGateway)
                {
                    case Lib.EnSmsTypGateway.Smsc_RU:
                        SmtpCli = new SmtpLib.SMTPClient(Com.Config.SmsTypGatewaySmtp, Com.Config.SmsTypGatewayPort, Com.Config.SmsTypGatewaySmtpLogin, Com.Config.SmsTypGatewaySmtpPassword, true, null);
                        SmtpCli.StartSend();
                        break;
                    case Lib.EnSmsTypGateway.Infobip_Com:
                        HttpCli = new SmtpLib.HTTPClient(@"https://api.infobip.com/sms/1/text/single", @"application/json", @"application/json", Com.SmtpLib.EnHttpMethod.POST, Com.SmtpLib.EnHttpAuthorizTyp.BasicToBase64, Com.Config.SmsTypGatewayLogin, Com.Config.SmsTypGatewayPassword);
                        HttpCli.StartSend();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отправки СМС с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.SmsFarm.SmsStart", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Остановка можуля отправки сообщений
        /// </summary>
        public static void SmsStop()
        {
            try
            {
                if (SmtpCli != null) SmtpCli.StopSend();
                if (HttpCli != null) HttpCli.StopSend();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при остановки модуля отправки СМС с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.SmsFarm.SmsStop", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Отправка SMS клиенту
        /// </summary>
        /// <param name="JsnSms">Сообщение которое надо отправить</param>
        /// <param name="From">От кого сообщение</param>
        public static void AddMessageSms(JsonSms JsnSms, string From)
        {
            try
            {
                string TxtFrom = From ?? Config.NameCompany;
                string MesEmeilBody;
                switch (Com.Config.SmsTypGateway)
                {
                    case Lib.EnSmsTypGateway.Smsc_RU:
                        if (SmtpCli == null) throw new ApplicationException("Не инициирован класс SmtpCli отправка сообщения невозможна");
                        MesEmeilBody = string.Format("{0}:{1}:::,0,,1:{2}:{3}", Config.SmsTypGatewayLogin, Config.SmsTypGatewayPassword, JsnSms.phone, JsnSms.text);
                        SmtpCli.AddEmail(new Com.SmtpLib.Mail(Config.SmsTypGatewayLogin, "send@send.smsc.ru", "Null", MesEmeilBody, null, Encoding.GetEncoding("koi8-r"), null));
                        break;
                    case Lib.EnSmsTypGateway.Infobip_Com:
                        if (HttpCli == null) throw new ApplicationException("Не инициирован класс HttpCli отправка сообщения невозможна");
                        MesEmeilBody = string.Format(@"
{  
    ""from"":""{0}"",
    ""to"":""{1}"",
    ""text"":""{2}""
}", TxtFrom, JsnSms.phone, JsnSms.text);
                        HttpCli.AddEmail(new Com.SmtpLib.Mail(TxtFrom, JsnSms.phone, "", MesEmeilBody, JsnSms.text, System.Text.Encoding.Default, null));
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытке добавить в очередь сообщение для отправки с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.SmsFarm.AddMessageSms", EventEn.Error);
                throw ae;
            }
        }
    }
}
