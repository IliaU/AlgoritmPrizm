using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using AlgoritmPrizm.Com.Provider.Lib;
using AlgoritmPrizm.BLL;
using AlgoritmPrizm.Com;

namespace AlgoritmPrizm.Lib
{
    /// <summary>
    /// Универсальный провайдер
    /// </summary>
    public class UProvider : ProviderBase.UProviderBase, ProviderI
    {
        /// <summary>
        /// Базовый провайдер
        /// </summary>
        private ProviderBase PrvB;

        /// <summary>
        /// Интерфейс провайдера
        /// </summary>
        private ProviderI PrvI;

        /// <summary>
        /// Количество секунд между попытками
        /// </summary>
        private int ReturnTimeSqlSec = 3;

        /// <summary>
        /// Сколько попыток есть для повтора
        /// </summary>
        private int CountSql = 3;

        /// <summary>
        /// Тип провайдера
        /// </summary>
        public string PrvInType
        {
            get { return (this.PrvB == null ? null : this.PrvB.PlugInType.Name); }
            private set { }
        }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get { return this.PrvB.ConnectionString; }
            private set { }
        }

        /// <summary>
        /// Версия источника данных
        /// </summary>
        /// <returns>Возвращет значение версии источника данных в случае возможности получения подключения</returns>
        public string VersionDB
        {
            get { return this.PrvB.VersionDB; }
            private set { }
        }

        /// <summary>
        /// Возвращаем версию драйвера
        /// </summary>
        /// <returns></returns>
        public string Driver
        {
            get { return this.PrvB.Driver; }
            private set { }
        }

        /// <summary>
        /// Доступно ли подключение или нет
        /// </summary>
        /// <returns>true Если смогли подключиться к базе данных</returns>
        public bool HashConnect
        {
            get { return this.PrvB.HashConnect(); }
            private set { }
        }

        /// <summary>
        /// Проверка валидности подключения
        /// </summary>
        /// <returns>Возврощает результат проверки</returns>
        public bool testConnection()
        {
            return this.PrvI.testConnection();
        }

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        public string PrintConnectionString()
        {
            try
            {
                return this.PrvI.PrintConnectionString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получаем элемент меню для получения информации по плагину
        /// </summary>
        public ToolStripMenuItem InfoToolStripMenuItem()
        {
            return (this.PrvB == null ? null : this.PrvB.InfoToolStripMenuItem);
        }

        /// <summary>
        /// Конструктор по созданию универсального плагина
        /// </summary>
        /// <param name="PrvFullName">Имя плагина с которым хотим работать</param>
        /// <param name="ConnectionString">Строка подключения</param>
        public UProvider(string PrvFullName, string ConnectionString)
        {
            if (PrvFullName == null || PrvFullName.Trim() == string.Empty) throw new ApplicationException(string.Format("Не можем создать провайдер указанного типа: ({0})", PrvFullName == null ? "" : PrvFullName.Trim()));

            // Получаем инфу о класса 1 параметр полный путь например "EducationAnyProvider.Provider.MSSQL.MsSqlProvider", 2 параметр пропускать или не пропускать ошибки сейчас пропускаем, а третий учитывать или нет регистр из первого параметра
            //, если первый параметр нужно взять из другой зборки то сначала её загружаем Assembly asm = Assembly.LoadFrom("MyApp.exe"); а потом тоже самое только первый параметр кажется будет так "Reminder.Common.PLUGIN.MonitoringSetNedost, РЕШЕНИЕ" 
            Type myType = Type.GetType("AlgoritmPrizm.Com.Provider." + PrvFullName.Trim(), false, true);

            // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
            bool flagI = false;
            foreach (Type i in myType.GetInterfaces())
            {
                if (i.FullName == "AlgoritmPrizm.Com.Provider.Lib.ProviderI") flagI = true;
            }
            if (!flagI) throw new ApplicationException("Класс который вы грузите не реализовывает интерфейс (ProviderI)");

            // Проверяем что наш клас наследует PlugInBase 
            bool flagB = false;
            foreach (MemberInfo mi in myType.GetMembers())
            {
                if (mi.DeclaringType.FullName == "AlgoritmPrizm.Com.Provider.Lib.ProviderBase") flagB = true;
            }
            if (!flagB) throw new ApplicationException("Класс который вы грузите не наследует от класса ProviderBase");


            // Проверяем конструктор нашего класса  
            bool flag = false;
            string nameConstructor;
            foreach (ConstructorInfo ctor in myType.GetConstructors())
            {
                nameConstructor = myType.Name;

                // получаем параметры конструктора  
                ParameterInfo[] parameters = ctor.GetParameters();

                // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                if (parameters.Length == 1)
                {

                    if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;

                }
            }
            if (!flag) throw new ApplicationException("Класс который вы грузите не имеет конструктора (string ConnectionString)");

            // Создаём экземпляр объекта  
            object[] targ = { ConnectionString };
            object obj = Activator.CreateInstance(myType, targ);
            this.PrvB = (ProviderBase)obj;
            this.PrvI = (ProviderI)obj;

            base.UPoviderSetupForProviderBase(this.PrvB, this);
        }
        public UProvider(string PrvFullName)
            : this(PrvFullName, null)
        { }


        /// <summary>
        /// Метод для записи информации в лог
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Source">Источник</param>
        /// <param name="evn">Тип события</param>
        public void EventSave(string Message, string Source, EventEn evn)
        {
            this.PrvB.EventSave(Message, Source, evn);
        }

        /// <summary>
        /// Получаем список доступных провайдеров
        /// </summary>
        /// <returns>Список имён доступных провайдеров</returns>
        public static List<string> ListProviderName()
        {
            List<string> ProviderName = new List<string>();

            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "AlgoritmPrizm.Com.Provider").ToArray();


            foreach (Type item in typelist)
            {
                // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                bool flagI = false;
                foreach (Type i in item.GetInterfaces())
                {
                    if (i.FullName == "AlgoritmPrizm.Com.Provider.Lib.ProviderI") flagI = true;
                }
                if (!flagI) continue;

                // Проверяем что наш клас наследует PlugInBase 
                bool flagB = false;
                foreach (MemberInfo mi in item.GetMembers())
                {
                    if (mi.DeclaringType.FullName == "AlgoritmPrizm.Com.Provider.Lib.ProviderBase") flagB = true;
                }
                if (!flagB) continue;


                // Проверяем конструктор нашего класса  
                bool flag = false;
                string nameConstructor;
                foreach (ConstructorInfo ctor in item.GetConstructors())
                {
                    nameConstructor = item.Name;

                    // получаем параметры конструктора  
                    ParameterInfo[] parameters = ctor.GetParameters();

                    // если в этом конструктаре 1 параметр то проверяем тип и имя параметра  
                    if (parameters.Length == 1)
                    {

                        if (parameters[0].ParameterType.Name == "String" && parameters[0].Name == "ConnectionString") flag = true;

                    }
                }
                if (!flag) continue;

                ProviderName.Add(item.Name);
            }

            return ProviderName;
        }

        /// <summary>
        /// Процедура вызывающая настройку подключения
        /// </summary>
        /// <param name="Uprv">Ссылка на универсальный провайдер</param>
        /// <returns>Возвращает значение требуется ли сохранить подключение как основное или нет</returns>
        public bool SetupConnectDB()
        {
            return this.PrvI.SetupConnectDB();
        }

        /// <summary>
        /// Получение любых данных из источника например чтобы плагины могли что-то дополнительно читать
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        /// <returns>Результата В виде таблицы</returns>
        public DataTable getData(string SQL)
        {
            return this.PrvB.getData(SQL);
        }

        /// <summary>
        /// Выполнение любых запросов на источнике
        /// </summary>
        /// <param name="SQL">Собственно запрос</param>
        public void setData(string SQL)
        {
            this.PrvB.setData(SQL);
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
            this.PrvI.SetPrizmCustPorog(CustInn, InvcNo, PosDate, TotalCashSum);
        }

        /// <summary>
        /// Необходимо изменить количество в товаре так как произошла ошибка при печати
        /// </summary>
        /// <param name="ProductSid">Идентификатор товара</param>
        /// <param name="qty">Количетсов товара которое необходимо добавить к текущему количеству</param>
        /// <param name="AddQty"></param>
        public void SetQtyRollbackItem(string ProductSid, double qty)
        {
            this.PrvI.SetQtyRollbackItem(ProductSid, qty);
        }


        /// <summary>
        /// Установка признака отложенного чека в документе
        /// </summary>
        /// <param name="DocumentSid">Идентификатор товара</param>
        /// <param name="IsHeld">Признак отложенного чека (0 активный | 1 отложенный)</param>
        public void SetIsHelperForDocements(string DocumentSid, int IsHeld)
        {
            this.PrvI.SetIsHelperForDocements(DocumentSid, IsHeld);
        }

        /// <summary>
        /// Получить сумму по клиенту за дату
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="Dt">Дата смены</param>
        /// <returns>Сумму по клиенту за выбранную дату</returns>
        public decimal GetTotalCashSum(string CustInn, DateTime Dt)
        {
            return this.PrvI.GetTotalCashSum(CustInn, Dt);
        }

        /// <summary>
        /// Получить сумууму бонусов клиента
        /// </summary>
        /// <param name="CustSid">Идентификатор клиента</param>
        /// <returns>Бонусы доступные клиенту</returns>
        public decimal GetCustBon(string CustSid)
        {
            return this.PrvI.GetCustBon(CustSid);
        }

        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <returns>сами строки документа</returns>
        public List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrder(string referDocSid)
        {
            return this.GetItemsForReturnOrder(referDocSid, this.CountSql);
        }
        //
        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>сами строки документа</returns>
        public List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrder(string referDocSid, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetItemsForReturnOrder(referDocSid);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetItemsForReturnOrder(referDocSid, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения строк от сида документа который узнали: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetItemsForReturnOrder", EventEn.Error);
                    throw ae;
                }
            }
        }

        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public string GetInvnSbsItemNo(string InvnSbsItemSid)
        {
            return this.GetInvnSbsItemNo(InvnSbsItemSid, this.CountSql);
        }
        //
        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public string GetInvnSbsItemNo(string InvnSbsItemSid, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetInvnSbsItemNo(InvnSbsItemSid);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetInvnSbsItemNo(InvnSbsItemSid, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения номера карточки товара по её сиду: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetInvnSbsItemNo", EventEn.Error);
                    throw ae;
                }
            }
        }

        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Получаем номер документа</returns>
        public Int64 GetDocNoFromDocument(string sid)
        {
            return this.GetDocNoFromDocument(sid, this.CountSql);
        }
        //
        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>Получаем номер документа</returns>
        public Int64 GetDocNoFromDocument(string sid, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetDocNoFromDocument(sid);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetDocNoFromDocument(sid, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения номера документа из базы данных: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetDocNoFromDocument", EventEn.Error);
                    throw ae;
                }
            }
        }

        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public InvnSbsItemText GetInvnSbsItemText(string InvnSbsItemSid)
        {
            return this.GetInvnSbsItemText(InvnSbsItemSid, this.CountSql);
        }
        //
        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        public InvnSbsItemText GetInvnSbsItemText(string InvnSbsItemSid, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetInvnSbsItemText(InvnSbsItemSid);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetInvnSbsItemText(InvnSbsItemSid, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения содержимого полей text1-10 из карточки товаров: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetInvnSbsItemText", EventEn.Error);
                    throw ae;
                }
            }
        }

        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <returns>сама строка тендера</returns>
        public BLL.JsonPrintFiscDocTender GetTenderForReturnOrder(BLL.JsonPrintFiscDocTender itemTender)
        {
            return this.GetTenderForReturnOrder(itemTender, this.CountSql);
        }
        //
        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>сама строка тендера</returns>
        public BLL.JsonPrintFiscDocTender GetTenderForReturnOrder(BLL.JsonPrintFiscDocTender itemTender, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetTenderForReturnOrder(itemTender);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetTenderForReturnOrder(itemTender, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения возврата тендера из ссылки на документ указанного в линке массива tenders: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetTenderForReturnOrder", EventEn.Error);
                    throw ae;
                }
            }
        }

        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <returns>строки тендера из документа</returns>
        public List<BLL.JsonPrintFiscDocTender> GetTendersForDocument(string docsid)
        {
            return this.GetTendersForDocument(docsid, this.CountSql);
        }
        //
        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <param name="TmpCountSql">Количество оставшихся попыток</param>
        /// <returns>строки тендера из документа</returns>
        public List<BLL.JsonPrintFiscDocTender> GetTendersForDocument(string docsid, int TmpCountSql)
        {
            try
            {
                return this.PrvI.GetTendersForDocument(docsid);
            }
            catch (Exception ex)
            {
                if (TmpCountSql > 0)
                {
                    Thread.Sleep(this.ReturnTimeSqlSec * 1000);
                    this.PrvI.testConnection();
                    return this.GetTendersForDocument(docsid, TmpCountSql - 1);
                }
                else
                {
                    ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при повторной попытке получения возврата строки тендера по номеру документа: {0}", ex.Message));
                    Log.EventSave(ae.Message, "Com.UProvider.GetTendersForDocument", EventEn.Error);
                    throw ae;
                }
            }
        }
    }
}
