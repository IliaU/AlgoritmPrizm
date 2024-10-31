using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.BLL;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com
{
    public static class JsonCdnFarm
    {
        /// <summary>
        /// Для блокировки чтобы не шли запросы парралельно а только последовательно
        /// </summary>
        private static object obj =new object();

        private static List<JsonCdnForIsmpResponce> ListIsmpResponce = new List<JsonCdnForIsmpResponce>();

        /// <summary>
        /// Чистка мусора из оперативы в части протухших ответов площадки ЦДН
        /// </summary>
        public static void ClearOld()
        {
            try
            {
                lock(obj)
                {
                    for (int i = ListIsmpResponce.Count-1; i >=0 ; i--)
                    {
                        if(ListIsmpResponce[i].DateTimeValide < DateTime.Now.AddMinutes(Config.ClearJsonCdnFarmMin*-1))
                        {
                            ListIsmpResponce.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".ClearOld", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Добавление ответа от площадки СДН в буффер
        /// </summary>
        /// <param name="item">Ответ который надо добавить в буфер</param>
        public static void BufferAdd(JsonCdnForIsmpResponce item)
        {
            try
            {
                lock (obj)
                {
                    ListIsmpResponce.Add(item);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".BufferAdd", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Удаление из буфера обработанного ответа от площадки СДН
        /// </summary>
        /// <param name="MatrixCode">Матрикс код который надо найти</param>
        /// <returns></returns>
        public static JsonCdnForIsmpResponce BufferRemove(string MatrixCode)
        {
            try
            {
                JsonCdnForIsmpResponce rez = null;

                lock (obj)
                {
                    for (int i = ListIsmpResponce.Count - 1; i >= 0; i--)
                    {
                        for (int ii = ListIsmpResponce[i].codes.Count - 1; ii >= 0; ii--)
                        {
                            if (ListIsmpResponce[i].codes[ii].requestMatrixCode == MatrixCode)
                            {
                                rez = new JsonCdnForIsmpResponce();
                                rez.code = ListIsmpResponce[i].code;
                                rez.codes.Add(ListIsmpResponce[i].codes[ii]);
                                rez.DateTimeValide = ListIsmpResponce[i].DateTimeValide;
                                rez.description = ListIsmpResponce[i].description;
                                rez.reqId = ListIsmpResponce[i].reqId;
                                rez.reqTimestamp = ListIsmpResponce[i].reqTimestamp;

                                ListIsmpResponce[i].codes.RemoveAt(ii);
                            }
                        }

                        if (ListIsmpResponce[i].codes.Count == 0) ListIsmpResponce.RemoveAt(i);
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".BufferRemove", EventEn.Error);
                throw ae;
            }
        }
    }
}
