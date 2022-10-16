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
    /// Список imei устроств с валидной датой
    /// </summary>
    public class JsonImei
    {
        private static JsonSerializerSettings _settings;

        public List<string> imei = new List<string>();

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"
{
    ""imei"":[""wwwwde"",""wwwwdewww""]
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonImei))
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
        /// Сериализация в джейсон
        /// </summary>
        /// <param name="json">Объект представляющий из себя наш конфиг</param>
        /// <returns>JSON</returns>
        public static string SerializeObject(JsonImei json)
        {
            try
            {
                return JsonConvert.SerializeObject(json);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сериализации объекта JsonImei с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".SerializeObject", EventEn.Error);
                throw ae;
            }
        }

        // Десерилизовать из json
        public static JsonImei DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BLL.JsonImei>(json, BLL.JsonImei.GetSettings());
        }
    }
}
