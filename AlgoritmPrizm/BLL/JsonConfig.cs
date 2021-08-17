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
    public class JsonConfig
    {
        private static JsonSerializerSettings _settings;


        public bool Trace = Config.Trace;
        public string Host = Config.Host;
        public int Port = Config.Port;
        public int TenderTypeCash = Config.TenderTypeCash;
        public int TenderTypeCredit = Config.TenderTypeCredit;
        public int TenderTypeGiftCert = Config.TenderTypeGiftCert;
        public int TenderTypeGiftCard = Config.TenderTypeGiftCard;

        public List<Custumer> customers = new List<Custumer>();

        public string GiftCardCode = Config.GiftCardCode;
        public bool GiftCardEnable = Config.GiftCardEnable;
        public int GiftCardTax = Config.GiftCardTax;

        [JsonConverter(typeof(JsonConfigFfdConverter))]
        public FfdEn Ffd = Config.Ffd;

        public int FrPort = Config.FrPort;

        [JsonConverter(typeof(JsonConfigFieldItemConverter))]
        public FieldItemEn FieldItem = Config.FieldItem;

        [JsonConverter(typeof(JsonConfigFieldDocNumConverter))]
        public FieldDocNumEn FieldDocNum = Config.FieldDocNum;

        public string HostPrizmApi = Config.HostPrizmApi;
        public string PrizmApiSystemLogon = Config.PrizmApiSystemLogon;
        public string PrizmApiSystemPassord = Config.PrizmApiSystemPassord;
        public int PrizmApiTimeLiveTockenMinute = Config.PrizmApiTimeLiveTockenMinute;



        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
    ""Trace"":false,
    ""Host"":""CHUDAKOV"",
    ""Port"":5000,
    ""TenderTypeCash"":0,
    ""TenderTypeCredit"":2,
    ""TenderTypeGiftCert"":9,
    ""TenderTypeGiftCard"":10,
    ""customers"":[
        {
            ""login"":""sysadmin"",
            ""fio_fo_check"":""Куприянчук Вадим"",
            ""inn"":""501210148104""
        }
    ],
    ""GiftCardCode"":""GFT"",
    ""GiftCardEnable"":false,
    ""GiftCardTax"":5,
    ""Ffd"":""v1_05"",
    ""FrPort"":1,
    ""FieldItem"":""Description2"",
    ""FieldDocNum"":""Comment2"",
    ""HostPrizmApi"":""http://172.16.1.102"",
    ""PrizmApiSystemLogon"":""sysadmin"",
    ""PrizmApiSystemPassord"":""sysadmin"",
    ""PrizmApiTimeLiveTockenMinute"":5}
}";
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="InitTek">Инициализировать особые параметры из текущих настроек или нет</param>
        public JsonConfig(bool InitTek)
        {
            if (InitTek)
            {
                this.customers = Config.customers;
            }
        }
        public JsonConfig() :this(false)
        {

        }


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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonConfig))
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
        public static JsonConfig DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BLL.JsonConfig>(json, BLL.JsonConfig.GetSettings());
        }

        /// <summary>
        /// Сериализация в джейсон
        /// </summary>
        /// <param name="json">Объект представляющий из себя наш конфиг</param>
        /// <returns>JSON</returns>
        public static string SerializeObject(JsonConfig json)
        {
            try
            {
                return JsonConvert.SerializeObject(json);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при обновлении конфигурационного в файла с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".UpdateVersionXml(XmlElement root, int oldVersion)", EventEn.Error);
                throw ae;
            }
        }
    }
}

