using System;
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


/*
        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Успех обработки функции</returns>
        public override bool getCheck(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (Com.Config.Mode == ModeEn.NotDB) return this.getCheckNotDB(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getCheckNotDB(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return getCheckORA(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
                        case "myodbc8a.dll":
                            return getCheckMySql(FuncTarget, CnfL, NextScenary, FirstDate, FilCustSid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "getCheck", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Заполнение справочника текущих пользователей
        /// </summary>
        /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
        /// <returns>Успех обработки функции</returns>
        public override bool getCustumers(Func<Customer, bool> FuncTarget)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи  NotDB
                if (Com.Config.Mode == ModeEn.NotDB) return this.getCustumersNotDB(FuncTarget);
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getCustumersNotDB(FuncTarget);
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return getCustumersORA(FuncTarget);
                        case "myodbc8a.dll":
                            return getCustumersMySql(FuncTarget);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "getCustumers", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Заполнение справочника причин скидок
        /// </summary>
        /// <returns>Успех обработки функции</returns>
        public override bool getDiscReasons()
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (Com.Config.Mode == ModeEn.NotDB) return this.getDiscReasonsNotDB();
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.getDiscReasonsNotDB();
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return getDiscReasonsORA();
                        case "myodbc8a.dll":
                            return getDiscReasonsMySql();
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "getDiscReasons", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Объединение клиентов
        /// </summary>
        /// <param name="MergeClientMain">Основной клиент</param>
        /// <param name="MergeClientDonors">Клинеты доноры</param>
        public void MergeClient(Customer MergeClientMain, List<Customer> MergeClientDonors)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (Com.Config.Mode == ModeEn.NotDB) return;
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) { this.MergeClientNotDB(MergeClientMain, MergeClientDonors); return; }
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            MergeClientORA(MergeClientMain, MergeClientDonors);
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".MergeClient", EventEn.Error);
                //if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".setData", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Установка расчитанной скидки в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
        /// <returns>Успех обработки функции</returns>
        public override bool AployDMCalkMaxDiscPerc(CustomerBase Cst, decimal CalkMaxDiscPerc)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (Com.Config.Mode == ModeEn.NotDB) return this.AployDMCalkMaxDiscPercNotDB(Cst, CalkMaxDiscPerc);
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.AployDMCalkMaxDiscPercNotDB(Cst, CalkMaxDiscPerc);
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return AployDMCalkMaxDiscPercORA(Cst, CalkMaxDiscPerc);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "AployDMCalkMaxDiscPerc", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return false;
            }
        }

        /// <summary>
        /// Установка бонусного бала в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
        /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <param name="OldCalkStoreCredit">Старый бонусный бал который мы устанавливаем</param>
        /// <param name="OldCalcScPerc">Старый процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <returns>Успех обработки функции</returns>
        public override bool AployDMCalkStoreCredit(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc, decimal OldCalkStoreCredit, decimal OldCalcScPerc)
        {
            try
            {
                // Если мы работаем в режиме без базы то выводим тестовые записи
                if (Com.Config.Mode == ModeEn.NotDB) return this.AployDMCalkStoreCreditNotDB(Cst, CalkStoreCredit, CalcScPerc);
                else if (Com.Config.Mode == ModeEn.NotData && this.HashConnect()) return this.AployDMCalkStoreCreditNotDB(Cst, CalkStoreCredit, CalcScPerc);
                else if (Com.Config.Mode == ModeEn.NotData && !this.HashConnect()) throw new ApplicationException("Не установлено подключение с базой данных.");
                else
                {
                    if (!base.HashConnect() && Com.Config.Mode != ModeEn.NotDB) new ApplicationException("Нет подключение к базе данных." + this.Driver);

                    // Проверка типа трайвера мы не можем обрабатьывать любой тип у каждого типа могут быть свои особенности
                    switch (this.Driver)
                    {
                        case "SQORA32.DLL":
                        case "SQORA64.DLL":
                            return AployDMCalkStoreCreditORA(Cst, CalkStoreCredit, CalcScPerc, OldCalkStoreCredit, OldCalcScPerc);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "AployDMCalkStoreCredit", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return false;
            }
        }
*/
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
        /*

                /// <summary>
                /// Выкачивание чеков
                /// </summary>
                /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
                /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
                /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
                /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
                /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
                /// <returns>Успех обработки функции</returns>
                private bool getCheckORA(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
                {
                    // Вне зависимости есть чеки или нет, для тех у кого есть бонусы которые должны примениться спустя указанные период, они должны примениться
                    this.setApplayNextStoreCgreditORA();


                    string CommandSql = @"select i.INVC_SID, i.invc_type, i.invc_no, ii.Item_Pos, i.created_date, i.post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
                inv.siz, To_Char(ii.QTY) QTY, i.cust_sid, i.STORE_NO, i.disc_reason_id, ii.ITEM_SID, To_Char(ii.ORIG_PRICE) ORIG_PRICE, To_Char(ii.PRICE) PRICE, To_Char(ii.USR_DISC_PERC) USR_DISC_PERC
            from CMS.invoice i 
                inner join CMS.INVC_ITEM ii on i.INVC_SID=ii.INVC_SID
                inner join invn_sbs inv on ii.ITEM_SID=inv.item_sid
                inner join customer s on i.cust_sid=s.CUST_SID
            Where i.cust_sid is not null " + (FilCustSid == null ? "" : @"
                and i.cust_sid=" + ((long)FilCustSid).ToString() + @"
            ") + @"Order by i.post_date, i.invc_no, ii.Item_Pos";

                    // Если указана какая-то спецефическая обработка чеков
                    if (!string.IsNullOrWhiteSpace(Com.Config.SpecificProcessBonus))
                    {
                        switch (Com.Config.SpecificProcessBonus)
                        {
                            case "BonusDM":
                                CommandSql = @"select i.INVC_SID, i.invc_type, i.invc_no, ii.Item_Pos, i.created_date, i.post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
                inv.siz, To_Char(ii.QTY) QTY, i.cust_sid, i.STORE_NO, i.disc_reason_id, ii.ITEM_SID, To_Char(ii.ORIG_PRICE) ORIG_PRICE, To_Char(ii.PRICE) PRICE, To_Char(ii.USR_DISC_PERC) USR_DISC_PERC
            from CMS.invoice i 
                left join CMS.INVC_ITEM ii on i.INVC_SID=ii.INVC_SID
                left join invn_sbs inv on ii.ITEM_SID=inv.item_sid
                inner join customer s on i.cust_sid=s.CUST_SID
            Where i.cust_sid is not null " + (FilCustSid == null ? "" : @"
                and i.cust_sid=" + ((long)FilCustSid).ToString() + @"
            ") + @"Order by i.post_date, i.invc_no, ii.Item_Pos";
                                break;
                            default:
                                break;
                        }
                    }

                    //CommandSql = "Select 1 invc_type, 1 invc_no, sysdate created_date, '12345' alu, 'Оп 1' description1, 'Оп 2' DESCRIPTION2, 'XXL' siz, 42 QTY, 1 cust_sid, 1 STORE_NO, 4 disc_reason_id, 12344224 ITEM_SID, 7 ORIG_PRICE, 7 PRICE, 5.45 USR_DISC_PERC From Dual";

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckORA", EventEn.Dump);

                        bool rez = true;

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


                                        DateTime? tmpFirstDate = null;

                                        // пробегаем по строкам
                                        while (dr.Read())
                                        {

                                            try
                                            {
                                                Int64 tmpInvcSid = -1;
                                                int tmpInvcType = -1;
                                                int tmpInvcNo = -1;
                                                int tmpItemPos = -1;
                                                DateTime tmpCreatedDate = DateTime.Now;
                                                DateTime tmpPostDate = DateTime.Now;
                                                string tmpAlu = null;
                                                string tmpDescription1 = null;
                                                string tmpDescription2 = null;
                                                string tmpSiz = null;
                                                decimal tmpQty = -1;
                                                long? tmpCustSid = null;
                                                int tmpStoreNo = -1;
                                                long tmpDiscReasonId = 0;
                                                long tmpItemSid = -1;
                                                decimal tmpOrigPrice = 0;
                                                decimal tmpPrice = 0;
                                                decimal tmpUsrDiscPerc = 0;
                                                for (int i = 0; i < dr.FieldCount; i++)
                                                {
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_type").ToUpper()) tmpInvcType = int.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_no").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("Item_Pos").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("created_date").ToUpper()) tmpCreatedDate = dr.GetDateTime(i);
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("post_date").ToUpper()) tmpPostDate = dr.GetDateTime(i);
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("alu").ToUpper()) tmpAlu = dr.GetValue(i).ToString();
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("description1").ToUpper()) tmpDescription1 = dr.GetValue(i).ToString();
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION2").ToUpper()) tmpDescription2 = dr.GetValue(i).ToString();
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("siz").ToUpper()) tmpSiz = dr.GetValue(i).ToString();
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("QTY").ToUpper()) tmpQty = decimal.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_NO").ToUpper()) tmpStoreNo = int.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("disc_reason_id").ToUpper()) tmpDiscReasonId = long.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_SID").ToUpper()) tmpItemSid = long.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ORIG_PRICE").ToUpper()) tmpOrigPrice = decimal.Parse(dr.GetValue(i).ToString());
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("PRICE").ToUpper()) tmpPrice = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                                    if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("USR_DISC_PERC").ToUpper()) tmpUsrDiscPerc = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                                }
                                                Check nChk = new Check(tmpInvcSid, tmpInvcType, tmpInvcNo, tmpItemPos, tmpCreatedDate, tmpPostDate, tmpAlu, tmpDescription1, tmpDescription2, tmpSiz, tmpQty, tmpCustSid, tmpStoreNo, tmpDiscReasonId, tmpItemSid, tmpOrigPrice, tmpPrice, tmpUsrDiscPerc);

                                                // Запоминаем первую дату для коректной работы прогресс бара
                                                if (tmpFirstDate == null) tmpFirstDate = tmpCreatedDate;

                                                // Передаём добытый чек обработчику
                                                if (FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, tmpFirstDate);

                                                // Проверяем необходимость продолжения дальнейшей работы
                                                if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека {0} за дату {1} с штрих кодом {2} обработчику Func<Check, ConfigurationList, int, bool>", tmpInvcNo, tmpCreatedDate, tmpAlu));
                                            }
                                            catch (Exception ex)
                                            {

                                                if (Com.Config.Trace)
                                                {
                                                    string tmpMes = null;

                                                    for (int i = 0; i < dr.FieldCount; i++)
                                                    {
                                                        if (tmpMes != null) tmpMes += Environment.NewLine;
                                                        else tmpMes = "";

                                                        tmpMes += dr.GetName(i) + " = " + dr.GetValue(i).ToString();
                                                    }

                                                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0} \n\r На обработке строки: \n\r{1}", ex.Message, tmpMes), GetType().Name + ".getCheckORA", EventEn.Dump);
                                                }
                                                throw;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return rez;
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckORA", EventEn.Dump);
                        throw;
                    }
                }
                //
                /// <summary>
                /// Обновление по отложенным записям связанным с бонусом, которые ещё не начислились
                /// </summary>
                private void setApplayNextStoreCgreditORA()
                {
                    // Пробегаем по доступным сценариям
                    int Delay_Period = 0;
                    foreach (UScenariy item in Com.ScenariyFarm.List)
                    {
                        if (item.ScenariyInType.Name == "BonusDMscn")
                        {
                            AlgoritmDM.Com.Scenariy.BonusDMscn bdm = (AlgoritmDM.Com.Scenariy.BonusDMscn)item.getScenariyPlugIn();
                            Delay_Period = bdm.Delay_Period;
                        }
                    }

                    string CommandSql = string.Format(@"Select INVC_SID, CUST_SID, POST_DATE, INVC_NO, ITEM_POS, To_Char(NEXT_STORE_CREDIT) As NEXT_STORE_CREDIT
        From AKS.INVC_SC_DOWN
        Where (APPLAY_NEXT_STORE_CREDIT is null or APPLAY_NEXT_STORE_CREDIT=0)
            and POST_DATE<=Trunc(sysdate-{0})", Delay_Period);


                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditORA", EventEn.Dump);

                        // Закрывать конект не нужно он будет закрыт деструктором
                        using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                        {
                            con.Open();

                            // Читаем строчки которые нужно обработать
                            using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                            {
                                com.CommandTimeout = 900;  // 15 минут
                                using (OdbcDataReader dr = com.ExecuteReader())
                                {
                                    // Если есть что обрабатывать
                                    if (dr.HasRows)
                                    {
                                        while (dr.Read())
                                        {
                                            Int64 tmpInvcSid = -1;
                                            Int64 tmpCustSid = -1;
                                            DateTime tmpPosDate = DateTime.Now;
                                            int tmpInvcNo = -1;
                                            int tmpItemPos = -1;
                                            string tmpNextStoreCredit = "0";

                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = Int64.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("POST_DATE").ToUpper()) tmpPosDate = dr.GetDateTime(i);
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_NO").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_POS").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NEXT_STORE_CREDIT").ToUpper()) tmpNextStoreCredit = dr.GetValue(i).ToString().Replace(",", ".");
                                            }

                                            string CommandSql2 = String.Format(@"--Select 
        Update AKS.INVC_SC_DOWN Set 
            STORE_CREDIT = STORE_CREDIT+{4}, 
            NEXT_STORE_CREDIT = Case When NEXT_STORE_CREDIT is null then 0 else NEXT_STORE_CREDIT end,
            APPLAY_NEXT_STORE_CREDIT = Case When INVC_SID = {5} Then 1 else APPLAY_NEXT_STORE_CREDIT End 
        --From AKS.INVC_SC_DOWN 
        Where (CUST_SID={0} and POST_DATE>To_Date('{1}','DD.MM.YYYY'))
            or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY') and invc_no>{2})
            or (CUST_SID={0} and POST_DATE=To_Date('{1}','DD.MM.YYYY') and invc_no={2} and Item_Pos>={3})", tmpCustSid, tmpPosDate.Day.ToString().PadLeft(2, '0') + "." + tmpPosDate.Month.ToString().PadLeft(2, '0') + "." + tmpPosDate.Year.ToString(), tmpInvcNo, tmpItemPos, tmpNextStoreCredit, tmpInvcSid);

                                            // обновляем информацию по этому чеку и по всем последующим
                                            using (OdbcCommand com2 = new OdbcCommand(CommandSql2, con))
                                            {
                                                com2.CommandTimeout = 900;  // 15 минут
                                                int dr2 = com2.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditORA", EventEn.Dump);
                        throw;
                    }
                }

                /// <summary>
                /// Заполнение справочника текущих пользователей
                /// </summary>
                /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
                /// <returns>Успех обработки функции</returns>
                private bool getCustumersORA(Func<Customer, bool> FuncTarget)
                {
                    string CommandSql = null;
                    if (string.IsNullOrWhiteSpace(Com.Config.CustomerCountryList))      // Если код региона не определён
                    {
                        if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                            || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                        {
                            CommandSql = @"select distinct s.cust_sid, s.first_name, s.last_name, s.cust_id, To_Char(s.MAX_DISC_PERC) MAX_DISC_PERC, To_Char(s.STORE_CREDIT) STORE_CREDIT,
            a.phone1, a.address1, s.FST_SALE_DATE, s.lst_sale_date, s.EMAIL_ADDR, p.SC_PERC
        from CMS.customer s
            left join CMS.cust_address a on s.cust_sid=a.cust_sid and ADDR_NO=1
            left join AKS.CUST_SC_PARAM p on s.cust_sid=p.cust_sid";
                        }
                        else
                        {
                            CommandSql = @"select distinct s.cust_sid, s.first_name, s.last_name, s.cust_id, To_Char(s.MAX_DISC_PERC) MAX_DISC_PERC, To_Char(s.STORE_CREDIT) STORE_CREDIT,
            a.phone1, a.address1, s.FST_SALE_DATE, s.lst_sale_date, s.EMAIL_ADDR, p.SC_PERC
        from CMS.customer s
            left join CMS.cust_address a on s.cust_sid=a.cust_sid and ADDR_NO=1
            left join AKS.CUST_SC_PARAM p on s.cust_sid=p.cust_sid
        Where a.phone1 like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                        }
                    }
                    else    // Если код региона указан
                    {
                        if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                            || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                        {
                            CommandSql = @"select distinct s.cust_sid, s.first_name, s.last_name, s.cust_id, To_Char(s.MAX_DISC_PERC) MAX_DISC_PERC, To_Char(s.STORE_CREDIT) STORE_CREDIT,
            a.phone1, a.address1, s.FST_SALE_DATE, s.lst_sale_date, s.EMAIL_ADDR, p.SC_PERC
        from CMS.customer s
            inner join CMS.cust_address a on s.cust_sid=a.cust_sid and ADDR_NO=1
            left join AKS.CUST_SC_PARAM p on s.cust_sid=p.cust_sid
        Where nvl(a.COUNTRY_ID,0) in (" + Com.Config.CustomerCountryList + @")";
                        }
                        else
                        {
                            CommandSql = @"select distinct s.cust_sid, s.first_name, s.last_name, s.cust_id, To_Char(s.MAX_DISC_PERC) MAX_DISC_PERC, To_Char(s.STORE_CREDIT) STORE_CREDIT,
            a.phone1, a.address1, s.FST_SALE_DATE, s.lst_sale_date, s.EMAIL_ADDR, p.SC_PERC
        from CMS.customer s
            inner join CMS.cust_address a on s.cust_sid=a.cust_sid and ADDR_NO=1
            left join AKS.CUST_SC_PARAM p on s.cust_sid=p.cust_sid
        Where nvl(a.COUNTRY_ID,0) in (" + Com.Config.CustomerCountryList + @")
            and a.phone1 like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                        }
                    }


                    //CommandSql = "Select 1 cust_sid, 'Илья Михайлович' first_name, 'Погодин' last_name, '100001' cust_id, 5.5 MAX_DISC_PERC, '9163253757' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, 'ilia82@mail.ru' EMAIL_ADDR From Dual union Select 2 cust_sid, 'Константин' first_name, 'Чудаков' last_name, '100002' cust_id, 3.3 MAX_DISC_PERC, '91632' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, null EMAIL_ADDR From Dual";

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersORA", EventEn.Dump);

                        bool rez = true;

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

                                            long tmpCustSid = -1;
                                            string tmpFirstName = null;
                                            string tmpLastName = null;
                                            string tmpCustId = null;
                                            decimal tmpMaxDiscPerc = 0;
                                            decimal tmpStoreCredit = 0;
                                            decimal tmpScPerc = 0;
                                            string tmpPhone1 = null;
                                            string tmpAddress1 = null;
                                            DateTime? tmpFstSaleDate = null;
                                            DateTime? tmpLstSaleDate = null;
                                            string tmpEmailAddr = null;
                                            int Active = 0;

                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("first_name").ToUpper()) tmpFirstName = dr.GetValue(i).ToString();
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("last_name").ToUpper()) tmpLastName = dr.GetValue(i).ToString();
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_id").ToUpper()) tmpCustId = dr.GetValue(i).ToString();
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("MAX_DISC_PERC").ToUpper()) tmpMaxDiscPerc = decimal.Parse(dr.GetValue(i).ToString()); }
                                                catch { }
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_CREDIT").ToUpper()) tmpStoreCredit = decimal.Parse(dr.GetValue(i).ToString()); }
                                                catch { }
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("SC_PERC").ToUpper()) tmpScPerc = decimal.Parse(dr.GetValue(i).ToString()); }
                                                catch { }
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("phone1").ToUpper()) tmpPhone1 = dr.GetValue(i).ToString();
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("address1").ToUpper()) tmpAddress1 = dr.GetValue(i).ToString();
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("FST_SALE_DATE").ToUpper()) tmpFstSaleDate = dr.GetDateTime(i); }
                                                catch { }
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("lst_sale_date").ToUpper()) tmpLstSaleDate = dr.GetDateTime(i); }
                                                catch { }
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("EMAIL_ADDR").ToUpper()) tmpEmailAddr = dr.GetValue(i).ToString();
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ACTIVE").ToUpper()) Active = int.Parse(dr.GetValue(i).ToString()); }
                                                catch { }
                                            }
                                            Customer nCust = new Customer(tmpCustSid, tmpFirstName, tmpLastName, tmpCustId, tmpMaxDiscPerc, tmpStoreCredit, tmpScPerc, tmpPhone1, tmpAddress1, tmpFstSaleDate, tmpLstSaleDate, tmpEmailAddr, Active);

                                            if (nCust.CustSid == -3834734573147189252)
                                            { }

                                            // Проверяем если в конфиге определено несколько префиксов к телефону то нужно их проверить и передать только если они найдены
                                            if (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)
                                            {
                                                string[] tmpCustomerPrefixPhoneList = Com.Config.CustomerPrefixPhoneList.Split(',');
                                                bool flag = false;
                                                foreach (string item in tmpCustomerPrefixPhoneList)
                                                {
                                                    if (nCust.Phone1.Trim().IndexOf(item.Trim()) == 0) flag = true;
                                                }
                                                if (flag)
                                                {
                                                    // Передаём добытого пользователя обработчику
                                                    if (FuncTarget != null)
                                                    {
                                                        setActiveCustomerORA(nCust);       // Предварительно проверяем активность клиента и если он не активный, принудительно выставляем флаг активности
                                                        // Проверяем на валидность этого пользователя если валиден, то передаём его обработчику
                                                        if (GetValidCustomerORA(nCust)) rez = FuncTarget(nCust);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Передаём добытого пользователя обработчику
                                                if (FuncTarget != null)
                                                {
                                                    setActiveCustomerORA(nCust);           // Предварительно проверяем активность клиента и если он не активный, принудительно выставляем флаг активности
                                                    // Проверяем на валидность этого пользователя если валиден, то передаём его обработчику
                                                    if (GetValidCustomerORA(nCust)) rez = FuncTarget(nCust);
                                                }
                                            }

                                            // Проверяем необходимость продолжения дальнейшей работы
                                            if (!rez) { throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи пользователя сид {0} обработчику Func<Customer, bool>", tmpCustSid)); }
                                        }
                                    }
                                }
                            }
                        }

                        return rez;
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersORA", EventEn.Dump);
                        throw;
                    }
                }
                //
                /// <summary>
                /// Проверка статуса клиента active если 0, то нужно выставить 1
                /// </summary>
                /// <param name="Cst">Клиент у которого нужно сделать проверку на активность и если он не активный, принудительно выставить её.</param>
                private void setActiveCustomerORA(Customer Cst)
                {
                    if (Cst.Active == 0)
                    {
                        string CommandSql = string.Format("Update CMS.customer Set ACTIVE=1 Where cust_sid={0} and ACTIVE=0", Cst.CustSid);

                        try
                        {
                            if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerORA", EventEn.Dump);

                            // Закрывать конект не нужно он будет закрыт деструктором
                            using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                            {
                                con.Open();

                                using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                                {
                                    com.CommandTimeout = 900;  // 15 минут
                                    int dr = com.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (OdbcException ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerORA", EventEn.Dump);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerORA", EventEn.Dump);
                            throw;
                        }
                    }
                }
                //
                /// <summary>
                /// Проверяем валидность этого пользователя, если он невалиден, то не передаём его обработчику, если валиден, то передаём 
                /// </summary>
                /// <param name="Cst">Клиент у которого нужно сделать проверку на валидность</param>
                /// <returns></returns>
                private bool GetValidCustomerORA(Customer Cst)
                {
                    bool rez = false;

                    string CommandSql = string.Format("Select CUST_SID, FIRST_NAME, LAST_NAME, PHONE From AKS.CUSTOMER Where CUST_SID={0}", Cst.CustSid);

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerORA", EventEn.Dump);

                        // Закрывать конект не нужно он будет закрыт деструктором
                        using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                        {
                            con.Open();

                            using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                            {
                                com.CommandTimeout = 900;  // 15 минут
                                using (OdbcDataReader dr = com.ExecuteReader())
                                {
                                    long tmpCustSid = -1;
                                    string tmpFirstName = null;
                                    string tmpLastName = null;
                                    string tmpPhone = null;

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
                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("first_name").ToUpper()) tmpFirstName = dr.GetValue(i).ToString();
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("last_name").ToUpper()) tmpLastName = dr.GetValue(i).ToString();
                                                if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("phone").ToUpper()) tmpPhone = dr.GetValue(i).ToString();
                                            }

                                            // Проверяем наличие данных
                                            if (!string.IsNullOrWhiteSpace(tmpFirstName) && !string.IsNullOrWhiteSpace(tmpLastName) && !string.IsNullOrWhiteSpace(tmpPhone))
                                            {
                                                // Проверяем если есть разница, то мы должны записать в лог эту инфу и неразрешить передать обработчику
                                                if (Cst.FirstName != tmpFirstName || Cst.LastName != tmpLastName || Cst.Phone1 != tmpPhone)
                                                {
                                                    Com.Log.EventSave(Com.Config.LogNotValidCustomer,
                                                        string.Format(@"Обнаружено расхождение в клиенте CustSid={0}.
            Было:
                FirstName={1}
                LastName={2}
                Phone={3}
            Стало:
                FirstName={1}
                LastName={2}
                Phone={3}", Cst.CustSid, tmpFirstName, tmpLastName, tmpPhone, Cst.FirstName, Cst.LastName, Cst.Phone1),
                                                        ".GetValidCustomerORA", EventEn.Warning);
                                                }
                                                else rez = true;  // Если изменений нет, то передаём клиента обработчику
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Cst.CustSid != -1 && !string.IsNullOrWhiteSpace(Cst.FirstName) && !string.IsNullOrWhiteSpace(Cst.LastName) && !string.IsNullOrWhiteSpace(Cst.Phone1))
                                        {
                                            CommandSql = string.Format(@"Insert into AKS.CUSTOMER (CUST_SID, FIRST_NAME, LAST_NAME, PHONE)
        Values({0},'{1}','{2}','{3}')", Cst.CustSid, Cst.FirstName.Replace("'", "''"), Cst.LastName.Replace("'", "''"), Cst.Phone1.Replace("'", "''"));

                                            if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerORA", EventEn.Dump);

                                            using (OdbcCommand com1 = new OdbcCommand(CommandSql, con))
                                            {
                                                com1.CommandTimeout = 900;  // 15 минут
                                                int dr1 = com1.ExecuteNonQuery();
                                            }

                                            rez = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".GetValidCustomerORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".GetValidCustomerORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerORA", EventEn.Dump);
                        throw;
                    }


                    return rez;
                }

                /// <summary>
                /// Заполнение справочника причин скидок
                /// </summary>
                /// <returns>Успех обработки функции</returns>
                private bool getDiscReasonsORA()
                {
                    string CommandSql = @"Select Distinct DISC_REASON_ID, DISC_REASON_NAME From CMS.DISC_REASON Where SBS_NO=-1";

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsORA", EventEn.Dump);

                        bool rez = true;

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
                                            int tmpDscReasId = -1;
                                            string tmpDscReasName = null;
                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i) == "DISC_REASON_ID") tmpDscReasId = int.Parse(dr.GetValue(i).ToString()); }
                                                catch (Exception) { }
                                                try { if (!dr.IsDBNull(i) && dr.GetName(i) == "DISC_REASON_NAME") tmpDscReasName = dr.GetValue(i).ToString(); }
                                                catch (Exception) { }
                                            }

                                            // Проверка полученных данных
                                            if (tmpDscReasId != -1 && !string.IsNullOrWhiteSpace(tmpDscReasName))
                                            {
                                                // Добавляем новую причину скидки
                                                DiscReason nDscReas = new DiscReason(tmpDscReasId, tmpDscReasName);
                                                rez = Com.DiscReasonFarm.List.Add(nDscReas, true);

                                                // Проверяем необходимость продолжения дальнейшей работы
                                                if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке сохранения причины скидкиid {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return rez;
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDiscReasonsORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDiscReasonsORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsORA", EventEn.Dump);
                        throw;
                    }

                }



                /// <summary>
                /// Объединение клиентов
                /// </summary>
                /// <param name="MergeClientMain">Основной клиент</param>
                /// <param name="MergeClientDonors">Клинеты доноры</param>
                private void MergeClientORA(Customer MergeClientMain, List<Customer> MergeClientDonors)
                {
                    string SQL = null;
                    foreach (Customer item in MergeClientDonors)
                    {
                        // Правим таблицу CMS.INVOICE 
                        SQL = string.Format(@"update CMS.INVOICE set CUST_SID={0}, SHIPTO_CUST_SID={0}
        where CUST_SID={1}", MergeClientMain.CustSid, item.CustSid);
                        try
                        {
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);

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
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }


                        // Правим таблицу CMS.customer 
                        SQL = string.Format(@"update CMS.customer set ACTIVE=1
        where CUST_SID={0}", item.CustSid);
                        try
                        {
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);

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
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }


                        // Правим таблицу CMS.cust_address
                        SQL = string.Format(@"update CMS.cust_address set phone1='+0'
        where ADDR_NO=1 
            and CUST_SID={0}", item.CustSid);
                        try
                        {
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);

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
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                    }

                    List<string> trankT = new List<string>();
                    trankT.Add("AKS.CUSTOMER");
                    trankT.Add("AKS.CUST_SC_PARAM");
                    trankT.Add("AKS.INVC_SC_DOWN");
                    trankT.Add("AKS.SMTP_EVENTS");

                    foreach (string item in trankT)
                    {
                        SQL = string.Format(@"truncate table {0}", item);

                        try
                        {
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);

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
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                        catch (Exception ex)
                        {
                            base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientORA", EventEn.Error);
                            if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientORA", EventEn.Dump);
                            throw;
                        }
                    }

                }


                /// <summary>
                /// Установка расчитанной скидки в базе у конкртеного клиента
                /// </summary>
                /// <param name="Cst">Клиент</param>
                /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
                /// <returns>Успех обработки функции</returns>
                private bool AployDMCalkMaxDiscPercORA(CustomerBase Cst, decimal CalkMaxDiscPerc)
                {
                    string CommandSql = string.Format("Update CMS.customer Set MAX_DISC_PERC={0} Where cust_sid={1}", CalkMaxDiscPerc.ToString().Replace(",", "."), Cst.CustSid);

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercORA", EventEn.Dump);

                        bool rez = false;

                        // Закрывать конект не нужно он будет закрыт деструктором
                        using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                        {
                            con.Open();

                            using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                            {
                                com.CommandTimeout = 900;  // 15 минут
                                int dr = com.ExecuteNonQuery();

                                // Проверяем кол-во обновлённых строк
                                if (dr > 0)
                                {
                                    rez = true;
                                }
                                else
                                {
                                    throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                                }
                            }
                        }

                        return rez;
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercORA", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercORA", EventEn.Dump);
                        throw;
                    }
                }

                /// <summary>
                /// Установка бонусного бала в базе у конкртеного клиента
                /// </summary>
                /// <param name="Cst">Клиент</param>
                /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
                /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
                /// <param name="OldCalkStoreCredit">Старый бонусный бал который был до изменения</param>
                /// <param name="OldCalcScPerc">Старый процент по которому считался бонусный бал который был до изменения</param>
                /// <returns>Успех обработки функции</returns>
                private bool AployDMCalkStoreCreditORA(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc, decimal OldCalkStoreCredit, decimal OldCalcScPerc)
                {
                    bool rez = false;
                    string CommandSql = string.Format("Update CMS.customer Set Store_Credit={0} Where cust_sid={1}", CalkStoreCredit.ToString().Replace(",", "."), Cst.CustSid);

                    try
                    {
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA", EventEn.Dump);

                        // Закрывать конект не нужно он будет закрыт деструктором
                        using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                        {
                            con.Open();

                            using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                            {
                                com.CommandTimeout = 900;  // 15 минут
                                int dr = com.ExecuteNonQuery();

                                // Проверяем кол-во обновлённых строк
                                if (dr > 0)
                                {
                                    rez = true;
                                }
                                else
                                {
                                    throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_1", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_1", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_1", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_1", EventEn.Dump);
                        throw;
                    }

                    // Получаем пааметр в котором храниться значение имени поля для плагина BonusDMax
                    string SC_Perc = Com.ConfigurationFarm.ParamsOfScenatiy["SC_Perc"];
                    // сохранение параметра в базу в таблицу CMS.cust_address
                    try
                    {
                        // Если параметр найден, то необходимо сохранить инфу в ещё одну таблицу
                        if (!String.IsNullOrWhiteSpace(SC_Perc))
                        {
                            string CommandSql2 = string.Format("Update CMS.cust_address Set {0}='{1}' Where CUST_SID={2}", SC_Perc, CalcScPerc.ToString(), Cst.CustSid);
                            rez = false;

                            // Закрывать конект не нужно он будет закрыт деструктором
                            bool flaginsert = false;
                            using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                            {
                                con.Open();

                                using (OdbcCommand com = new OdbcCommand(CommandSql2, con))
                                {
                                    com.CommandTimeout = 900;  // 15 минут
                                    int dr = com.ExecuteNonQuery();

                                    // Проверяем кол-во обновлённых строк
                                    if (dr > 0)
                                    {
                                        rez = true;
                                    }
                                    else
                                    {
                                        flaginsert = true;
                                    }
                                }
                            }

                            // Если обновление не прошло успешно так как строка не найдена, то будем делать вставку
                            if (flaginsert)
                            {
                                string CommandSql3 = string.Format("Insert into CMS.cust_address(CUST_SID, {0}) values({1},'{2}')", SC_Perc, Cst.CustSid, CalcScPerc.ToString());

                                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                                {
                                    con.Open();

                                    using (OdbcCommand com = new OdbcCommand(CommandSql3, con))
                                    {
                                        com.CommandTimeout = 900;  // 15 минут
                                        int dr = com.ExecuteNonQuery();

                                        // Проверяем кол-во обновлённых строк
                                        if (dr > 0)
                                        {
                                            rez = true;
                                        }
                                        else
                                        {
                                            throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_2", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_2", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_2", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_2", EventEn.Dump);
                        throw;
                    }

                    try
                    {
                        if (Cst.CustSid == 2417549001015889916)
                        { }

                        // Если параметр найден, то необходимо сохранить инфу в ещё одну таблицу
                        if (Com.Config.SaveSmtpEvent && CalkStoreCredit != OldCalkStoreCredit)
                        {
                            string CommandSql3 = string.Format("Insert into AKS.SMTP_EVENTS(CUST_SID,EVENT_VALUE,EVENT_NAME) VALUES({0},{1},'AployDMCalkMaxDiscPerc')", Cst.CustSid, (CalkStoreCredit - OldCalkStoreCredit).ToString().Replace(",", "."));
                            rez = false;

                            // Закрывать конект не нужно он будет закрыт деструктором
                            using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                            {
                                con.Open();

                                using (OdbcCommand com = new OdbcCommand(CommandSql3, con))
                                {
                                    int dr = com.ExecuteNonQuery();
                                    rez = true;
                                }
                            }
                        }
                    }
                    catch (OdbcException ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_3", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_3", EventEn.Dump);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercORA_3", EventEn.Error);
                        if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditORA_3", EventEn.Dump);
                        throw;
                    }

                    return rez;
                }
        */
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


/*
        /// <summary>
        /// Выкачивание чеков
        /// </summary>
        /// <param name="FuncTarget">Функция которой передать строчку из чека</param>
        /// <param name="CnfL">Текущая конфигурация в которой обрабатывается строка чека</param>
        /// <param name="NextScenary">Индекс следующего элемента конфигурации который будет обрабатывать строку чека</param>
        /// <param name="FirstDate">Первая дата чека, предпологается использовать для прогресс бара</param>
        /// <param name="FilCustSid">Фильтр для получения данных по конкретному клиенту. null значит пользователь выгребает по всем клиентам</param>
        /// <returns>Успех обработки функции</returns>
        private bool getCheckMySql(Func<Check, ConfigurationList, int, DateTime?, bool> FuncTarget, ConfigurationList CnfL, int NextScenary, DateTime? FirstDate, long? FilCustSid)
        {
            // Вне зависимости есть чеки или нет, для тех у кого есть бонусы которые должны примениться спустя указанные период, они должны примениться
            this.setApplayNextStoreCgreditMySql();


            string CommandSql = @"select  i.`sid` As INVC_SID, i.`tender_type` As invc_type, i.`doc_no` As invc_no, ii.`item_pos` As Item_Pos, i.`created_datetime` As created_date, i.`post_date` As post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
       inv.`item_size` As siz, ii.`qty` As QTY, i.`bt_cuid` As cust_sid, i.`STORE_NO`, r.`sid` as disc_reason_id, 
       iid.`sid` As ITEM_SID, iid.`prev_price` AS ORIG_PRICE, iid.`new_price` As PRICE, iid.`new_disc_perc` As USR_DISC_PERC
    from `rpsods`.`document` i 
     inner join `rpsods`.`document_item` ii on i.SID=ii.`doc_sid`
     inner join `rpsods`.`invn_sbs_item` inv on ii.invn_sbs_item_sid=inv.sid
     inner join `rpsods`.`document_item_disc` iid on ii.sid=iid.doc_item_sid
     inner join `rpsods`.`customer` s on i.`bt_cuid`=s.`sid`
     inner join `rpsods`.`pref_reason` r On iid.`disc_reason`=r.`name` and r.reason_type = 10
    Where i.`bt_cuid` is not null " + (FilCustSid == null ? "" : @"
        and i.`bt_cuid`=" + ((long)FilCustSid).ToString() + @"
    ") + @"Order by i.`post_date`, i.`doc_no`, ii.`item_pos`";

            // Если указана какая-то спецефическая обработка чеков
            if (!string.IsNullOrWhiteSpace(Com.Config.SpecificProcessBonus))
            {
                switch (Com.Config.SpecificProcessBonus)
                {
                    case "BonusDM":
                        CommandSql = @"select  i.`sid` As INVC_SID, i.`tender_type` As invc_type, i.`doc_no` As invc_no, ii.`item_pos` As Item_Pos, i.`created_datetime` As created_date, i.`post_date` As post_date, inv.alu, inv.description1, inv.DESCRIPTION2,
       inv.`item_size` As siz, ii.`qty` As QTY, i.`bt_cuid` As cust_sid, i.`STORE_NO`, r.`sid` as disc_reason_id, 
       iid.`sid` As ITEM_SID, iid.`prev_price` AS ORIG_PRICE, iid.`new_price` As PRICE, iid.`new_disc_perc` As USR_DISC_PERC
    from `rpsods`.`document` i 
     left join `rpsods`.`document_item` ii on i.SID=ii.`doc_sid`
     left join `rpsods`.`invn_sbs_item` inv on ii.invn_sbs_item_sid=inv.sid
     left join `rpsods`.`document_item_disc` iid on ii.sid=iid.doc_item_sid
     inner join `rpsods`.`customer` s on i.`bt_cuid`=s.`sid`
     inner join `rpsods`.`pref_reason` r On iid.`disc_reason`=r.`name` and r.reason_type = 10
    Where i.`bt_cuid` is not null " + (FilCustSid == null ? "" : @"
        and i.`bt_cuid`=" + ((long)FilCustSid).ToString() + @"
    ") + @"Order by i.`post_date`, i.`doc_no`, ii.`item_pos`";
                        break;
                    default:
                        break;
                }
            }

            //CommandSql = "Select 1 invc_type, 1 invc_no, sysdate created_date, '12345' alu, 'Оп 1' description1, 'Оп 2' DESCRIPTION2, 'XXL' siz, 42 QTY, 1 cust_sid, 1 STORE_NO, 4 disc_reason_id, 12344224 ITEM_SID, 7 ORIG_PRICE, 7 PRICE, 5.45 USR_DISC_PERC From Dual";

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);

                bool rez = true;

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


                                DateTime? tmpFirstDate = null;

                                // пробегаем по строкам
                                while (dr.Read())
                                {

                                    try
                                    {
                                        Int64 tmpInvcSid = -1;
                                        int tmpInvcType = -1;
                                        int tmpInvcNo = -1;
                                        int tmpItemPos = -1;
                                        DateTime tmpCreatedDate = DateTime.Now;
                                        DateTime tmpPostDate = DateTime.Now;
                                        string tmpAlu = null;
                                        string tmpDescription1 = null;
                                        string tmpDescription2 = null;
                                        string tmpSiz = null;
                                        decimal tmpQty = -1;
                                        long? tmpCustSid = null;
                                        int tmpStoreNo = -1;
                                        long tmpDiscReasonId = 0;
                                        long tmpItemSid = -1;
                                        decimal tmpOrigPrice = 0;
                                        decimal tmpPrice = 0;
                                        decimal tmpUsrDiscPerc = 0;
                                        for (int i = 0; i < dr.FieldCount; i++)
                                        {
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_type").ToUpper()) tmpInvcType = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("invc_no").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("Item_Pos").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("created_date").ToUpper()) tmpCreatedDate = dr.GetDateTime(i);
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("post_date").ToUpper()) tmpPostDate = dr.GetDateTime(i);
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("alu").ToUpper()) tmpAlu = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("description1").ToUpper()) tmpDescription1 = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION2").ToUpper()) tmpDescription2 = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("siz").ToUpper()) tmpSiz = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("QTY").ToUpper()) tmpQty = decimal.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_NO").ToUpper()) tmpStoreNo = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("disc_reason_id").ToUpper()) tmpDiscReasonId = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_SID").ToUpper()) tmpItemSid = long.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ORIG_PRICE").ToUpper()) tmpOrigPrice = decimal.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("PRICE").ToUpper()) tmpPrice = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("USR_DISC_PERC").ToUpper()) tmpUsrDiscPerc = decimal.Parse(dr.GetValue(i).ToString().Replace(".", Com.Config.TekDelitel).Replace(",", Com.Config.TekDelitel));
                                        }
                                        Check nChk = new Check(tmpInvcSid, tmpInvcType, tmpInvcNo, tmpItemPos, tmpCreatedDate, tmpPostDate, tmpAlu, tmpDescription1, tmpDescription2, tmpSiz, tmpQty, tmpCustSid, tmpStoreNo, tmpDiscReasonId, tmpItemSid, tmpOrigPrice, tmpPrice, tmpUsrDiscPerc);

                                        // Запоминаем первую дату для коректной работы прогресс бара
                                        if (tmpFirstDate == null) tmpFirstDate = tmpCreatedDate;

                                        // Передаём добытый чек обработчику
                                        if (FuncTarget != null) rez = FuncTarget(nChk, CnfL, NextScenary, tmpFirstDate);

                                        // Проверяем необходимость продолжения дальнейшей работы
                                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи чека {0} за дату {1} с штрих кодом {2} обработчику Func<Check, ConfigurationList, int, bool>", tmpInvcNo, tmpCreatedDate, tmpAlu));
                                    }
                                    catch (Exception ex)
                                    {

                                        if (Com.Config.Trace)
                                        {
                                            string tmpMes = null;

                                            for (int i = 0; i < dr.FieldCount; i++)
                                            {
                                                if (tmpMes != null) tmpMes += Environment.NewLine;
                                                else tmpMes = "";

                                                tmpMes += dr.GetName(i) + " = " + dr.GetValue(i).ToString();
                                            }

                                            base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0} \n\r На обработке строки: \n\r{1}", ex.Message, tmpMes), GetType().Name + ".getCheckMySql", EventEn.Dump);
                                        }
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCheckMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCheckMySql", EventEn.Dump);
                throw;
            }
        }
        //
        /// <summary>
        /// Обновление по отложенным записям связанным с бонусом, которые ещё не начислились
        /// </summary>
        private void setApplayNextStoreCgreditMySql()
        {
            // Пробегаем по доступным сценариям
            int Delay_Period = 0;
            foreach (UScenariy item in Com.ScenariyFarm.List)
            {
                if (item.ScenariyInType.Name == "BonusDMscn")
                {
                    AlgoritmDM.Com.Scenariy.BonusDMscn bdm = (AlgoritmDM.Com.Scenariy.BonusDMscn)item.getScenariyPlugIn();
                    Delay_Period = bdm.Delay_Period;
                }
            }

            string CommandSql = string.Format(@"Select INVC_SID, CUST_SID, POST_DATE, INVC_NO, ITEM_POS, NEXT_STORE_CREDIT As NEXT_STORE_CREDIT
From `aks`.`invc_sc_down`
Where (APPLAY_NEXT_STORE_CREDIT is null or APPLAY_NEXT_STORE_CREDIT=0)
    and POST_DATE<=date(sysdate()-{0})", Delay_Period);


            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    // Читаем строчки которые нужно обработать
                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {
                            // Если есть что обрабатывать
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    Int64 tmpInvcSid = -1;
                                    Int64 tmpCustSid = -1;
                                    DateTime tmpPosDate = DateTime.Now;
                                    int tmpInvcNo = -1;
                                    int tmpItemPos = -1;
                                    string tmpNextStoreCredit = "0";

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_SID").ToUpper()) tmpInvcSid = Int64.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = Int64.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("POST_DATE").ToUpper()) tmpPosDate = dr.GetDateTime(i);
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVC_NO").ToUpper()) tmpInvcNo = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEM_POS").ToUpper()) tmpItemPos = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NEXT_STORE_CREDIT").ToUpper()) tmpNextStoreCredit = dr.GetValue(i).ToString().Replace(",", ".");
                                    }

                                    string CommandSql2 = String.Format(@"#SELECT STR_TO_DATE('2014,2,28 19,30,05', '%Y,%m,%d %H,%i,%s');
#Select *
Update `aks`.`invc_sc_down` Set 
    STORE_CREDIT = STORE_CREDIT+{4}, 
    NEXT_STORE_CREDIT = Case When NEXT_STORE_CREDIT is null then 0 else NEXT_STORE_CREDIT end,
    APPLAY_NEXT_STORE_CREDIT = Case When INVC_SID = {5} Then 1 else APPLAY_NEXT_STORE_CREDIT End 
#From `aks`.`invc_sc_down` 
Where (CUST_SID={0} and POST_DATE>STR_TO_DATE('{1}','%d.%m.%Y'))
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y') and invc_no>{2})
    or (CUST_SID={0} and POST_DATE=STR_TO_DATE('{1}','%d.%m.%Y') and invc_no={2} and Item_Pos>={3}) ", tmpCustSid, tmpPosDate.Day.ToString().PadLeft(2, '0') + "." + tmpPosDate.Month.ToString().PadLeft(2, '0') + "." + tmpPosDate.Year.ToString(), tmpInvcNo, tmpItemPos, tmpNextStoreCredit, tmpInvcSid);

                                    // обновляем информацию по этому чеку и по всем последующим
                                    using (OdbcCommand com2 = new OdbcCommand(CommandSql2, con))
                                    {
                                        com2.CommandTimeout = 900;  // 15 минут
                                        int dr2 = com2.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка по отложенным записям связанным с бонусом, которые ещё не начислились после {1} дней. {0}", ex.Message, Delay_Period), GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setApplayNextStoreCgreditMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Заполнение справочника текущих пользователей
        /// </summary>
        /// <param name="FuncTarget">Функция котороая юудет заполнять справочник</param>
        /// <returns>Успех обработки функции</returns>
        private bool getCustumersMySql(Func<Customer, bool> FuncTarget)
        {
            string CommandSql = null;
            if (string.IsNullOrWhiteSpace(Com.Config.CustomerCountryList))      // Если код региона не определён
            {
                if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                    || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, Char(s.`max_disc_perc`) MAX_DISC_PERC, Char(s.`store_credit`) STORE_CREDIT,
  ap.`phone_no` As phone1, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid";
                }
                else
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, Char(s.`max_disc_perc`) MAX_DISC_PERC, Char(s.`store_credit`) STORE_CREDIT,
  ap.`phone_no` As phone1, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where ap.`phone_no` like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                }
            }
            else    // Если код региона указан
            {
                if (string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList)  // Если префикс телефона не определён
                    || (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)) // Или если определено несколько префиксов телефона
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, Char(s.`max_disc_perc`) MAX_DISC_PERC, Char(s.`store_credit`) STORE_CREDIT,
  ap.`phone_no` As phone1, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where coalesce(a.`country_sid`,0) in (" + Com.Config.CustomerCountryList + @")";
                }
                else
                {
                    CommandSql = @"select s.sid As cust_sid, s.first_name, s.last_name, s.cust_id, Char(s.`max_disc_perc`) MAX_DISC_PERC, Char(s.`store_credit`) STORE_CREDIT,
  ap.`phone_no` As phone1, a.`address_1` As address1, s.`first_sale_date` As FST_SALE_DATE, s.`last_sale_date` As lst_sale_date, ae.`email_address` As EMAIL_ADDR, p.SC_PERC
from `rpsods`.`customer` s
    left join `rpsods`.`customer_address` a on s.sid=a.cust_sid and a.seq_no=1
    left join `rpsods`.`customer_phone` ap on s.sid=ap.cust_sid and ap.seq_no=1
    left join `rpsods`.`customer_email` ae on s.sid=ae.cust_sid and ae.seq_no=1
    left join `aks`.`cust_sc_param` p on s.sid=p.cust_sid
Where coalesce(a.`country_sid`,0) in (" + Com.Config.CustomerCountryList + @")
    and ap.`phone_no` like '" + Com.Config.CustomerPrefixPhoneList.Trim() + @"%'";
                }
            }


            //CommandSql = "Select 1 cust_sid, 'Илья Михайлович' first_name, 'Погодин' last_name, '100001' cust_id, 5.5 MAX_DISC_PERC, '9163253757' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, 'ilia82@mail.ru' EMAIL_ADDR From Dual union Select 2 cust_sid, 'Константин' first_name, 'Чудаков' last_name, '100002' cust_id, 3.3 MAX_DISC_PERC, '91632' phone1, 'Москва...' address1, sysdate FST_SALE_DATE, sysdate lst_sale_date, null EMAIL_ADDR From Dual";

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersMySql", EventEn.Dump);

                bool rez = true;

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

                                    long tmpCustSid = -1;
                                    string tmpFirstName = null;
                                    string tmpLastName = null;
                                    string tmpCustId = null;
                                    decimal tmpMaxDiscPerc = 0;
                                    decimal tmpStoreCredit = 0;
                                    decimal tmpScPerc = 0;
                                    string tmpPhone1 = null;
                                    string tmpAddress1 = null;
                                    DateTime? tmpFstSaleDate = null;
                                    DateTime? tmpLstSaleDate = null;
                                    string tmpEmailAddr = null;
                                    int Active = 0;

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("first_name").ToUpper()) tmpFirstName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("last_name").ToUpper()) tmpLastName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_id").ToUpper()) tmpCustId = dr.GetValue(i).ToString();
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("MAX_DISC_PERC").ToUpper()) tmpMaxDiscPerc = decimal.Parse(dr.GetValue(i).ToString()); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("STORE_CREDIT").ToUpper()) tmpStoreCredit = decimal.Parse(dr.GetValue(i).ToString()); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("SC_PERC").ToUpper()) tmpScPerc = decimal.Parse(dr.GetValue(i).ToString()); }
                                        catch { }
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("phone1").ToUpper()) tmpPhone1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("address1").ToUpper()) tmpAddress1 = dr.GetValue(i).ToString();
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("FST_SALE_DATE").ToUpper()) tmpFstSaleDate = dr.GetDateTime(i); }
                                        catch { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("lst_sale_date").ToUpper()) tmpLstSaleDate = dr.GetDateTime(i); }
                                        catch { }
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("EMAIL_ADDR").ToUpper()) tmpEmailAddr = dr.GetValue(i).ToString();
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ACTIVE").ToUpper()) Active = int.Parse(dr.GetValue(i).ToString()); }
                                        catch { }
                                    }
                                    Customer nCust = new Customer(tmpCustSid, tmpFirstName, tmpLastName, tmpCustId, tmpMaxDiscPerc, tmpStoreCredit, tmpScPerc, tmpPhone1, tmpAddress1, tmpFstSaleDate, tmpLstSaleDate, tmpEmailAddr, Active);

                                    if (nCust.CustSid == -3834734573147189252)
                                    { }

                                    // Проверяем если в конфиге определено несколько префиксов к телефону то нужно их проверить и передать только если они найдены
                                    if (!string.IsNullOrWhiteSpace(Com.Config.CustomerPrefixPhoneList) && Com.Config.CustomerPrefixPhoneList.IndexOf(",") > -1)
                                    {
                                        string[] tmpCustomerPrefixPhoneList = Com.Config.CustomerPrefixPhoneList.Split(',');
                                        bool flag = false;
                                        foreach (string item in tmpCustomerPrefixPhoneList)
                                        {
                                            if (nCust.Phone1.Trim().IndexOf(item.Trim()) == 0) flag = true;
                                        }
                                        if (flag)
                                        {
                                            // Передаём добытого пользователя обработчику
                                            if (FuncTarget != null)
                                            {
                                                setActiveCustomerMySql(nCust);       // Предварительно проверяем активность клиента и если он не активный, принудительно выставляем флаг активности
                                                // Проверяем на валидность этого пользователя если валиден, то передаём его обработчику
                                                if (GetValidCustomerMySql(nCust)) rez = FuncTarget(nCust);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // Передаём добытого пользователя обработчику
                                        if (FuncTarget != null)
                                        {
                                            setActiveCustomerMySql(nCust);           // Предварительно проверяем активность клиента и если он не активный, принудительно выставляем флаг активности
                                            // Проверяем на валидность этого пользователя если валиден, то передаём его обработчику
                                            if (GetValidCustomerMySql(nCust)) rez = FuncTarget(nCust);
                                        }
                                    }

                                    // Проверяем необходимость продолжения дальнейшей работы
                                    if (!rez) { throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке передачи пользователя сид {0} обработчику Func<Customer, bool>", tmpCustSid)); }
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getCustumersMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getCustumersMySql", EventEn.Dump);
                throw;
            }
        }
        //
        /// <summary>
        /// Проверка статуса клиента active если 0, то нужно выставить 1
        /// </summary>
        /// <param name="Cst">Клиент у которого нужно сделать проверку на активность и если он не активный, принудительно выставить её.</param>
        private void setActiveCustomerMySql(Customer Cst)
        {
            if (Cst.Active == 0)
            {
                string CommandSql = string.Format("Update `rpsods`.`customer` Set ACTIVE=1 Where sid={0} and ACTIVE=0", Cst.CustSid);

                try
                {
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);

                    // Закрывать конект не нужно он будет закрыт деструктором
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                        {
                            com.CommandTimeout = 900;  // 15 минут
                            int dr = com.ExecuteNonQuery();
                        }
                    }
                }
                catch (OdbcException ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".setActiveCustomerMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".setActiveCustomerMySql", EventEn.Dump);
                    throw;
                }
            }
        }
        //
        /// <summary>
        /// Проверяем валидность этого пользователя, если он невалиден, то не передаём его обработчику, если валиден, то передаём 
        /// </summary>
        /// <param name="Cst">Клиент у которого нужно сделать проверку на валидность</param>
        /// <returns></returns>
        private bool GetValidCustomerMySql(Customer Cst)
        {
            bool rez = false;

            string CommandSql = string.Format(@"Select C.cust_sid As CUST_SID, C.FIRST_NAME, C.LAST_NAME, P.phone_no As PHONE 
From `aks`.`customer` C
  inner join `rpsods`.`customer_phone` P On C.cust_sid = P.cust_sid
Where C.cust_sid={0}", Cst.CustSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        using (OdbcDataReader dr = com.ExecuteReader())
                        {
                            long tmpCustSid = -1;
                            string tmpFirstName = null;
                            string tmpLastName = null;
                            string tmpPhone = null;

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
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("cust_sid").ToUpper()) tmpCustSid = long.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("first_name").ToUpper()) tmpFirstName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("last_name").ToUpper()) tmpLastName = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("phone").ToUpper()) tmpPhone = dr.GetValue(i).ToString();
                                    }

                                    // Проверяем наличие данных
                                    if (!string.IsNullOrWhiteSpace(tmpFirstName) && !string.IsNullOrWhiteSpace(tmpLastName) && !string.IsNullOrWhiteSpace(tmpPhone))
                                    {
                                        // Проверяем если есть разница, то мы должны записать в лог эту инфу и неразрешить передать обработчику
                                        if (Cst.FirstName != tmpFirstName || Cst.LastName != tmpLastName || Cst.Phone1 != tmpPhone)
                                        {
                                            Com.Log.EventSave(Com.Config.LogNotValidCustomer,
                                                string.Format(@"Обнаружено расхождение в клиенте CustSid={0}.
    Было:
        FirstName={1}
        LastName={2}
        Phone={3}
    Стало:
        FirstName={1}
        LastName={2}
        Phone={3}", Cst.CustSid, tmpFirstName, tmpLastName, tmpPhone, Cst.FirstName, Cst.LastName, Cst.Phone1),
                                                ".GetValidCustomerMySql", EventEn.Warning);
                                        }
                                        else rez = true;  // Если изменений нет, то передаём клиента обработчику
                                    }
                                }
                            }
                            else
                            {
                                if (Cst.CustSid != -1 && !string.IsNullOrWhiteSpace(Cst.FirstName) && !string.IsNullOrWhiteSpace(Cst.LastName) && !string.IsNullOrWhiteSpace(Cst.Phone1))
                                {
                                    CommandSql = string.Format(@"Insert into `aks`.`customer` (CUST_SID, FIRST_NAME, LAST_NAME, PHONE)
Values({0},'{1}','{2}','{3}')", Cst.CustSid, Cst.FirstName.Replace("'", "''"), Cst.LastName.Replace("'", "''"), Cst.Phone1.Replace("'", "''"));

                                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerMySql", EventEn.Dump);

                                    using (OdbcCommand com1 = new OdbcCommand(CommandSql, con))
                                    {
                                        com1.CommandTimeout = 900;  // 15 минут
                                        int dr1 = com1.ExecuteNonQuery();
                                    }

                                    rez = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".GetValidCustomerMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".GetValidCustomerMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetValidCustomerMySql", EventEn.Dump);
                throw;
            }


            return rez;
        }

        /// <summary>
        /// Заполнение справочника причин скидок
        /// </summary>
        /// <returns>Успех обработки функции</returns>
        private bool getDiscReasonsMySql()
        {
            string CommandSql = @"Select Distinct `sid` As DISC_REASON_ID, `name` As DISC_REASON_NAME From `rpsods`.`pref_reason` Where reason_type = 10";

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsMySql", EventEn.Dump);

                bool rez = true;

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
                                    Int64 tmpDscReasId = -1;
                                    string tmpDscReasName = null;
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i) == "DISC_REASON_ID") tmpDscReasId = Int64.Parse(dr.GetValue(i).ToString()); }
                                        catch (Exception) { }
                                        try { if (!dr.IsDBNull(i) && dr.GetName(i) == "DISC_REASON_NAME") tmpDscReasName = dr.GetValue(i).ToString(); }
                                        catch (Exception) { }
                                    }

                                    // Проверка полученных данных
                                    if (tmpDscReasId != -1 && !string.IsNullOrWhiteSpace(tmpDscReasName))
                                    {
                                        // Добавляем новую причину скидки
                                        DiscReason nDscReas = new DiscReason(tmpDscReasId, tmpDscReasName);
                                        rez = Com.DiscReasonFarm.List.Add(nDscReas, true);

                                        // Проверяем необходимость продолжения дальнейшей работы
                                        if (!rez) throw new ApplicationException(string.Format("Нет смысла продолжать дальше упали при попытке сохранения причины скидкиid {0}, Name {1}. ", nDscReas.DiscReasonId.ToString(), nDscReas.DiscReasonName));
                                    }
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDiscReasonsMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".getDiscReasonsMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".getDiscReasonsMySql", EventEn.Dump);
                throw;
            }

        }


        /// <summary>
        /// Объединение клиентов
        /// </summary>
        /// <param name="MergeClientMain">Основной клиент</param>
        /// <param name="MergeClientDonors">Клинеты доноры</param>
        private void MergeClientMySql(Customer MergeClientMain, List<Customer> MergeClientDonors)
        {
            string SQL = null;
            foreach (Customer item in MergeClientDonors)
            {
                // Правим таблицу CMS.INVOICE 
                SQL = string.Format(@"update CMS.INVOICE set CUST_SID={0}, SHIPTO_CUST_SID={0}
where CUST_SID={1}", MergeClientMain.CustSid, item.CustSid);
                try
                {
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);

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
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }


                // Правим таблицу CMS.customer 
                SQL = string.Format(@"update `rpsods`.`customer` set ACTIVE=1
where sid={0}", item.CustSid);
                try
                {
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);

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
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }


                // Правим таблицу CMS.cust_address
                SQL = string.Format(@"update `rpsods`.`customer_phone` set phone_no='+0'
where `seq_no`=1 
    and cust_sid={0}", item.CustSid);
                try
                {
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);

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
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
            }

            List<string> trankT = new List<string>();
            trankT.Add("`aks`.`customer`");
            trankT.Add("`aks`.`cust_sc_param`");
            trankT.Add("`aks`.`invc_sc_down`");
            trankT.Add("`aks`.`smtp_events`");

            foreach (string item in trankT)
            {
                SQL = string.Format(@"truncate table {0}", item);

                try
                {
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);

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
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при объединениии карточек. {0}", ex.Message), GetType().Name + ".MergeClientMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(SQL, GetType().Name + ".MergeClientMySql", EventEn.Dump);
                    throw;
                }
            }

        }


        /// <summary>
        /// Установка расчитанной скидки в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkMaxDiscPerc">Процент скидки который мы устанавливаем</param>
        /// <returns>Успех обработки функции</returns>
        private bool AployDMCalkMaxDiscPercMySql(CustomerBase Cst, decimal CalkMaxDiscPerc)
        {
            string CommandSql = string.Format("Update CMS.customer Set MAX_DISC_PERC={0} Where cust_sid={1}", CalkMaxDiscPerc.ToString().Replace(",", "."), Cst.CustSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercMySql", EventEn.Dump);

                bool rez = false;

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        int dr = com.ExecuteNonQuery();

                        // Проверяем кол-во обновлённых строк
                        if (dr > 0)
                        {
                            rez = true;
                        }
                        else
                        {
                            throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkMaxDiscPercMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Установка бонусного бала в базе у конкртеного клиента
        /// </summary>
        /// <param name="Cst">Клиент</param>
        /// <param name="CalkStoreCredit">Бонусный бал который мы устанавливаем</param>
        /// <param name="CalcScPerc">Процент по которому считался бонусный бал который мы устанавливаем</param>
        /// <param name="OldCalkStoreCredit">Старый бонусный бал который был до изменения</param>
        /// <param name="OldCalcScPerc">Старый процент по которому считался бонусный бал который был до изменения</param>
        /// <returns>Успех обработки функции</returns>
        private bool AployDMCalkStoreCreditMySql(CustomerBase Cst, decimal CalkStoreCredit, decimal CalcScPerc, decimal OldCalkStoreCredit, decimal OldCalcScPerc)
        {
            bool rez = false;
            string CommandSql = string.Format("Update CMS.customer Set Store_Credit={0} Where cust_sid={1}", CalkStoreCredit.ToString().Replace(",", "."), Cst.CustSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql", EventEn.Dump);

                // Закрывать конект не нужно он будет закрыт деструктором
                using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                {
                    con.Open();

                    using (OdbcCommand com = new OdbcCommand(CommandSql, con))
                    {
                        com.CommandTimeout = 900;  // 15 минут
                        int dr = com.ExecuteNonQuery();

                        // Проверяем кол-во обновлённых строк
                        if (dr > 0)
                        {
                            rez = true;
                        }
                        else
                        {
                            throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_1", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_1", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_1", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_1", EventEn.Dump);
                throw;
            }

            // Получаем пааметр в котором храниться значение имени поля для плагина BonusDMax
            string SC_Perc = Com.ConfigurationFarm.ParamsOfScenatiy["SC_Perc"];
            // сохранение параметра в базу в таблицу CMS.cust_address
            try
            {
                // Если параметр найден, то необходимо сохранить инфу в ещё одну таблицу
                if (!String.IsNullOrWhiteSpace(SC_Perc))
                {
                    string CommandSql2 = string.Format("Update CMS.cust_address Set {0}='{1}' Where CUST_SID={2}", SC_Perc, CalcScPerc.ToString(), Cst.CustSid);
                    rez = false;

                    // Закрывать конект не нужно он будет закрыт деструктором
                    bool flaginsert = false;
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql2, con))
                        {
                            com.CommandTimeout = 900;  // 15 минут
                            int dr = com.ExecuteNonQuery();

                            // Проверяем кол-во обновлённых строк
                            if (dr > 0)
                            {
                                rez = true;
                            }
                            else
                            {
                                flaginsert = true;
                            }
                        }
                    }

                    // Если обновление не прошло успешно так как строка не найдена, то будем делать вставку
                    if (flaginsert)
                    {
                        string CommandSql3 = string.Format("Insert into CMS.cust_address(CUST_SID, {0}) values({1},'{2}')", SC_Perc, Cst.CustSid, CalcScPerc.ToString());

                        using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                        {
                            con.Open();

                            using (OdbcCommand com = new OdbcCommand(CommandSql3, con))
                            {
                                com.CommandTimeout = 900;  // 15 минут
                                int dr = com.ExecuteNonQuery();

                                // Проверяем кол-во обновлённых строк
                                if (dr > 0)
                                {
                                    rez = true;
                                }
                                else
                                {
                                    throw new ApplicationException("Количество строк которое обновилось в базе менее 1.");
                                }
                            }
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_2", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_2", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_2", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_2", EventEn.Dump);
                throw;
            }

            try
            {
                if (Cst.CustSid == 2417549001015889916)
                { }

                // Если параметр найден, то необходимо сохранить инфу в ещё одну таблицу
                if (Com.Config.SaveSmtpEvent && CalkStoreCredit != OldCalkStoreCredit)
                {
                    string CommandSql3 = string.Format("Insert into `aks`.`smtp_events`(CUST_SID,EVENT_VALUE,EVENT_NAME) VALUES({0},{1},'AployDMCalkMaxDiscPerc')", Cst.CustSid, (CalkStoreCredit - OldCalkStoreCredit).ToString().Replace(",", "."));
                    rez = false;

                    // Закрывать конект не нужно он будет закрыт деструктором
                    using (OdbcConnection con = new OdbcConnection(base.ConnectionString))
                    {
                        con.Open();

                        using (OdbcCommand com = new OdbcCommand(CommandSql3, con))
                        {
                            int dr = com.ExecuteNonQuery();
                            rez = true;
                        }
                    }
                }
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_3", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_2", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при записи данных в источник. {0}", ex.Message), GetType().Name + ".AployDMCalkMaxDiscPercMySql_3", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".AployDMCalkStoreCreditMySql_2", EventEn.Dump);
                throw;
            }

            return rez;
        }
*/
        #endregion

    }
}
