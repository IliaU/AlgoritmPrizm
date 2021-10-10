using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Список параметров
    /// </summary>
    public class ParamList : IEnumerable
    {
        private static volatile object MyObj = new object();
        private List<Param> ParList = new List<Param>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ParamsLine">Список параметров который нужно распарсить. Например @Par1:Супер|@Par2:Круть</param>
        public ParamList(string ParamsLine)
        {
            foreach (string item in ParamsLine.Split('|'))
            {
                // Начинаем бить строку на части
                string[] tmpprm = item.Split(':');
                if (tmpprm.Length == 2)
                {
                    Param prm;
                    if (tmpprm[0].Trim().IndexOf(@"@") == 0) prm = new Param(tmpprm[0].Trim().Replace(@"@", ""), tmpprm[1].Trim());
                    else prm = new Param(tmpprm[1].Trim().Replace(@"@", ""), tmpprm[0].Trim());

                    this.ParList.Add(prm);
                }
            }
        }

        /// <summary>
        /// Добавление сервера на который будем отправлять сообщения
        /// </summary>
        /// <param name="CliPar">Параметры клиента</param>
        public void Add(Param Par)
        {
            try
            {
                this.ParList.Add(Par);
            }
            catch (Exception ex)
            {
                // Обработка события
                Com.Log.EventSave(string.Format("Произошло падение при добавлении параметра {0} в список ({1})", Par, ex.Message), "ParamList.Add", EventEn.Error);
            }
        }

        /// <summary>
        /// Передаём нумератор
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            lock (MyObj)
            {
                return this.ParList.GetEnumerator();
            }
        }
    }
}
