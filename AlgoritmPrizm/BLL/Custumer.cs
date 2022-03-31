using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Класс представляющий из серя кассира и его соответствие в чпрограмме и в чеке как будем пропивать
    /// </summary>
    public class Custumer
    {
        /// <summary>
        /// Логин в программе RPro
        /// </summary>
        public string login;

        /// <summary>
        /// Как писать в чеке fio
        /// </summary>
        public string fio_fo_check;

        /// <summary>
        /// Должность
        /// </summary>
        public string Job;

        /// <summary>
        /// Какой ИНН у кассира для подстановки в FR
        /// </summary>
        public string inn;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="login">Логин в программе RPro</param>
        /// <param name="fio_fo_check">Как писать в чеке fio</param>
        /// <param name="Job">Должность</param>
        /// <param name="inn">Какой ИНН у кассира для подстановки в FR</param>
        public Custumer(string login, string fio_fo_check, string Job, string inn)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(fio_fo_check) && !string.IsNullOrWhiteSpace(inn))
                {
                    this.login = login;
                    this.fio_fo_check = fio_fo_check;
                    this.Job = Job;
                    this.inn = inn;
                    return;
                }
                throw new ApplicationException("Все поля обязательные и пустыми оставлять нельзя.");
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
            
        }
    }
}
