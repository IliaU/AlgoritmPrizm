using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace AlgoritmPrizm.BLL
{
    /// <summary>
    /// Позиция в списке документов
    /// </summary>
    public class JsonGetDocumentsData
    {
        /// <summary>
        /// Идентификатор докумнта
        /// </summary>
        public string sid;
        /// <summary>
        /// Кто создал документ
        /// </summary>
        public string createdby;
        /// <summary>
        /// Кто модифицировал
        /// </summary>
        public string modifiedb;
        /// <summary>
        /// Время модификации
        /// </summary>
        public DateTime? modifieddatetime;
        public string controllersid;
        public string originapplication;
        public int? rowversion;
        public int? status;
        public bool usevat;
        public bool wasaudited;
        public bool isheld;
        public bool detaxflag;
        public bool detaxflagmanually;
        public int? archived;
        /// <summary>
        /// Тип оплаты 
        /// 4  - нал 
        /// </summary>
        public int? tendertype;
        /// <summary>
        /// Идентификатор кассы в магазине SBS
        /// </summary>
        public int? sbsno;
        /// <summary>
        /// Магазин
        /// </summary>
        public int? storeno;
        public bool btprimary;
        public bool stprimary;
        public string tenantsid;
        public int? docno;
        public int? docrefno;
        public int? trackingno;
        public int? vatoptions;
        public int? custpono;
        public double? discperc;
        public double? discamt;
        public double? taxrebateperc;
        public double? taxrebateamt;
        public double? overtaxperc;
        public double? overtaxperc2;
        public int? roundingoffset;
        public int? wsseqno;
        public string custfield;
        public int drawerno;
        public DateTime? elapsedtime;
        public double activityperc;
        public double? activity2perc;
        public double? activity3perc;
        public double? activity4perc;
        public double? activity5perc;
        public int? eftinvcno;
        public double? shippingperc;
        public string discpercspread;
        public int? fiscaldocno;
        public string udf1string;
        public string udf2string;
        public string udf3string;
        public string udf4string;
        public string udf5string;
        public bool? udf1float;
        public bool? udf2float;
        public bool? udf3float;
        public bool? udf4float;
        public bool? udf5float;
        public string udfclob;
        public double? totalfeeamt;
        public double? totaldiscountamt;
        public double? saletotaltaxamt;
        public double? saletotalamt;
        public double? salesubtotal;
        public double? depositamtrequired;
        public double? dueamt;
        public int? soldqty;
        public int? returnqty;
        public int? orderqty;
        public int? orderquantityfilled;
        public double? takenamt;
        public string storecode;
        public int? origstoreno;
        public string origstorecode;
        public string taxareaname;
        public string taxarea2name;
        public string discountreasonname;
        public string disbursementreasonname;
        public int workstationno;
        /// <summary>
        /// Логин кассира
        /// </summary>
        public string cashierloginname;
        /// <summary>
        /// Полное имя кассира
        /// </summary>
        public string cashierfullname;
        /// <summary>
        /// Логин сотрудника
        /// </summary>
        public string employee1loginname;
        /// <summary>
        /// Полное имя сотрудника
        /// </summary>
        public string employee1fullname;
        /// <summary>
        /// Логин сотрудника 2
        /// </summary>
        public string employee2loginname;
        /// <summary>
        /// Полное имя сотрудника 2
        /// </summary>
        public string employee2fullname;
        /// <summary>
        /// Логин сотрудника 3
        /// </summary>
        public string employee3loginname;
        /// <summary>
        /// Полное имя сотрудника 3
        /// </summary>
        public string employee3fullname;
        /// <summary>
        /// Логин сотрудника 4
        /// </summary>
        public string employee4loginname;
        /// <summary>
        /// Полное имя сотрудника 4
        /// </summary>
        public string employee4fullname;
        /// <summary>
        /// Логин сотрудника 5
        /// </summary>
        public string employee5loginname;
        /// <summary>
        /// Полное имя сотрудника 5
        /// </summary>
        public string employee5fullname;
        public string btcuid;
        public int? btid;
        public string btlastname;
        public string btfirstname;
        public string btcompanyname;
        public string bttitle;
        public string bttaxareaname;
        public string bttaxarea2name;
        public bool btdetaxflag;
        public string btpricelvlname;
        public string btpricelvl;
        public string btsecuritylvl;
        public int? btprimaryphoneno;
        public string btaddressline1;
        public string btaddressline2;
        public string btaddressline3;
        public string btaddressline4;
        public string btaddressline5;
        public string btaddressline6;
        public string btcountry;
        public string btpostalcode;
        public string btpostalcodeextension;
        public string staddressline1;
        public string staddressline2;
        public string staddressline3;
        public string staddressline4;
        public string staddressline5;
        public string staddressline6;
        public string stcountry;
        public string stpostalcode;
        public string stpostalcodeextension;
        public int? stcuid;
        public int? stid;
        public string stlastname;
        public string stfirstname;
        public string sttitle;
        public string sttaxareaname;
        public string sttaxarea2name;
        public bool stdetaxflag;
        public string stpricelvlname;
        public string stpricelvl;
        public string stsecuritylvl;
        public int? stprimaryphoneno;
        public string stcompanyname;
        public string feetype1;
        public string feename1;
        public string feetype2;
        public string feename2;
        public string feetype3;
        public string feename3;
        public string feetype4;
        public string feename4;
        public string feetype5;
        public string feename5;
        public string tendername;
        public string currencyname;
        public string tillname;
        public string btemail;
        public string stemail;
        public double? histdiscamt1;
        public double? histdiscperc1;
        public string histdiscreason1;
        public double? histdiscamt2;
        public double? histdiscperc2;
        public string histdiscreason2;
        public double? histdiscamt3;
        public double? histdiscperc3;
        public string histdiscreason3;
        public double? histdiscamt4;
        public double? histdiscperc4;
        public string histdiscreason4;
        public double? histdiscamt5;
        public double? histdiscperc5;
        public string histdiscreason5;
        public DateTime createddatetime;
        public DateTime? postdate;
        public DateTime? udf1date;
        public DateTime? udf2date;
        public DateTime? udf3date;
        public DateTime? invcpostdate;
        public bool feetaxincluded1;
        public double? feetaxperc1;
        public bool feetaxincluded2;
        public double? feetaxperc2;
        public bool feetaxincluded3;
        public double? feetaxperc3;
        public bool feetaxincluded4;
        public double? feetaxperc4;
        public bool feetaxincluded5;
        public double? feetaxperc5;
        public int pricelvl;
        public string pricelvlname;
        public double? shippingamt;
        public double? shippingtaxperc;
        public double? shippingtaxamt;
        public double? ordertotaltaxamt;
        public double? transactiontotaltaxamt;
        public double? saletax1amt;
        public double? ordertax1amt;
        public double? transactiontax1amt;
        public double? saletax2amt;
        public double? ordertax2amt;
        public double? transactiontax2amt;
        public double? ordertotalamt;
        public double? transactiontotalamt;
        public double? ordersubtotal;
        public double? salesubtotalwithtax;
        public double? ordersubtotalwithtax;
        public double? transactionsubtotalwithtax;
        public double? sodepositamt;
        public double? sodepositamtpaid;
        public int? totallineitem;
        public int? totalitemcount;
        public double? transactionsubtotal;
        public double? givenamt;
        public double? feeamt1;
        public double? feetaxamt1;
        public double? feeamt2;
        public double? feetaxamt2;
        public double? feeamt3;
        public double? feetaxamt3;
        public double? feeamt4;
        public double? feetaxamt4;
        public double? feeamt5;
        public double? feetaxamt5;
        public bool shippingtaxincluded;
        public string workstationuid;
        public string workstationname;
        /// <summary>
        /// Имя магазина
        /// </summary>
        public string storename;
        public double? taxareaperc;
        public double? taxarea2perc;
        public double? taxareaamt;
        public double? taxarea2amt;
        public double? taxareasalestaxamt;
        public double? taxarea2salestaxamt;
        public double? taxareaordertaxamt;
        public double? taxarea2ordertaxamt;
        public string reasoncode;
        public string reasondescription;
        public string posflag1;
        public string posflag2;
        public string posflag3;
        public string comment1;
        public string comment2;
        public string notes;
        public double? depositamttaken;
        public string depositrefdocsid;
        public double? totaldeposittaken;
        public string subsidiarysid;
        public string storesid;
        public string notesgeneral;
        public string notesorder;
        public string notessale;
        public string notesreturn;
        public string noteslostdoc;
        public DateTime? ordereddate;
        public DateTime? shipdate;
        public DateTime? canceldate;
        public bool shippartial;
        public string shippriority;
        public string tillsid;
        public string cashiersid;
        public string employee1sid;
        public string employee2sid;
        public string employee3sid;
        public string employee4sid;
        public string employee5sid;
        public int? exchangeqty;
        public int? discounttype;
        public string refordersid;
        public double? orderdiscperc;
        public double? orderdiscamt;
        public string orderdiscountreasonname;
        public int? orderdiscounttype;
        public bool socancelflag;
        public double? totaldepositused;
        public double? orderfeeamt1;
        public string orderfeetype1;
        public string orderfeename1;
        public double? orderfeetaxperc1;
        public bool orderfeetaxincluded1;
        public double? orderfeetaxamt1;
        public double? ordershippingamt;
        public double? ordershippingtaxperc;
        public bool ordershippingtaxincluded;
        public double? ordershippingtaxamt;
        public string feetype1sid;
        public string shippingsid;
        public string orderfeetype1sid;
        public string ordershippingsid;
        public double? ordershippingperc;
        public int? orderdocno;
        public double? useddiscamt;
        public double? usedsubtotal;
        public double? usedtax;
        public double? usedfeeamt1;
        public double? usedshippingamt;
        public bool orderchangedflag;
        public bool promogdmandisc;
        public string originalstoresid;
        public int? receipttype;
        public string ordertype;
        public bool hassale;
        public bool hasreturn;
        public bool hasdeposit;
        public string ordershipmethod;
        public string ordershipmethodsid;
        public string ordershipmethodid;
        public string shipmethod;
        public string shipmethodsid;
        public string shipmethodid;
        public string sscopy;
        public string ssstatus;
        public string sslastevent;
        public string sslasterror;
        public bool ssfulfillment;
        public double? detaxamt;
        public string ordershippingamtmanual;
        public string shippingamtmanual;
        public string ordershippingamtmanualused;
        public int? ordertrackingno;
        public string orderstatus;
        public int? taxrebatepersisted;
        public double? returnsubtotal;
        public double? returnsubtotalwithtax;
        public double? returntax1amt;
        public double? returntax2amt;
        public double? returntotaltaxamt;
        public string shippingamtmanualbdt;
        public string ordershippingamtmanualbdt;
        public string feeamt1bdt;
        public string orderfeeamt1bdt;
        public string refunddocumentsid;
        public string reforderbalancedue;
        public int? reforderorderdocno;
        public string ltylvlsid;
        public string ltypgmname;
        public string ltypgmsid;
        public string ltystartbalance;
        public string ltyendbalance;
        public double? ltyredeemamt;
        public double? ltygiftamt;
        public double? ltypercrewarddiscperc;
        public double? ltypercrewarddiscamt;
        public string ltycentralstatus;
        public string ltypgmsid2;
        public string ltypgmname2;
        public string ltypgmsid3;
        public string ltypgmname3;
        public string ltyitemearnpgmsid;
        public string ltyitemearnpgmname;
        public string ltyitemredeempgmsid;
        public string ltyitemredeempgmname;
        public string ltyitemgiftpgmsid;
        public string ltyitemgiftpgmname;
        public string ltytotalredeemmultiplier;
        public string ltytotalearnmultiplier;
        public string ltyitemredeemmultiplier;
        public string ltyitemearnmultiplier;
        public double? saletransdiscamt;
        public double? ordertransdiscamt;
        public double? ordertransdiscamtused;
        public string ltysaletotalbaseddisc;
        public string ltyordertotalbaseddisc;
        public string ltysaleearnedpointsp;
        public string ltysaleearnedpointsn;
        public string ltyorderearnedpointsp;
        public string ltyorderearnedpointsn;
        public string ltysaleusedpointsp;
        public string ltysaleusedpointsn;
        public string ltyorderusedpointsp;
        public string ltyorderusedpointsn;
        public string feeamtreturned1;
        public string shippingamtmanualreturned;
        public string refsalesid;
        public string giftreceipttype;
        public DateTime? orderduedate;
        public int? shippingfeetype;
        public int? employee1id;
        public int? employee2id;
        public int? employee3id;
        public int? employee4id;
        public int? employee5id;
        public int? cashierid;
        public int? controllerno;
        public string cashiername;
        public int? cashierorigsbsno;
        public string employee1name;
        public int? employee1origsbsno;
        public string employee2name;
        public int? employee2origsbsno;
        public string employee3name;
        public int? employee3origsbsno;
        public string employee4name;
        public int? employee4origsbsno;
        public string employee5name;
        public int? employee5origsbsno;
        public string origintimezone;
    }
}
