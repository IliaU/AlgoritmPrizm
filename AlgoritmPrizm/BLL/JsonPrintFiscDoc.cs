﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

//  BLL.JsonPrintFiscDoc root = JsonConvert.DeserializeObject<BLL.JsonPrintFiscDoc>(BLL.JsonPrintFiscDoc.SampleTest,BLL.JsonPrintFiscDoc.GetSettings());

namespace AlgoritmPrizm.BLL
{
    public class JsonPrintFiscDoc
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
        public int row_version;
        public int? document_number;
        public DateTime? invoice_posted_date;
        public int status;
        public string tracking_number;
        public double? discount_amount;
        public double? discount_perc;
        public string tax_rebate_percent;
        public double? tax_rebate_amt;
        public int rounding_offset;
        public bool is_held;
        public int activity_percent;
        public string activity2_percent;
        public string activity3_percent;
        public string activity4_percent;
        public string activity5_percent;
        public double? eft_invoice_number;
        public bool detax_flag;
        public string shipping_percentage;
        public string shipping_amt;
        public string shipping_tax_included;
        public string shipping_tax_percentage;
        public string shipping_tax_amt;
        public string ship_method;
        public double total_fee_amt;
        public double? total_discount_amt;
        public double? sale_total_tax_amt;
        public double? sale_subtotal;
        public double? deposit_amt_required;
        public double? due_amt;
        public double? sold_qty;
        public double? return_qty;
        public double? order_qty;
        public double? taken_amt;
        public int subsidiary_number;
        public int store_number;
        public string store_code;
        public string tax_area_name;
        public string tax_area2_name;
        public string employee1_login_name;
        public string cashier_login_name;
        public string fee_name1;
        public string fee_amt1;
        public string fee_tax_included1;
        public string fee_tax_amt1;
        public string fee_tax_perc1;
        public string bt_cuid;
        public string bt_id;
        public string bt_last_name;
        public string bt_first_name;
        public string bt_company_name;
        public string bt_primary_phone_no;
        public string bt_address_line1;
        public string bt_address_line2;
        public string bt_address_line3;
        public string bt_postal_code;
        public string bt_email;
        public string st_cuid;
        public string st_last_name;
        public string st_first_name;
        public string st_primary_phone_no;
        public string st_address_line1;
        public string st_address_line2;
        public string st_address_line3;
        public string st_address_line4;
        public string st_address_line5;
        public string st_address_line6;
        public string st_country;
        public string st_postal_code;
        public List<JsonPrintFiscDocTender> tenders = new List<JsonPrintFiscDocTender>();
        public double? order_total_tax_amt;
        public double? transaction_total_tax_amt;
        public double? sale_tax1_amt;
        public double? order_tax1_amt;
        public double? transaction_tax1_amt;
        public string sale_tax2_amt;
        public string order_tax2_amt;
        public string transaction_tax2_amt;
        public double? transaction_total_amt;
        public double? order_subtotal;
        public double? order_subtotal_with_tax;
        public double? so_deposit_amt_paid;
        public double? transaction_subtotal;
        public double? given_amt;
        public int ? price_lvl;
        public string price_lvl_name;
        public string discount_reason_name;
        public string subsidiary_name;
        public string store_name;
        public double? tax_area_percent;
        public double? tax_area2_percent;
        public string reason_code;
        public string pos_flag1;
        public string pos_flag2;
        public string pos_flag3;
        public string comment1;
        public string comment2;
        public string deposit_amt_taken;
        public string deposit_ref_doc_sid;
        public string total_deposit_taken;
        public string notes_general;
        public string store_uid;
        public DateTime? ordered_date;
        public DateTime? cancel_date;
        public string ship_partial;
        public string ship_priority;
        public string employee1_sid;
        public string employee2_sid;
        public string employee3_sid;
        public string employee4_sid;
        public string employee5_sid;
        public string cashier_sid;
        public string ref_order_sid;
        public int order_discount_type;
        public string order_discount_reason_name;
        public double? order_discount_perc;
        public double? order_discount_amount;
        public bool? so_cancel_flag;
        public string total_deposit_used;
        public double? deposit_available_amt;
        public string order_fee_amt1;
        public string order_fee_tax_amt1;
        public string order_fee_tax_included1;
        public string order_fee_tax_perc1;
        public string order_shipping_amt;
        public string order_shipping_tax_amt;
        public string order_shipping_tax_included;
        public string order_shipping_tax_perc;
        public string fee_type1_sid;
        public string shipping_sid;
        public string order_fee_type1_sid;
        public string order_shipping_sid;
        public string order_shipping_percentage;
        public double? fee_amt1_no_tax;
        public double? fee_amt1_with_tax;
        public double? order_shipping_amt_no_tax;
        public double? order_shipping_amt_with_tax;
        public double? shipping_amt_no_tax;
        public double? shipping_amt_with_tax;
        public string order_document_number;
        public string used_fee_amt1;
        public bool order_changed_flag;
        public double? total_fee_amt_no_tax;
        public double? total_fee_tax_amt;
        public double? order_balance_due;
        public string original_store_uid;
        public bool has_sale;
        public bool has_return;
        public bool has_deposit;
        public int receipt_type;
        public string order_type;
        public string ship_method_id;
        public string ship_method_sid;
        public string order_ship_method;
        public string order_ship_method_id;
        public string order_ship_method_sid;
        public double? from_centrals;
        public bool send_sale_fulfillment;
        public double? detax_amount;
        public string order_shipping_amt_manual;
        public double? tax_rebate_persisted;
        public string order_status;
        public string shipping_amt_manual;
        public string order_shipping_amt_manual_used;
        public string order_tracking_number;
        public double? transaction_total_shipping_tax;
        public double? transaction_total_shipping_amt_no_tax;
        public double? transaction_total_shipping_amt_with_tax;
        public double? return_subtotal;
        public double? return_total_tax_amt;
        public double? order_fulfilling_amt;
        public double? tax_rebate_available;
        public string manual_disc_type;
        public string manual_order_disc_type;
        public string manual_disc_value;
        public string manual_order_disc_value;
        public string manual_disc_reason;
        public string manual_order_disc_reason;
        //""coupons"":[],
        public string lty_total_redeem_pgm_name;
        public string lty_start_balance;
        public string lty_end_balance;
        public string lty_redeem_amt;
        public string fee_amt_returned1;
        public string lty_gift_amt;
        public string lty_sale_total_based_disc;
        public string lty_lvl_sid;
        public string lty_perc_reward_disc_perc;
        public string lty_perc_reward_disc_amt;
        public string lty_total_earn_pgm_name;
        public double? lty_earned_points_positive;
        public double? lty_earned_points_negative;
        public double? lty_used_points_positive;
        public double? lty_used_points_negative;
        public double? lty_used_points;
        public double? lty_earned_points;
        public string shipping_amt_manual_returned;
        public string lty_item_earn_pgm_name;
        public string ref_sale_sid;
        public string lty_item_redeem_pgm_name;
        public string lty_item_gift_pgm_name;
        public string ref_sale_doc_no;
        public string ref_sale_tax_area_name;
        public string lty_total_redeem_multiplier;
        public string lty_total_earn_multiplier;
        public double? lty_balance_for_item_redeem;
        public string lty_lvl_name;
        public double? lty_balance_for_total_redeem;
        public string lty_order_total_based_disc;
        public string lty_order_earned_points_positive;
        public string lty_order_earned_points_negative;
        public string lty_order_used_points_positive;
        public string lty_order_used_points_negative;
        public string lty_sale_earned_points_positive;
        public string lty_sale_earned_points_negative;
        public string lty_sale_used_points_positive;
        public string lty_sale_used_points_negative;
        public string gift_receipt_type;
        public DateTime? order_due_date;
        public double? override_max_disc_perc;
        public double? sale_total_tax_amt_rounded;
        public double? return_total_tax_amt_rounded;
        [JsonProperty("params")]
        public JsonPrintFiscDocParam Params = new JsonPrintFiscDocParam();
        public JsonPrintFiscDocHeader headers = new JsonPrintFiscDocHeader();
        public List<JsonPrintFiscDocItem> items = new List<JsonPrintFiscDocItem>();
        public int? doc_tender_type;

        /// <summary>
        /// Пример на котором тестировали
        /// </summary>
        [JsonIgnore]
        public static string SampleTest = @"{
  ""resource"":""document"",
  ""endpoint"":""document/:sid"",
  ""dirty"":{},
  ""origin_application"":""RProPrismWeb"",
  ""link"":""/v1/rest/document/592070915000115108"",
  ""sid"":""592070915000115108"",
  ""created_by"":""sysadmin"",
  ""created_datetime"":""2021-03-18T19:41:55.000+03:00"",
  ""row_version"":9,
  ""document_number"":231,
  ""invoice_posted_date"":""2021-03-18T19:45:36.000+03:00"",
  ""status"":4,
  ""tracking_number"":"""",
  ""discount_amount"":0,
  ""discount_perc"":0,
  ""tax_rebate_percent"":"""",
  ""tax_rebate_amt"":0,
  ""rounding_offset"":0,
  ""is_held"":false,
  ""activity_percent"":100,
  ""activity2_percent"":"""",
  ""activity3_percent"":"""",
  ""activity4_percent"":"""",
  ""activity5_percent"":"""",
  ""eft_invoice_number"":605,
  ""detax_flag"":false,
  ""shipping_percentage"":"""",
  ""shipping_amt"":"""",
  ""shipping_tax_included"":"""",
  ""shipping_tax_percentage"":"""",
  ""shipping_tax_amt"":"""",
  ""ship_method"":"""",
  ""total_fee_amt"":0,
  ""total_discount_amt"":0,
  ""sale_total_tax_amt"":43.41,
  ""sale_subtotal"":738,
  ""deposit_amt_required"":0,
  ""due_amt"":0,
  ""sold_qty"":2,
  ""return_qty"":0,
  ""order_qty"":0,
  ""taken_amt"":738,
  ""subsidiary_number"":1,
  ""store_number"":1,
  ""store_code"":""ORD"",
  ""tax_area_name"":""ILLINOIS"",
  ""tax_area2_name"":"""",
  ""employee1_login_name"":""sysadmin"",
  ""cashier_login_name"":""sysadmin"",
  ""fee_name1"":"""",
  ""fee_amt1"":"""",
  ""fee_tax_included1"":"""",
  ""fee_tax_amt1"":"""",
  ""fee_tax_perc1"":"""",
  ""bt_cuid"":"""",
  ""bt_id"":"""",
  ""bt_last_name"":"""",
  ""bt_first_name"":"""",
  ""bt_company_name"":"""",
  ""bt_primary_phone_no"":"""",
  ""bt_address_line1"":"""",
  ""bt_address_line2"":"""",
  ""bt_address_line3"":"""",
  ""bt_postal_code"":"""",
  ""bt_email"":"""",
  ""st_cuid"":"""",
  ""st_last_name"":"""",
  ""st_first_name"":"""",
  ""st_primary_phone_no"":"""",
  ""st_address_line1"":"""",
  ""st_address_line2"":"""",
  ""st_address_line3"":"""",
  ""st_address_line4"":"""",
  ""st_address_line5"":"""",
  ""st_address_line6"":"""",
  ""st_country"":"""",
  ""st_postal_code"":"""",
  ""tenders"":[
    {
      ""link"":""/v1/rest/document/592070915000115108/tender/592071073000171112"",
      ""sid"":""592071073000171112"",
      ""created_by"":""sysadmin"",
      ""created_datetime"":""2021-03-18T19:44:33.000+03:00"",
      ""modified_by"":"""",
      ""modified_datetime"":"""",
      ""post_date"":""2021-03-18T19:44:33.000+03:00"",
      ""tender_type"":0,
      ""tender_pos"":1,
      ""amount"":738,
      ""taken"":738,
      ""given"":0,
      ""matched"":false,
      ""eft_res_tender_state"":"""",
      ""manual_name"":"""",
      ""manual_remark"":"""",
      ""currency_sid"":""535145049000100249"",
      ""currency_name"":""RUBLE"",
      ""tender_name"":""Cash"",
      ""alphabetic_code"":""RUB"",
      ""tender_sid"":"""",
      ""check_type"":"""",
      ""check_number"":"""",
      ""company"":"""",
      ""first_name"":"""",
      ""last_name"":"""",
      ""work_phone"":"""",
      ""home_phone"":"""",
      ""state"":"""",
      ""drivers_license"":"""",
      ""drivers_license_expiration"":"""",
      ""date_of_birth"":"""",
      ""authorization_code"":"""",
      ""eft_transaction_id"":"""",
      ""avs_response_code"":"""",
      ""failure_message"":"""",
      ""eftdata1"":"""",
      ""eftdata2"":"""",
      ""eftdata3"":"""",
      ""eftdata4"":"""",
      ""eftdata5"":"""",
      ""eftdata6"":"""",
      ""eftdata7"":"""",
      ""eftdata8"":"""",
      ""eftdata9"":"""",
      ""eftdata0"":"""",
      ""entry_method"":"""",
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
      ""eftdatabsmer"":"""",
      ""eftdatabscust"":"""",
      ""card_number"":"""",
      ""card_type_sid"":"""",
      ""card_holder_name"":"""",
      ""card_expiration_month"":"""",
      ""card_expiration_year"":"""",
      ""is_normal_sale"":"""",
      ""is_present"":"""",
      ""card_postal_code"":"""",
      ""l2_result_code"":"""",
      ""card_type_name"":"""",
      ""emv_ai_applabel"":"""",
      ""emv_ai_aid"":"""",
      ""emv_ci_cardexpirydate"":"""",
      ""emv_crypto_cryptogramtype"":"""",
      ""emv_crypto_cryptogram"":"""",
      ""emv_pinstatement"":"""",
      ""charge_net_days"":"""",
      ""charge_discount_days"":"""",
      ""charge_discount_percent"":"""",
      ""base_taken"":"""",
      ""base_given"":"""",
      ""give_rate"":"""",
      ""take_rate"":"""",
      ""foreign_currency_sid"":"""",
      ""foreign_currency_name"":"""",
      ""foreign_alphabetic_code"":"""",
      ""trace_number"":"""",
      ""internal_reference_number"":"""",
      ""balance"":"""",
      ""certificate_number"":"""",
      ""payment_date"":"""",
      ""payment_remark"":"""",
      ""central_payment_id"":"""",
      ""central_credit_balance"":"""",
      ""central_commit_state"":"""",
      ""redeem_credit_id_10"":"""",
      ""new_credit_id_10"":"""",
      ""new_credit_value"":"""",
      ""new_credit_id"":"""",
      ""redeem_credit_id"":"""",
      ""central_activation_id"":"""",
      ""central_card_number"":""""
    }
  ],
  ""order_total_tax_amt"":0,
  ""transaction_total_tax_amt"":43.41,
  ""sale_tax1_amt"":43.41,
  ""order_tax1_amt"":0,
  ""transaction_tax1_amt"":43.41,
  ""sale_tax2_amt"":"""",
  ""order_tax2_amt"":"""",
  ""transaction_tax2_amt"":"""",
  ""transaction_total_amt"":738,
  ""order_subtotal"":0,
  ""order_subtotal_with_tax"":0,
  ""so_deposit_amt_paid"":0,
  ""transaction_subtotal"":738,
  ""given_amt"":0,
  ""price_lvl"":1,
  ""price_lvl_name"":""Retail"",
  ""discount_reason_name"":"""",
  ""subsidiary_name"":""1"",
  ""store_name"":""Chicago"",
  ""tax_area_percent"":6.25,
  ""tax_area2_percent"":0,
  ""reason_code"":"""",
  ""pos_flag1"":"""",
  ""pos_flag2"":"""",
  ""pos_flag3"":"""",
  ""comment1"":"""",
  ""comment2"":"""",
  ""deposit_amt_taken"":"""",
  ""deposit_ref_doc_sid"":"""",
  ""total_deposit_taken"":"""",
  ""notes_general"":"""",
  ""store_uid"":""535145030000147005"",
  ""ordered_date"":"""",
  ""cancel_date"":"""",
  ""ship_partial"":"""",
  ""ship_priority"":"""",
  ""employee1_sid"":""535145030000146004"",
  ""employee2_sid"":"""",
  ""employee3_sid"":"""",
  ""employee4_sid"":"""",
  ""employee5_sid"":"""",
  ""cashier_sid"":""535145030000146004"",
  ""ref_order_sid"":"""",
  ""order_discount_type"":0,
  ""order_discount_reason_name"":"""",
  ""order_discount_perc"":0,
  ""order_discount_amount"":0,
  ""so_cancel_flag"":false,
  ""total_deposit_used"":"""",
  ""deposit_available_amt"":0,
  ""order_fee_amt1"":"""",
  ""order_fee_tax_amt1"":"""",
  ""order_fee_tax_included1"":"""",
  ""order_fee_tax_perc1"":"""",
  ""order_shipping_amt"":"""",
  ""order_shipping_tax_amt"":"""",
  ""order_shipping_tax_included"":"""",
  ""order_shipping_tax_perc"":"""",
  ""fee_type1_sid"":"""",
  ""shipping_sid"":"""",
  ""order_fee_type1_sid"":"""",
  ""order_shipping_sid"":"""",
  ""order_shipping_percentage"":"""",
  ""fee_amt1_no_tax"":0,
  ""fee_amt1_with_tax"":0,
  ""order_shipping_amt_no_tax"":0,
  ""order_shipping_amt_with_tax"":0,
  ""shipping_amt_no_tax"":0,
  ""shipping_amt_with_tax"":0,
  ""order_document_number"":"""",
  ""used_fee_amt1"":"""",
  ""order_changed_flag"":false,
  ""total_fee_amt_no_tax"":0,
  ""total_fee_tax_amt"":0,
  ""order_balance_due"":0,
  ""original_store_uid"":""535145030000147005"",
  ""has_sale"":true,
  ""has_return"":false,
  ""has_deposit"":false,
  ""receipt_type"":0,
  ""order_type"":"""",
  ""ship_method_id"":"""",
  ""ship_method_sid"":"""",
  ""order_ship_method"":"""",
  ""order_ship_method_id"":"""",
  ""order_ship_method_sid"":"""",
  ""from_centrals"":0,
  ""send_sale_fulfillment"":false,
  ""detax_amount"":0,
  ""order_shipping_amt_manual"":"""",
  ""tax_rebate_persisted"":0,
  ""order_status"":"""",
  ""shipping_amt_manual"":"""",
  ""order_shipping_amt_manual_used"":"""",
  ""order_tracking_number"":"""",
  ""transaction_total_shipping_tax"":0,
  ""transaction_total_shipping_amt_no_tax"":0,
  ""transaction_total_shipping_amt_with_tax"":0,
  ""return_subtotal"":0,
  ""return_total_tax_amt"":0,
  ""order_fulfilling_amt"":0,
  ""tax_rebate_available"":0,
  ""manual_disc_type"":"""",
  ""manual_order_disc_type"":"""",
  ""manual_disc_value"":"""",
  ""manual_order_disc_value"":"""",
  ""manual_disc_reason"":"""",
  ""manual_order_disc_reason"":"""",
  ""coupons"":[],
  ""lty_total_redeem_pgm_name"":"""",
  ""lty_start_balance"":"""",
  ""lty_end_balance"":"""",
  ""lty_redeem_amt"":"""",
  ""fee_amt_returned1"":"""",
  ""lty_gift_amt"":"""",
  ""lty_sale_total_based_disc"":"""",
  ""lty_lvl_sid"":"""",
  ""lty_perc_reward_disc_perc"":"""",
  ""lty_perc_reward_disc_amt"":"""",
  ""lty_total_earn_pgm_name"":"""",
  ""lty_earned_points_positive"":0,
  ""lty_earned_points_negative"":0,
  ""lty_used_points_positive"":0,
  ""lty_used_points_negative"":0,
  ""lty_used_points"":0,
  ""lty_earned_points"":0,
  ""shipping_amt_manual_returned"":"""",
  ""lty_item_earn_pgm_name"":"""",
  ""ref_sale_sid"":"""",
  ""lty_item_redeem_pgm_name"":"""",
  ""lty_item_gift_pgm_name"":"""",
  ""ref_sale_doc_no"":"""",
  ""ref_sale_tax_area_name"":"""",
  ""lty_total_redeem_multiplier"":"""",
  ""lty_total_earn_multiplier"":"""",
  ""lty_balance_for_item_redeem"":0,
  ""lty_lvl_name"":"""",
  ""lty_balance_for_total_redeem"":0,
  ""lty_order_total_based_disc"":"""",
  ""lty_order_earned_points_positive"":"""",
  ""lty_order_earned_points_negative"":"""",
  ""lty_order_used_points_positive"":"""",
  ""lty_order_used_points_negative"":"""",
  ""lty_sale_earned_points_positive"":"""",
  ""lty_sale_earned_points_negative"":"""",
  ""lty_sale_used_points_positive"":"""",
  ""lty_sale_used_points_negative"":"""",
  ""gift_receipt_type"":"""",
  ""order_due_date"":"""",
  ""override_max_disc_perc"":0,
  ""sale_total_tax_amt_rounded"":43.41,
  ""return_total_tax_amt_rounded"":0,
  ""params"":
    {
      ""cols"":""sid,row_version,bt_first_name,bt_last_name,bt_cuid,bt_primary_phone_no,bt_email,bt_id,bt_address_line1,bt_address_line2,bt_address_line3,bt_postal_code,bt_company_name,st_cuid,st_first_name,st_last_name,st_primary_phone_no,st_address_line1,st_address_line2,st_address_line3,st_address_line4,st_address_line5,st_address_line6,st_postal_code,st_country,tender.sid,tender.created_by,tender.created_datetime,tender.modified_by,tender.modified_datetime,tender.post_date,tender.tender_type,tender.tender_pos,tender.amount,tender.taken,tender.given,tender.matched,tender.eft_res_tender_state,tender.manual_name,tender.manual_remark,tender.currency_sid,tender.currency_name,tender.tender_name,tender.alphabetic_code,tender.tender_sid,tender.check_type,tender.check_number,tender.company,tender.first_name,tender.last_name,tender.work_phone,tender.home_phone,tender.state,tender.drivers_license,tender.drivers_license_expiration,tender.date_of_birth,tender.authorization_code,tender.eft_transaction_id,tender.avs_response_code,tender.failure_message,tender.eftdata0,tender.eftdata1,tender.eftdata2,tender.eftdata3,tender.eftdata4,tender.eftdata5,tender.eftdata6,tender.eftdata7,tender.eftdata8,tender.eftdata9,tender.eftdata0,tender.entry_method,tender.eftdata10,tender.eftdata11,tender.eftdata12,tender.eftdata13,tender.eftdata14,tender.eftdata15,tender.eftdata16,tender.eftdata17,tender.eftdata18,tender.eftdata19,tender.eftdatabsmer,tender.eftdatabscust,tender.card_number,tender.card_type_sid,tender.card_holder_name,tender.card_expiration_month,tender.card_expiration_year,tender.is_normal_sale,tender.is_present,tender.card_postal_code,tender.l2_result_code,tender.card_type_name,tender.emv_ai_applabel,tender.emv_ai_aid,tender.emv_ci_cardexpirydate,tender.emv_crypto_cryptogramtype,tender.emv_crypto_cryptogram,tender.emv_pinstatement,tender.charge_net_days,tender.charge_discount_days,tender.charge_discount_percent,tender.base_taken,tender.base_given,tender.give_rate,tender.take_rate,tender.foreign_currency_sid,tender.foreign_currency_name,tender.foreign_alphabetic_code,tender.trace_number,tender.internal_reference_number,tender.balance,tender.certificate_number,tender.payment_date,tender.payment_remark,tender.central_payment_id,tender.central_credit_balance,tender.central_commit_state,tender.redeem_credit_id_10,tender.new_credit_id_10,tender.new_credit_value,tender.new_credit_id,tender.redeem_credit_id,tender.central_activation_id,tender.central_card_number,eft_invoice_number,gift_receipt_type,cashier_sid,cashier_login_name,employee1_sid,employee1_login_name,activity4_percent,employee2_sid,employee5_sid,activity2_percent,activity5_percent,employee3_sid,activity3_percent,sold_qty,transaction_subtotal,store_uid,reason_code,comment1,comment2,created_datetime,subsidiary_number,subsidiary_name,price_lvl_name,store_number,store_code,store_name,notes_general,price_lvl,from_centrals,created_by,status,taken_amt,invoice_posted_date,given_amt,original_store_uid,transaction_total_amt,rounding_offset,document_number,is_held,receipt_type,has_sale,has_return,has_deposit,employee4_sid,activity_percent,send_sale_fulfillment,pos_flag1,pos_flag2,pos_flag3,return_qty,transaction_total_shipping_amt_no_tax,transaction_total_shipping_amt_with_tax,transaction_total_shipping_tax,sale_subtotal,sale_total_tax_amt_rounded,sale_total_tax_amt,manual_disc_value,manual_disc_type,manual_disc_reason,override_max_disc_perc,total_discount_amt,discount_perc,discount_amount,discount_reason_name,total_fee_amt,used_fee_amt1,fee_type1_sid,fee_amt1,fee_name1,fee_tax_amt1,fee_tax_perc1,fee_amt1_no_tax,fee_amt1_with_tax,fee_tax_included1,total_fee_amt_no_tax,total_fee_tax_amt,transaction_total_tax_amt,tax_rebate_available,tax_rebate_persisted,due_amt,tax_area_name,tax_area2_name,tax_area_percent,tax_area2_percent,transaction_tax1_amt,transaction_tax2_amt,sale_tax1_amt,sale_tax2_amt,tax_rebate_percent,tax_rebate_amt,detax_flag,detax_amount,sale_total_tax_amt_rounded,sale_total_tax_amt,ref_sale_doc_no,ref_sale_tax_area_name,ref_sale_created_datetimeship_date,ship_method,ship_method_id,ship_method_sid,ship_partial,ship_priority,shipping_amt,shipping_amt_no_tax,shipping_amt_with_tax,shipping_percentage,shipping_sid,shipping_tax_amt,shipping_tax_included,shipping_tax_percentage,shipping_amt_manual,tracking_number,order_document_number,order_qty,order_fulfilling_amt,order_subtotal,order_changed_flag,order_balance_due,order_type,total_deposit_taken,total_deposit_used,order_subtotal_with_tax,deposit_ref_doc_sid,so_deposit_amt_paid,order_status,deposit_amt_required,deposit_available_amt,deposit_amt_taken,ref_order_sid,so_cancel_flag,order_total_tax_amt,ordered_date,cancel_date,order_due_date,manual_order_disc_value,manual_order_disc_type,manual_order_disc_reason,order_discount_perc,order_discount_reason_name,order_discount_type,order_discount_amount,order_fee_tax_perc1,order_fee_tax_included1,order_fee_amt1,order_fee_type1_sid,order_ship_method_id,order_ship_method,order_ship_method_sid,order_shipping_amt,order_tracking_number,order_shipping_amt_no_tax,order_shipping_amt_with_tax,order_shipping_percentage,order_shipping_sid,order_shipping_tax_amt,order_shipping_tax_included,order_shipping_tax_perc,order_shipping_amt_manual,order_shipping_amt_manual_used,order_tax1_amt,order_tax2_amt,order_fee_tax_amt1,return_subtotal,return_total_tax_amt_rounded,return_total_tax_amt,fee_amt_returned1,shipping_amt_manual_returned,ref_sale_sid,lty_start_balance,lty_earned_points,lty_earned_points_positive,lty_earned_points_negative,lty_balance_for_item_redeem,lty_balance_for_total_redeem,lty_used_points,lty_used_points_positive,lty_used_points_negative,lty_end_balance,lty_total_based_disc,lty_perc_reward_disc_amt,lty_gift_amt,lty_redeem_amt,lty_total_earn_pgm_name,lty_lvl_name,lty_lvl_sid,lty_available_balance,lty_item_earn_pgm_name,lty_total_redeem_pgm_name,lty_item_redeem_pgm_name,lty_perc_reward_disc_perc,lty_item_gift_pgm_name,lty_total_earn_multiplier,lty_total_redeem_multiplier,coupon.sid,"",
      ""sid"":""592070915000115108""
    },
  ""headers"":
    {
      ""accept"":""application/xml,application/json"",
      ""accept-charset"":""utf-8, iso-8859-1, iso-8859-5, unicode-1-1;q=0.8"",
      ""access-control-allow-headers"":""accept, origin, auth-session, auth-session-v9, content-type, auth-nonce, auth-nonce-response"",
      ""access-control-allow-methods"":""GET, POST, PUT, DELETE"",
      ""access-control-allow-origin"":""*"",
      ""access-control-expose-headers"":""auth-nonce, auth-session, auth-session-v9"",
      ""allow"":""DELETE,GET,HEAD,OPTIONS,POST,PUT"",
      ""auto-delete"":""true"",
      ""cache-control"":""NO-CACHE"",
      ""connection"":""Keep-Alive"",
      ""content-length"":""7941"",
      ""content-type"":""text/json; charset=UTF-8"",
      ""date"":""Thu, 18 Mar 2021 16:45:35 GMT"",
      ""expiration"":""10000"",
      ""http-status-code"":""200"",
      ""keep-alive"":""timeout=5, max=100"",
      ""reqduration"":""D=158009"",
      ""server"":""Apache/2.4.27 (Win32)"",
      ""tid"":""YFODr6n @g2QAAJogI@IAAAA5"",
      ""x-powered-by"":""Delphi"",
      ""x-remote-server"":""demo2"",
      ""x-ua-compatible"":""IE=edge""
    },
  ""items"":
    [
      {
        ""resource"":""item"",
        ""endpoint"":""document/:document_sid/item/:sid"",
        ""dirty"":{},
        ""origin_application"":""RProPrismWeb"",
        ""link"":""/v1/rest/document/592070915000115108/item/592070922000121109"",
        ""sid"":""592070922000121109"",
        ""created_by"":""sysadmin"",
        ""created_datetime"":""2021-03-18T19:42:02.000+03:00"",
        ""modified_by"":""sysadmin"",
        ""modified_datetime"":""2021-03-18T19:42:46.000+03:00"",
        ""controller_sid"":""535145030000008255"",
        ""post_date"":""2021-03-18T19:42:02.000+03:00"",
        ""row_version"":1889918431,
        ""tenant_sid"":""535145030000135000"",
        ""document_sid"":""592070915000115108"",
        ""item_pos"":1,
        ""quantity"":2,
        ""original_price"":369,
        ""original_tax_amount"":21.7059,
        ""price"":369,
        ""tax_percent"":6.25,
        ""tax_amount"":21.70588,
        ""tax2_percent"":0,
        ""tax2_amount"":0,
        ""detax_flag"":false,
        ""price_before_detax"":"""",
        ""original_price_before_detax"":"""",
        ""cost"":184.5,
        ""spif"":0,
        ""schedule_number"":"""",
        ""scan_upc"":""1212"",
        ""serial_number"":"""",
        ""kit_flag"":0,
        ""package_item_sid"":"""",
        ""original_component_item_sid"":"""",
        ""user_discount_percent"":"""",
        ""package_sequence_number"":"""",
        ""lot_number"":"""",
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
        ""original_cost"":184.5,
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
        ""note1"":""010290000066650421Jsid2E\""4oh2>T91002a92/1tPzrragHUbOA+cq0FIp54OZZF6GcVCJhA9W6Mnb7W6LvZEn9r9thrj+HsBFpqyH/zl5Ri6pXxF3HTwjuWeKG=="",
        ""note2"":""010290000066650421Jsid2E\""4oh2>T91002a92/1tPzrragHUbOA+cq0FIp54OZZF6GcVCJhA9W6Mnb7W6LvZEn9r9thrj+HsBFpqyH/zl5Ri6pXxF3HTwjuWeKG=="",
        ""note3"":"""",
        ""note4"":"""",
        ""note5"":"""",
        ""note6"":"""",
        ""note7"":"""",
        ""note8"":"""",
        ""note9"":"""",
        ""note10"":"""",
        ""total_discount_amount"":0,
        ""item_type"":1,
        ""dcs_code"":""MENSHOSNE"",
        ""vendor_code"":""JS"",
        ""item_description1"":""DIEMME VENETO LOW TOPS - MENS"",
        ""item_description2"":""SMDI8157"",
        ""item_description3"":"""",
        ""item_description4"":"""",
        ""attribute"":""GREEN"",
        ""item_size"":""9"",
        ""alu"":""SHOE"",
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
        ""employee1_full_name"":""Sysadmin Sysadmin"",
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
        ""inventory_on_hand_quantity"":-79,
        ""inventory_quantity_per_case"":"""",
        ""subsidiary_number"":1,
        ""discount_amt"":0,
        ""discount_perc"":0,
        ""discount_reason"":"""",
        ""returned_item_qty"":"""",
        ""returned_item_invoice_sid"":"""",
        ""delete_discount"":"""",
        ""invn_sbs_item_sid"":""535151649000156102"",
        ""custom_flag"":"""",
        ""return_reason"":"""",
        ""price_lvl"":1,
        ""order_quantity_filled"":0,
        ""gift_quantity"":0,
        ""so_deposit_amt"":0,
        ""invn_item_uid"":""38908"",
        ""ref_order_item_sid"":"""",
        ""tax_area_name"":""ILLINOIS"",
        ""tax_area2_name"":"""",
        ""serial_type"":0,
        ""lot_type"":0,
        ""price_lvl_sid"":""535145053000116190"",
        ""price_lvl_name"":""Retail"",
        ""so_cancel_flag"":"""",
        ""ref_sale_doc_sid"":"""",
        ""fulfill_store_sid"":""535145030000147005"",
        ""fulfill_store_no"":1,
        ""fulfill_store_sbs_no"":1,
        ""central_document_sid"":"""",
        ""central_item_pos"":"""",
        ""ref_order_doc_sid"":"""",
        ""employee1_sid"":""535145030000146004"",
        ""employee2_sid"":"""",
        ""employee3_sid"":"""",
        ""employee4_sid"":"""",
        ""employee5_sid"":"""",
        ""employee1_id"":""1"",
        ""employee2_id"":"""",
        ""employee3_id"":"""",
        ""employee4_id"":"""",
        ""employee5_id"":"""",
        ""dip_discount_amt"":0,
        ""dip_price"":369,
        ""dip_tax_amount"":21.70588,
        ""dip_tax2_amount"":0,
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
        ""tax_code_rule_sid"":""535151613000198036"",
        ""tax_code_rule2_sid"":"""",
        ""st_address_uid"":"""",
        ""style_sid"":""-1238070183164763463"",
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
        ""kit_type"":0,
        ""activation_sid"":"""",
        ""gift_expire_date"":"""",
        ""maxdiscpercent"":100,
        ""maxaccumdiscpercent"":100,
        ""override_max_disc_perc"":"""",
        ""promo_gift_item"":0,
        ""hrefs"":
          {
            ""controller_sid"":""/v1/rest/controller/535145030000008255""
          },
        ""params"":
          {
            ""cols"":""lty_orig_points_earned,note4,item_path,discount_reason,is_competing_component,activity2_percent,ship_method_id,order_ship_method_id,employee3_name,item_origin,employee1_sid,employee3_full_name,central_return_commit_state,so_cancel_flag,employee2_login_name,inventory_item_type,sid,item_description3,note8,item_description1,lty_piece_of_tbe_points,hist_discount_perc5,cost,udf_string04,st_address_line3,package_sequence_number,invn_sbs_item_sid,item_status,qty_available_for_return,discounts,original_cost,st_primary,hist_discount_reason4,gift_add_value,note3,lot_type,ref_order_item_sid,gift_transaction_id,hist_discount_amt1,employee1_full_name,total_discount_reason,order_quantity_filled,st_company_name,post_date,ship_method,commission2_amount,inventory_on_hand_quantity,tax_perc_lock,dcs_code,activity3_percent,price,employee1_login_name,central_document_sid,order_ship_method_sid,employee3_login_name,tax2_amount,st_address_line2,fulfill_store_sbs_no,employee5_sid,returned_item_invoice_sid,udf_string01,udf_float01,price_before_detax,ship_id,hist_discount_reason5,employee2_name,commission_code,spif,price_lvl_name,style_sid,commission3_amount,dip_discount_amt,hist_discount_perc2,st_postal_code_extension,employee3_id,commission_percent,item_pos,st_id,activity4_percent,returned_item_qty,detax_flag,custom_flag,st_price_lvl,discount_amt,total_discount_amount,row_version,dip_tax_amount,employee5_id,activation_sid,lty_redeem_applicable_manually,order_ship_method,enhanced_item_pos,item_description2,employee5_name,manual_disc_reason,tax_code_rule2_sid,udf_string05,total_discount_percent,tax_amount,package_number,scan_upc,so_number,shipping_amt_bdt,employee3_sid,invn_item_uid,item_type,employee5_full_name,subsidiary_number,note10,tax_code_rule_sid,employee2_sid,customer_field,tax2_percent,udf_date01,st_address_line5,original_component_item_uid,ref_sale_item_pos,manual_disc_value,tax_code2,original_price_before_detax,lty_points_earned,st_last_name,fulfill_store_sid,st_country,udf_string03,st_first_name,tax_code,hist_discount_reason2,employee4_sid,price_lvl,lty_orig_price_in_points,note2,commission_level,item_lookup,vendor_code,commission5_amount,alu,schedule_number,serial_number,promotion_flag,udf_float02,so_deposit_amt,lty_price_in_points,st_security_lvl,ship_method_sid,created_datetime,hist_discount_amt3,inventory_quantity_per_case,inventory_use_quantity_decimals,commission_amount,hist_discount_perc3,st_address_line1,user_discount_percent,central_item_pos,tracking_number,st_title,ref_order_doc_sid,hist_discount_perc4,tenant_sid,kit_flag,kit_type,lot_number,discount_perc,override_max_disc_perc,modified_datetime,udf_float03,original_tax_amount,commission4_amount,hist_discount_amt4,st_detax_flag,st_tax_area_name,orig_document_number,activity5_percent,archived,package_item_uid,lty_price_in_points_ext,item_size,original_price,store_number,lty_pgm_name,order_type,employee4_id,st_price_lvl_name,attribute,lty_piece_of_tbr_points,orig_subsidiary_number,employee1_name,delete_discount,original_component_item_sid,employee2_id,item_description4,serial_type,activity_percent,employee4_login_name,gift_activation_code,gift_expire_date,tax_char,hist_discount_reason3,document_sid,hist_discount_perc1,udf_string02,created_by,ref_sale_doc_sid,shipping_amt,package_item_sid,note5,st_primary_phone_no,return_reason,employee5_login_name,note9,employee1_id,note1,lty_type,price_lvl_sid,st_address_uid,st_address_line6,apply_type_to_all_items,lty_piece_of_tbr_disc_amt,tax_area_name,promo_disc_modifiedmanually,st_cuid,note6,hist_discount_amt5,orig_sale_price,tax_area2_name,dip_price,modified_by,st_postal_code,st_email,orig_store_number,quantity,note7,hist_discount_amt2,gift_quantity,controller_sid,lty_pgm_sid,employee2_full_name,dip_tax2_amount,origin_application,employee4_full_name,employee4_name,st_tax_area2_name,hist_discount_reason1,manual_disc_type,st_address_line4,tax_percent,tax_char2,fulfill_store_no,from_centrals,st_customer_lookup,maxaccumdiscpercent,maxdiscpercent,promo_gift_item"",
            ""sort"":""enhanced_item_pos,desc"",
            ""document_sid"":""592070915000115108""
          },
        ""headers"":
          {
            ""accept"":""application/xml,application/json"",
            ""accept-charset"":""utf-8, iso-8859-1, iso-8859-5, unicode-1-1;q=0.8"",
            ""access-control-allow-headers"":""accept, origin, auth-session, auth-session-v9, content-type, auth-nonce, auth-nonce-response"",
            ""access-control-allow-methods"":""GET, POST, PUT, DELETE"",
            ""access-control-allow-origin"":""*"",
            ""access-control-expose-headers"":""auth-nonce, auth-session, auth-session-v9"",
            ""allow"":""DELETE,GET,HEAD,OPTIONS,POST,PUT"",
            ""auto-delete"":""true"",
            ""cache-control"":""NO-CACHE"",
            ""connection"":""Keep-Alive"",
            ""content-length"":""6192"",
            ""content-type"":""text/json; charset=UTF-8"",
            ""contentrange"":""-"",
            ""date"":""Thu, 18 Mar 2021 16:45:35 GMT"",
            ""expiration"":""10000"",
            ""http-status-code"":""200"",
            ""keep-alive"":""timeout=5, max=100"",
            ""reqduration"":""D=98006"",
            ""server"":""Apache/2.4.27 (Win32)"",
            ""tid"":""YFODr6n @g2QAAJogI@MAAAA5"",
            ""x-powered-by"":""Delphi"",
            ""x-remote-server"":""demo2"",
            ""x-ua-compatible"":""IE=edge""
          }
      }
    ],
  ""doc_tender_type"":0
}";
/*
 {
  "resource":"document",
  "endpoint":"document/:sid",
  "dirty":{},
  "origin_application":"RProPrismWeb","link":"/v1/rest/document/592070915000115108","sid":"592070915000115108","created_by":"sysadmin","created_datetime":"2021-03-18T19:41:55.000+03:00","row_version":9,"document_number":231,"invoice_posted_date":"2021-03-18T19:45:36.000+03:00","status":4,"tracking_number":"","discount_amount":0,"discount_perc":0,"tax_rebate_percent":"","tax_rebate_amt":0,"rounding_offset":0,"is_held":false,"activity_percent":100,"activity2_percent":"","activity3_percent":"","activity4_percent":"","activity5_percent":"","eft_invoice_number":605,"detax_flag":false,"shipping_percentage":"","shipping_amt":"","shipping_tax_included":"","shipping_tax_percentage":"","shipping_tax_amt":"","ship_method":"","total_fee_amt":0,"total_discount_amt":0,"sale_total_tax_amt":43.41,"sale_subtotal":738,"deposit_amt_required":0,"due_amt":0,"sold_qty":2,"return_qty":0,"order_qty":0,"taken_amt":738,"subsidiary_number":1,"store_number":1,"store_code":"ORD","tax_area_name":"ILLINOIS","tax_area2_name":"","employee1_login_name":"sysadmin","cashier_login_name":"sysadmin","fee_name1":"","fee_amt1":"","fee_tax_included1":"","fee_tax_amt1":"","fee_tax_perc1":"","bt_cuid":"","bt_id":"","bt_last_name":"","bt_first_name":"","bt_company_name":"","bt_primary_phone_no":"","bt_address_line1":"","bt_address_line2":"","bt_address_line3":"","bt_postal_code":"","bt_email":"","st_cuid":"","st_last_name":"","st_first_name":"","st_primary_phone_no":"","st_address_line1":"","st_address_line2":"","st_address_line3":"","st_address_line4":"","st_address_line5":"","st_address_line6":"","st_country":"","st_postal_code":"","tenders":[{"link":"/v1/rest/document/592070915000115108/tender/592071073000171112","sid":"592071073000171112","created_by":"sysadmin","created_datetime":"2021-03-18T19:44:33.000+03:00","modified_by":"","modified_datetime":"","post_date":"2021-03-18T19:44:33.000+03:00","tender_type":0,"tender_pos":1,"amount":738,"taken":738,"given":0,"matched":false,"eft_res_tender_state":"","manual_name":"","manual_remark":"","currency_sid":"535145049000100249","currency_name":"RUBLE","tender_name":"Cash","alphabetic_code":"RUB","tender_sid":"","check_type":"","check_number":"","company":"","first_name":"","last_name":"","work_phone":"","home_phone":"","state":"","drivers_license":"","drivers_license_expiration":"","date_of_birth":"","authorization_code":"","eft_transaction_id":"","avs_response_code":"","failure_message":"","eftdata1":"","eftdata2":"","eftdata3":"","eftdata4":"","eftdata5":"","eftdata6":"","eftdata7":"","eftdata8":"","eftdata9":"","eftdata0":"","entry_method":"","eftdata10":"","eftdata11":"","eftdata12":"","eftdata13":"","eftdata14":"","eftdata15":"","eftdata16":"","eftdata17":"","eftdata18":"","eftdata19":"","eftdatabsmer":"","eftdatabscust":"","card_number":"","card_type_sid":"","card_holder_name":"","card_expiration_month":"","card_expiration_year":"","is_normal_sale":"","is_present":"","card_postal_code":"","l2_result_code":"","card_type_name":"","emv_ai_applabel":"","emv_ai_aid":"","emv_ci_cardexpirydate":"","emv_crypto_cryptogramtype":"","emv_crypto_cryptogram":"","emv_pinstatement":"","charge_net_days":"","charge_discount_days":"","charge_discount_percent":"","base_taken":"","base_given":"","give_rate":"","take_rate":"","foreign_currency_sid":"","foreign_currency_name":"","foreign_alphabetic_code":"","trace_number":"","internal_reference_number":"","balance":"","certificate_number":"","payment_date":"","payment_remark":"","central_payment_id":"","central_credit_balance":"","central_commit_state":"","redeem_credit_id_10":"","new_credit_id_10":"","new_credit_value":"","new_credit_id":"","redeem_credit_id":"","central_activation_id":"","central_card_number":""}],"order_total_tax_amt":0,"transaction_total_tax_amt":43.41,"sale_tax1_amt":43.41,"order_tax1_amt":0,"transaction_tax1_amt":43.41,"sale_tax2_amt":"","order_tax2_amt":"","transaction_tax2_amt":"","transaction_total_amt":738,"order_subtotal":0,"order_subtotal_with_tax":0,"so_deposit_amt_paid":0,"transaction_subtotal":738,"given_amt":0,"price_lvl":1,"price_lvl_name":"Retail","discount_reason_name":"","subsidiary_name":"1","store_name":"Chicago","tax_area_percent":6.25,"tax_area2_percent":0,"reason_code":"","pos_flag1":"","pos_flag2":"","pos_flag3":"","comment1":"","comment2":"","deposit_amt_taken":"","deposit_ref_doc_sid":"","total_deposit_taken":"","notes_general":"","store_uid":"535145030000147005","ordered_date":"","cancel_date":"","ship_partial":"","ship_priority":"","employee1_sid":"535145030000146004","employee2_sid":"","employee3_sid":"","employee4_sid":"","employee5_sid":"","cashier_sid":"535145030000146004","ref_order_sid":"","order_discount_type":0,"order_discount_reason_name":"","order_discount_perc":0,"order_discount_amount":0,"so_cancel_flag":false,"total_deposit_used":"","deposit_available_amt":0,"order_fee_amt1":"","order_fee_tax_amt1":"","order_fee_tax_included1":"","order_fee_tax_perc1":"","order_shipping_amt":"","order_shipping_tax_amt":"","order_shipping_tax_included":"","order_shipping_tax_perc":"","fee_type1_sid":"","shipping_sid":"","order_fee_type1_sid":"","order_shipping_sid":"","order_shipping_percentage":"","fee_amt1_no_tax":0,"fee_amt1_with_tax":0,"order_shipping_amt_no_tax":0,"order_shipping_amt_with_tax":0,"shipping_amt_no_tax":0,"shipping_amt_with_tax":0,"order_document_number":"","used_fee_amt1":"","order_changed_flag":false,"total_fee_amt_no_tax":0,"total_fee_tax_amt":0,"order_balance_due":0,"original_store_uid":"535145030000147005","has_sale":true,"has_return":false,"has_deposit":false,"receipt_type":0,"order_type":"","ship_method_id":"","ship_method_sid":"","order_ship_method":"","order_ship_method_id":"","order_ship_method_sid":"","from_centrals":0,"send_sale_fulfillment":false,"detax_amount":0,"order_shipping_amt_manual":"","tax_rebate_persisted":0,"order_status":"","shipping_amt_manual":"","order_shipping_amt_manual_used":"","order_tracking_number":"","transaction_total_shipping_tax":0,"transaction_total_shipping_amt_no_tax":0,"transaction_total_shipping_amt_with_tax":0,"return_subtotal":0,"return_total_tax_amt":0,"order_fulfilling_amt":0,"tax_rebate_available":0,"manual_disc_type":"","manual_order_disc_type":"","manual_disc_value":"","manual_order_disc_value":"","manual_disc_reason":"","manual_order_disc_reason":"","coupons":[],"lty_total_redeem_pgm_name":"","lty_start_balance":"","lty_end_balance":"","lty_redeem_amt":"","fee_amt_returned1":"","lty_gift_amt":"","lty_sale_total_based_disc":"","lty_lvl_sid":"","lty_perc_reward_disc_perc":"","lty_perc_reward_disc_amt":"","lty_total_earn_pgm_name":"","lty_earned_points_positive":0,"lty_earned_points_negative":0,"lty_used_points_positive":0,"lty_used_points_negative":0,"lty_used_points":0,"lty_earned_points":0,"shipping_amt_manual_returned":"","lty_item_earn_pgm_name":"","ref_sale_sid":"","lty_item_redeem_pgm_name":"","lty_item_gift_pgm_name":"","ref_sale_doc_no":"","ref_sale_tax_area_name":"","lty_total_redeem_multiplier":"","lty_total_earn_multiplier":"","lty_balance_for_item_redeem":0,"lty_lvl_name":"","lty_balance_for_total_redeem":0,"lty_order_total_based_disc":"","lty_order_earned_points_positive":"","lty_order_earned_points_negative":"","lty_order_used_points_positive":"","lty_order_used_points_negative":"","lty_sale_earned_points_positive":"","lty_sale_earned_points_negative":"","lty_sale_used_points_positive":"","lty_sale_used_points_negative":"","gift_receipt_type":"","order_due_date":"","override_max_disc_perc":0,"sale_total_tax_amt_rounded":43.41,"return_total_tax_amt_rounded":0,"params":{"cols":"sid,row_version,bt_first_name,bt_last_name,bt_cuid,bt_primary_phone_no,bt_email,bt_id,bt_address_line1,bt_address_line2,bt_address_line3,bt_postal_code,bt_company_name,st_cuid,st_first_name,st_last_name,st_primary_phone_no,st_address_line1,st_address_line2,st_address_line3,st_address_line4,st_address_line5,st_address_line6,st_postal_code,st_country,tender.sid,tender.created_by,tender.created_datetime,tender.modified_by,tender.modified_datetime,tender.post_date,tender.tender_type,tender.tender_pos,tender.amount,tender.taken,tender.given,tender.matched,tender.eft_res_tender_state,tender.manual_name,tender.manual_remark,tender.currency_sid,tender.currency_name,tender.tender_name,tender.alphabetic_code,tender.tender_sid,tender.check_type,tender.check_number,tender.company,tender.first_name,tender.last_name,tender.work_phone,tender.home_phone,tender.state,tender.drivers_license,tender.drivers_license_expiration,tender.date_of_birth,tender.authorization_code,tender.eft_transaction_id,tender.avs_response_code,tender.failure_message,tender.eftdata0,tender.eftdata1,tender.eftdata2,tender.eftdata3,tender.eftdata4,tender.eftdata5,tender.eftdata6,tender.eftdata7,tender.eftdata8,tender.eftdata9,tender.eftdata0,tender.entry_method,tender.eftdata10,tender.eftdata11,tender.eftdata12,tender.eftdata13,tender.eftdata14,tender.eftdata15,tender.eftdata16,tender.eftdata17,tender.eftdata18,tender.eftdata19,tender.eftdatabsmer,tender.eftdatabscust,tender.card_number,tender.card_type_sid,tender.card_holder_name,tender.card_expiration_month,tender.card_expiration_year,tender.is_normal_sale,tender.is_present,tender.card_postal_code,tender.l2_result_code,tender.card_type_name,tender.emv_ai_applabel,tender.emv_ai_aid,tender.emv_ci_cardexpirydate,tender.emv_crypto_cryptogramtype,tender.emv_crypto_cryptogram,tender.emv_pinstatement,tender.charge_net_days,tender.charge_discount_days,tender.charge_discount_percent,tender.base_taken,tender.base_given,tender.give_rate,tender.take_rate,tender.foreign_currency_sid,tender.foreign_currency_name,tender.foreign_alphabetic_code,tender.trace_number,tender.internal_reference_number,tender.balance,tender.certificate_number,tender.payment_date,tender.payment_remark,tender.central_payment_id,tender.central_credit_balance,tender.central_commit_state,tender.redeem_credit_id_10,tender.new_credit_id_10,tender.new_credit_value,tender.new_credit_id,tender.redeem_credit_id,tender.central_activation_id,tender.central_card_number,eft_invoice_number,gift_receipt_type,cashier_sid,cashier_login_name,employee1_sid,employee1_login_name,activity4_percent,employee2_sid,employee5_sid,activity2_percent,activity5_percent,employee3_sid,activity3_percent,sold_qty,transaction_subtotal,store_uid,reason_code,comment1,comment2,created_datetime,subsidiary_number,subsidiary_name,price_lvl_name,store_number,store_code,store_name,notes_general,price_lvl,from_centrals,created_by,status,taken_amt,invoice_posted_date,given_amt,original_store_uid,transaction_total_amt,rounding_offset,document_number,is_held,receipt_type,has_sale,has_return,has_deposit,employee4_sid,activity_percent,send_sale_fulfillment,pos_flag1,pos_flag2,pos_flag3,return_qty,transaction_total_shipping_amt_no_tax,transaction_total_shipping_amt_with_tax,transaction_total_shipping_tax,sale_subtotal,sale_total_tax_amt_rounded,sale_total_tax_amt,manual_disc_value,manual_disc_type,manual_disc_reason,override_max_disc_perc,total_discount_amt,discount_perc,discount_amount,discount_reason_name,total_fee_amt,used_fee_amt1,fee_type1_sid,fee_amt1,fee_name1,fee_tax_amt1,fee_tax_perc1,fee_amt1_no_tax,fee_amt1_with_tax,fee_tax_included1,total_fee_amt_no_tax,total_fee_tax_amt,transaction_total_tax_amt,tax_rebate_available,tax_rebate_persisted,due_amt,tax_area_name,tax_area2_name,tax_area_percent,tax_area2_percent,transaction_tax1_amt,transaction_tax2_amt,sale_tax1_amt,sale_tax2_amt,tax_rebate_percent,tax_rebate_amt,detax_flag,detax_amount,sale_total_tax_amt_rounded,sale_total_tax_amt,ref_sale_doc_no,ref_sale_tax_area_name,ref_sale_created_datetimeship_date,ship_method,ship_method_id,ship_method_sid,ship_partial,ship_priority,shipping_amt,shipping_amt_no_tax,shipping_amt_with_tax,shipping_percentage,shipping_sid,shipping_tax_amt,shipping_tax_included,shipping_tax_percentage,shipping_amt_manual,tracking_number,order_document_number,order_qty,order_fulfilling_amt,order_subtotal,order_changed_flag,order_balance_due,order_type,total_deposit_taken,total_deposit_used,order_subtotal_with_tax,deposit_ref_doc_sid,so_deposit_amt_paid,order_status,deposit_amt_required,deposit_available_amt,deposit_amt_taken,ref_order_sid,so_cancel_flag,order_total_tax_amt,ordered_date,cancel_date,order_due_date,manual_order_disc_value,manual_order_disc_type,manual_order_disc_reason,order_discount_perc,order_discount_reason_name,order_discount_type,order_discount_amount,order_fee_tax_perc1,order_fee_tax_included1,order_fee_amt1,order_fee_type1_sid,order_ship_method_id,order_ship_method,order_ship_method_sid,order_shipping_amt,order_tracking_number,order_shipping_amt_no_tax,order_shipping_amt_with_tax,order_shipping_percentage,order_shipping_sid,order_shipping_tax_amt,order_shipping_tax_included,order_shipping_tax_perc,order_shipping_amt_manual,order_shipping_amt_manual_used,order_tax1_amt,order_tax2_amt,order_fee_tax_amt1,return_subtotal,return_total_tax_amt_rounded,return_total_tax_amt,fee_amt_returned1,shipping_amt_manual_returned,ref_sale_sid,lty_start_balance,lty_earned_points,lty_earned_points_positive,lty_earned_points_negative,lty_balance_for_item_redeem,lty_balance_for_total_redeem,lty_used_points,lty_used_points_positive,lty_used_points_negative,lty_end_balance,lty_total_based_disc,lty_perc_reward_disc_amt,lty_gift_amt,lty_redeem_amt,lty_total_earn_pgm_name,lty_lvl_name,lty_lvl_sid,lty_available_balance,lty_item_earn_pgm_name,lty_total_redeem_pgm_name,lty_item_redeem_pgm_name,lty_perc_reward_disc_perc,lty_item_gift_pgm_name,lty_total_earn_multiplier,lty_total_redeem_multiplier,coupon.sid,","sid":"592070915000115108"},"headers":{"accept":"application/xml,application/json","accept-charset":"utf-8, iso-8859-1, iso-8859-5, unicode-1-1;q=0.8","access-control-allow-headers":"accept, origin, auth-session, auth-session-v9, content-type, auth-nonce, auth-nonce-response","access-control-allow-methods":"GET, POST, PUT, DELETE","access-control-allow-origin":"*","access-control-expose-headers":"auth-nonce, auth-session, auth-session-v9","allow":"DELETE,GET,HEAD,OPTIONS,POST,PUT","auto-delete":"true","cache-control":"NO-CACHE","connection":"Keep-Alive","content-length":"7941","content-type":"text/json; charset=UTF-8","date":"Thu, 18 Mar 2021 16:45:35 GMT","expiration":"10000","http-status-code":"200","keep-alive":"timeout=5, max=100","reqduration":"D=158009","server":"Apache/2.4.27 (Win32)","tid":"YFODr6n@g2QAAJogI@IAAAA5","x-powered-by":"Delphi","x-remote-server":"demo2","x-ua-compatible":"IE=edge"},"items":[{"resource":"item","endpoint":"document/:document_sid/item/:sid","dirty":{},"origin_application":"RProPrismWeb","link":"/v1/rest/document/592070915000115108/item/592070922000121109","sid":"592070922000121109","created_by":"sysadmin","created_datetime":"2021-03-18T19:42:02.000+03:00","modified_by":"sysadmin","modified_datetime":"2021-03-18T19:42:46.000+03:00","controller_sid":"535145030000008255","post_date":"2021-03-18T19:42:02.000+03:00","row_version":1889918431,"tenant_sid":"535145030000135000","document_sid":"592070915000115108","item_pos":1,"quantity":2,"original_price":369,"original_tax_amount":21.7059,"price":369,"tax_percent":6.25,"tax_amount":21.70588,"tax2_percent":0,"tax2_amount":0,"detax_flag":false,"price_before_detax":"","original_price_before_detax":"","cost":184.5,"spif":0,"schedule_number":"","scan_upc":"1212","serial_number":"","kit_flag":0,"package_item_sid":"","original_component_item_sid":"","user_discount_percent":"","package_sequence_number":"","lot_number":"","activity_percent":100,"activity2_percent":"","activity3_percent":"","activity4_percent":"","activity5_percent":"","commission_amount":"","commission2_amount":"","commission3_amount":"","commission4_amount":"","commission5_amount":"","item_origin":0,"package_number":"","ship_id":"","ship_method":"","shipping_amt":0,"tracking_number":"","original_cost":184.5,"promotion_flag":false,"gift_activation_code":"","gift_transaction_id":"","gift_add_value":false,"customer_field":"","udf_string01":"","udf_string02":"","udf_string03":"","udf_string04":"","udf_string05":"","udf_date01":"","udf_float01":"","udf_float02":0,"udf_float03":0,"archived":false,"total_discount_percent":0,"note1":"123","note2":"12345","note3":"","note4":"","note5":"","note6":"","note7":"","note8":"","note9":"","note10":"","total_discount_amount":0,"item_type":1,"dcs_code":"MENSHOSNE","vendor_code":"JS","item_description1":"DIEMME VENETO LOW TOPS - MENS","item_description2":"SMDI8157","item_description3":"","item_description4":"","attribute":"GREEN","item_size":"9","alu":"SHOE","tax_code":"0","tax_code2":"","commission_code":"","commission_level":"","commission_percent":"","st_cuid":"","st_id":"","st_last_name":"","st_first_name":"","st_company_name":"","st_title":"","st_tax_area_name":"","st_tax_area2_name":"","st_detax_flag":false,"st_price_lvl_name":"","st_price_lvl":"","st_security_lvl":"","st_primary_phone_no":"","st_country":"","st_postal_code":"","st_postal_code_extension":"","st_primary":false,"st_address_line1":"","st_address_line2":"","st_address_line3":"","st_address_line4":"","st_address_line5":"","st_address_line6":"","st_email":"","st_customer_lookup":"","employee1_login_name":"sysadmin","employee1_full_name":"Sysadmin Sysadmin","employee2_login_name":"","employee2_full_name":"","employee3_login_name":"","employee3_full_name":"","employee4_login_name":"","employee4_full_name":"","employee5_login_name":"","employee5_full_name":"","hist_discount_amt1":"","hist_discount_perc1":"","hist_discount_reason1":"","hist_discount_amt2":"","hist_discount_perc2":"","hist_discount_reason2":"","hist_discount_amt3":"","hist_discount_perc3":"","hist_discount_reason3":"","hist_discount_amt4":"","hist_discount_perc4":"","hist_discount_reason4":"","hist_discount_amt5":"","hist_discount_perc5":"","hist_discount_reason5":"","store_number":1,"total_discount_reason":"","order_type":"","so_number":"","item_lookup":"","inventory_item_type":"","inventory_on_hand_quantity":-79,"inventory_quantity_per_case":"","subsidiary_number":1,"discount_amt":0,"discount_perc":0,"discount_reason":"","returned_item_qty":"","returned_item_invoice_sid":"","delete_discount":"","invn_sbs_item_sid":"535151649000156102","custom_flag":"","return_reason":"","price_lvl":1,"order_quantity_filled":0,"gift_quantity":0,"so_deposit_amt":0,"invn_item_uid":"38908","ref_order_item_sid":"","tax_area_name":"ILLINOIS","tax_area2_name":"","serial_type":0,"lot_type":0,"price_lvl_sid":"535145053000116190","price_lvl_name":"Retail","so_cancel_flag":false,"ref_sale_doc_sid":"","fulfill_store_sid":"535145030000147005","fulfill_store_no":1,"fulfill_store_sbs_no":1,"central_document_sid":"","central_item_pos":"","ref_order_doc_sid":"","employee1_sid":"535145030000146004","employee2_sid":"","employee3_sid":"","employee4_sid":"","employee5_sid":"","employee1_id":"1","employee2_id":"","employee3_id":"","employee4_id":"","employee5_id":"","dip_discount_amt":0,"dip_price":369,"dip_tax_amount":21.70588,"dip_tax2_amount":0,"ref_sale_item_pos":"","item_status":0,"enhanced_item_pos":10000,"is_competing_component":"","package_item_uid":"","original_component_item_uid":"","promo_disc_modifiedmanually":false,"ship_method_sid":"","ship_method_id":"","order_ship_method_sid":"","order_ship_method_id":"","order_ship_method":"","from_centrals":0,"tax_perc_lock":false,"employee1_name":"SYSADMIN","employee2_name":"","employee3_name":"","employee4_name":"","employee5_name":"","qty_available_for_return":"","central_return_commit_state":"","inventory_use_quantity_decimals":0,"shipping_amt_bdt":"","tax_code_rule_sid":"535151613000198036","tax_code_rule2_sid":"","st_address_uid":"","style_sid":"-1238070183164763463","discounts":[],"manual_disc_value":"","manual_disc_type":"","manual_disc_reason":"","tax_char":"","tax_char2":"","apply_type_to_all_items":"","lty_pgm_sid":"","lty_pgm_name":"","lty_points_earned":"","lty_orig_points_earned":0,"lty_price_in_points":"","lty_orig_price_in_points":0,"orig_sale_price":"","lty_type":"","lty_redeem_applicable_manually":false,"lty_price_in_points_ext":"","lty_piece_of_tbe_points":"","lty_piece_of_tbr_points":"","lty_piece_of_tbr_disc_amt":"","orig_document_number":"","orig_store_number":"","orig_subsidiary_number":"","kit_type":0,"activation_sid":"","gift_expire_date":"","maxdiscpercent":100,"maxaccumdiscpercent":100,"override_max_disc_perc":"","promo_gift_item":0,"hrefs":{"controller_sid":"/v1/rest/controller/535145030000008255"},"params":{"cols":"lty_orig_points_earned,note4,item_path,discount_reason,is_competing_component,activity2_percent,ship_method_id,order_ship_method_id,employee3_name,item_origin,employee1_sid,employee3_full_name,central_return_commit_state,so_cancel_flag,employee2_login_name,inventory_item_type,sid,item_description3,note8,item_description1,lty_piece_of_tbe_points,hist_discount_perc5,cost,udf_string04,st_address_line3,package_sequence_number,invn_sbs_item_sid,item_status,qty_available_for_return,discounts,original_cost,st_primary,hist_discount_reason4,gift_add_value,note3,lot_type,ref_order_item_sid,gift_transaction_id,hist_discount_amt1,employee1_full_name,total_discount_reason,order_quantity_filled,st_company_name,post_date,ship_method,commission2_amount,inventory_on_hand_quantity,tax_perc_lock,dcs_code,activity3_percent,price,employee1_login_name,central_document_sid,order_ship_method_sid,employee3_login_name,tax2_amount,st_address_line2,fulfill_store_sbs_no,employee5_sid,returned_item_invoice_sid,udf_string01,udf_float01,price_before_detax,ship_id,hist_discount_reason5,employee2_name,commission_code,spif,price_lvl_name,style_sid,commission3_amount,dip_discount_amt,hist_discount_perc2,st_postal_code_extension,employee3_id,commission_percent,item_pos,st_id,activity4_percent,returned_item_qty,detax_flag,custom_flag,st_price_lvl,discount_amt,total_discount_amount,row_version,dip_tax_amount,employee5_id,activation_sid,lty_redeem_applicable_manually,order_ship_method,enhanced_item_pos,item_description2,employee5_name,manual_disc_reason,tax_code_rule2_sid,udf_string05,total_discount_percent,tax_amount,package_number,scan_upc,so_number,shipping_amt_bdt,employee3_sid,invn_item_uid,item_type,employee5_full_name,subsidiary_number,note10,tax_code_rule_sid,employee2_sid,customer_field,tax2_percent,udf_date01,st_address_line5,original_component_item_uid,ref_sale_item_pos,manual_disc_value,tax_code2,original_price_before_detax,lty_points_earned,st_last_name,fulfill_store_sid,st_country,udf_string03,st_first_name,tax_code,hist_discount_reason2,employee4_sid,price_lvl,lty_orig_price_in_points,note2,commission_level,item_lookup,vendor_code,commission5_amount,alu,schedule_number,serial_number,promotion_flag,udf_float02,so_deposit_amt,lty_price_in_points,st_security_lvl,ship_method_sid,created_datetime,hist_discount_amt3,inventory_quantity_per_case,inventory_use_quantity_decimals,commission_amount,hist_discount_perc3,st_address_line1,user_discount_percent,central_item_pos,tracking_number,st_title,ref_order_doc_sid,hist_discount_perc4,tenant_sid,kit_flag,kit_type,lot_number,discount_perc,override_max_disc_perc,modified_datetime,udf_float03,original_tax_amount,commission4_amount,hist_discount_amt4,st_detax_flag,st_tax_area_name,orig_document_number,activity5_percent,archived,package_item_uid,lty_price_in_points_ext,item_size,original_price,store_number,lty_pgm_name,order_type,employee4_id,st_price_lvl_name,attribute,lty_piece_of_tbr_points,orig_subsidiary_number,employee1_name,delete_discount,original_component_item_sid,employee2_id,item_description4,serial_type,activity_percent,employee4_login_name,gift_activation_code,gift_expire_date,tax_char,hist_discount_reason3,document_sid,hist_discount_perc1,udf_string02,created_by,ref_sale_doc_sid,shipping_amt,package_item_sid,note5,st_primary_phone_no,return_reason,employee5_login_name,note9,employee1_id,note1,lty_type,price_lvl_sid,st_address_uid,st_address_line6,apply_type_to_all_items,lty_piece_of_tbr_disc_amt,tax_area_name,promo_disc_modifiedmanually,st_cuid,note6,hist_discount_amt5,orig_sale_price,tax_area2_name,dip_price,modified_by,st_postal_code,st_email,orig_store_number,quantity,note7,hist_discount_amt2,gift_quantity,controller_sid,lty_pgm_sid,employee2_full_name,dip_tax2_amount,origin_application,employee4_full_name,employee4_name,st_tax_area2_name,hist_discount_reason1,manual_disc_type,st_address_line4,tax_percent,tax_char2,fulfill_store_no,from_centrals,st_customer_lookup,maxaccumdiscpercent,maxdiscpercent,promo_gift_item","sort":"enhanced_item_pos,desc","document_sid":"592070915000115108"},"headers":{"accept":"application/xml,application/json","accept-charset":"utf-8, iso-8859-1, iso-8859-5, unicode-1-1;q=0.8","access-control-allow-headers":"accept, origin, auth-session, auth-session-v9, content-type, auth-nonce, auth-nonce-response","access-control-allow-methods":"GET, POST, PUT, DELETE","access-control-allow-origin":"*","access-control-expose-headers":"auth-nonce, auth-session, auth-session-v9","allow":"DELETE,GET,HEAD,OPTIONS,POST,PUT","auto-delete":"true","cache-control":"NO-CACHE","connection":"Keep-Alive","content-length":"6192","content-type":"text/json; charset=UTF-8","contentrange":"-","date":"Thu, 18 Mar 2021 16:45:35 GMT","expiration":"10000","http-status-code":"200","keep-alive":"timeout=5, max=100","reqduration":"D=98006","server":"Apache/2.4.27 (Win32)","tid":"YFODr6n@g2QAAJogI@MAAAA5","x-powered-by":"Delphi","x-remote-server":"demo2","x-ua-compatible":"IE=edge"}}],"doc_tender_type":0}
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
                        args.ErrorContext.OriginalObject.GetType() == typeof(JsonPrintFiscDoc))
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
        public static JsonPrintFiscDoc DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BLL.JsonPrintFiscDoc> (json, BLL.JsonPrintFiscDoc.GetSettings());
        }


    }
}
