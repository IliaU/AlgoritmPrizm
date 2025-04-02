using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Ответ от базы Енисей
    /// </summary>
    public class JsonEniseyResponce
    {
        private static JsonSerializerSettings _settings;

        public List<JsonEniseyResponceResult> results = new List<JsonEniseyResponceResult>();

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
  ""results"":[
    {
      ""version"":""7312a9ab-e26d-476d-848f-4d18030eb5d3"",
      ""reqTimestamp"":1743626629705,
      ""reqId"":""ab88a664-88d5-8e92-5159-6cf2b90f4ec6"",
      ""inst"":""9ddcca85-eeba-4c30-98ec-5c7c0db67094"",
      ""description"":""ok"",
      ""codes"":[
        {
          ""printView"":""01048657365749062155esJWe"",
          ""isGreyGtin"":false,
          ""isBlocked"":false,
          ""gtin"":""04865736574906"",
          ""cis"":""01048657365749062155esJWe""
        }
      ],
      ""code"":0
    }
  ]
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonEniseyResponce))
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
        public static JsonEniseyResponce DeserializeJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<BLL.JsonEniseyResponce>(json, BLL.JsonEniseyResponce.GetSettings());
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
        public static string SerializeObject(JsonEniseyResponce json)
        {
            try
            {
                return JsonConvert.SerializeObject(json);
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
