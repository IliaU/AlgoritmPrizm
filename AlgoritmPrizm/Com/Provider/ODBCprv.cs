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

using AlgoritmPrizm.BLL;

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
        /// Проверка валидности подключения
        /// </summary>
        /// <returns>Возврощает результат проверки</returns>
        public bool testConnection()
        {
            try
            {
                testConnection(base.ConnectionString, false);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        /// Необходимо изменить количество в товаре так как произошла ошибка при печати
        /// </summary>
        /// <param name="ProductSid">Идентификатор товара</param>
        /// <param name="qty">Количетсов товара которое необходимо добавить к текущему количеству</param>
        /// <param name="AddQty"></param>
        public void SetQtyRollbackItem(string ProductSid, double qty)
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
                            SetQtyRollbackItemORA(ProductSid, qty);
                            break;
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            SetQtyRollbackItemMySql(ProductSid, qty);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "SetQtyRollbackItem", EventEn.Error);

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

        /// <summary>
        /// Получить сумууму бонусов клиента
        /// </summary>
        /// <param name="CustSid">Идентификатор клиента</param>
        /// <returns>Бонусы доступные клиенту</returns>
        public decimal GetCustBon(string CustSid)
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
                            return GetCustBonORA(CustSid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetCustBonMySql(CustSid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetCustBon", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return 0;
            }
        }

        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <returns>сами строки документа</returns>
        public List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrder(string referDocSid)
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
                            return GetItemsForReturnOrderORA(referDocSid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetItemsForReturnOrderMySql(referDocSid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetItemsForReturnOrder", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public string GetInvnSbsItemNo(string InvnSbsItemSid)
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
                            return GetInvnSbsItemNoORA(InvnSbsItemSid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetInvnSbsItemNoMySql(InvnSbsItemSid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetInvnSbsItemNo", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Получаем номер документа</returns>
        public Int64 GetDocNoFromDocument(string sid)
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
                            return GetDocNoFromDocumentORA(sid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetDocNoFromDocumentMySql(sid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetDocNoFromDocument", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return 0;
            }
        }

        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public InvnSbsItemText GetInvnSbsItemText(string InvnSbsItemSid)
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
                            return GetInvnSbsItemTextORA(InvnSbsItemSid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetInvnSbsItemTextMySql(InvnSbsItemSid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetInvnSbsItemText", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <returns>сама строка тендера</returns>
        public BLL.JsonPrintFiscDocTender GetTenderForReturnOrder(BLL.JsonPrintFiscDocTender itemTender)
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
                            return GetTenderForReturnOrderORA(itemTender);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetTenderForReturnOrderMySql(itemTender);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetTenderForReturnOrder", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <returns>строки тендера из документа</returns>
        public List<BLL.JsonPrintFiscDocTender> GetTendersForDocument(string docsid)
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
                            return GetTendersForDocumentORA(docsid);
                        case "myodbc8a.dll":
                        case "myodbc8w.dll":
                            return GetTendersForDocumentMySql(docsid);
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
                if (Com.Config.Trace) base.EventSave(ex.Message, "GetTendersForDocument", EventEn.Error);

                // Отображаем ошибку если это нужно
                MessageBox.Show(ex.Message);

                return null;
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
        /// Необходимо изменить количество в товаре так как произошла ошибка при печати
        /// </summary>
        /// <param name="ProductSid">Идентификатор товара</param>
        /// <param name="qty">Количетсов товара которое необходимо добавить к текущему количеству</param>
        /// <param name="AddQty"></param>
        public void SetQtyRollbackItemORA(string ProductSid, double qty)
        {
            string CommandSql = String.Format(@"update rpsods.invn_sbs_item_qty Set qty=qty+{1} Where Sid='{0}'", ProductSid, qty);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemORA", EventEn.Dump);

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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetQtyRollbackItemORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetQtyRollbackItemORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemORA", EventEn.Dump);
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

        /// <summary>
        /// Получить сумууму бонусов клиента
        /// </summary>
        /// <param name="CustSid">Идентификатор клиента</param>
        /// <returns>Бонусы доступные клиенту</returns>
        public decimal GetCustBonORA(string CustSid)
        {
            string CommandSql = String.Format(@"Select CALL_OFF_SC 
From AKS.CUST_SC_PARAM
Where dt=To_Date('{1}.{2}.{3}', 'YYYY.MM.DD')
    and CUST_SID='{0}'", CustSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonORA", EventEn.Dump);

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
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CALL_OFF_SC").ToUpper()) rez = decimal.Parse(dr.GetValue(i).ToString());
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetCustBonORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetCustBonORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <returns>сами строки документа</returns>
        public List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrderORA(string referDocSid)
        {
            string CommandSql = String.Format(@"SELECT i.qty, i.price, i.tax_perc, i.scan_upc, i.description1, i.description2, i.description3, i.description4, i.sid As itemsid,
	i.note1, i.note2, i.note3, i.note4, i.note5, i.note6, i.note7, i.note8, i.note9, i.note10, i.disc_amt
from document d
	inner join document_item i On d.sid=i.doc_sid
where d.sid ={0}", referDocSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderORA", EventEn.Dump);

                List<BLL.JsonPrintFiscDocItem> rez = new List<BLL.JsonPrintFiscDocItem>();

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("QTY").ToUpper()) nitem.quantity = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("PRICE").ToUpper()) nitem.price = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAX_PERC").ToUpper()) nitem.tax_percent = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAX_PERC").ToUpper()) nitem.tax_percent = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("SCAN_UPC").ToUpper()) nitem.scan_upc = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION1").ToUpper()) nitem.item_description1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION2").ToUpper()) nitem.item_description2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION3").ToUpper()) nitem.item_description3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION4").ToUpper()) nitem.item_description4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEMSID").ToUpper()) nitem.sid = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE1").ToUpper()) nitem.note1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE2").ToUpper()) nitem.note2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE3").ToUpper()) nitem.note3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE4").ToUpper()) nitem.note4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE5").ToUpper()) nitem.note5 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE6").ToUpper()) nitem.note6 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE7").ToUpper()) nitem.note7 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE8").ToUpper()) nitem.note8 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE9").ToUpper()) nitem.note9 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE10").ToUpper()) nitem.note10 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DISC_AMT").ToUpper()) nitem.discount_amt = double.Parse(dr.GetValue(i).ToString());
                                    }
                                    rez.Add(nitem);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetItemsForReturnOrderORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetItemsForReturnOrderORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public string GetInvnSbsItemNoORA(string InvnSbsItemSid)
        {
            string CommandSql = String.Format(@"SELECT item_no
from rpsods.invn_sbs_item
where sid ={0}", InvnSbsItemSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoORA", EventEn.Dump);

                string rez = null;

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("item_no").ToUpper()) rez = dr.GetValue(i).ToString();
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemNoORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemNoORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Получаем номер документа</returns>
        public Int64 GetDocNoFromDocumentORA(string sid)
        {
            string CommandSql = String.Format(@"select doc_no 
from rpsods.document 
where sid = {0}", sid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentORA", EventEn.Dump);

                Int64 rez = 0;

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("doc_no").ToUpper()) try { rez = Int64.Parse(dr.GetValue(i).ToString()); } catch (Exception) { }
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDocNoFromDocumentORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDocNoFromDocumentORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public InvnSbsItemText GetInvnSbsItemTextORA(string InvnSbsItemSid)
        {
            string CommandSql = String.Format(@"SELECT text1, text2, text3, text4, text5, text6, text7, text8, text9, text10
from rpsods.invn_sbs_item
where sid ={0}", InvnSbsItemSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextORA", EventEn.Dump);

                InvnSbsItemText rez = null;

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
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text1").ToUpper()) rez.Text1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text2").ToUpper()) rez.Text2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text3").ToUpper()) rez.Text3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text4").ToUpper()) rez.Text4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text5").ToUpper()) rez.Text5 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text6").ToUpper()) rez.Text6 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text7").ToUpper()) rez.Text7 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text8").ToUpper()) rez.Text8 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text9").ToUpper()) rez.Text9 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text10").ToUpper()) rez.Text10 = dr.GetValue(i).ToString();
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemTextORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemTextORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <returns>сама строка тендера</returns>
        public BLL.JsonPrintFiscDocTender GetTenderForReturnOrderORA(BLL.JsonPrintFiscDocTender itemTender)
        {
            string referLink = itemTender.link.Substring(itemTender.link.IndexOf(@"/tender/") + 8);

            string CommandSql = String.Format(@"SELECT t.taken, t.tender_name, t.given, t.amount, t.tender_pos, t.currency_name
FROM rpsods.tender t 
Where t.sid ='{0}'", referLink);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderORA", EventEn.Dump);

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
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAKEN").ToUpper()) itemTender.taken = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_NAME").ToUpper()) itemTender.tender_name = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("GIVEN").ToUpper()) itemTender.given = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("AMOUNT").ToUpper()) itemTender.amount = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_POS").ToUpper()) itemTender.tender_pos = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CURRENCY_NAME").ToUpper()) itemTender.currency_name = dr.GetValue(i).ToString();
                                    }
                                    return itemTender;
                                }
                            }
                        }
                    }
                }

                return itemTender;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTenderForReturnOrderORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTenderForReturnOrderORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderORA", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <returns>строки тендера из документа</returns>
        public List<BLL.JsonPrintFiscDocTender> GetTendersForDocumentORA(string docsid)
        {
            List<BLL.JsonPrintFiscDocTender> rez = new List<JsonPrintFiscDocTender>();

            string CommandSql = String.Format(@"SELECT t.taken, t.tender_name, t.given, t.amount, t.tender_pos, t.currency_name
FROM rpsods.tender t 
Where t.doc_sid ='{0}'", docsid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentORA", EventEn.Dump);

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
                                    BLL.JsonPrintFiscDocTender nitem = new JsonPrintFiscDocTender();
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAKEN").ToUpper()) nitem.taken = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_NAME").ToUpper()) nitem.tender_name = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("GIVEN").ToUpper()) nitem.given = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("AMOUNT").ToUpper()) nitem.amount = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_POS").ToUpper()) nitem.tender_pos = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CURRENCY_NAME").ToUpper()) nitem.currency_name = dr.GetValue(i).ToString();
                                    }
                                    rez.Add(nitem);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTendersForDocumentORA", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentORA", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTendersForDocument", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentORA", EventEn.Dump);
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
            string CommandSql = String.Format(@"insert into `aks`.`prizm_cust_porog`(`cust_inn`,`invc_no`,`dt`,`pos_date`, `total_cash_sum`) Values('{0}', {1}, STR_TO_DATE('{2},{3},{4}', '%Y,%m,%d'), STR_TO_DATE('{2},{3},{4} {5},{6},{7}', '%Y,%m,%d %H,%i,%s'), {8})", CustInn, InvcNo, PosDate.Year, PosDate.Month, PosDate.Day, PosDate.Hour, PosDate.Minute, PosDate.Second, TotalCashSum.ToString().Replace(',', '.'));

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
        /// Необходимо изменить количество в товаре так как произошла ошибка при печати
        /// </summary>
        /// <param name="ProductSid">Идентификатор товара</param>
        /// <param name="qty">Количетсов товара которое необходимо добавить к текущему количеству</param>
        /// <param name="AddQty"></param>
        public void SetQtyRollbackItemMySql(string ProductSid, double qty)
        {
            string CommandSql = String.Format(@"update `rpsods`.`invn_sbs_item_qty` Set qty=qty+{1} Where Sid='{0}'", ProductSid, qty);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemMySql", EventEn.Dump);

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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetQtyRollbackItemMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".SetQtyRollbackItemMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".SetQtyRollbackItemMySql", EventEn.Dump);
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

        /// <summary>
        /// Получить сумууму бонусов клиента
        /// </summary>
        /// <param name="CustSid">Идентификатор клиента</param>
        /// <returns>Бонусы доступные клиенту</returns>
        public decimal GetCustBonMySql(string CustSid)
        {
            string CommandSql = String.Format(@"Select `CALL_OFF_SC` 
From `aks`.`cust_sc_param`
Where dt=To_Date('{1}.{2}.{3}', 'YYYY.MM.DD')
    and `CUST_SID`='{0}'", CustSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonMySql", EventEn.Dump);

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
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CALL_OFF_SC").ToUpper()) rez = decimal.Parse(dr.GetValue(i).ToString());
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetCustBonMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetCustBonMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetCustBonMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <returns>сами строки документа</returns>
        public List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrderMySql(string referDocSid)
        {
            string CommandSql = String.Format(@"SELECT i.qty, i.price, i.tax_perc, i.scan_upc, i.description1, i.description2, i.description3, i.description4, i.sid As itemsid,
	i.note1, i.note2, i.note3, i.note4, i.note5, i.note6, i.note7, i.note8, i.note9, i.note10, i.disc_amt,
    i.invn_sbs_item_sid
from `rpsods`.`document` d
	inner join `rpsods`.`document_item` i On d.sid=i.doc_sid
where d.sid ={0}", referDocSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderMySql", EventEn.Dump);

                List<BLL.JsonPrintFiscDocItem> rez = new List<BLL.JsonPrintFiscDocItem>();

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("QTY").ToUpper()) nitem.quantity = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("PRICE").ToUpper()) nitem.price = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAX_PERC").ToUpper()) nitem.tax_percent = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAX_PERC").ToUpper()) nitem.tax_percent = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("SCAN_UPC").ToUpper()) nitem.scan_upc = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION1").ToUpper()) nitem.item_description1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION2").ToUpper()) nitem.item_description2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION3").ToUpper()) nitem.item_description3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DESCRIPTION4").ToUpper()) nitem.item_description4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("ITEMSID").ToUpper()) nitem.sid = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE1").ToUpper()) nitem.note1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE2").ToUpper()) nitem.note2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE3").ToUpper()) nitem.note3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE4").ToUpper()) nitem.note4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE5").ToUpper()) nitem.note5 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE6").ToUpper()) nitem.note6 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE7").ToUpper()) nitem.note7 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE8").ToUpper()) nitem.note8 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE9").ToUpper()) nitem.note9 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("NOTE10").ToUpper()) nitem.note10 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("DISC_AMT").ToUpper()) nitem.discount_amt = double.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("INVN_SBS_ITEM_SID").ToUpper()) nitem.invn_sbs_item_sid = dr.GetValue(i).ToString();
                                    }
                                    rez.Add(nitem);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetItemsForReturnOrderMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetItemsForReturnOrderMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetItemsForReturnOrderMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public string GetInvnSbsItemNoMySql(string InvnSbsItemSid)
        {
            string CommandSql = String.Format(@"SELECT item_no
from `rpsods`.`invn_sbs_item`
where sid ={0}", InvnSbsItemSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoMySql", EventEn.Dump);

                string rez = null;

                if (string.IsNullOrWhiteSpace(InvnSbsItemSid)) return rez;

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("item_no").ToUpper()) rez = dr.GetValue(i).ToString();
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemNoMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemNoMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemNoMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Получаем номер документа</returns>
        public Int64 GetDocNoFromDocumentMySql(string sid)
        {
            string CommandSql = String.Format(@"select doc_no 
from `rpsods`.`document`
where sid = {0}", sid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentMySql", EventEn.Dump);

                Int64 rez = 0;

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
                                    BLL.JsonPrintFiscDocItem nitem = new BLL.JsonPrintFiscDocItem();

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("doc_no").ToUpper()) try { rez = Int64.Parse(dr.GetValue(i).ToString()); } catch (Exception) { }
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
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDocNoFromDocumentMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetDocNoFromDocumentMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetDocNoFromDocumentMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public InvnSbsItemText GetInvnSbsItemTextMySql(string InvnSbsItemSid)
        {
            string CommandSql = String.Format(@"SELECT `sid`, `description1`,`description2`, `attribute`, `upc`, `item_size`, `item_no`, `text1`, `text2`, `text3`, `text4`, `text5`, `text6`, `text7`, `text8`, `text9`, `text10`
from `rpsods`.`invn_sbs_item`
where sid ={0}", InvnSbsItemSid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextMySql", EventEn.Dump);

                InvnSbsItemText rez = null;

                if (string.IsNullOrWhiteSpace(InvnSbsItemSid)) return rez;

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
                                    string sid = null;
                                    string description1 = null;
                                    string description2 = null;
                                    string attribute = null;
                                    string upc = null;
                                    string item_size = null;
                                    string item_no = null;
                                    string Text1 = null;
                                    string Text2 = null;
                                    string Text3 = null;
                                    string Text4 = null;
                                    string Text5 = null;
                                    string Text6 = null;
                                    string Text7 = null;
                                    string Text8 = null;
                                    string Text9 = null;
                                    string Text10 = null;

                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("sid").ToUpper()) sid = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("description1").ToUpper()) description1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("description2").ToUpper()) description2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("attribute").ToUpper()) attribute = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("upc").ToUpper()) upc = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("item_size").ToUpper()) item_size = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("item_no").ToUpper()) item_no = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text1").ToUpper()) Text1 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text2").ToUpper()) Text2 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text3").ToUpper()) Text3 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text4").ToUpper()) Text4 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text5").ToUpper()) Text5 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text6").ToUpper()) Text6 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text7").ToUpper()) Text7 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text8").ToUpper()) Text8 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text9").ToUpper()) Text9 = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("text10").ToUpper()) Text10 = dr.GetValue(i).ToString();
                                    }

                                    rez = new InvnSbsItemText(sid, description1, description2, attribute, upc, item_size, item_no, Text1, Text2, Text3, Text4, Text5, Text6, Text7, Text8, Text9, Text10);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemTextMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetInvnSbsItemTextMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetInvnSbsItemTextMySql", EventEn.Dump);
                throw;
            }
        }

        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <returns>сама строка тендера</returns>
        public BLL.JsonPrintFiscDocTender GetTenderForReturnOrderMySql(BLL.JsonPrintFiscDocTender itemTender)
        {
            {
                string referLink = itemTender.link.Substring(itemTender.link.IndexOf(@"/tender/") + 8);

                string CommandSql = String.Format(@"SELECT t.taken, t.tender_name, t.given, t.amount, t.tender_pos, t.currency_name
FROM rpsods.tender t 
Where t.sid ='{0}'", referLink);

                try
                {
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderMySql", EventEn.Dump);

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
                                        for (int i = 0; i < dr.FieldCount; i++)
                                        {
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAKEN").ToUpper()) itemTender.taken = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_NAME").ToUpper()) itemTender.tender_name = dr.GetValue(i).ToString();
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("GIVEN").ToUpper()) itemTender.given = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("AMOUNT").ToUpper()) itemTender.amount = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_POS").ToUpper()) itemTender.tender_pos = int.Parse(dr.GetValue(i).ToString());
                                            if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CURRENCY_NAME").ToUpper()) itemTender.currency_name = dr.GetValue(i).ToString();
                                        }
                                        return itemTender;
                                    }
                                }
                            }
                        }
                    }

                    return itemTender;
                }
                catch (OdbcException ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTenderForReturnOrderMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderMySql", EventEn.Dump);
                    throw;
                }
                catch (Exception ex)
                {
                    base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTenderForReturnOrderMySql", EventEn.Error);
                    if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTenderForReturnOrderMySql", EventEn.Dump);
                    throw;
                }
            }
        }

        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <returns>строки тендера из документа</returns>
        public List<BLL.JsonPrintFiscDocTender> GetTendersForDocumentMySql(string docsid)
        {
            List<BLL.JsonPrintFiscDocTender> rez = new List<JsonPrintFiscDocTender>();

            string CommandSql = String.Format(@"SELECT t.taken, t.tender_name, t.given, t.amount, t.tender_pos, t.currency_name
FROM rpsods.tender t 
Where t.doc_sid ='{0}'", docsid);

            try
            {
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentMySql", EventEn.Dump);

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
                                    BLL.JsonPrintFiscDocTender nitem = new JsonPrintFiscDocTender();
                                    for (int i = 0; i < dr.FieldCount; i++)
                                    {
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TAKEN").ToUpper()) nitem.taken = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_NAME").ToUpper()) nitem.tender_name = dr.GetValue(i).ToString();
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("GIVEN").ToUpper()) nitem.given = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("AMOUNT").ToUpper()) nitem.amount = double.Parse(dr.GetValue(i).ToString().Replace(".", ","));
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("TENDER_POS").ToUpper()) nitem.tender_pos = int.Parse(dr.GetValue(i).ToString());
                                        if (!dr.IsDBNull(i) && dr.GetName(i).ToUpper() == ("CURRENCY_NAME").ToUpper()) nitem.currency_name = dr.GetValue(i).ToString();
                                    }
                                    rez.Add(nitem);
                                }
                            }
                        }
                    }
                }

                return rez;
            }
            catch (OdbcException ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTendersForDocumentMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentMySql", EventEn.Dump);
                throw;
            }
            catch (Exception ex)
            {
                base.EventSave(string.Format("Произожла ошибка при получении данных с источника. {0}", ex.Message), GetType().Name + ".GetTendersForDocumentMySql", EventEn.Error);
                if (Com.Config.Trace) base.EventSave(CommandSql, GetType().Name + ".GetTendersForDocumentMySql", EventEn.Dump);
                throw;
            }
        }
        #endregion

    }
}
