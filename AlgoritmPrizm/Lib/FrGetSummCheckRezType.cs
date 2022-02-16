using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// тип оплаты
    /// </summary>
    public class FrGetSummCheckRezType
    {
        /// <summary>
        /// Тип оплаты например нал или безнал
        /// </summary>
        public FrGetSummCheckRezTypeEn TenderTyp;

        /// <summary>
        /// итог с учётом вложения в TenderName 
        /// </summary>
        public decimal PromItog;

        /// <summary>
        /// Тут список например карт виза и сумма по ним мастер карт и сумма по ним
        /// </summary>
        public List<FrGetSummCheckRezTypeTenderName> TenderName = new List<FrGetSummCheckRezTypeTenderName>();

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="TenderTyp"></param>
        /// <param name="PromItog"></param>
        public FrGetSummCheckRezType(FrGetSummCheckRezTypeEn TenderTyp, decimal PromItog)
        {
            try
            {
                this.TenderTyp = TenderTyp;
                this.PromItog = PromItog;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
