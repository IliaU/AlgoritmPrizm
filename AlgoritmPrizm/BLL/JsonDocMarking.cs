using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Класс представляет запрос который идёт от клиента при выборе товара в чеке
    /// </summary>
    public class JsonDocMarking
    {
        private static JsonSerializerSettings _settings;

        public string resource;
        public string endpoint;
        //""dirty"":{},
        public string origin_application;
        public string invn_sbs_item_sid;
        public string fulfill_store_sid;
        public int item_pos;
        public string document_sid;
        public string serial_number;
        public int kit_type;
        public int item_type;
        public string lot_number;
        public double? quantity;
        public string link;
        public string sid;
        public string created_by;
        public DateTime created_datetime;
        public string modified_by;
        public DateTime? modified_datetime;
        public string controller_sid;
        public DateTime post_date;
        public int row_version;
        public string tenant_sid;
        public double? original_price;
        public double? original_tax_amount;
        public double? price;
        public double? tax_percent;
        public double? tax_amount;
        public double? tax2_percent;
        public double? tax2_amount;
        public bool detax_flag;
        public double? price_before_detax;
        public double? original_price_before_detax;
        public double? cost;
        public double? spif;
        public int? schedule_number;
        public string scan_upc;
        public int kit_flag;
        public string package_item_sid;
        public string original_component_item_sid;
        public double? user_discount_percent;
        public int? package_sequence_number;
        public double? activity_percent;
        public double? activity2_percent;
        public double? activity3_percent;
        public double? activity4_percent;
        public double? activity5_percent;
        public double? commission_amount;
        public double? commission2_amount;
        public double? commission3_amount;
        public double? commission4_amount;
        public double? commission5_amount;
        public int item_origin;
        public int? package_number;
        public string ship_id;
        public string ship_method;
        public double? shipping_amt;
        public int? tracking_number;
        public double? original_cost;
        public bool promotion_flag;
        public string gift_activation_code;
        public int? gift_transaction_id;
        public bool gift_add_value;
        public string customer_field;
        public string udf_string01;
        public string udf_string02;
        public string udf_string03;
        public string udf_string04;
        public string udf_string05;
        public DateTime? udf_date01;
        public int? udf_float01;
        public int? udf_float02;
        public int? udf_float03;
        public bool archived;
        public double? total_discount_percent;
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
        public double? total_discount_amount;
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
        public double? commission_percent;
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
        public double? hist_discount_amt1;
        public double? hist_discount_perc1;
        public double? hist_discount_reason1;
        public double? hist_discount_amt2;
        public double? hist_discount_perc2;
        public double? hist_discount_reason2;
        public double? hist_discount_amt3;
        public double? hist_discount_perc3;
        public double? hist_discount_reason3;
        public double? hist_discount_amt4;
        public double? hist_discount_perc4;
        public double? hist_discount_reason4;
        public double? hist_discount_amt5;
        public double? hist_discount_perc5;
        public double? hist_discount_reason5;
        public int store_number;
        public double? total_discount_reason;
        public string order_type;
        public string so_number;
        public string item_lookup;
        public string inventory_item_type;
        public double? inventory_on_hand_quantity;
        public double? inventory_quantity_per_case;
        public int subsidiary_number;
        public double? discount_amt;
        public double? discount_perc;
        public double? discount_reason;
        public double? returned_item_qty;
        public string returned_item_invoice_sid;
        public string delete_discount;
        public string custom_flag;
        public string return_reason;
        public int price_lvl;
        public double order_quantity_filled;
        public double gift_quantity;
        public double so_deposit_amt;
        public string invn_item_uid;
        public string ref_order_item_sid;
        public string tax_area_name;
        public string tax_area2_name;
        public int serial_type;
        public int lot_type;
        public string price_lvl_sid;
        public string price_lvl_name;
        public string so_cancel_flag;
        public string ref_sale_doc_sid;
        public int fulfill_store_no;
        public int fulfill_store_sbs_no;
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
        public int? ref_sale_item_pos;
        public int item_status;
        public int? enhanced_item_pos;
        public string is_competing_component;
        public string package_item_uid;
        public string original_component_item_uid;
        public bool promo_disc_modifiedmanually;
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
        public double? inventory_use_quantity_decimals;
        public double? shipping_amt_bdt;
        public string tax_code_rule_sid;
        public string tax_code_rule2_sid;
        public string st_address_uid;
        public string style_sid;
        public string manual_disc_value;
        public string manual_disc_type;
        public string manual_disc_reason;

        public string tax_char;
        public string tax_char2;
        public string apply_type_to_all_items;
        public string lty_pgm_sid;
        public string lty_pgm_name;
        public int? lty_points_earned;
        public int? lty_orig_points_earned;
        public double? lty_price_in_points;
        public int lty_orig_price_in_points;
        public double? orig_sale_price;
        public string lty_type;
        public bool lty_redeem_applicable_manually;
        public int? lty_price_in_points_ext;
        public int? lty_piece_of_tbe_points;
        public int? lty_piece_of_tbr_points;
        public double? lty_piece_of_tbr_disc_amt;
        public int? orig_document_number;
        public int? orig_store_number;
        public int? orig_subsidiary_number;
        public DateTime? eftdata0;
        public string tax_name;
        public string tax_name2;
        public string eftdata1;
        public string eftdata2;
        public string eftdata3;
        public string eftdata4;
        public string eftdata5;
        public string eftdata6;
        public string eftdata7;
        public string eftdata8;
        public string eftdata9;
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
        public string eftdatabscust;
        public string eftdatabsmer;
        public string activation_sid;
        public DateTime? authorize_date;
        public DateTime? gift_expire_date;
        public string udf_string06;
        public string udf_string07;
        public string udf_string08;
        public string udf_string09;
        public string udf_string10;
        public string udf_string11;
        public string udf_string12;
        public string udf_string13;
        public string udf_string14;
        public string udf_string15;
        public double? maxdiscpercent;
        public double? maxaccumdiscpercent;
        public double? override_max_disc_perc;
        public string image_path;
        public int promo_gift_item;
        public int employee1_orig_sbs_no;
        public int? employee2_orig_sbs_no;
        public int? employee3_orig_sbs_no;
        public int? employee4_orig_sbs_no;
        public int? employee5_orig_sbs_no;
        public string style_image_path;
        public int internal_item_pos;
        public bool special_order;
        public double? dip_price_bdt;
        public double? detaxed_ext_disc_amt;
        public JsonDocMarkingHrefs hrefs;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
	""resource"":""item"",
	""endpoint"":""document/:document_sid/item/:sid"",
	""dirty"":{},
	""origin_application"":""RProPrismWeb"",
	""invn_sbs_item_sid"":""601964952000197965"",
	""fulfill_store_sid"":""609967778000138005"",
	""item_pos"":1,
	""document_sid"":""612664819000269551"",
	""serial_number"":"""",
	""kit_type"":0,
	""item_type"":1,
	""lot_number"":"""",
	""quantity"":1,
	""link"":""/v1/rest/document/612664819000269551/item/612680446000236572"",
	""sid"":""612680446000236572"",
	""created_by"":""SYSADMIN"",
	""created_datetime"":""2021-09-28T22:20:46.000+03:00"",
	""modified_by"":"""",
	""modified_datetime"":"""",
	""controller_sid"":""609967778000033002"",
	""post_date"":""2021-09-28T22:20:46.000+03:00"",
	""row_version"":1,
	""tenant_sid"":""609967778000134003"",
	""original_price"":114800,
	""original_tax_amount"":19133.33,
	""price"":114800,
	""tax_percent"":20,
	""tax_amount"":19133.33333,
	""tax2_percent"":0,
	""tax2_amount"":0,
	""detax_flag"":false,
	""price_before_detax"":"""",
	""original_price_before_detax"":"""",
	""cost"":0,
	""spif"":0,
	""schedule_number"":"""",
	""scan_upc"":""8099159091"",
	""kit_flag"":0,
	""package_item_sid"":"""",
	""original_component_item_sid"":"""",
	""user_discount_percent"":"""",
	""package_sequence_number"":"""",
	""activity_percent"":100,
	""activity2_percent"":"""",
	""activity3_percent"":"""",
	""activity4_percent"":"""",
	""activity5_percent"":"""",
	""commission_amount"":"""",
	""commission2_amount"":"""",
	""commission3_amount"":"""",
	""commission4_amount"":"""",
	""commission5_amount"":"""",
	""item_origin"":0,
	""package_number"":"""",
	""ship_id"":"""",
	""ship_method"":"""",
	""shipping_amt"":0,
	""tracking_number"":"""",
	""original_cost"":0,
	""promotion_flag"":false,
	""gift_activation_code"":"""",
	""gift_transaction_id"":"""",
	""gift_add_value"":false,
	""customer_field"":"""",
	""udf_string01"":"""",
	""udf_string02"":"""",
	""udf_string03"":"""",
	""udf_string04"":"""",
	""udf_string05"":"""",
	""udf_date01"":"""",
	""udf_float01"":"""",
	""udf_float02"":0,
	""udf_float03"":0,
	""archived"":false,
	""total_discount_percent"":0,
	""note1"":"""",
	""note2"":"""",
	""note3"":"""",
	""note4"":"""",
	""note5"":"""",
	""note6"":"""",
	""note7"":"""",
	""note8"":"""",
	""note9"":"""",
	""note10"":"""",
	""total_discount_amount"":0,
	""dcs_code"":""110105217"",
	""vendor_code"":""GUCCI"",
	""item_description1"":""21113122DCN"",
	""item_description2"":""СУМКА"",
	""item_description3"":""БЕЖЕВЫЙ"",
	""item_description4"":""ХЛОПОК+ПОЛИЭСТЕР+ПОЛИУРЕТАН"",
	""attribute"":""8303"",
	""item_size"":""NOSIZ"",
	""alu"":""8099159091"",
	""tax_code"":""0"",
	""tax_code2"":"""",
	""commission_code"":"""",
	""commission_level"":"""",
	""commission_percent"":"""",
	""st_cuid"":"""",
	""st_id"":"""",
	""st_last_name"":"""",
	""st_first_name"":"""",
	""st_company_name"":"""",
	""st_title"":"""",
	""st_tax_area_name"":"""",
	""st_tax_area2_name"":"""",
	""st_detax_flag"":false,
	""st_price_lvl_name"":"""",
	""st_price_lvl"":"""",
	""st_security_lvl"":"""",
	""st_primary_phone_no"":"""",
	""st_country"":"""",
	""st_postal_code"":"""",
	""st_postal_code_extension"":"""",
	""st_primary"":false,
	""st_address_line1"":"""",
	""st_address_line2"":"""",
	""st_address_line3"":"""",
	""st_address_line4"":"""",
	""st_address_line5"":"""",
	""st_address_line6"":"""",
	""st_email"":"""",
	""st_customer_lookup"":"""",
	""employee1_login_name"":""sysadmin"",
	""employee1_full_name"":""sysadmin"",
	""employee2_login_name"":"""",
	""employee2_full_name"":"""",
	""employee3_login_name"":"""",
	""employee3_full_name"":"""",
	""employee4_login_name"":"""",
	""employee4_full_name"":"""",
	""employee5_login_name"":"""",
	""employee5_full_name"":"""",
	""hist_discount_amt1"":"""",
	""hist_discount_perc1"":"""",
	""hist_discount_reason1"":"""",
	""hist_discount_amt2"":"""",
	""hist_discount_perc2"":"""",
	""hist_discount_reason2"":"""",
	""hist_discount_amt3"":"""",
	""hist_discount_perc3"":"""",
	""hist_discount_reason3"":"""",
	""hist_discount_amt4"":"""",
	""hist_discount_perc4"":"""",
	""hist_discount_reason4"":"""",
	""hist_discount_amt5"":"""",
	""hist_discount_perc5"":"""",
	""hist_discount_reason5"":"""",
	""store_number"":1,
	""total_discount_reason"":"""",
	""order_type"":"""",
	""so_number"":"""",
	""item_lookup"":"""",
	""inventory_item_type"":"""",
	""inventory_on_hand_quantity"":342,
	""inventory_quantity_per_case"":"""",
	""subsidiary_number"":1,
	""discount_amt"":0,
	""discount_perc"":0,
	""discount_reason"":"""",
	""returned_item_qty"":"""",
	""returned_item_invoice_sid"":"""",
	""delete_discount"":"""",
	""custom_flag"":"""",
	""return_reason"":"""",
	""price_lvl"":1,
	""order_quantity_filled"":0,
	""gift_quantity"":0,
	""so_deposit_amt"":0,
	""invn_item_uid"":""259173091067"",
	""ref_order_item_sid"":"""",
	""tax_area_name"":""RUS"",
	""tax_area2_name"":"""",
	""serial_type"":0,
	""lot_type"":0,
	""price_lvl_sid"":""609967813000117305"",
	""price_lvl_name"":""001"",
	""so_cancel_flag"":"""",
	""ref_sale_doc_sid"":"""",
	""fulfill_store_no"":1,
	""fulfill_store_sbs_no"":1,
	""central_document_sid"":"""",
	""central_item_pos"":"""",
	""ref_order_doc_sid"":"""",
	""employee1_sid"":""609967778000141009"",
	""employee2_sid"":"""",
	""employee3_sid"":"""",
	""employee4_sid"":"""",
	""employee5_sid"":"""",
	""employee1_id"":""1"",
	""employee2_id"":"""",
	""employee3_id"":"""",
	""employee4_id"":"""",
	""employee5_id"":"""",
	""dip_discount_amt"":"""",
	""dip_price"":"""",
	""dip_tax_amount"":"""",
	""dip_tax2_amount"":"""",
	""ref_sale_item_pos"":"""",
	""item_status"":0,
	""enhanced_item_pos"":10000,
	""is_competing_component"":"""",
	""package_item_uid"":"""",
	""original_component_item_uid"":"""",
	""promo_disc_modifiedmanually"":false,
	""ship_method_sid"":"""",
	""ship_method_id"":"""",
	""order_ship_method_sid"":"""",
	""order_ship_method_id"":"""",
	""order_ship_method"":"""",
	""from_centrals"":0,
	""tax_perc_lock"":false,
	""employee1_name"":""SYSADMIN"",
	""employee2_name"":"""",
	""employee3_name"":"""",
	""employee4_name"":"""",
	""employee5_name"":"""",
	""qty_available_for_return"":"""",
	""central_return_commit_state"":"""",
	""inventory_use_quantity_decimals"":0,
	""shipping_amt_bdt"":"""",
	""tax_code_rule_sid"":""601946104610179003"",
	""tax_code_rule2_sid"":"""",
	""st_address_uid"":"""",
	""style_sid"":""6677905728552028805"",
	""discounts"":[],
	""manual_disc_value"":"""",
	""manual_disc_type"":"""",
	""manual_disc_reason"":"""",
	""tax_char"":"""",
	""tax_char2"":"""",
	""apply_type_to_all_items"":"""",
	""lty_pgm_sid"":"""",
	""lty_pgm_name"":"""",
	""lty_points_earned"":"""",
	""lty_orig_points_earned"":0,
	""lty_price_in_points"":"""",
	""lty_orig_price_in_points"":0,
	""orig_sale_price"":"""",
	""lty_type"":"""",
	""lty_redeem_applicable_manually"":false,
	""lty_price_in_points_ext"":"""",
	""lty_piece_of_tbe_points"":"""",
	""lty_piece_of_tbr_points"":"""",
	""lty_piece_of_tbr_disc_amt"":"""",
	""orig_document_number"":"""",
	""orig_store_number"":"""",
	""orig_subsidiary_number"":"""",
	""eftdata0"":"""",
	""tax_name"":""TAXABLE"",
	""tax_name2"":"""",
	""eftdata1"":"""",
	""eftdata2"":"""",
	""eftdata3"":"""",
	""eftdata4"":"""",
	""eftdata5"":"""",
	""eftdata6"":"""",
	""eftdata7"":"""",
	""eftdata8"":"""",
	""eftdata9"":"""",
	""eftdata10"":"""",
	""eftdata11"":"""",
	""eftdata12"":"""",
	""eftdata13"":"""",
	""eftdata14"":"""",
	""eftdata15"":"""",
	""eftdata16"":"""",
	""eftdata17"":"""",
	""eftdata18"":"""",
	""eftdata19"":"""",
	""eftdatabscust"":"""",
	""eftdatabsmer"":"""",
	""activation_sid"":"""",
	""authorize_date"":"""",
	""gift_expire_date"":"""",
	""udf_string06"":""1400.000"",
	""udf_string07"":"""",
	""udf_string08"":""LUGGAGE"",
	""udf_string09"":"""",
	""udf_string10"":"""",
	""udf_string11"":"""",
	""udf_string12"":"""",
	""udf_string13"":"""",
	""udf_string14"":"""",
	""udf_string15"":"""",
	""maxdiscpercent"":100,
	""maxaccumdiscpercent"":100,
	""override_max_disc_perc"":"""",
	""image_path"":""inventory\\0000\\0000\\2591\\7309\\"",
	""promo_gift_item"":0,
	""employee1_orig_sbs_no"":1,
	""employee2_orig_sbs_no"":"""",
	""employee3_orig_sbs_no"":"""",
	""employee4_orig_sbs_no"":"""",
	""employee5_orig_sbs_no"":"""",
	""style_image_path"":""invn_style\\0667\\7905\\7285\\5202\\"",
	""internal_item_pos"":1,
	""special_order"":false,
	""dip_price_bdt"":"""",
	""detaxed_ext_disc_amt"":0,
	""hrefs"":
		{
			""controller_sid"":""/v1/rest/controller/609967778000033002""
		}
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
                      args.ErrorContext.OriginalObject.GetType() == typeof(JsonDocMarking))
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
        public static JsonDocMarking DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BLL.JsonDocMarking>(json, BLL.JsonDocMarking.GetSettings());
        }
    }
}
