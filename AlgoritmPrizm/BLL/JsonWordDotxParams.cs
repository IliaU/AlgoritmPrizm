using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{

    /// <summary>
    /// Класс через которы будем передавать параметры
    /// </summary>
    public class JsonWordDotxParams
    {
        // http://ruggmoofffod1.emea.guccigroup.dom/v1/rest/inventory?cols=*
        private static JsonSerializerSettings _settings;

        public string string1;
        public string string2;
        public int? int1;
        public int? int2;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"[
    {
        ""string1"": null,
        ""string2"": null,
        ""int1"":null,
        ""int2"":null
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonWordDotxParams))
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
        /// <param name="JnWdDotxParn">Объект JnWdDotxPar</param>
        /// <returns>Строковоке представление объекта</returns>
        public static string SerializeJson(List<JsonWordDotxParams> JnWdDotxPar)
        {
            return JsonConvert.SerializeObject(JnWdDotxPar);
        }

        /// <summary>
        /// Десерилизовать из json
        /// </summary>
        /// <param name="json">Строковоке представление объекта</param>
        /// <returns>Объект JnWdDotxPar</returns>
        public static List<JsonWordDotxParams> DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<List<BLL.JsonWordDotxParams>>(json, BLL.JsonWordDotxParams.GetSettings());
        }
    }
}
