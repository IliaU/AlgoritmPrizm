using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using AlgoritmPrizm.Com;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm
{
    public partial class FCashiries : Form
    {
        DataTable dt;
        DataView dv;

        public FCashiries()
        {
            try
            {
                InitializeComponent();

                // создание таблицы
                if (this.dt == null)
                {
                    this.dt = new DataTable();
                    this.dt.Columns.Add(new DataColumn("Login", typeof(string)));
                    this.dt.Columns.Add(new DataColumn("Fio", typeof(string)));
                    this.dt.Columns.Add(new DataColumn("INN", typeof(string)));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке формы FCashiries с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
            
        }


        private void FCashiries_Load(object sender, EventArgs e)
        {
            try
            {
                // Наполняем таблицу данными и подключаем к гриду
                if (this.dt != null && this.dt.Rows.Count==0)
                {
                    foreach (Custumer item in Config.customers)
                    {
                        DataRow nrow = this.dt.NewRow();
                        nrow["Login"] = item.login;
                        nrow["Fio"] = item.fio_fo_check;
                        nrow["Inn"] = item.inn;
                        this.dt.Rows.Add(nrow);
                    }

                    this.dv = new DataView(dt);
                    this.dGView.DataSource = this.dv;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении конфигурации списка кассиров: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<Custumer> NewCustumers = new List<Custumer>();
                for (int i = 0; i < this.dGView.Rows.Count; i++)
                {
                    string strLogin = null;
                    if (this.dGView.Rows[i].Cells["Login"].Value!=null) strLogin = this.dGView.Rows[i].Cells["Login"].Value.ToString();
                    string strFio = null;
                    if (this.dGView.Rows[i].Cells["Fio"].Value != null) strFio = this.dGView.Rows[i].Cells["Fio"].Value.ToString();
                    string strINN = null;
                    if (this.dGView.Rows[i].Cells["INN"].Value !=null) strINN = this.dGView.Rows[i].Cells["INN"].Value.ToString();

                    if (strLogin != null && strFio != null && strINN != null)
                    {
                        if (string.IsNullOrWhiteSpace(strLogin))
                        {
                            Log.EventSave("Не указано обязательное поле Логин строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(strFio))
                        {
                            Log.EventSave("Не указано обязательное поле ФИО строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(strINN))
                        {
                            Log.EventSave("Не указано обязательное поле ИНН строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }

                        NewCustumers.Add(new Custumer(strLogin, strFio, strINN));
                    }               
                }

                Config.SetCustomers(NewCustumers);

                this.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении сохранении списка кассиров: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }
    }
}
