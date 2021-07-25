using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Структура для отображения и расшифровки кодов ошибки
    /// </summary>
    public class FrStatError
    {
        /// <summary>
        /// Код ошибки ECRMode
        /// </summary>
        public int CodeECRMode=0;

        /// <summary>
        /// Код ошибки операции
        /// </summary>
        public int CodeResult=0;

        /// <summary>
        /// Описание ошибки ECRMode
        /// </summary>
        public string DescriptionECRMode;

        /// <summary>
        /// Описание ошибки Result
        /// </summary>
        public string DescriptionResult;

        /// <summary>
        /// Отображает состояние ошибки есть или нет
        /// </summary>
        public bool HashError
        {
            get
            {
                if (CodeECRMode != 0 || CodeResult != 0) return true;
                else return false;
            }
            private set { }
        }

        /// <summary>
        /// Сообщение про ошибку с описанием кодов состояния
        /// </summary>
        public string Description
        {
            get
            {
                string rez=null;
                if (CodeECRMode != 0)
                {
                    rez = string.Format("ECRMode = {0}({1})", CodeECRMode, DescriptionECRMode);
                }

                if  (CodeResult != 0)
                {
                    if (rez != null) rez += "\r\n";
                    rez += string.Format("CodeResult = {0}({1})", CodeResult, DescriptionResult);
                }

                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Конструтор
        /// </summary>
        public FrStatError()
        {

        }
    }
}
