using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.Ports;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;
using System.Net;

namespace AlgoritmPrizm
{
    public partial class FConfig : Form
    {
        DataTable dt;
        DataView dv;

        public FConfig()
        {
            try
            {
                InitializeComponent();

                // создание таблицы
                if (this.dt == null)
                {
                    this.dt = new DataTable();
                    this.dt.Columns.Add(new DataColumn("ProductClass", typeof(string)));
                    this.dt.Columns.Add(new DataColumn("Mandatory", typeof(bool)));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке формы FConfig с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        private void FConfig_Load(object sender, EventArgs e)
        {
            try
            {
                this.chkTrace.Checked = Config.Trace;
                this.txtBoxNameCompany.Text = Config.NameCompany;
                this.txtBoxHost.Text = Config.Host;
                this.txtBoxPort.Text = Config.Port.ToString();
                
                cmbBoxFfd.Items.Clear();
                int Position = -1;
                int SelectPosition = -1;
                foreach (FfdEn item in FfdEn.GetValues(typeof(FfdEn)))
                {
                    Position++;
                    cmbBoxFfd.Items.Add(item.ToString());
                    if (item == Config.Ffd) SelectPosition = Position;
                }
                if (SelectPosition > -1) cmbBoxFfd.SelectedIndex = SelectPosition;
                //
                this.txtBoxFrPort.Text = Config.FrPort.ToString();
                this.txtBoxFrSerialNumber.Text = Config.FrSerialNumber;
                //
                this.chkBoxProcessingUrikForFr.Checked = Config.ProcessingUrikForFr;
                this.chkBoxPrintingUrikForFr.Checked = Config.PrintingUrikForFr;
                this.chkBoxPrintingIpForFr.Checked = Config.PrintingIpForFr;

                this.cmbBoxDisplayDspFullName.Items.Clear();
                cmbBoxDisplayDspFullName.Items.Add("Без дисплея"); 
                int PositionDisplayDspFullName = 0;
                int SelectDisplayDspFullName = -1;
                foreach (string item in Com.DisplayFarm.ListDisplayName)
                {
                    PositionDisplayDspFullName++;
                    cmbBoxDisplayDspFullName.Items.Add(item.ToString());
                    if (item == Config.DisplayDspFullName) SelectDisplayDspFullName = PositionDisplayDspFullName;
                }
                if (SelectDisplayDspFullName > -1) cmbBoxDisplayDspFullName.SelectedIndex = SelectDisplayDspFullName;
                else cmbBoxDisplayDspFullName.SelectedIndex = 0;
                //
                this.txtBoxDisplayPort.Text = Config.DisplayPort.ToString();
                this.txtBoxDisplayBaudRate.Text = Config.DisplayBaudRate.ToString();
                //
                this.cmbBoxDisplayParity.Items.Clear();
                int PositionDisplayParity = -1;
                int SelectDisplayParity = -1;
                foreach (Parity item in Parity.GetValues(typeof(Parity)))
                {
                    PositionDisplayParity++;
                    cmbBoxDisplayParity.Items.Add(item.ToString());
                    if (item == Config.DisplayParity) SelectDisplayParity = PositionDisplayParity;
                }
                if (SelectDisplayParity > -1) cmbBoxDisplayParity.SelectedIndex = SelectDisplayParity;
                //
                this.txtBoxDisplayDataBits.Text = Config.DisplayDataBits.ToString();
                //
                this.cmbBoxStopBits.Items.Clear();
                int PositionStopBits = -1;
                int SelectStopBits = -1;
                foreach (StopBits item in StopBits.GetValues(typeof(StopBits)))
                {
                    PositionStopBits++;
                    cmbBoxStopBits.Items.Add(item.ToString());
                    if (item == Config.DisplayStpBits) SelectStopBits = PositionStopBits;
                }
                if (SelectStopBits > -1) cmbBoxStopBits.SelectedIndex = SelectStopBits;

                this.cmbBoxDisplayFieldItem.Items.Clear();
                int PositionDisplayFieldItem = -1;
                int SelectPositionDisplayFieldItem = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionDisplayFieldItem++;
                    cmbBoxDisplayFieldItem.Items.Add(item.ToString());
                    if (item == Config.DisplayFieldItem) SelectPositionDisplayFieldItem = PositionDisplayFieldItem;
                }
                if (SelectPositionDisplayFieldItem > -1) cmbBoxDisplayFieldItem.SelectedIndex = SelectPositionDisplayFieldItem;

                cmbBoxSmsTypGateway.SelectedIndexChanged -= cmbBoxSmsTypGateway_SelectedIndexChanged;
                cmbBoxSmsTypGateway.Items.Clear();
                Position = -1;
                SelectPosition = -1;
                foreach (EnSmsTypGateway item in EnSmsTypGateway.GetValues(typeof(EnSmsTypGateway)))
                {
                    Position++;
                    cmbBoxSmsTypGateway.Items.Add(item.ToString());
                    if (item == Config.SmsTypGateway) SelectPosition = Position;
                }
                if (SelectPosition > -1) cmbBoxSmsTypGateway.SelectedIndex = SelectPosition;
                cmbBoxSmsTypGateway.SelectedIndexChanged += cmbBoxSmsTypGateway_SelectedIndexChanged;
                cmbBoxSmsTypGateway_SelectedIndexChanged(null,null);

                this.txtBoxSmsTypGatewaySmtp.Text = Config.SmsTypGatewaySmtp;
                this.txtBoxSmsTypGatewayPort.Text = Config.SmsTypGatewayPort.ToString();
                this.txtBoxSmsTypGatewayLogin.Text = Config.SmsTypGatewayLogin;
                this.txtBoxSmsTypGatewaySmtpLogin.Text = Config.SmsTypGatewaySmtpLogin;
                this.txtBoxSmsTypGatewayPassword.Text = Config.SmsTypGatewayPassword;
                this.txtBoxSmsTypGatewaySmtpPassword.Text = Config.SmsTypGatewaySmtpPassword;

                this.txtBoxTenderTypeCash.Text = Config.TenderTypeCash.ToString();
                this.txtBoxTenderTypeCredit.Text = Config.TenderTypeCredit.ToString();
                this.txtBoxTenderTypeGiftCert.Text = Config.TenderTypeGiftCert.ToString();
                this.txtBoxTenderTypeGiftCard.Text = Config.TenderTypeGiftCard.ToString();
                this.txtBoxTenderTypeAvans.Text = Config.TenderTypeAvans.ToString();

                this.txtBoxGiftCardCode.Text = Config.GiftCardCode;
                this.chkBoxGiftCardEnable.Checked = Config.GiftCardEnable;
                this.txtBoxGiftCardTax.Text = Config.GiftCardTax.ToString();


                this.cmbBoxFieldItem.Items.Clear();
                int PositionFieldItem = -1;
                int SelectPositionFieldItem = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem++;
                    cmbBoxFieldItem.Items.Add(item.ToString());
                    if (item == Config.FieldItem) SelectPositionFieldItem = PositionFieldItem;
                }
                if (SelectPositionFieldItem > -1) cmbBoxFieldItem.SelectedIndex = SelectPositionFieldItem;
                //
                this.cmbBoxFieldItem1.Items.Clear();
                int PositionFieldItem1 = -1;
                int SelectPositionFieldItem1 = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem1++;
                    cmbBoxFieldItem1.Items.Add(item.ToString());
                    if (item == Config.FieldItem1) SelectPositionFieldItem1 = PositionFieldItem1;
                }
                if (SelectPositionFieldItem1 > -1) cmbBoxFieldItem1.SelectedIndex = SelectPositionFieldItem1;
                //
                this.cmbBoxFieldItem2.Items.Clear();
                int PositionFieldItem2 = -1;
                int SelectPositionFieldItem2 = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem2++;
                    cmbBoxFieldItem2.Items.Add(item.ToString());
                    if (item == Config.FieldItem2) SelectPositionFieldItem2 = PositionFieldItem2;
                }
                if (SelectPositionFieldItem2 > -1) cmbBoxFieldItem2.SelectedIndex = SelectPositionFieldItem2;
                //
                this.cmbBoxFieldItem3.Items.Clear();
                int PositionFieldItem3 = -1;
                int SelectPositionFieldItem3 = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem3++;
                    cmbBoxFieldItem3.Items.Add(item.ToString());
                    if (item == Config.FieldItem3) SelectPositionFieldItem3 = PositionFieldItem3;
                }
                if (SelectPositionFieldItem3 > -1) cmbBoxFieldItem3.SelectedIndex = SelectPositionFieldItem3;
                //
                this.cmbBoxFieldItem4.Items.Clear();
                int PositionFieldItem4 = -1;
                int SelectPositionFieldItem4 = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem4++;
                    cmbBoxFieldItem4.Items.Add(item.ToString());
                    if (item == Config.FieldItem4) SelectPositionFieldItem4 = PositionFieldItem4;
                }
                if (SelectPositionFieldItem4 > -1) cmbBoxFieldItem4.SelectedIndex = SelectPositionFieldItem4;
                //
                this.cmbBoxFieldItem5.Items.Clear();
                int PositionFieldItem5 = -1;
                int SelectPositionFieldItem5 = -1;
                foreach (FieldItemEn item in FieldItemEn.GetValues(typeof(FieldItemEn)))
                {
                    PositionFieldItem5++;
                    cmbBoxFieldItem5.Items.Add(item.ToString());
                    if (item == Config.FieldItem5) SelectPositionFieldItem5 = PositionFieldItem5;
                }
                if (SelectPositionFieldItem5 > -1) cmbBoxFieldItem5.SelectedIndex = SelectPositionFieldItem5;

                this.cmbBoxFieldDocNum.Items.Clear();
                int PositionFieldDocNum = -1;
                int SelectPositionFieldDocNum = -1;
                foreach (FieldDocNumEn item in FieldDocNumEn.GetValues(typeof(FieldDocNumEn)))
                {
                    PositionFieldDocNum++;
                    cmbBoxFieldDocNum.Items.Add(item.ToString());
                    if (item == Config.FieldDocNum) SelectPositionFieldDocNum = PositionFieldDocNum;
                }
                if (SelectPositionFieldDocNum > -1) cmbBoxFieldDocNum.SelectedIndex = SelectPositionFieldDocNum;

                this.cmbBoxFieldInnTyp.Items.Clear();
                int PositionFieldInnTyp = -1;
                int SelectPositionFieldInnTyp = -1;
                foreach (FieldDocNumEn item in FieldDocNumEn.GetValues(typeof(FieldDocNumEn)))
                {
                    PositionFieldInnTyp++;
                    cmbBoxFieldInnTyp.Items.Add(item.ToString());
                    if (item == Config.FieldInnTyp) SelectPositionFieldInnTyp = PositionFieldInnTyp;
                }
                if (SelectPositionFieldInnTyp > -1) cmbBoxFieldInnTyp.SelectedIndex = SelectPositionFieldInnTyp;



                this.txtBoxHostPrizmApi.Text = Config.HostPrizmApi;
                this.txtBoxPrizmApiSystemLogon.Text = Config.PrizmApiSystemLogon;
                this.txtBoxPrizmApiSystemPassord.Text = Config.PrizmApiSystemPassord;
                this.txtBoxPrizmApiTimeLiveTockenMinute.Text = Config.PrizmApiTimeLiveTockenMinute.ToString();
                this.txtBoxFileCheckLog.Text = Config.FileCheckLog;

                this.chkBoxMandatoryDefault.Checked = Config.MandatoryDefault;
                this.chkBox_GetMatrixAlways.Checked = Config.GetMatrixAlways;
                this.txtBoxProductMatrixEndOff.Text = Config.ProductMatrixEndOff.ToString();
                //
                this.cmbBoxProductMatrixClassType.Items.Clear();
                int PositionProductMatrixClassType = -1;
                int SelectPositionProductMatrixClassType = -1;
                foreach (EnProductMatrixClassType item in EnProductMatrixClassType.GetValues(typeof(EnProductMatrixClassType)))
                {
                    PositionProductMatrixClassType++;
                    cmbBoxProductMatrixClassType.Items.Add(item.ToString());
                    if (item == Config.ProductMatrixClassType) SelectPositionProductMatrixClassType = PositionProductMatrixClassType;
                }
                if (SelectPositionProductMatrixClassType > -1) cmbBoxProductMatrixClassType.SelectedIndex = SelectPositionProductMatrixClassType;
                //
                this.cmbBoxMatrixParceTyp.Items.Clear();
                int PositionMatrixParceTyp = -1;
                int SelectPositionMatrixParceTyp = -1;
                foreach (EnMatrixParceTyp item in EnMatrixParceTyp.GetValues(typeof(EnMatrixParceTyp)))
                {
                    PositionMatrixParceTyp++;
                    cmbBoxMatrixParceTyp.Items.Add(item.ToString());
                    if (item == Config.MatrixParceTyp) SelectPositionMatrixParceTyp = PositionMatrixParceTyp;
                }
                if (SelectPositionMatrixParceTyp > -1) cmbBoxMatrixParceTyp.SelectedIndex = SelectPositionMatrixParceTyp;
                //
                // Наполняем таблицу данными и подключаем к гриду
                if (this.dt != null && this.dt.Rows.Count == 0)
                {
                    foreach (BLL.ProdictMatrixClass item in Config.ProdictMatrixClassList)
                    {
                        DataRow nrow = this.dt.NewRow();
                        nrow["ProductClass"] = item.ProductClass;
                        nrow["Mandatory"] = item.Mandatory;
                        this.dt.Rows.Add(nrow);
                    }

                    this.dv = new DataView(dt);
                    this.dgProdictMatrixClass.DataSource = this.dv;
                }

                this.chkBoxCalculatedDaySumForUrik.Checked = Config.CalculatedDaySumForUrik;

                this.txtBoxLimitCachForUrik.Text = Config.LimitCachForUrik.ToString();

                this.chkBoxEmployeePrintingForEveryLine.Checked = Config.EmployeePrintingForEveryLine;

                // Параметры обработки ошибок при печати чека (откат чека)
                this.chkBoxHeldForDocements.Checked = Config.IsHeldForDocements;
                this.txtBoxHeldForDocementsTimeout.Text = Config.HeldForDocementsTimeout.ToString();

                // Заполняем настройку по паролю и доступу к системному меню
                this.txtBoxBlockActionPassword.Text = Config.BlockActionPassword;
                this.txtBoxBlockActionTimeOut.Text = Config.BlockActionTimeOut.ToString();

                // Поддержка реализации меха
                this.chkBox_MexSendItemBarcode.Checked = Config.MexSendItemBarcode;

                // Сайт с которым мы работам для обработки документов
                this.txtBoxWebSiteForIsmp.Text = Config.WebSiteForIsmp;
                // Через какое время чистить из оперативы ответ если вдруг фронт лёг, чтобы небыло утечки памяти
                this.txtBoxClearJsonCdnFarmMin.Text = Config.ClearJsonCdnFarmMin.ToString();
                // Значение токена по умолчанию если не удалось получить используя ЕЦП
                this.txtBoxDefaultTokenEcp.Text = Config.DefaultTokenEcp;
                // Установка таймаута для ответа  онлайн (ms)
                this.txtBoxCdnRequestTimeout.Text = Config.CdnRequestTimeout.ToString();

                // идентификатор ФОИВ; фиксировано
                this.txtBoxFrTag1262.Text = Config.FrTag1262;
                // дата документа основания; фиксировано
                this.txtBoxFrTag1263.Text = Config.FrTag1263;
                // номер документа основания; фиксировано
                this.txtBoxFrTag1264.Text = Config.FrTag1264;

                // SSL протокол который будет использовать наша программа
                this.cmbBoxWebSecurityProtocolType.Items.Clear();
                int PositionWebSecurityProtocolType = -1;
                int SelectWebSecurityProtocolType = -1;
                foreach (SecurityProtocolType item in SecurityProtocolType.GetValues(typeof(SecurityProtocolType)))
                {
                    PositionWebSecurityProtocolType++;
                    cmbBoxWebSecurityProtocolType.Items.Add(item.ToString());
                    if (item == Config.WebSecurityProtocolType) SelectWebSecurityProtocolType = PositionWebSecurityProtocolType;
                }
                if (SelectWebSecurityProtocolType > -1) cmbBoxWebSecurityProtocolType.SelectedIndex = SelectWebSecurityProtocolType;

                // Статус фискального регистратора включён или выключен
                this.chkIsFrEnable.Checked = Config.isFrEnable;

                // Хост
                this.txtBoxEniseyHost.Text = Config.EniseyHost;
                // Порт
                this.txtBoxEniseyPort.Text = Config.EniseyPort.ToString();
                // Логин
                this.txtBoxEniseyLogin.Text = Config.EniseyLogin;
                // Пароль
                this.txtBoxEniseyPassword.Text = Config.EniseyPassword;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                Com.Config.Trace = this.chkTrace.Checked;
                Config.NameCompany = this.txtBoxNameCompany.Text;
                Com.Config.Host = this.txtBoxHost.Text;
                try
                {
                    Com.Config.Port = int.Parse(this.txtBoxPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxPort.Text), GetType().Name, EventEn.Message);
                }

                Config.Ffd = EventConvertor.Convert(cmbBoxFfd.Items[cmbBoxFfd.SelectedIndex].ToString(),Config.Ffd);



                string DefaultDisplayDspFullName = null;
                foreach (string item in Com.DisplayFarm.ListDisplayName)
                {
                    if (cmbBoxDisplayDspFullName.SelectedIndex>-1)
                    {
                        if (item == this.cmbBoxDisplayDspFullName.Items[cmbBoxDisplayDspFullName.SelectedIndex].ToString())
                        {
                            DefaultDisplayDspFullName = item;
                        }
                    }
                }
                if (DefaultDisplayDspFullName != null) Config.DisplayDspFullName = DefaultDisplayDspFullName;
                else Config.DisplayDspFullName = "";
                //
                try
                {
                    Config.DisplayPort = int.Parse(this.txtBoxDisplayPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxDisplayPort.Text), GetType().Name, EventEn.Message);
                }
                //
                try
                {
                    Config.DisplayBaudRate = int.Parse(this.txtBoxDisplayBaudRate.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxDisplayBaudRate.Text), GetType().Name, EventEn.Message);
                }
                //
                Config.DisplayParity = EventConvertor.Convert(cmbBoxDisplayParity.Items[cmbBoxDisplayParity.SelectedIndex].ToString(), Config.DisplayParity);
                //
                try
                {
                    Config.DisplayDataBits = int.Parse(this.txtBoxDisplayDataBits.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxDisplayDataBits.Text), GetType().Name, EventEn.Message);
                }
                //
                Config.DisplayStpBits = EventConvertor.Convert(cmbBoxStopBits.Items[cmbBoxStopBits.SelectedIndex].ToString(), Config.DisplayStpBits);
                //
                if (DefaultDisplayDspFullName != null) Com.DisplayFarm.SetCurrentDisplay(Com.DisplayFarm.CreateNewDisplay("DisplayDSP840"));
                else Com.DisplayFarm.SetCurrentDisplay(null);

                Config.DisplayFieldItem = EventConvertor.Convert(cmbBoxDisplayFieldItem.Items[cmbBoxDisplayFieldItem.SelectedIndex].ToString(), Config.DisplayFieldItem);

                Config.SmsTypGateway = EventConvertor.Convert(cmbBoxSmsTypGateway.Items[cmbBoxSmsTypGateway.SelectedIndex].ToString(), Config.SmsTypGateway);

                Config.SmsTypGatewaySmtp = this.txtBoxSmsTypGatewaySmtp.Text;
                try
                {
                    Config.SmsTypGatewayPort = int.Parse(this.txtBoxSmsTypGatewayPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxSmsTypGatewayPort.Text), GetType().Name, EventEn.Message);
                }
                Config.SmsTypGatewayLogin= this.txtBoxSmsTypGatewayLogin.Text;
                Config.SmsTypGatewaySmtpLogin = this.txtBoxSmsTypGatewaySmtpLogin.Text;
                Config.SmsTypGatewayPassword= this.txtBoxSmsTypGatewayPassword.Text;
                Config.SmsTypGatewaySmtpPassword = this.txtBoxSmsTypGatewaySmtpPassword.Text;

                try
                {
                    Com.Config.FrPort = int.Parse(this.txtBoxFrPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxFrPort.Text), GetType().Name, EventEn.Message);
                }
                Config.FrSerialNumber = this.txtBoxFrSerialNumber.Text;
                Config.ProcessingUrikForFr = this.chkBoxProcessingUrikForFr.Checked;
                Config.PrintingUrikForFr = this.chkBoxPrintingUrikForFr.Checked;
                Config.PrintingIpForFr = this.chkBoxPrintingIpForFr.Checked;

                try
                {
                    Com.Config.TenderTypeCash = int.Parse(this.txtBoxTenderTypeCash.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxTenderTypeCash.Text), GetType().Name, EventEn.Message);
                }
                try
                {
                    Com.Config.TenderTypeCredit = int.Parse(this.txtBoxTenderTypeCredit.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxTenderTypeCredit.Text), GetType().Name, EventEn.Message);
                }
                try
                {
                    Com.Config.TenderTypeGiftCert = int.Parse(this.txtBoxTenderTypeGiftCert.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxTenderTypeGiftCert.Text), GetType().Name, EventEn.Message);
                }
                try
                {
                    Com.Config.TenderTypeGiftCard = int.Parse(this.txtBoxTenderTypeGiftCard.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxTenderTypeGiftCard.Text), GetType().Name, EventEn.Message);
                }
                try
                {
                    Com.Config.TenderTypeAvans = int.Parse(this.txtBoxTenderTypeAvans.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxTenderTypeAvans.Text), GetType().Name, EventEn.Message);
                }

                Config.GiftCardCode = this.txtBoxGiftCardCode.Text;
                Config.GiftCardEnable = this.chkBoxGiftCardEnable.Checked;
                try
                {
                    Com.Config.GiftCardTax = int.Parse(this.txtBoxGiftCardTax.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxGiftCardTax.Text), GetType().Name, EventEn.Message);
                }

                Config.FieldItem = EventConvertor.Convert(cmbBoxFieldItem.Items[cmbBoxFieldItem.SelectedIndex].ToString(), Config.FieldItem);
                Config.FieldItem1 = EventConvertor.Convert(cmbBoxFieldItem1.Items[cmbBoxFieldItem1.SelectedIndex].ToString(), Config.FieldItem1);
                Config.FieldItem2 = EventConvertor.Convert(cmbBoxFieldItem2.Items[cmbBoxFieldItem2.SelectedIndex].ToString(), Config.FieldItem2);
                Config.FieldItem3 = EventConvertor.Convert(cmbBoxFieldItem3.Items[cmbBoxFieldItem3.SelectedIndex].ToString(), Config.FieldItem3);
                Config.FieldItem4 = EventConvertor.Convert(cmbBoxFieldItem4.Items[cmbBoxFieldItem4.SelectedIndex].ToString(), Config.FieldItem4);
                Config.FieldItem5 = EventConvertor.Convert(cmbBoxFieldItem5.Items[cmbBoxFieldItem5.SelectedIndex].ToString(), Config.FieldItem5);

                Config.FieldDocNum = EventConvertor.Convert(cmbBoxFieldDocNum.Items[cmbBoxFieldDocNum.SelectedIndex].ToString(), Config.FieldDocNum);

                Config.FieldInnTyp = EventConvertor.Convert(cmbBoxFieldInnTyp.Items[cmbBoxFieldInnTyp.SelectedIndex].ToString(), Config.FieldInnTyp);

                Config.HostPrizmApi = this.txtBoxHostPrizmApi.Text;
                Config.PrizmApiSystemLogon = this.txtBoxPrizmApiSystemLogon.Text;
                Config.PrizmApiSystemPassord = this.txtBoxPrizmApiSystemPassord.Text;
                try
                {
                    Config.PrizmApiTimeLiveTockenMinute = int.Parse(this.txtBoxPrizmApiTimeLiveTockenMinute.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxPrizmApiTimeLiveTockenMinute.Text), GetType().Name, EventEn.Message);
                }

                Config.FileCheckLog = this.txtBoxFileCheckLog.Text;

                Config.GetMatrixAlways = this.chkBox_GetMatrixAlways.Checked;
                if (!Config.GetMatrixAlways) Config.MandatoryDefault = this.chkBoxMandatoryDefault.Checked;
                //
                Config.ProductMatrixEndOff = Char.Parse(this.txtBoxProductMatrixEndOff.Text);
                Config.ProductMatrixClassType = EventConvertor.Convert(cmbBoxProductMatrixClassType.Items[cmbBoxProductMatrixClassType.SelectedIndex].ToString(), Config.ProductMatrixClassType);
                Config.MatrixParceTyp = EventConvertor.Convert(cmbBoxMatrixParceTyp.Items[cmbBoxMatrixParceTyp.SelectedIndex].ToString(), Config.MatrixParceTyp);
                //
                List<BLL.ProdictMatrixClass> NewProdictMatrixClass = new List<BLL.ProdictMatrixClass>();
                for (int i = 0; i < this.dgProdictMatrixClass.Rows.Count; i++)
                {
                    string strProductClass = null;
                    if (this.dgProdictMatrixClass.Rows[i].Cells["ProductClass"].Value != null) strProductClass = this.dgProdictMatrixClass.Rows[i].Cells["ProductClass"].Value.ToString();
                    bool boolMandatory = false;
                    if (this.dgProdictMatrixClass.Rows[i].Cells["Mandatory"].Value != null) try { boolMandatory = Boolean.Parse(this.dgProdictMatrixClass.Rows[i].Cells["Mandatory"].Value.ToString()); } catch (Exception){}
                        
                    if (strProductClass != null)
                    {
                        if (string.IsNullOrWhiteSpace(strProductClass))
                        {
                            Log.EventSave("Не указано обязательное поле с индетификатором класса продуктов строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }

                        bool HashFlagProductMatrinxClass = true;
                        foreach (BLL.ProdictMatrixClass itemProdMatrixClF in NewProdictMatrixClass)
                        {
                            if (itemProdMatrixClF.ProductClass == strProductClass)
                            {
                                HashFlagProductMatrinxClass = false;
                                break;
                            }
                        }

                        if (HashFlagProductMatrinxClass) NewProdictMatrixClass.Add(new BLL.ProdictMatrixClass(strProductClass, boolMandatory));
                    }
                }

                Config.SetProdictMatrixClassList(NewProdictMatrixClass);
                
                try
                {
                    Config.LimitCachForUrik = decimal.Parse(this.txtBoxLimitCachForUrik.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в формат decimal.", this.txtBoxLimitCachForUrik.Text), GetType().Name, EventEn.Message);
                }

                Config.EmployeePrintingForEveryLine = this.chkBoxEmployeePrintingForEveryLine.Checked;


                Config.CalculatedDaySumForUrik = this.chkBoxCalculatedDaySumForUrik.Checked;


                // Поддержка реализации меха
                Config.MexSendItemBarcode = this.chkBox_MexSendItemBarcode.Checked;

                // Параметры обработки ошибок при печати чека (откат чека)
                Config.IsHeldForDocements = this.chkBoxHeldForDocements.Checked;
                try
                {
                    Config.HeldForDocementsTimeout = int.Parse(this.txtBoxHeldForDocementsTimeout.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxHeldForDocementsTimeout.Text), GetType().Name, EventEn.Message);
                }

                // Заполняем настройку по паролю и доступу к системному меню
                if (String.IsNullOrWhiteSpace(this.txtBoxBlockActionPassword.Text)) Com.Log.EventSave(string.Format(@"Пароль для администратора обязателен.", this.txtBoxBlockActionTimeOut.Text), GetType().Name, EventEn.Warning, true, true);
                else Config.BlockActionPassword = this.txtBoxBlockActionPassword.Text;
                try
                {
                    Config.BlockActionTimeOut = int.Parse(this.txtBoxBlockActionTimeOut.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxBlockActionTimeOut.Text), GetType().Name, EventEn.Warning);
                }

                // Сайт с которым мы работам для обработки документов
                Config.WebSiteForIsmp = this.txtBoxWebSiteForIsmp.Text;
                //
                // Через какое время чистить из оперативы ответ если вдруг фронт лёг, чтобы небыло утечки памяти
                try
                {
                    Config.ClearJsonCdnFarmMin = int.Parse(this.txtBoxClearJsonCdnFarmMin.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxClearJsonCdnFarmMin.Text), GetType().Name, EventEn.Warning);
                }
                //
                // Значение токена по умолчанию если не удалось получить используя ЕЦП
                Config.DefaultTokenEcp = this.txtBoxDefaultTokenEcp.Text;
                // Установка таймаута для ответа  онлайн (ms)
                try
                {
                    Config.CdnRequestTimeout = int.Parse(this.txtBoxCdnRequestTimeout.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxCdnRequestTimeout.Text), GetType().Name, EventEn.Warning);
                }

                // идентификатор ФОИВ; фиксировано
                Config.FrTag1262 = this.txtBoxFrTag1262.Text;
                // дата документа основания; фиксировано
                Config.FrTag1263 = this.txtBoxFrTag1263.Text;
                // номер документа основания; фиксировано
                Config.FrTag1264 = this.txtBoxFrTag1264.Text;

                // SSL протокол который будет использовать наша программ
                Config.WebSecurityProtocolType = EventConvertor.Convert(cmbBoxWebSecurityProtocolType.Items[cmbBoxWebSecurityProtocolType.SelectedIndex].ToString(), Config.WebSecurityProtocolType);

                //Статус фискального регистратора включён или выключен
                Config.isFrEnable = this.chkIsFrEnable.Checked;

                // Хост
                Config.EniseyHost = this.txtBoxEniseyHost.Text;
                // Порт
                try
                {
                    Config.EniseyPort = int.Parse(this.txtBoxEniseyPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxEniseyPort.Text), GetType().Name, EventEn.Warning);
                }
                // Логин
                Config.EniseyLogin = this.txtBoxEniseyLogin.Text;
                // Пароль
                Config.EniseyPassword = this.txtBoxEniseyPassword.Text;

                this.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        private void chkBox_GetMatrixAlways_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkBox_GetMatrixAlways.Checked) this.chkBoxMandatoryDefault.Visible = false;
                else this.chkBoxMandatoryDefault.Visible = true;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытке изменить видимость элемента chkBoxMandatoryDefault: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        // Пользователь меняет тип СМС провайдера
        private void cmbBoxSmsTypGateway_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                EnSmsTypGateway TypSms = EventConvertor.Convert(cmbBoxSmsTypGateway.Items[cmbBoxSmsTypGateway.SelectedIndex].ToString(), EnSmsTypGateway.Empty);

                this.lblSmsTypGatewayLogin.Visible = false;
                this.txtBoxSmsTypGatewayLogin.Visible = false;
                this.lblSmsTypGatewayPassword.Visible = false;
                this.txtBoxSmsTypGatewayPassword.Visible = false;
                this.PnlSmsTypGateway.Visible = false;

                if (TypSms != EnSmsTypGateway.Empty)
                {
                    this.lblSmsTypGatewayLogin.Visible = true;
                    this.txtBoxSmsTypGatewayLogin.Visible = true;
                    this.lblSmsTypGatewayPassword.Visible = true;
                    this.txtBoxSmsTypGatewayPassword.Visible = true;
                }

                if (TypSms == EnSmsTypGateway.Smsc_RU) PnlSmsTypGateway.Visible = true;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытке изменить видимость элемента cmbBoxSmsTypGateway_SelectedIndexChanged: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        // Пользователь изменил режим парсинга строки
        private void cmbBoxMatrixParceTyp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.lblProductMatrixEndOff.Visible = false;
                this.txtBoxProductMatrixEndOff.Visible = false;

                // Если произошёл какой-то выбор
                if (cmbBoxMatrixParceTyp.SelectedIndex>-1)
                {
                    EnMatrixParceTyp CurMatrixParceTyp = EventConvertor.Convert(cmbBoxMatrixParceTyp.Items[cmbBoxMatrixParceTyp.SelectedIndex].ToString(), Config.MatrixParceTyp);

                    switch (CurMatrixParceTyp)
                    {
                        case EnMatrixParceTyp.Seporate:
                            this.lblProductMatrixEndOff.Visible = true;
                            this.txtBoxProductMatrixEndOff.Visible = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при событии cmbBoxMatrixParceTyp_SelectedIndexChanged: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        // Пользователь решает обрабатывать чеки с тегами юрлиц в фискальнике
        private void chkBoxProcessingUrikForFr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkBoxProcessingUrikForFr.Checked)
                {
                    this.chkBoxPrintingUrikForFr.Visible = true;
                    this.chkBoxPrintingIpForFr.Visible = true;
                }
                else
                {
                    this.chkBoxPrintingUrikForFr.Visible = false;
                    this.chkBoxPrintingIpForFr.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытке изменить видимость элемента chkBoxPrintingUrikForFr: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Статус базы данных Енисей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEniseyStatus_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Web.GetStatusEnisey());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при получении статуса Базы: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                //throw ae;
            }
        }

        /// <summary>
        /// Инициализация базы данных Енисей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEniseyInit_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(Web.GetInitEnisey());
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации Базы: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error, true, true);
                //throw ae;
            }
        }
    }
}
