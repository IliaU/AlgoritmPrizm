﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AlgoritmPrizmCom.Lib;
using AlgoritmPrizmCom.Com;

namespace AlgoritmPrizmCom
{
    public class CdnResponce
    {
        private static JsonSerializerSettings _settings;

        public bool Rezult = false;

        public string Message;

        public string reqId;

        public long reqTimestamp;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
    ""Rezult"":""false"",
    ""Message"":""""
}";

        /// <summary>
        /// Параметры которые надо использовать при десериализации
        /// </summary>
        private static JsonSerializerSettings GetSettings()
        {
            if (_settings == null)
            {
                _settings = new JsonSerializerSettings();
                // Обработка ошибок
                _settings.Error = (sender, args) =>
                {
                    if (object.Equals(args.ErrorContext.Member, "note1") &&
                      args.ErrorContext.OriginalObject.GetType() == typeof(CdnResponce))
                    {
                        args.ErrorContext.Handled = true;
                    }
                };
            }
            return _settings;

            /*
             JsonPrintFiscDoc test = JsonConvert.DeserializeObject<JsonPrintFiscDoc>(json, settings);
            Этот код не будет генерировать исключение.
            Будет вызван обработчик Error в объекте настроек, и если член, 
            генерирующий исключение, будет назван "dirty" и принадлежит JsonPrintFiscDoc, 
            ошибка пропускается и JSON.NET продолжает работать.

            Свойство ErrorContext также имеет другие свойства, 
            которые вы можете использовать для обработки только тех ошибок, 
            в которых вы абсолютно уверены, что хотите игнорировать.
      
            */
        }


        /// <summary>
        /// Десерилизовать из json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static CdnResponce DeserializeJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<CdnResponce>(json, CdnResponce.GetSettings());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при десериализации объекта CdnResponce с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".DeserializeJson", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сериализация в джейсон
        /// </summary>
        /// <param name="json">Объект представляющий из себя наш конфиг</param>
        /// <returns>JSON</returns>
        public static string SerializeObject(CdnResponce json)
        {
            try
            {
                return JsonConvert.SerializeObject(json);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сериализации объекта CdnResponce с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SerializeObject", EventEn.Error);
                throw ae;
            }
        }


    }
}
