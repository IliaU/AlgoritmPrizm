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
    /// Для получения результата запроса полей text1-10 из карточки товаров
    /// </summary>
    public class InvnSbsItemText
    {
        public string Text1;
        public string Text2;
        public string Text3;
        public string Text4;
        public string Text5;
        public string Text6;
        public string Text7;
        public string Text8;
        public string Text9;
        public string Text10;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Text1"></param>
        /// <param name="Text2"></param>
        /// <param name="Text3"></param>
        /// <param name="Text4"></param>
        /// <param name="Text5"></param>
        /// <param name="Text6"></param>
        /// <param name="Text7"></param>
        /// <param name="Text8"></param>
        /// <param name="Text9"></param>
        /// <param name="Text10"></param>
        public InvnSbsItemText(string Text1, string Text2, string Text3, string Text4, string Text5, string Text6, string Text7, string Text8, string Text9, string Text10)
        {
            try
            {
                this.Text1 = Text1;
                this.Text2 = Text2;
                this.Text3 = Text3;
                this.Text4 = Text4;
                this.Text5 = Text5;
                this.Text6 = Text6;
                this.Text7 = Text7;
                this.Text8 = Text8;
                this.Text9 = Text9;
                this.Text10 = Text10;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Не смогли создать объект. Ошибка: {0}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }
    }
}
