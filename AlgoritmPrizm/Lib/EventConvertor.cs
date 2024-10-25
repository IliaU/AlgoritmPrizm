using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Net;

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
        /// <param name="DefaultEvent">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static EventEn Convert(string EventStr, EventEn DefaultEvent)
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
                return DefaultEvent;
            }
            catch (Exception)
            {
                return DefaultEvent;
            }
        }

        /// <summary>
        /// Конвертация в объект FfdEn
        /// </summary>
        /// <param name="FfdStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultFfd">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FfdEn Convert(string FfdStr, FfdEn DefaultFfd)
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
                return DefaultFfd;
            }
            catch (Exception)
            {
                return DefaultFfd;
            }
        }

        /// <summary>
        /// Конвертация в объект FieldItemEn
        /// </summary>
        /// <param name="FieldItemStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultFieldItem">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FieldItemEn Convert(string FieldItemStr, FieldItemEn DefaultFieldItem)
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
                return DefaultFieldItem;
            }
            catch (Exception)
            {
                return DefaultFieldItem;
            }
        }

        /// <summary>
        /// Конвертация в объект FieldDocNumEn
        /// </summary>
        /// <param name="FieldDocNumStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultFieldDocNum">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static FieldDocNumEn Convert(string FieldDocNumStr, FieldDocNumEn DefaultFieldDocNum)
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
                return DefaultFieldDocNum;
            }
            catch (Exception)
            {
                return DefaultFieldDocNum;
            }
        }


        /// <summary>
        /// Конвертация в объект EnSmsTypGateway
        /// </summary>
        /// <param name="SmsTypGatewayStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultSmsTypGateway">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static EnSmsTypGateway Convert(string SmsTypGatewayStr, EnSmsTypGateway DefaultSmsTypGateway)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SmsTypGatewayStr))
                {
                    foreach (EnSmsTypGateway item in EnSmsTypGateway.GetValues(typeof(EnSmsTypGateway)))
                    {
                        if (item.ToString().ToUpper() == SmsTypGatewayStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultSmsTypGateway;
            }
            catch (Exception)
            {
                return DefaultSmsTypGateway;
            }
        }

        /// <summary>
        /// Конвертация в объект EnProductMatrixClassType
        /// </summary>
        /// <param name="FieldDocNumStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultFieldDocNum">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static EnProductMatrixClassType Convert(string FieldDocNumStr, EnProductMatrixClassType DefaultFieldDocNum)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(FieldDocNumStr))
                {
                    foreach (EnProductMatrixClassType item in EnProductMatrixClassType.GetValues(typeof(EnProductMatrixClassType)))
                    {
                        if (item.ToString().ToUpper() == FieldDocNumStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultFieldDocNum;
            }
            catch (Exception)
            {
                return DefaultFieldDocNum;
            }
        }

        // <summary>
        /// Конвертация в объект EnProductMatrixClassType
        /// </summary>
        /// <param name="MatrixParceTypStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultFieldDocNum">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static EnMatrixParceTyp Convert(string MatrixParceTypStr, EnMatrixParceTyp DefaultMatrixParceTyp)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(MatrixParceTypStr))
                {
                    foreach (EnMatrixParceTyp item in EnMatrixParceTyp.GetValues(typeof(EnMatrixParceTyp)))
                    {
                        if (item.ToString().ToUpper() == MatrixParceTypStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultMatrixParceTyp;
            }
            catch (Exception)
            {
                return DefaultMatrixParceTyp;
            }
        }

        /// <summary>
        /// Конвертация в объект Parity
        /// </summary>
        /// <param name="ParityStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultParity">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static Parity Convert(string ParityStr, Parity DefaultParity)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(ParityStr))
                {
                    foreach (Parity item in Parity.GetValues(typeof(Parity)))
                    {
                        if (item.ToString().ToUpper() == ParityStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultParity;
            }
            catch (Exception)
            {
                return DefaultParity;
            }
        }

        /// <summary>
        /// Конвертация в объект StopBits
        /// </summary>
        /// <param name="StopBitsStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultStopBits">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static StopBits Convert(string StopBitsStr, StopBits DefaultStopBits)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(StopBitsStr))
                {
                    foreach (StopBits item in StopBits.GetValues(typeof(StopBits)))
                    {
                        if (item.ToString().ToUpper() == StopBitsStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultStopBits;
            }
            catch (Exception)
            {
                return DefaultStopBits;
            }
        }

        /// <summary>
        /// Конвертация в объект SecurityProtocolType
        /// </summary>
        /// <param name="SecurityProtocolTypeStr">Строка которую надо конвертнуть</param>
        /// <param name="DefaultSecurityProtocolType">Если не можем конвертнуть что в этом случае вернуть</param>
        /// <returns></returns>
        public static SecurityProtocolType Convert(string SecurityProtocolTypeStr, SecurityProtocolType DefaultSecurityProtocolType)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SecurityProtocolTypeStr))
                {
                    foreach (SecurityProtocolType item in SecurityProtocolType.GetValues(typeof(SecurityProtocolType)))
                    {
                        if (item.ToString().ToUpper() == SecurityProtocolTypeStr.Trim().ToUpper()) return item;
                    }
                }
                return DefaultSecurityProtocolType;
            }
            catch (Exception)
            {
                return DefaultSecurityProtocolType;
            }
        }

    }
}
