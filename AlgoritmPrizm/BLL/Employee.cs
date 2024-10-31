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
    /// Сотрудник по логину в Prizm делаем сопоставление с данными от туда
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Логин в программе RPro
        /// </summary>
        public string PrizmLogin;

        /// <summary>
        /// Как писать в чеке fio
        /// </summary>
        public string fio_fo_check;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="PrizmLogin">Логин в программе RPro</param>
        /// <param name="fio_fo_check">Как писать в чеке fio</param>
        public Employee(string PrizmLogin, string fio_fo_check)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(PrizmLogin) && !string.IsNullOrWhiteSpace(fio_fo_check))
                {
                    this.PrizmLogin = PrizmLogin;
                    this.fio_fo_check = fio_fo_check;
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
