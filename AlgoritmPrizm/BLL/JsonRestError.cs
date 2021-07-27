using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    public class JsonRestError
    {
        private static JsonSerializerSettings _settings;

        public DateTime? date;
        public int? errorcode;
        public string errormsg;
        public int? httpcode;
        [JsonProperty("class")]
        public string v_class;
        public string functionname;
        public string area;
        public bool? replication;
        public string exterrormsg;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"[
    {
	    ""date"":""2021-07-28T02:40:08.235+03:00"",
	    ""errorcode"":0,
	    ""errormsg"":""500 - Server error - Exception while calling DoDispatch : EAccessViolation : Access violation at address 01975164 in module 'PrismPOSV1.exe'. Read of address 00000000"",
	    ""httpcode"":500,
	    ""class"":""ERPSServicesException"",
	    ""functionname"":null,
	    ""area"":""Dispatcher"",
	    ""replication"":false,
	    ""exterrormsg"":""An unexpected error occurred""
    }
]";

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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonRestError))
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


        // Десерилизовать из json
        public static List<JsonRestError> DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<List<BLL.JsonRestError>>(json, BLL.JsonRestError.GetSettings());
        }
    }
}
