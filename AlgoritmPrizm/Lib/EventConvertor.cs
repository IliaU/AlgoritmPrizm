using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Класс для конверсации cобытий из строк в энумератор
    /// </summary>
    public static class EventConvertor
    {
        /// <summary>
        /// Конвертация в объект eventEn
        /// </summary>
        /// <param name="EventStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaulfEvent">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static EventEn Convert(string EventStr, EventEn DefaulfEvent)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(EventStr))
                {
                    foreach (EventEn item in EventEn.GetValues(typeof(EventEn)))
                    {
                        if (item.ToString().ToUpper() == EventStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaulfEvent;
            }
            catch (Exception)
            {
                return DefaulfEvent;
            }
        }

        /// <summary>
        /// Конвертация в объект FfdEn
        /// </summary>
        /// <param name="FfdStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaulfFfd">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FfdEn Convert(string FfdStr, FfdEn DefaulfFfd)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FfdStr))
                {
                    foreach (FfdEn item in FfdEn.GetValues(typeof(FfdEn)))
                    {
                        if (item.ToString().ToUpper() == FfdStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaulfFfd;
            }
            catch (Exception)
            {
                return DefaulfFfd;
            }
        }

        /// <summary>
        /// Конвертация в объект FieldItemEn
        /// </summary>
        /// <param name="FieldItemStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaulfFieldItem">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FieldItemEn Convert(string FieldItemStr, FieldItemEn DefaulfFieldItem)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FieldItemStr))
                {
                    foreach (FieldItemEn item in FfdEn.GetValues(typeof(FieldItemEn)))
                    {
                        if (item.ToString().ToUpper() == FieldItemStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaulfFieldItem;
            }
            catch (Exception)
            {
                return DefaulfFieldItem;
            }
        }

        /// <summary>
        /// Конвертация в объект FieldDocNumEn
        /// </summary>
        /// <param name="FieldDocNumStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaulfFieldDocNum">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FieldDocNumEn Convert(string FieldDocNumStr, FieldDocNumEn DefaulfFieldDocNum)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FieldDocNumStr))
                {
                    foreach (FieldDocNumEn item in FfdEn.GetValues(typeof(FieldDocNumEn)))
                    {
                        if (item.ToString().ToUpper() == FieldDocNumStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaulfFieldDocNum;
            }
            catch (Exception)
            {
                return DefaulfFieldDocNum;
            }
        }
    }
}
