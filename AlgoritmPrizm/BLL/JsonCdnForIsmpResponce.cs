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
    public class JsonCdnForIsmpResponce
    {
        //
        private static JsonSerializerSettings _settings;

        public int code;
        public string description;
        public List<JsonCdnForIsmpResponceItem> codes = new List<JsonCdnForIsmpResponceItem>();
        public string reqId;
        public Int64 reqTimestamp;

        [JsonIgnore]
        public DateTime DateTimeValide = DateTime.Now;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
    ""code"":0,
    ""description"":""ok"",
    ""codes"":[
        {
            ""cis"":""ghfghjdhjk"",
            ""valid"":false,
            ""verified"":false,
            ""message"":""cannot parse code.StringIndexOutOfBoundsException: Range[0, 14) out of bounds for length 10"",
            ""found"":false,
            ""realizable"":false,
            ""utilised"":false,
            ""isBlocked"":false,
            ""errorCode"":1
        }
    ],
    ""reqId"":""4070a182 - bd10 - 4ae2 - bdc7 - dbb446c495fa"",
    ""reqTimestamp"":1728761617481
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonCdnForIsmpResponce))
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
        public static JsonCdnForIsmpResponce DeserializeJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<BLL.JsonCdnForIsmpResponce>(json, BLL.JsonCdnForIsmpResponce.GetSettings());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при десериализации объекта JsonCdnForIsmpResponce с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".DeserializeJson", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сериализация в джейсон
        /// </summary>
        /// <param name="json">Объект представляющий из себя наш конфиг</param>
        /// <returns>JSON</returns>
        public static string SerializeObject(JsonCdnForIsmpResponce json)
        {
            try
            {
                return JsonCdnForIsmpResponce.SerializeObject(json);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сериализации объекта JsonCdnForIsmpResponce с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SerializeObject", EventEn.Error);
                throw ae;
            }
        }
    }
}
