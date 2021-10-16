using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Типы СМС шлюхов которые мы поддерживаем
    /// </summary>
    public enum EnSmsTypGateway
    {
        /// <summary>
        /// СМС шлюз выключен и не используется
        /// </summary>
        Empty,

        /// <summary>
        /// Используется в качествен провайдера Smsc.ru который получает сообщения через электронное письмо
        /// </summary>
        Smsc_RU,

        /// <summary>
        /// Используется в качестве провайдера Infobip.Com который получает сообщение через API по http протоколу
        /// </summary>
        Infobip_Com
    }
}
