using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgoritmPrizm
{
    public partial class FTerminalSD : Form
    {
        private string pwd = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + "115446";
        private DataTable dtIMEI = null;
        private DataTable dtIMEIOut = null;

        // Конструктор
        public FTerminalSD()
        {
            InitializeComponent();

            if (this.dtIMEI == null)
            {
                this.dtIMEI = new DataTable("IMEI");
                this.dtIMEI.Columns.Add(new DataColumn("IMEI", Type.GetType("System.String")));
                this.dgIMEI.DataSource = this.dtIMEI;
            }

            if (this.dtIMEIOut == null)
            {
                this.dtIMEIOut = new DataTable("IMEIOut");
                this.dtIMEIOut.Columns.Add(new DataColumn("MachineName", Type.GetType("System.String")));
                this.dtIMEIOut.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
                this.dtIMEIOut.Columns.Add(new DataColumn("ActivNumber", Type.GetType("System.String")));
                this.dtIMEIOut.Columns.Add(new DataColumn("ValidToYYYYMMDD", Type.GetType("System.Int32")));
                this.dtIMEIOut.Columns.Add(new DataColumn("Info", Type.GetType("System.String")));
                this.dgIMEIOut.DataSource = this.dtIMEIOut;
            }
        }

        // Загрузка формы
        private void FTerminalSD_Load(object sender, EventArgs e)
        {
            this.txtBoxActNum.Text = Com.Lic.GetActivNumber();

            // Выводим информацию по всем ключам из текужего конфига
            this.dtIMEIOut.Clear();
            foreach (Com.LicLib.onLicEventKey item in Com.Lic._LicImeiKey)
            {
                DataRow nr = this.dtIMEIOut.NewRow();
                nr["MachineName"] = item.MachineName;
                nr["UserName"] = item.UserName;
                nr["ActivNumber"] = item.ActivNumber;
                nr["ValidToYYYYMMDD"] = item.ValidToYYYYMMDD;
                nr["Info"] = String.Format("{0} {1}", string.Join(",", item.ScnFullNameList.ToArray()), item.Info);
                this.dtIMEIOut.Rows.Add(nr);
            }

            
        }


        // Пользователь захотел продлить лицензию
        private void btn_Prolongat_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"mailto:support@aks.ru?subject=Запрос продления регистрации лицензионного ключа к терминалу сбора данных програмного комплекса " + Com.Lic.ProductSid +
    @"&cc=support@aks.ru&body=Мой номер активации:" + Environment.NewLine + "\r\n" + Com.Lic.GetActivNumber() + "\r\n IMEI моего устройства <набирите на устройстве *#06# и воткните идентификатор>");
        }

        // Пользователь ввёл лицензию для пролонгации
        private void btnProlongSetKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBox_KeyProlong.Text.Replace("\r\n", "") == this.pwd)
                {
                    txtBox_KeyProlong.Text = txtBox_KeyProlong.Text.Replace("\r\n", "");
                    this.panelGetActivationKey.Visible = true;
                }
                else
                {
                    //Com.Lic.RegNewKey(txtBox_KeyProlong.Text, Com.Lic.GetActivNumber());
                } 
                    
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            //Собственно можно обновить статус
            FTerminalSD_Load(null, null);
        }

        // Запуск генератора ключа
        private void btnCreateKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.dtIMEI != null && this.dtIMEI.Rows.Count > 0)
                {
                    List<string> ScnFullNameList = new List<string>();
                    foreach (DataRow item in this.dtIMEI.Rows)
                    {
                        ScnFullNameList.Add(item["IMEI"].ToString());
                    }

                    string tmp = null;
                    tmp = Com.Lic.InCode(textBoxNewActivNumber.Text, this.textBoxNewLicDate.Text, this.txtBoxInfo.Text.Replace("@Info", ""), this.chBox_HashUserOS.Checked, ScnFullNameList);

                    if (tmp != null)
                    {
                        this.textBoxNewLicKey.Visible = true;
                        this.textBoxNewLicKey.Text = tmp;
                    }
                }
                else throw new ApplicationException("Вы не выбрали ни одного IMEI которым может воспользоваться клиент.");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
