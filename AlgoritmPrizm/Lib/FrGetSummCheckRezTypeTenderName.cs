using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    public class FrGetSummCheckRezTypeTenderName
    {
        /// <summary>
        /// Имя карты например виза или мастеркард
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Сумма
        /// </summary>
        public decimal Value;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Name">Имя карты например виза или мастеркард</param>
        /// <param name="Value">Сумма</param>
        public FrGetSummCheckRezTypeTenderName (string Name, decimal Value)
        {
            try
            {
                this.Name = Name;
                this.Value = Value;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
