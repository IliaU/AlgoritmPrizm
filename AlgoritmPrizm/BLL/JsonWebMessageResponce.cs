using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Ответ пользователю в вормате json через веб морду
    /// </summary>
    public class JsonWebMessageResponce
    {
        // http://ruggmoofffod1.emea.guccigroup.dom/v1/rest/inventory?cols=*
        private static JsonSerializerSettings _settings;

        /// <summary>
        /// Сообщение для пользователя
        /// </summary>
        public string Message;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"
{
        ""Message"": null
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonWebMessageResponce))
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
        /// Сериализация списка
        /// </summary>
        /// <param name="JnWbMesResp">Объект JsonWebMessageResponce</param>
        /// <returns>Строковоке представление объекта</returns>
        public static string SerializeJson(JsonWebMessageResponce JnWbMesResp)
        {
            return JsonConvert.SerializeObject(JnWbMesResp);
        }

        /// <summary>
        /// Десерилизовать из json
        /// </summary>
        /// <param name="json">Строковоке представление объекта</param>
        /// <returns>Объект JsonWebMessageResponce</returns>
        public static JsonWebMessageResponce DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BLL.JsonWebMessageResponce>(json, BLL.JsonWebMessageResponce.GetSettings());
        }


    }
}
