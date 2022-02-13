using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    public class JsonPrintFiscDocTender
    {
        private static JsonSerializerSettings _settings;

        public string link;
        public string sid;
        public string created_by;
        public DateTime created_datetime;
        public string modified_by;
        public DateTime? modified_datetime;
        public DateTime post_date;
        public int tender_type;
        public int tender_pos;
        public double amount;
        public double taken;
        public double given;
        public bool matched;
        public string eft_res_tender_state;
        public string manual_name;
        public string manual_remark;
        public string currency_sid;
        public string currency_name;
        public string tender_name;
        public string alphabetic_code;
        public string tender_sid;
        public string check_type;
        public string check_number;
        public string company;
        public string first_name;
        public string last_name;
        public string work_phone;
        public string home_phone;
        public string state;
        public string drivers_license;
        public string drivers_license_expiration;
        public DateTime? date_of_birth;
        public string authorization_code;
        public string eft_transaction_id;
        public string avs_response_code;
        public string failure_message;
        public string eftdata1;
        public string eftdata2;
        public string eftdata3;
        public string eftdata4;
        public string eftdata5;
        public string eftdata6;
        public string eftdata7;
        public string eftdata8;
        public string eftdata9;
        public string eftdata0;
        public string entry_method;
        public string eftdata10;
        public string eftdata11;
        public string eftdata12;
        public string eftdata13;
        public string eftdata14;
        public string eftdata15;
        public string eftdata16;
        public string eftdata17;
        public string eftdata18;
        public string eftdata19;
        public string eftdatabsmer;
        public string eftdatabscust;
        public string card_number;
        public string card_type_sid;
        public string card_holder_name;
        public string card_expiration_month;
        public string card_expiration_year;
        public string is_normal_sale;
        public string is_present;
        public string card_postal_code;
        public string l2_result_code;
        public string card_type_name;
        public string emv_ai_applabel;
        public string emv_ai_aid;
        public string emv_ci_cardexpirydate;
        public string emv_crypto_cryptogramtype;
        public string emv_crypto_cryptogram;
        public string emv_pinstatement;
        public string charge_net_days;
        public string charge_discount_days;
        public string charge_discount_percent;
        public string base_taken;
        public string base_given;
        public string give_rate;
        public string take_rate;
        public string foreign_currency_sid;
        public string foreign_currency_name;
        public string foreign_alphabetic_code;
        public string trace_number;
        public string internal_reference_number;
        public string balance;
        public string certificate_number;
        public DateTime? payment_date;
        public string payment_remark;
        public string central_payment_id;
        public string central_credit_balance;
        public string central_commit_state;
        public string redeem_credit_id_10;
        public string new_credit_id_10;
        public string new_credit_value;
        public string new_credit_id;
        public string redeem_credit_id;
        public string central_activation_id;
        public string central_card_number;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"[
    {
        ""link"":""\/v1\/rest\/document\/626460034000353723\/tender\/626460163000374730"",
        ""sid"":""626460163000374730"",
        ""created_by"":""SYSADMIN"",
        ""created_datetime"":""2022-02-13T16:42:44.000+03:00"",
        ""modified_by"":null,
        ""modified_datetime"":null,
        ""controller_sid"":""603242764000070002"",
        ""origin_application"":""RProPrismWeb"",
        ""post_date"":""2022-02-13T16:42:44.000+03:00"",
        ""row_version"":1,
        ""tenant_sid"":""603242764000172003"",
        ""document_sid"":""626460034000353723"",
        ""tender_type"":0,
        ""tender_pos"":1,
        ""amount"":114000,
        ""taken"":114000,
        ""given"":0,
        ""matched"":false,
        ""eft_res_tender_state"":null,
        ""manual_name"":null,
        ""manual_remark"":null,
        ""currency_sid"":""603242786000192548"",
        ""currency_name"":""RUB"",
        ""tender_name"":""\u041D\u0430\u043B\u0438\u0447\u043D\u044B\u0435"",
        ""alphabetic_code"":""RUB"",
        ""prevent_void"":null,
        ""tender_sid"":null,
        ""check_type"":null,
        ""check_number"":null,
        ""company"":null,
        ""first_name"":null,
        ""last_name"":null,
        ""work_phone"":null,
        ""home_phone"":null,
        ""state"":null,
        ""drivers_license"":null,
        ""drivers_license_expiration"":null,
        ""date_of_birth"":null,
        ""authorization_code"":null,
        ""eft_transaction_id"":null,
        ""avs_response_code"":null,
        ""failure_message"":null,
        ""eftdata1"":null,
        ""eftdata2"":null,
        ""eftdata3"":null,
        ""eftdata4"":null,
        ""eftdata5"":null,
        ""eftdata6"":null,
        ""eftdata7"":null,
        ""eftdata8"":null,
        ""eftdata9"":null,
        ""eftdata0"":null,
        ""entry_method"":null,
        ""eftdata10"":null,
        ""eftdata11"":null,
        ""eftdata12"":null,
        ""eftdata13"":null,
        ""eftdata14"":null,
        ""eftdata15"":null,
        ""eftdata16"":null,
        ""eftdata17"":null,
        ""eftdata18"":null,
        ""eftdata19"":null,
        ""eftdatabsmer"":null,
        ""eftdatabscust"":null,
        ""card_number"":null,
        ""card_type_sid"":null,
        ""card_holder_name"":null,
        ""card_expiration_month"":null,
        ""card_expiration_year"":null,
        ""is_normal_sale"":null,
        ""is_present"":null,
        ""card_postal_code"":null,
        ""l2_result_code"":null,
        ""card_type_name"":null,
        ""emv_ai_applabel"":null,
        ""emv_ai_aid"":null,
        ""emv_ci_cardexpirydate"":null,
        ""emv_crypto_cryptogramtype"":null,
        ""emv_crypto_cryptogram"":null,
        ""emv_pinstatement"":null,
        ""charge_net_days"":null,
        ""charge_discount_days"":null,
        ""charge_discount_percent"":null,
        ""base_taken"":null,
        ""base_given"":null,
        ""give_rate"":null,
        ""take_rate"":null,
        ""foreign_currency_sid"":null,
        ""foreign_currency_name"":null,
        ""foreign_alphabetic_code"":null,
        ""trace_number"":null,
        ""internal_reference_number"":null,
        ""balance"":null,
        ""certificate_number"":null,
        ""payment_date"":null,
        ""payment_remark"":null,
        ""store_credit_balance"":null,
        ""central_payment_id"":null,
        ""central_credit_balance"":null,
        ""central_commit_state"":null,
        ""redeem_credit_id_10"":null,
        ""new_credit_id_10"":null,
        ""new_credit_value"":null,
        ""new_credit_id"":null,
        ""redeem_credit_id"":null,
        ""central_activation_id"":null,
        ""central_card_number"":null,
        ""central_card_expiredate"":null,
        ""hrefs"":
        {
                ""controller_sid"":""\/v1\/rest\/controller\/603242764000070002"",
                ""document_sid"":""\/v1\/rest\/document\/626460034000353723""
        }
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonPrintFiscDocTender))
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
        public static List<JsonPrintFiscDocTender> DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<List<BLL.JsonPrintFiscDocTender>>(json, BLL.JsonPrintFiscDocTender.GetSettings());
        }
    }
}
