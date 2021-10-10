using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com.SmtpLib;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Класс представляющий из себя конкретный элемент запроса
    /// </summary>
    public class ItemSQL : ItemSQLBase
    {
        /// <summary>
        /// Тело запроса
        /// </summary>
        public string SQL { get; private set; }

        /// <summary>
        /// Пароль который будем использовать при отправке писем, но шифруем его в запросе
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Список параметров
        /// </summary>
        public ParamList ParList;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Name">Имя элемента</param>
        /// <param name="SmtpTyp">Тип запроса SMTP или HTTP</param>
        /// <param name="SQL">Тело запроса</param>
        /// <param name="Password">Пароль который будем использовать при отправке писем, но шифруем его в запросе</param>
        public ItemSQL(string Name, EnSmtpTyp SmtpTyp, string SQL, string Password)
        {
            this.SQL = SQL;
            this.Password = Password;
            base.InitialConfiguration(Name, SmtpTyp);
        }
        //
        /// <summary>
        /// Дополнительный конструктор
        /// </summary>
        /// <param name="Name">Имя элемента</param>
        /// <param name="SmtpTyp">Тип запроса SMTP или HTTP</param>
        /// <param name="SQL">Тело запроса</param>
        /// <param name="Password">Пароль который будем использовать при отправке писем, но шифруем его в запросе</param>
        /// <param name="ParList">Список параметров</param>
        public ItemSQL(string Name, EnSmtpTyp SmtpTyp, string SQL, string Password, ParamList ParList)
            : this(Name, SmtpTyp, SQL, Password)
        {
            this.ParList = ParList;
        }

        /// <summary>
        /// Возвращает запрос к источнику данных с учётом применения параметров и пароля к запросу
        /// </summary>
        /// <returns></returns>
        public string QwerySql()
        {
            string rez = null;

            // Подменяем пароль в запросе.
            rez = this.SQL.Replace(@"@Password", this.Password);

            //Если существует список параметров
            if (this.ParList != null)
            {
                // Пробегаем по списку параметров
                foreach (Param item in this.ParList)
                {
                    rez = rez.Replace(@"@" + item.Name, item.Value);
                }
            }


            return rez;
        }

    }
}
