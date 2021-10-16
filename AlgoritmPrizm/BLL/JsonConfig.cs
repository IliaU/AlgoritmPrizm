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


        public bool Trace;
        public string Host;
        public int Port;
        public int TenderTypeCash;
        public int TenderTypeCredit;
        public int TenderTypeGiftCert;
        public int TenderTypeGiftCard;

        public List<Custumer> customers = new List<Custumer>();

        public string GiftCardCode;
        public bool GiftCardEnable;
        public int GiftCardTax;

        [JsonConverter(typeof(JsonConfigFfdConverter))]
        public FfdEn Ffd = Config.Ffd;

        public int FrPort;

        [JsonConverter(typeof(JsonConfigFieldItemConverter))]
        public FieldItemEn FieldItem = Config.FieldItem;

        [JsonConverter(typeof(JsonConfigFieldDocNumConverter))]
        public FieldDocNumEn FieldDocNum = Config.FieldDocNum;

        public string HostPrizmApi;
        public string PrizmApiSystemLogon;
        public int PrizmApiTimeLiveTockenMinute;

        public int LimitCachForUrik;

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
                this.Trace = Config.Trace;
                this.Host = Config.Host;
                this.Port = Config.Port;
                this.TenderTypeCash = Config.TenderTypeCash;
                this.TenderTypeCredit = Config.TenderTypeCredit;
                this.TenderTypeGiftCert = Config.TenderTypeGiftCert;
                this.TenderTypeGiftCard = Config.TenderTypeGiftCard;

                this.customers = Config.customers;

                this.GiftCardCode = Config.GiftCardCode;
                this.GiftCardEnable = Config.GiftCardEnable;
                this.GiftCardTax = Config.GiftCardTax;

                this.Ffd = Config.Ffd;

                this.FrPort = Config.FrPort;

                this.FieldItem = Config.FieldItem;

                this.FieldDocNum = Config.FieldDocNum;

                this.HostPrizmApi = Config.HostPrizmApi;
                this.PrizmApiSystemLogon = Config.PrizmApiSystemLogon;
                this.PrizmApiTimeLiveTockenMinute = Config.PrizmApiTimeLiveTockenMinute;

                this.LimitCachForUrik = (int)Config.LimitCachForUrik;
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
            try
            {
                return JsonConvert.DeserializeObject<BLL.JsonConfig>(json, BLL.JsonConfig.GetSettings());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при десериализации объекта JsonConfig с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".DeserializeJson", EventEn.Error);
                throw ae;
            }
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при сериализации объекта JsonConfig с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".SerializeObject", EventEn.Error);
                throw ae;
            }
        }
    }
}

