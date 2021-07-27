using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Для парсинга списка продуктов
    /// </summary>
    public class JsonGetInventory
    {
        // http://ruggmoofffod1.emea.guccigroup.dom/v1/rest/inventory?cols=*
        private static JsonSerializerSettings _settings;

        public string link;
        public string sid;
        public string created_by;
        public DateTime created_datetime;
        public string modified_by;
        public DateTime? modified_datetime;
        public string controller_sid;
        public string origin_application;
        public DateTime? post_date;
        public int row_version;
        public string tenant_sid;
        public string inventory_item_uid;
        public string subsidiary_sid;
        public string alu;
        public string style_sid;
        public string dcs_sid;
        public string vendor_sid;
        public int? scale_number;
        public string description1;
        public string description2;
        public string description3;
        public string attribute;
        public string item_size;
        public double? cost;
        public int? spif;
        public double? foreign_currency_cost;
        public string currency_sid;
        public DateTime? first_received_date;
        public DateTime? last_received_date;
        public DateTime? last_sold_date;
        public DateTime? markdown_date;
        public DateTime? discontinued_date;
        public string tax_code_sid;
        public string commission_sid;
        public string discount_schedule_sid;
        public string udf1_string;
        public string udf2_string;
        public string udf3_string;
        public string udf4_string;
        public string udf5_string;
        public bool? udf1_float;
        public bool? udf2_float;
        public bool? udf3_float;
        public DateTime? udf1_date;
        public DateTime? udf2_date;
        public DateTime? udf3_date;
        public string upc;
        public double? use_quantity_decimals;
        public string subsidiary_name;
        public string style_code;
        public string vendor_name;
        public string vendor_account_number;
        public string description4;
        public DateTime? orderable_date;
        public DateTime? sellable_date;
        public int? subsidiary_number;
        public bool? regional;
        public bool? active;
        public double? quantity_per_case;
        public List<JsonGetInventorySbsinventoryprices> sbsinventoryprices = new List<JsonGetInventorySbsinventoryprices>();
        public List<JsonGetInventorySbsinventoryqtys> sbsinventoryqtys = new List<JsonGetInventorySbsinventoryqtys>();
        public string tax_code;
        public string currency_code;
        public string dcs_code;
        public string vendor_code;
        public double? max_discount_percent;
        public double? max_accum_discount_percent;
        public int? item_number;
        public int serial_type;
        public int lot_type;
        public int kit_type;
        public string scale_sid;
        public int? promo_qtydiscweight;
        public bool? promo_invenexclude;
        public List<JsonGetInventoryExtended> extended = new List<JsonGetInventoryExtended>();
        public string department_name;
        public string class_name;
        public string subclass_name;
        public bool? non_inventory;
        public bool? non_commited;
        public double? last_rcvd_cost;
        public List<JsonGetInventorySbsinventorystoreqtys> sbsinventorystoreqtys = new List<JsonGetInventorySbsinventorystoreqtys>();
        public List<JsonGetInventorySbsinventoryactiveprices> sbsinventoryactiveprices = new List<JsonGetInventorySbsinventoryactiveprices>();
        public double? active_price;
        public double? store_qty;
        public string tax_name;
        public double? active_coefficient;
        public double? active_margin_amt;
        public double? active_margin_percent;
        public double? active_margin_with_tax_amt;
        public double? active_markup_percent;
        public bool? orderable;
        public double? order_cost;
        public string lty_price_in_points;
        public string lty_points_earned;
        public string text1;
        public string text2;
        public string text3;
        public string text4;
        public string text5;
        public string text6;
        public string text7;
        public string text8;
        public string text9;
        public string text10;
        public string actstrpricewt;
        public string image_path;
        public double? vendor_list_cost;
        public double? min_ord_qty;
        public bool? special_order;
        public JsonGetInventoryHrefs hrefs;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"[
    {
        ""link"": ""/v1/rest/inventory/601941244000146465"",
        ""sid"": ""601941244000146465"",
        ""created_by"": ""Replication"",
        ""created_datetime"": ""2021-04-16T18:18:25.000+03:00"",
        ""modified_by"": null,
        ""modified_datetime"": ""2021-04-16T18:18:25.000+03:00"",
        ""controller_sid"": ""599457373000052255"",
        ""origin_application"": ""PrismMQ"",
        ""post_date"": ""2021-07-02T13:23:06.000+03:00"",
        ""row_version"": 2,
        ""tenant_sid"": ""603242764000172003"",
        ""inventory_item_uid"": ""258630555642"",
        ""subsidiary_sid"": ""603242764000175004"",
        ""alu"": ""8082204858"",
        ""style_sid"": ""6677905730558680340"",
        ""dcs_sid"": ""601846185000182304"",
        ""vendor_sid"": ""601846101000127587"",
        ""scale_number"": null,
        ""description1"": ""519801CAOTT"",
        ""description2"": ""WOMENS SMLG"",
        ""description3"": null,
        ""attribute"": ""3020"",
        ""item_size"": ""NOSIZ"",
        ""cost"": null,
        ""spif"": 0,
        ""foreign_currency_cost"": 0,
        ""currency_sid"": null,
        ""first_received_date"": null,
        ""last_received_date"": null,
        ""last_sold_date"": null,
        ""markdown_date"": null,
        ""discontinued_date"": null,
        ""tax_code_sid"": ""603242804000115339"",
        ""commission_sid"": null,
        ""discount_schedule_sid"": null,
        ""udf1_string"": null,
        ""udf2_string"": null,
        ""udf3_string"": null,
        ""udf4_string"": null,
        ""udf5_string"": null,
        ""udf1_float"": null,
        ""udf2_float"": null,
        ""udf3_float"": null,
        ""udf1_date"": null,
        ""udf2_date"": null,
        ""udf3_date"": null,
        ""upc"": ""8082204858"",
        ""use_quantity_decimals"": 0,
        ""subsidiary_name"": ""001"",
        ""style_code"": null,
        ""vendor_name"": null,
        ""vendor_account_number"": null,
        ""description4"": null,
        ""orderable_date"": null,
        ""sellable_date"": null,
        ""subsidiary_number"": 1,
        ""regional"": false,
        ""active"": true,
        ""quantity_per_case"": 0,
        ""sbsinventoryprices"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryprice/601941248124557001"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryprice/601941248124557000"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryprice/601941248124557003"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryprice/601941248124557002"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            }
        ],
        ""sbsinventoryqtys"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554000""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554001""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554002""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554003""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554004""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554005""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554006""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554007""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554008""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554009""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554010""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554011""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554012""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554013""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554014""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554015""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554016""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryqty/601941248124554017""
            }
        ],
        ""sbsinventoryvendors"": [],
        ""sbsinventorykits"": [],
        ""sbsinventorymedias"": [],
        ""tax_code"": ""0"",
        ""currency_code"": null,
        ""dcs_code"": ""120050420"",
        ""vendor_code"": ""GUCCI"",
        ""max_discount_percent"": 0,
        ""max_accum_discount_percent"": 0,
        ""item_number"": 706206,
        ""serial_type"": 0,
        ""lot_type"": 0,
        ""kit_type"": 0,
        ""scale_sid"": null,
        ""promo_qtydiscweight"": 0,
        ""promo_invenexclude"": false,
        ""extended"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/extended/601941248124560000""
            }
        ],
        ""department_name"": null,
        ""class_name"": null,
        ""subclass_name"": null,
        ""non_inventory"": false,
        ""non_commited"": false,
        ""last_rcvd_cost"": 0,
        ""sbsinventorystoreqtys"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventorystoreqty/601941248124554001""
            }
        ],
        ""sbsinventoryactiveprices"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000146465/sbsinventoryactiveprice/601941248124557003"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            }
        ],
        ""active_price"": null,
        ""store_qty"": null,
        ""tax_name"": ""TAXABLE"",
        ""active_coefficient"": null,
        ""active_margin_amt"": null,
        ""active_margin_percent"": null,
        ""active_margin_with_tax_amt"": null,
        ""active_markup_percent"": null,
        ""orderable"": true,
        ""order_cost"": 0,
        ""lty_price_in_points"": null,
        ""lty_points_earned"": null,
        ""sbsinventoryltys"": [],
        ""text1"": null,
        ""text2"": null,
        ""text3"": null,
        ""text4"": null,
        ""text5"": null,
        ""text6"": null,
        ""text7"": null,
        ""text8"": null,
        ""text9"": null,
        ""text10"": null,
        ""actstrpricewt"": null,
        ""image_path"": ""inventory\\0000\\0000\\2586\\3055\\"",
        ""vendor_list_cost"": 0,
        ""min_ord_qty"": 0,
        ""special_order"": false,
        ""hrefs"": {
            ""subsidiary_sid"": ""/v1/rest/subsidiary/603242764000175004"",
            ""dcs_sid"": ""/v1/rest/dcs/601846185000182304"",
            ""controller_sid"": ""/v1/rest/controller/599457373000052255"",
            ""vendor_sid"": ""/v1/rest/vendor/601846101000127587"",
            ""tax_code_sid"": ""/v1/rest/taxcode/603242804000115339""
        }
    },
    {
        ""link"": ""/v1/rest/inventory/601941244000163472"",
        ""sid"": ""601941244000163472"",
        ""created_by"": ""Replication"",
        ""created_datetime"": ""2021-04-16T18:18:25.000+03:00"",
        ""modified_by"": null,
        ""modified_datetime"": ""2021-04-16T18:18:25.000+03:00"",
        ""controller_sid"": ""599457373000052255"",
        ""origin_application"": ""PrismMQ"",
        ""post_date"": ""2021-07-02T13:23:06.000+03:00"",
        ""row_version"": 2,
        ""tenant_sid"": ""603242764000172003"",
        ""inventory_item_uid"": ""258630555128"",
        ""subsidiary_sid"": ""603242764000175004"",
        ""alu"": ""8082204840"",
        ""style_sid"": ""6677905730558680340"",
        ""dcs_sid"": ""601846185000182304"",
        ""vendor_sid"": ""601846101000127587"",
        ""scale_number"": null,
        ""description1"": ""519801CAOTT"",
        ""description2"": ""WOMENS SMLG"",
        ""description3"": null,
        ""attribute"": ""1000"",
        ""item_size"": ""NOSIZ"",
        ""cost"": null,
        ""spif"": 0,
        ""foreign_currency_cost"": 0,
        ""currency_sid"": null,
        ""first_received_date"": null,
        ""last_received_date"": null,
        ""last_sold_date"": null,
        ""markdown_date"": null,
        ""discontinued_date"": null,
        ""tax_code_sid"": ""603242804000115339"",
        ""commission_sid"": null,
        ""discount_schedule_sid"": null,
        ""udf1_string"": null,
        ""udf2_string"": null,
        ""udf3_string"": null,
        ""udf4_string"": null,
        ""udf5_string"": null,
        ""udf1_float"": null,
        ""udf2_float"": null,
        ""udf3_float"": null,
        ""udf1_date"": null,
        ""udf2_date"": null,
        ""udf3_date"": null,
        ""upc"": ""8082204840"",
        ""use_quantity_decimals"": 0,
        ""subsidiary_name"": ""001"",
        ""style_code"": null,
        ""vendor_name"": null,
        ""vendor_account_number"": null,
        ""description4"": null,
        ""orderable_date"": null,
        ""sellable_date"": null,
        ""subsidiary_number"": 1,
        ""regional"": false,
        ""active"": true,
        ""quantity_per_case"": 0,
        ""sbsinventoryprices"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryprice/601941248124577001"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryprice/601941248124577000"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryprice/601941248124577003"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryprice/601941248124577002"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            }
        ],
        ""sbsinventoryqtys"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565000""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565001""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565002""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565003""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565004""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565005""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565006""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565007""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565008""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565009""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565010""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565011""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565012""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565013""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565014""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565015""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565016""
            },
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryqty/601941248124565017""
            }
        ],
        ""sbsinventoryvendors"": [],
        ""sbsinventorykits"": [],
        ""sbsinventorymedias"": [],
        ""tax_code"": ""0"",
        ""currency_code"": null,
        ""dcs_code"": ""120050420"",
        ""vendor_code"": ""GUCCI"",
        ""max_discount_percent"": 0,
        ""max_accum_discount_percent"": 0,
        ""item_number"": 706207,
        ""serial_type"": 0,
        ""lot_type"": 0,
        ""kit_type"": 0,
        ""scale_sid"": null,
        ""promo_qtydiscweight"": 0,
        ""promo_invenexclude"": false,
        ""extended"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/extended/601941248124583000""
            }
        ],
        ""department_name"": null,
        ""class_name"": null,
        ""subclass_name"": null,
        ""non_inventory"": false,
        ""non_commited"": false,
        ""last_rcvd_cost"": 0,
        ""sbsinventorystoreqtys"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventorystoreqty/601941248124565001""
            }
        ],
        ""sbsinventoryactiveprices"": [
            {
                ""link"": ""/v1/rest/inventory/601941244000163472/sbsinventoryactiveprice/601941248124577003"",
                ""margin_amt"": 0,
                ""margin_with_tax_amt"": 0,
                ""margin_percent"": 0,
                ""markup_percent"": 0,
                ""coefficient"": 0
            }
        ],
        ""active_price"": null,
        ""store_qty"": null,
        ""tax_name"": ""TAXABLE"",
        ""active_coefficient"": 0,
        ""active_margin_amt"": 0,
        ""active_margin_percent"": 0,
        ""active_margin_with_tax_amt"": 0,
        ""active_markup_percent"": 0,
        ""orderable"": true,
        ""order_cost"": 0,
        ""lty_price_in_points"": null,
        ""lty_points_earned"": null,
        ""sbsinventoryltys"": [],
        ""text1"": null,
        ""text2"": null,
        ""text3"": null,
        ""text4"": null,
        ""text5"": null,
        ""text6"": null,
        ""text7"": null,
        ""text8"": null,
        ""text9"": null,
        ""text10"": null,
        ""actstrpricewt"": null,
        ""image_path"": ""inventory\\0000\\0000\\2586\\3055\\"",
        ""vendor_list_cost"": 0,
        ""min_ord_qty"": 0,
        ""special_order"": false,
        ""hrefs"": {
            ""subsidiary_sid"": ""/v1/rest/subsidiary/603242764000175004"",
            ""dcs_sid"": ""/v1/rest/dcs/601846185000182304"",
            ""controller_sid"": ""/v1/rest/controller/599457373000052255"",
            ""vendor_sid"": ""/v1/rest/vendor/601846101000127587"",
            ""tax_code_sid"": ""/v1/rest/taxcode/603242804000115339""
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonGetInventory))
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
        public static List<JsonGetInventory> DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<List<BLL.JsonGetInventory>>(json, BLL.JsonGetInventory.GetSettings());
        }
    }
}
