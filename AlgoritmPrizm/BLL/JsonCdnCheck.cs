﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.BLL
{
    public class JsonCdnCheck
    {

        //
        private static JsonSerializerSettings _settings;

        public int code;
        public string description;
        public int avgTimeMs;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
""code"":0,
""description"":""ok"",
""avgTimeMs"":1
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonCdnCheck))
                    {
                        args.ErrorContext.Handled = true;
                    }
                };
            }
            return _settings;

            /*
             JsonPrintFiscDoc test = JsonConvert.DeserializeObject<JsonCdnForIsmp>(json, settings);
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
        public static JsonCdnCheck DeserializeJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<BLL.JsonCdnCheck>(json, BLL.JsonCdnCheck.GetSettings());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при десериализации объекта JsonCdnCheck с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".DeserializeJson", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сериализация в джейсон
        /// </summary>
        /// <param name="json">Объект представляющий из себя наш конфиг</param>
        /// <returns>JSON</returns>
        public static string SerializeObject(JsonCdnCheck json)
        {
            try
            {
                return JsonCdnCheck.SerializeObject(json);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сериализации объекта JsonCdnCheck с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SerializeObject", EventEn.Error);
                throw ae;
            }
        }
    }
}