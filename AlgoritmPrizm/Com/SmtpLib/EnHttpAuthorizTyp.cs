using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Com.SmtpLib
{
    /// <summary>
    /// Тип авторизации используемый в HTTP клиенте, например BasicToBase64
    /// </summary>
    public enum EnHttpAuthorizTyp
    {
        /// <summary>
        /// Не задан
        /// </summary>
        Empty,
        /// <summary>
        /// Базовая авторизация с шифрованием креденшина базовым 64 битным
        /// </summary>
        BasicToBase64,
    }
}
