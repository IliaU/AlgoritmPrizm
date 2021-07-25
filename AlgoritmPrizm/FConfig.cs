using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm
{
    public partial class FConfig : Form
    {
        public FConfig()
        {
            try
            {
                InitializeComponent();
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

                this.txtBoxFrPort.Text = Config.FrPort.ToString();

                this.txtBoxTenderTypeCash.Text = Config.TenderTypeCash.ToString();
                this.txtBoxTenderTypeCredit.Text = Config.TenderTypeCredit.ToString();
                this.txtBoxTenderTypeGiftCert.Text = Config.TenderTypeGiftCert.ToString();
                this.txtBoxTenderTypeGiftCard.Text = Config.TenderTypeGiftCard.ToString();

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

                this.txtBoxHostPrizmApi.Text = Config.HostPrizmApi;
                this.txtBoxPrizmApiSystemLogon.Text = Config.PrizmApiSystemLogon;
                this.txtBoxPrizmApiSystemPassord.Text = Config.PrizmApiSystemPassord;
                this.txtBoxPrizmApiTimeLiveTockenMinute.Text = Config.PrizmApiTimeLiveTockenMinute.ToString();
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
                try
                {
                    Com.Config.FrPort = int.Parse(this.txtBoxFrPort.Text);
                }
                catch (Exception)
                {
                    Com.Log.EventSave(string.Format("Не смогли преобраовать {0} в число.", this.txtBoxFrPort.Text), GetType().Name, EventEn.Message);
                }


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

                this.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }
    }
}
