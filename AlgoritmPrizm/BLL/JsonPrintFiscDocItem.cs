using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    public class JsonPrintFiscDocItem
    {
        private static JsonSerializerSettings _settings;

        public string resource;
        public string endpoint;
        //""dirty"":{},
        public string origin_application;
        public string link;
        public string sid;
        public string created_by;
        public DateTime created_datetime;
        public string modified_by;
        public DateTime? modified_datetime;
        public string controller_sid;
        public DateTime? post_date;
        public Int64 row_version;
        public string tenant_sid;
        public string document_sid;
        public int item_pos;
        public double quantity;
        public double original_price;
        public double? original_tax_amount;
        public double price;
        public double tax_percent;
        public double tax_amount;
        public double tax2_percent;
        public double tax2_amount;
        public bool detax_flag;
        public string price_before_detax;
        public string original_price_before_detax;
        public double cost;
        public double spif;
        public string schedule_number;
        public string scan_upc;
        public string serial_number;
        public int kit_flag;
        public string package_item_sid;
        public string original_component_item_sid;
        public string user_discount_percent;
        public string package_sequence_number;
        public string lot_number;
        public double? activity_percent;
        public double? activity2_percent;
        public double? activity3_percent;
        public double? activity4_percent;
        public double? activity5_percent;
        public string commission_amount;
        public string commission2_amount;
        public string commission3_amount;
        public string commission4_amount;
        public string commission5_amount;
        public int item_origin;
        public string package_number;
        public string ship_id;
        public string ship_method;
        public double? shipping_amt;
        public string tracking_number;
        public double original_cost;
        public bool promotion_flag;
        public string gift_activation_code;
        public string gift_transaction_id;
        public bool gift_add_value;
        public string customer_field;
        public string udf_string01;
        public string udf_string02;
        public string udf_string03;
        public string udf_string04;
        public string udf_string05;
        public DateTime? udf_date01;
        public float? udf_float01;
        public float? udf_float02;
        public float? udf_float03;
        public bool archived;
        public double total_discount_percent;
        public string note1;
        public string note2;
        public string note3;
        public string note4;
        public string note5;
        public string note6;
        public string note7;
        public string note8;
        public string note9;
        public string note10;
        public double total_discount_amount;
        public int item_type;
        public string dcs_code;
        public string vendor_code;
        public string item_description1;
        public string item_description2;
        public string item_description3;
        public string item_description4;
        public string attribute;
        public string item_size;
        public string alu;
        public string tax_code;
        public string tax_code2;
        public string commission_code;
        public string commission_level;
        public string commission_percent;
        public string st_cuid;
        public string st_id;
        public string st_last_name;
        public string st_first_name;
        public string st_company_name;
        public string st_title;
        public string st_tax_area_name;
        public string st_tax_area2_name;
        public bool st_detax_flag;
        public string st_price_lvl_name;
        public string st_price_lvl;
        public string st_security_lvl;
        public string st_primary_phone_no;
        public string st_country;
        public string st_postal_code;
        public string st_postal_code_extension;
        public bool st_primary;
        public string st_address_line1;
        public string st_address_line2;
        public string st_address_line3;
        public string st_address_line4;
        public string st_address_line5;
        public string st_address_line6;
        public string st_email;
        public string st_customer_lookup;
        public string employee1_login_name;
        public string employee1_full_name;
        public string employee2_login_name;
        public string employee2_full_name;
        public string employee3_login_name;
        public string employee3_full_name;
        public string employee4_login_name;
        public string employee4_full_name;
        public string employee5_login_name;
        public string employee5_full_name;
        public string hist_discount_amt1;
        public string hist_discount_perc1;
        public string hist_discount_reason1;
        public string hist_discount_amt2;
        public string hist_discount_perc2;
        public string hist_discount_reason2;
        public string hist_discount_amt3;
        public string hist_discount_perc3;
        public string hist_discount_reason3;
        public string hist_discount_amt4;
        public string hist_discount_perc4;
        public string hist_discount_reason4;
        public string hist_discount_amt5;
        public string hist_discount_perc5;
        public string hist_discount_reason5;
        public int store_number;
        public string total_discount_reason;
        public string order_type;
        public string so_number;
        public string item_lookup;
        public string inventory_item_type;
        public double inventory_on_hand_quantity;
        public string inventory_quantity_per_case;
        public int subsidiary_number;
        public double discount_amt;
        public double discount_perc;
        public string discount_reason;
        public string returned_item_qty;
        public string returned_item_invoice_sid;
        public string delete_discount;
        public string invn_sbs_item_sid;
        public string custom_flag;
        public string return_reason;
        public int? price_lvl;
        public int? order_quantity_filled;
        public double int_quantity;
        public double? so_deposit_amt;
        public string invn_item_uid;
        public string ref_order_item_sid;
        public string tax_area_name;
        public string tax_area2_name;
        public int serial_type;
        public int lot_type;
        public string price_lvl_sid;
        public string price_lvl_name;
        public bool? so_cancel_flag;
        public string ref_sale_doc_sid;
        public string fulfill_store_sid;
        public int? fulfill_store_no;
        public int? fulfill_store_sbs_no;
        public string central_document_sid;
        public string central_item_pos;
        public string ref_order_doc_sid;
        public string employee1_sid;
        public string employee2_sid;
        public string employee3_sid;
        public string employee4_sid;
        public string employee5_sid;
        public string employee1_id;
        public string employee2_id;
        public string employee3_id;
        public string employee4_id;
        public string employee5_id;
        public double? dip_discount_amt;
        public double? dip_price;
        public double? dip_tax_amount;
        public double? dip_tax2_amount;
        public string ref_sale_item_pos;
        public int item_status;
        public int enhanced_item_pos;
        public string is_competing_component;
        public string package_item_uid;
        public string original_component_item_uid;
        public bool? promo_disc_modifiedmanually;
        public string ship_method_sid;
        public string ship_method_id;
        public string order_ship_method_sid;
        public string order_ship_method_id;
        public string order_ship_method;
        public int from_centrals;
        public bool tax_perc_lock;
        public string employee1_name;
        public string employee2_name;
        public string employee3_name;
        public string employee4_name;
        public string employee5_name;
        public string qty_available_for_return;
        public string central_return_commit_state;
        public double inventory_use_quantity_decimals;
        public string shipping_amt_bdt;
        public string tax_code_rule_sid;
        public string tax_code_rule2_sid;
        public string st_address_uid;
        public string style_sid;
        //""discounts"":[],
        public string manual_disc_value;
        public string manual_disc_type;
        public string manual_disc_reason;
        public string tax_char;
        public string tax_char2;
        public string apply_type_to_all_items;
        public string lty_pgm_sid;
        public string lty_pgm_name;
        public string lty_points_earned;
        public int? lty_orig_points_earned;
        public string lty_price_in_points;
        public double? lty_orig_price_in_points;
        public double? orig_sale_price;
        public string lty_type;
        public bool lty_redeem_applicable_manually;
        public string lty_price_in_points_ext;
        public string lty_piece_of_tbe_points;
        public string lty_piece_of_tbr_points;
        public string lty_piece_of_tbr_disc_amt;
        public string orig_document_number;
        public string orig_store_number;
        public string orig_subsidiary_number;
        public int kit_type;
        public string activation_sid;
        public DateTime? gift_expire_date;
        public double maxdiscpercent;
        public double maxaccumdiscpercent;
        public string override_max_disc_perc;
        public int promo_gift_item;
        public JsonPrintFiscDocItemHrefs hrefs = new JsonPrintFiscDocItemHrefs();
        [JsonProperty("params")]
        public JsonPrintFiscDocItemParam Params = new JsonPrintFiscDocItemParam();
        public JsonPrintFiscDocItemHeader headers = new JsonPrintFiscDocItemHeader();

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"[
    {
        ""link"":""\/v1\/rest\/document\/626460034000353723\/item\/626460038000318724"",
        ""sid"":""626460038000318724"",
        ""created_by"":""SYSADMIN"",
        ""created_datetime"":""2022-02-13T16:40:38.000+03:00"",
        ""modified_by"":""sysadmin"",
        ""modified_datetime"":""2022-02-13T16:42:45.000+03:00"",
        ""controller_sid"":""603242764000070002"",
        ""origin_application"":""RProPrismWeb"",
        ""post_date"":""2022-02-13T16:40:38.000+03:00"",
        ""row_version"":1971032541,
        ""tenant_sid"":""603242764000172003"",
        ""document_sid"":""626460034000353723"",
        ""item_pos"":1,
        ""quantity"":1,
        ""original_price"":114800,
        ""original_tax_amount"":19133.33,
        ""price"":114800,
        ""tax_percent"":20,
        ""tax_amount"":19133.33333,
        ""tax2_percent"":0,
        ""tax2_amount"":0,
        ""detax_flag"":false,
        ""price_before_detax"":null,
        ""original_price_before_detax"":null,
        ""cost"":0,""spif"":0,
        ""schedule_number"":null,
        ""scan_upc"":""8099159091"",
        ""serial_number"":null,
        ""kit_flag"":0,
        ""package_item_sid"":null,
        ""original_component_item_sid"":null,
        ""user_discount_percent"":null,
        ""package_sequence_number"":null,
        ""lot_number"":null,
        ""activity_percent"":100,
        ""activity2_percent"":null,
        ""activity3_percent"":null,
        ""activity4_percent"":null,
        ""activity5_percent"":null,
        ""commission_amount"":null,
        ""commission2_amount"":null,
        ""commission3_amount"":null,
        ""commission4_amount"":null,
        ""commission5_amount"":null,
        ""item_origin"":0,
        ""package_number"":null,
        ""ship_id"":null,
        ""ship_method"":null,
        ""shipping_amt"":0,
        ""tracking_number"":null,
        ""original_cost"":0,
        ""promotion_flag"":false,
        ""gift_activation_code"":null,
        ""gift_transaction_id"":null,
        ""gift_add_value"":false,
        ""customer_field"":null,
        ""udf_string01"":""7569i"",
        ""udf_string02"":null,
        ""udf_string03"":null,
        ""udf_string04"":null,
        ""udf_string05"":null,
        ""udf_date01"":null,
        ""udf_float01"":null,
        ""udf_float02"":0,
        ""udf_float03"":0,
        ""archived"":false,
        ""total_discount_percent"":0,
        ""note1"":null,
        ""note2"":null,
        ""note3"":null,
        ""note4"":null,
        ""note5"":null,
        ""note6"":null,
        ""note7"":null,
        ""note8"":null,
        ""note9"":null,
        ""note10"":null,
        ""total_discount_amount"":0,
        ""item_type"":1,
        ""dcs_code"":""110105217"",
        ""vendor_code"":""GUCCI"",
        ""item_description1"":""21113122DCN"",
        ""item_description2"":""W&U LIFESTYLE"",
        ""item_description3"":null,
        ""item_description4"":null,
        ""attribute"":""8303"",
        ""item_size"":""NOSIZ"",
        ""alu"":""8099159091"",
        ""tax_code"":""0"",
        ""tax_code2"":null,
        ""commission_code"":null,
        ""commission_level"":null,
        ""commission_percent"":null,
        ""st_cuid"":null,
        ""st_id"":null,
        ""st_last_name"":null,
        ""st_first_name"":null,
        ""st_company_name"":null,
        ""st_title"":null,
        ""st_tax_area_name"":null,
        ""st_tax_area2_name"":null,
        ""st_detax_flag"":false,
        ""st_price_lvl_name"":null,
        ""st_price_lvl"":null,
        ""st_security_lvl"":null,
        ""st_primary_phone_no"":null,
        ""st_country"":null,
        ""st_postal_code"":null,
        ""st_postal_code_extension"":null,
        ""st_primary"":false,
        ""st_address_line1"":null,
        ""st_address_line2"":null,
        ""st_address_line3"":null,
        ""st_address_line4"":null,
        ""st_address_line5"":null,
        ""st_address_line6"":null,
        ""st_email"":null,
        ""st_customer_lookup"":null,
        ""employee1_login_name"":""SYSADMIN"",
        ""employee1_full_name"":""sysadmin"",
        ""employee2_login_name"":null,
        ""employee2_full_name"":null,
        ""employee3_login_name"":null,
        ""employee3_full_name"":null,
        ""employee4_login_name"":null,
        ""employee4_full_name"":null,
        ""employee5_login_name"":null,
        ""employee5_full_name"":null,
        ""hist_discount_amt1"":null,
        ""hist_discount_perc1"":null,
        ""hist_discount_reason1"":null,
        ""hist_discount_amt2"":null,
        ""hist_discount_perc2"":null,
        ""hist_discount_reason2"":null,
        ""hist_discount_amt3"":null,
        ""hist_discount_perc3"":null,
        ""hist_discount_reason3"":null,
        ""hist_discount_amt4"":null,
        ""hist_discount_perc4"":null,
        ""hist_discount_reason4"":null,
        ""hist_discount_amt5"":null,
        ""hist_discount_perc5"":null,
        ""hist_discount_reason5"":null,
        ""store_number"":1,
        ""total_discount_reason"":null,
        ""order_type"":null,
        ""so_number"":null,
        ""item_lookup"":null,
        ""inventory_item_type"":null,
        ""inventory_on_hand_quantity"":24,
        ""inventory_quantity_per_case"":null,
        ""subsidiary_number"":1,
        ""discount_amt"":0,
        ""discount_perc"":0,
        ""discount_reason"":null,
        ""returned_item_qty"":null,
        ""returned_item_invoice_sid"":null,
        ""delete_discount"":null,
        ""invn_sbs_item_sid"":""601964952000197965"",
        ""custom_flag"":null,
        ""return_reason"":null,
        ""price_lvl"":1,
        ""order_quantity_filled"":0,
        ""gift_quantity"":0,
        ""so_deposit_amt"":0,
        ""invn_item_uid"":""259173091067"",
        ""ref_order_item_sid"":null,
        ""tax_area_name"":""RUS"",
        ""tax_area2_name"":null,
        ""serial_type"":0,
        ""lot_type"":0,
        ""price_lvl_sid"":""603242803000175314"",
        ""price_lvl_name"":""001"",
        ""so_cancel_flag"":null,
        ""ref_sale_doc_sid"":null,
        ""fulfill_store_sid"":""603242764000178005"",
        ""fulfill_store_no"":1,
        ""fulfill_store_sbs_no"":1,
        ""central_document_sid"":null,
        ""central_item_pos"":null,
        ""ref_order_doc_sid"":null,
        ""employee1_sid"":""603242764000183009"",
        ""employee2_sid"":null,
        ""employee3_sid"":null,
        ""employee4_sid"":null,
        ""employee5_sid"":null,
        ""employee1_id"":""1"",
        ""employee2_id"":null,
        ""employee3_id"":null,
        ""employee4_id"":null,
        ""employee5_id"":null,
        ""dip_discount_amt"":0,
        ""dip_price"":114800,
        ""dip_tax_amount"":19133.3333,
        ""dip_tax2_amount"":0,
        ""ref_sale_item_pos"":null,
        ""item_status"":0,
        ""enhanced_item_pos"":10000,
        ""is_competing_component"":null,
        ""package_item_uid"":null,
        ""original_component_item_uid"":null,
        ""promo_disc_modifiedmanually"":false,
        ""ship_method_sid"":null,
        ""ship_method_id"":null,
        ""order_ship_method_sid"":null,
        ""order_ship_method_id"":null,
        ""order_ship_method"":null,
        ""from_centrals"":0,
        ""tax_perc_lock"":false,
        ""employee1_name"":""SYSADMIN"",
        ""employee2_name"":null,
        ""employee3_name"":null,
        ""employee4_name"":null,
        ""employee5_name"":null,
        ""qty_available_for_return"":null,
        ""central_return_commit_state"":null,
        ""inventory_use_quantity_decimals"":0,
        ""shipping_amt_bdt"":null,
        ""tax_code_rule_sid"":""601946104610179003"",
        ""tax_code_rule2_sid"":null,
        ""st_address_uid"":null,
        ""style_sid"":""259173091067"",
        ""discounts"":[],
        ""manual_disc_value"":null,
        ""manual_disc_type"":null,
        ""manual_disc_reason"":null,
        ""tax_char"":null,
        ""tax_char2"":null,
        ""apply_type_to_all_items"":null,
        ""lty_pgm_sid"":null,
        ""lty_pgm_name"":null,
        ""lty_points_earned"":null,
        ""lty_orig_points_earned"":0,
        ""lty_price_in_points"":null,
        ""lty_orig_price_in_points"":0,
        ""orig_sale_price"":null,
        ""lty_type"":null,
        ""lty_redeem_applicable_manually"":false,
        ""lty_price_in_points_ext"":null,
        ""lty_piece_of_tbe_points"":null,
        ""lty_piece_of_tbr_points"":null,
        ""lty_piece_of_tbr_disc_amt"":null,
        ""orig_document_number"":null,
        ""orig_store_number"":null,
        ""orig_subsidiary_number"":null,
        ""eftdata0"":null,
        ""tax_name"":""TAXABLE"",
        ""tax_name2"":null,
        ""eftdata1"":null,
        ""eftdata2"":null,
        ""eftdata3"":null,
        ""eftdata4"":null,
        ""eftdata5"":null,
        ""eftdata6"":null,
        ""eftdata7"":null,
        ""eftdata8"":null,
        ""eftdata9"":null,
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
        ""eftdatabscust"":null,
        ""eftdatabsmer"":null,
        ""kit_type"":0,
        ""activation_sid"":null,
        ""authorize_date"":null,
        ""gift_expire_date"":null,
        ""udf_string06"":""1400.000"",
        ""udf_string07"":""Italy - CEE"",
        ""udf_string08"":""LUGGAGE"",
        ""udf_string09"":null,
        ""udf_string10"":null,
        ""udf_string11"":null,
        ""udf_string12"":null,
        ""udf_string13"":null,
        ""udf_string14"":null,
        ""udf_string15"":null,
        ""maxdiscpercent"":100,
        ""maxaccumdiscpercent"":100,
        ""override_max_disc_perc"":null,
        ""image_path"":""inventory\\0000\\0000\\2591\\7309\\"",
        ""promo_gift_item"":0,
        ""employee1_orig_sbs_no"":1,
        ""employee2_orig_sbs_no"":null,
        ""employee3_orig_sbs_no"":null,
        ""employee4_orig_sbs_no"":null,
        ""employee5_orig_sbs_no"":null,
        ""style_image_path"":""invn_style\\0000\\0000\\2591\\7309\\"",
        ""internal_item_pos"":1,
        ""special_order"":false,
        ""dip_price_bdt"":null,
        ""hrefs"":{""controller_sid"":""\/v1\/rest\/controller\/603242764000070002""}
    }
]";
        /*
         
        

         */


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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonPrintFiscDocItem))
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
        public static List<JsonPrintFiscDocItem> DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<List<BLL.JsonPrintFiscDocItem>>(json, BLL.JsonPrintFiscDocItem.GetSettings());
        }
    }
}
