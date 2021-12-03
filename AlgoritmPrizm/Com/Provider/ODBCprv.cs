﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Data.Odbc;
using System.Data;
using AlgoritmPrizm.Com.Provider.Lib;
using AlgoritmPrizm.Lib;
using System.Threading;

namespace AlgoritmPrizm.Com.Provider
{
    /// <summary>
    /// Провайдер для работы по подключению типа ODBC
    /// </summary>
    public sealed class ODBCprv : ProviderBase, ProviderI
    {
        #region Private Param
        private string ServerVersion;
        public string DriverOdbc;
        #endregion

        #region Puplic Param
        // Билдер строки подключения
        public OdbcConnectionStringBuilder Ocsb = new OdbcConnectionStringBuilder();
        #endregion

        #region Puplic metod

        /// <summary>
        /// Контруктор
        /// </summary>
        /// <param name="ConnectionString">Строка подключения</param>
        public ODBCprv(string ConnectionString)
        {
            try
            {
                //Генерим ячейку элемент меню для отображения информации по плагину
                using (ToolStripMenuItem InfoToolStripMenuItem = new ToolStripMenuItem(this.GetType().Name))
                {
                    InfoToolStripMenuItem.Text = "Провайдер для работы с базой через OLEDB";
                    InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
                    //InfoToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                    //InfoToolStripMenuItem.Image = (Image)(new Icon(Type.GetType("Reminder.Common.PLUGIN.DwMonPlg.DwMonInfo"), "DwMon.ico").ToBitmap()); // для нормальной раьботы ресурс должен быть внедрённый
                    InfoToolStripMenuItem.Click += new EventHandler(InfoToolStripMenuItem_Click);

                    // Настраиваем компонент
                    base.SetupProviderBase(this.GetType(), InfoToolStripMenuItem, ConnectionString);
                }
                // Тестируем подключение и получаем информациию по версии базе данных
                if (ConnectionString != null && ConnectionString.Trim() != string.Empty)
                {
                    testConnection(ConnectionString, false);

                    // Устанавливаем в базовом классе строку подключения (котрая не меняется) и версию источника, чтобы не нужно было делать проверки коннектов
                    base.SetupConnectionStringAndVersionDB(ConnectionString, this.ServerVersion, this.DriverOdbc);
                }
            }
            catch (Exception ex) { base.UPovider.EventSave(ex.Message, "ODBCprv", EventEn.Error); }
        }

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        public override string PrintConnectionString()
        {
            try
            {
                if (base.ConnectionString != null && base.ConnectionString.Trim() != string.Empty)
                {
                    this.Ocsb = new OdbcConnectionStringBuilder(base.ConnectionString);
                    object P;
                    this.Ocsb.TryGetValue("Pwd", out P);
                    string Pssword = P.ToString();

                    return base.ConnectionString.Replace(Pssword, "*****");
                }
            }
            catch (Exception ex) { base.UPovider.EventSave(ex.Message, "PrintConnectionString", EventEn.Error); }

            return null;
        }

        /// <summary>
        /// Процедура вызывающая настройку подключения
        /// </summary>
        /// <returns>Возвращаем значение диалога</returns>
        public bool SetupConnectDB()
        {
            bool rez = false;

            using (ODBC.FSetupConnectDB Frm = new ODBC.FSetupConnectDB(this))
            {
                DialogResult drez = Frm.ShowDialog();

                // Если пользователь сохраняет новую строку подключения то сохраняем её в нашем объекте
                if (drez == DialogResult.Yes)
                {
                    base.SetupConnectionStringAndVersionDB(Frm.ConnectionString, this.ServerVersion, this.DriverOdbc);
                    rez = true;
                }
            }

            return rez;
        }

        /// <summary>
        /// Установка параметров через форму провайдера(плагина)
        /// </summary>
        /// <param name="DSN">DSN</param>
        /// <param name="Login">Логин</param>
        /// <param name="Password">Пароль</param>
        /// <param name="VisibleError">Выкидывать сообщения при неудачных попытках подключения</param>
        /// <param name="WriteLog">Писать в лог информацию о проверке побключения или нет</param>
        /// <param name="InstalConnect">Установить текущую строку подключения в билдере или нет</param>
        public string InstalProvider(string DSN, string Login, string Password, bool VisibleError, bool WriteLog, bool InstalConnect)
        {
            OdbcConnectionStringBuilder OcsbTmp = new OdbcConnectionStringBuilder();
            OcsbTmp.Dsn = DSN;
            OcsbTmp.Add("Uid", Login);
            OcsbTmp.Add("Pwd", Password);

            try
            {
                if (testConnection(OcsbTmp.ConnectionString, VisibleError))
                {
                    if (InstalConnect) this.Ocsb = OcsbTmp;
                    return OcsbTmp.ConnectionString;
                }
                else return null;
            }
            catch (Exception)
            {
                if (WriteLog) Log.EventSave("Не удалось создать подключение: " + DSN, this.ToString(), EventEn.Error);
                throw;
            }
        }


        /// <summary>
        /// Получение любых данных из источника например чтобы плагины могли что-то дополнительно читать
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        /// <returns>Результата В виде таблицы</returns>
        public override DataTable getData(string SQL)
        {
            try
            {
                if (!this.HashConnect()) throw new ApplicationException("Нет подключение к базе данных." + this.Driver);
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return getDataORA(SQL);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return getDataMySql(SQL);
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (Com.Config.Trace) base.EventSave(ex.Message, "getData", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Выполнение любых запросов на источнике
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        public override void setData(string SQL)
        {
            try
            {
                if (!this.HashConnect()) new ApplicationException("Нет подключение к базе данных." + this.Driver);
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            setDataORA(SQL);
                            break;
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            setDataMySql(SQL);
                            break;
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getData", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setData", EventEn.Dump);
                throw;
            }
        }


        /// <summary>
        /// Устанавливаем факт по чеку
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="InvcNo">Сид докумнета</param>
        /// <param name="PosDate">Дата документа</param>
        /// <param name="TotalCashSum">Сумма по документу уплаченная налом</param>
        public void SetPrizmCustPorog(string CustInn, string InvcNo, DateTime PosDate, decimal TotalCashSum)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (!this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            SetPrizmCustPorogORA(CustInn, InvcNo, PosDate, TotalCashSum);
                            break;
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            SetPrizmCustPorogMySql(CustInn, InvcNo, PosDate, TotalCashSum);
                            break;
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (Com.Config.Trace) base.EventSave(ex.Message, "SetPrizmCustPorog", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Получить сумму по клиенту за дату
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="Dt">Дата смены</param>
        /// <returns>Сумму по клиенту за выбранную дату</returns>
        public decimal GetTotalCashSum(string CustInn, DateTime Dt)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (!this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return GetTotalCashSumORA(CustInn, Dt);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetTotalCashSumMySql(CustInn, Dt);
                        default:
                            throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + this.Driver);
                            //break;
                    }
                }
                //return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetTotalCashSum", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return 0;
            }
        }


        #endregion

        #region Private metod
        // Пользователь вызвал меню информации по провайдеру
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ODBC.FInfo Frm = new ODBC.FInfo(this))
            {
                Frm.ShowDialog();
            }
        }

        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <param name="ConnectionString">Строка подключения которую нужно проверить</param>
        /// <returns>Возврощает результат проверки</returns>
        private bool testConnection(string ConnectionString, bool VisibleError)
        {
            try
            {
                string tmpServerVersion;
                string tmpDriver;

                // Проверка подключения
                using (OdbcConnection con = new OdbcConnection(ConnectionString))
                {
                    con.Open();
                    tmpDriver = con.Driver;
                    tmpServerVersion = con.ServerVersion; // Если не упали, значит подключение создано верно, тогда сохраняем переданные параметры
                }


                // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                switch (tmpDriver)
                {
                    //case "SQLSRV32.DLL":
                    case "SQORA32.DLL":
                    case "SQORA64.DLL":
                        // Оракловая логика
                        break;
                    case "myodbc8a.dll":
                    case "myodbc8w.dll":
                        // MySql логика
                        break;
                    default:
                        throw new ApplicationException("Извините. Мы не умеем работать с драйвером: " + tmpDriver);
                }

                // Если не упали значит можно сохранить текущую версию
                this.DriverOdbc = tmpDriver;
                this.ServerVersion = tmpServerVersion; // Сохраняем версию базы

                return true;
            }
            catch (Exception ex)
            {
                // Логируем ошибку если её должен видеть пользователь или если взведён флаг трассировке в файле настройки программы
                if (VisibleError || Com.Config.Trace) base.EventSave(ex.Message, "testConnection", EventEn.Error);

                // Отображаем ошибку если это нужно
                if (VisibleError) MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Private metod For ORACLE

        /// <summary>
        /// Получение любых данных из источника например чтобы плагины могли что-то дополнительно читать
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        /// <returns>Результата В виде таблицы</returns>
        private DataTable getDataORA(string SQL)
        {
            DataTable rez = null;

            try
            {
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataORA", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                rez = new DataTable();

                                // Получаем схему таблицы
                                DataTable tt = dr.GetSchemaTable();
                                foreach (DataRow item in tt.Rows)
                                {
                                    rez.Columns.Add(new DataColumn(item["ColumnName"].ToString().ToUpper(), Type.GetType(item["DataType"].ToString())));
                                }


                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    DataRow newr = rez.NewRow();
                                    for (int i = 0; i < tt.Rows.Count; i++)
                                    {
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Double") { newr[i] = (double)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Decimal") { newr[i] = (Decimal)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.String") { newr[i] = (string)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.DateTime") { newr[i] = (DateTime)dr.GetValue(i); }
                                    }
                                    rez.Rows.Add(newr);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDataORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDataORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Выполнение любых запросов на источнике
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        private void setDataORA(string SQL)
        {
            try
            {
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataORA", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".setDataORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".setDataORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Устанавливаем факт по чеку
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="InvcNo">Сид докумнета</param>
        /// <param name="PosDate">Дата документа</param>
        /// <param name="TotalCashSum">Сумма по документу уплаченная налом</param>
        public void SetPrizmCustPorogORA(string CustInn, string InvcNo, DateTime PosDate, decimal TotalCashSum)
        {
            string CommandSql = String.Format(@"insert into aks.prizm_cust_porog(cust_inn, invc_no, dt, pos_date, total_cash_sum) Values('{0}', '{1}', TO_DATE('{2}.{3}.{4}', 'YYYY.MM.DD'), STR_TO_DATE('{2}.{3}.{4} {5}:{6}:{7}', 'YYYY.MM.DD HH24:MI:SS'), {8})", CustInn, InvcNo, PosDate.Year, PosDate.Month, PosDate.Day, PosDate.Hour, PosDate.Minute, PosDate.Second, TotalCashSum.ToString().Replace(',', '.'));

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogORA", EventEn.Dump);
                
                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetPrizmCustPorogORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetPrizmCustPorogORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogORA", EventEn.Dump);
                throw;
            }
        }


        /// <summary>
        /// Получить сумму по клиенту за дату
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="Dt">Дата смены</param>
        /// <returns>Сумму по клиенту за выбранную дату</returns>
        public decimal GetTotalCashSumORA(string CustInn, DateTime Dt)
        {
            string CommandSql = String.Format(@"Select Sum(total_cash_sum) As total_cash_sum 
From aks.prizm_cust_porog
Where dt=To_Date('{1}.{2}.{3}', 'YYYY.MM.DD')
    and Cust_inn='{0}'", CustInn, Dt.Year, Dt.Month, Dt.Day);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumORA", EventEn.Dump);

                decimal rez = 0;

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                // Получаем схему таблицы
                                //DataTable tt = dr.GetSchemaTable();

                                //foreach (DataRow item in tt.Rows)
                                //{
                                //    DataColumn ncol = new DataColumn(item["ColumnName"].ToString(), Type.GetType(item["DataType"].ToString()));
                                //ncol.SetOrdinal(int.Parse(item["ColumnOrdinal"].ToString()));
                                //ncol.MaxLength = (int.Parse(item["ColumnSize"].ToString()) < 300 ? 300 : int.Parse(item["ColumnSize"].ToString()));
                                //rez.Columns.Add(ncol);
                                //}
                                
                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    decimal total_cash_sum = 0;
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TOTAL_CASH_SUM").ToUpper()) total_cash_sum = decimal.Parse(dr.GetValue(i).ToString());
                                    }
                                    rez += total_cash_sum;
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTotalCashSumORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTotalCashSumORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumORA", EventEn.Dump);
                throw;
            }
        }


        #endregion

        #region Private method MySql

        /// <summary>
        /// Получение любых данных из источника например чтобы плагины могли что-то дополнительно читать
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        /// <returns>Результата В виде таблицы</returns>
        private DataTable getDataMySql(string SQL)
        {
            DataTable rez = null;

            try
            {
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                rez = new DataTable();

                                // Получаем схему таблицы
                                DataTable tt = dr.GetSchemaTable();
                                foreach (DataRow item in tt.Rows)
                                {
                                    rez.Columns.Add(new DataColumn(item["ColumnName"].ToString().ToUpper(), Type.GetType(item["DataType"].ToString())));
                                }


                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    DataRow newr = rez.NewRow();
                                    for (int i = 0; i < tt.Rows.Count; i++)
                                    {
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Int32") { newr[i] = (Int32)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Int64") { newr[i] = (Int64)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Double") { newr[i] = (double)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.Decimal") { newr[i] = (Decimal)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.String") { newr[i] = (string)dr.GetValue(i); }
                                        if (!dr.IsDBNull(i) && tt.Rows[i]["DataType"].ToString() == "System.DateTime") { newr[i] = (DateTime)dr.GetValue(i); }
                                    }
                                    rez.Rows.Add(newr);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDataMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDataMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".getDataMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Выполнение любых запросов на источнике
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        private void setDataMySql(string SQL)
        {
            try
            {
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(SQL, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".setDataMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".setDataMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setDataMySql", EventEn.Dump);
                throw;
            }
        }


        /// <summary>
        /// Устанавливаем факт по чеку
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="InvcNo">Сид докумнета</param>
        /// <param name="PosDate">Дата документа</param>
        /// <param name="TotalCashSum">Сумма по документу уплаченная налом</param>
        public void SetPrizmCustPorogMySql(string CustInn, string InvcNo, DateTime PosDate, decimal TotalCashSum)
        {
            string CommandSql = String.Format(@"insert into `aks`.`prizm_cust_porog`(`cust_inn`,`invc_no`,`dt`,`pos_date`, `total_cash_sum`) Values('{0}', {1}, STR_TO_DATE('{2},{3},{4}', '%Y,%m,%d'), STR_TO_DATE('{2},{3},{4} {5},{6},{7}', '%Y,%m,%d %H,%i,%s'), {8})", CustInn, InvcNo, PosDate.Year, PosDate.Month, PosDate.Day, PosDate.Hour, PosDate.Minute, PosDate.Second, TotalCashSum.ToString().Replace(',','.'));

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogMySql", EventEn.Dump);
                
                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        com.ExecuteNonQuery();
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetPrizmCustPorogMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetPrizmCustPorogMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetPrizmCustPorogMySql", EventEn.Dump);
                throw;
            }
        }


        /// <summary>
        /// Получить сумму по клиенту за дату
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="Dt">Дата смены</param>
        /// <returns>Сумму по клиенту за выбранную дату</returns>
        public decimal GetTotalCashSumMySql(string CustInn, DateTime Dt)
        {
            string CommandSql = String.Format(@"Select Sum(total_cash_sum) As total_cash_sum 
From `aks`.`prizm_cust_porog`
Where `dt`=STR_TO_DATE('{1},{2},{3}', '%Y,%m,%d')
    and `cust_inn`='{0}'", CustInn, Dt.Year, Dt.Month, Dt.Day);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumMySql", EventEn.Dump);

                decimal rez = 0;

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {

                            if (dr.HasRows)
                            {
                                // Получаем схему таблицы
                                //DataTable tt = dr.GetSchemaTable();

                                //foreach (DataRow item in tt.Rows)
                                //{
                                //    DataColumn ncol = new DataColumn(item["ColumnName"].ToString(), Type.GetType(item["DataType"].ToString()));
                                //ncol.SetOrdinal(int.Parse(item["ColumnOrdinal"].ToString()));
                                //ncol.MaxLength = (int.Parse(item["ColumnSize"].ToString()) < 300 ? 300 : int.Parse(item["ColumnSize"].ToString()));
                                //rez.Columns.Add(ncol);
                                //}

                                // пробегаем по строкам
                                while (dr.Read())
                                {
                                    decimal total_cash_sum = 0;
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TOTAL_CASH_SUM").ToUpper()) total_cash_sum = decimal.Parse(dr.GetValue(i).ToString());
                                    }
                                    rez += total_cash_sum;
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTotalCashSumMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTotalCashSumMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTotalCashSumMySql", EventEn.Dump);
                throw;
            }
        }

        #endregion

    }
}
