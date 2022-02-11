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
    public partial class FEmployee : Form
    {
        DataTable dt;
        DataView dv;

        public FEmployee()
        {
            try
            {
                InitializeComponent();

                // создание таблицы
                if (this.dt == null)
                {
                    this.dt = new DataTable();
                    this.dt.Columns.Add(new DataColumn("PrizmLogin", typeof(string)));
                    this.dt.Columns.Add(new DataColumn("Fio", typeof(string)));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке формы FCashiries с ошибкой: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
            
        }


        private void FEmployee_Load(object sender, EventArgs e)
        {
            try
            {
                // Наполняем таблицу данными и подключаем к гриду
                if (this.dt != null && this.dt.Rows.Count==0)
                {
                    foreach (Employee item in Config.employees)
                    {
                        DataRow nrow = this.dt.NewRow();
                        nrow["PrizmLogin"] = item.PrizmLogin;
                        nrow["Fio"] = item.fio_fo_check;
                        this.dt.Rows.Add(nrow);
                    }

                    this.dv = new DataView(dt);
                    this.dGView.DataSource = this.dv;
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении конфигурации списка сотрудников: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                List<Employee> NewEmployees = new List<Employee>();
                for (int i = 0; i < this.dGView.Rows.Count; i++)
                {
                    string strPrizmLogin = null;
                    if (this.dGView.Rows[i].Cells["PrizmLogin"].Value!=null) strPrizmLogin = this.dGView.Rows[i].Cells["PrizmLogin"].Value.ToString();
                    string strFio = null;
                    if (this.dGView.Rows[i].Cells["Fio"].Value != null) strFio = this.dGView.Rows[i].Cells["Fio"].Value.ToString();
                    
                    if (strPrizmLogin != null && strFio != null)
                    {
                        if (string.IsNullOrWhiteSpace(strPrizmLogin))
                        {
                            Log.EventSave("Не указано обязательное поле Логин строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(strFio))
                        {
                            Log.EventSave("Не указано обязательное поле ФИО строчка будет пропущена", GetType().Name, EventEn.Warning);
                            continue;
                        }
                        

                        bool HashFlagEmployee = true;
                        foreach (Employee itemEmployeeF in NewEmployees)
                        {
                            if (itemEmployeeF.PrizmLogin == strPrizmLogin)
                            {
                                HashFlagEmployee = false;
                                break;
                            }
                        }

                        if (HashFlagEmployee) NewEmployees.Add(new Employee(strPrizmLogin, strFio));
                    }               
                }

                Config.SetEmployees(NewEmployees);

                this.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чтении сохранении списка сотрудника: ({0})", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }
    }
}
