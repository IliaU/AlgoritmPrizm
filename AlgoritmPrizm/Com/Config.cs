using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Класс для работы с конфигурационным фалом
    /// </summary>
    public class Config
    {
        #region Private Param
        private static Config obj = null;

        /// <summary>
        /// Версия XML файла
        /// </summary>
        private static int _Version = 1;

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        private static bool _Trace = false;

        /// <summary>
        /// Хост на котором будет крутиться вебсервис
        /// </summary>
        private static string _Host = Environment.MachineName;

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        private static int _Port = 5000;

        /// <summary>
        /// Список продавцов
        /// </summary>
        private static List<Custumer> _customers = new List<Custumer>();

        /// <summary>
        /// Объект XML файла
        /// </summary>
        private static XmlDocument Document = new XmlDocument();

        /// <summary>
        /// Корневой элемент нашего документа
        /// </summary>
        private static XmlElement xmlRoot;

        /// <summary>
        /// Корневой элемент лицензий
        /// </summary>
        private static XmlElement xmlLics;

        /// <summary>
        /// Корневой элемент наших продавцов в настроечном файле
        /// </summary>
        private static XmlElement xmlCustomers;

        /// <summary>
        /// Корневой элемент нашего списка со списком классов
        /// </summary>
        private static XmlElement xmlProdictMatrixClassList;

        /// <summary>
        /// Идентификатор типа оплаты за нал
        /// </summary>
        private static int _TenderTypeCash = 0;

        /// <summary>
        /// Идентификатор типа оплаты за карта
        /// </summary>
        private static int _TenderTypeCredit = 2;

        /// <summary>
        /// Идентификатор типа оплаты подарочный сертификат
        /// </summary>
        private static int _TenderTypeGiftCert = 9;


        /// <summary>
        /// Идентификатор типа оплаты подарочная карта
        /// </summary>
        private static int _TenderTypeGiftCard = 10;

        /// <summary>
        /// Код подарочной карты
        /// </summary>
        private static string _GiftCardCode = "GFT";

        /// <summary>
        /// Включение отключение использоания подарочных карт
        /// </summary>
        private static bool _GiftCardEnable = false;

        /// <summary>
        /// Группа налогов при использовании подарочных карт
        /// </summary>
        private static int _GiftCardTax = 5;

        /// <summary>
        /// Версия фискальной памяти
        /// </summary>
        private static FfdEn _Ffd = FfdEn.v1_05;

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        private static int _FrPort = 3;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem = FieldItemEn.Description1;

        /// <summary>
        /// Наименование поля с номером ФД
        /// </summary>
        private static FieldDocNumEn _FieldDocNum = FieldDocNumEn.Comment1;

        /// <summary>
        /// Хост с приложением Prizm на котором развёрнут API
        /// </summary>
        private static string _HostPrizmApi= "http://172.16.1.102";

        /// <summary>
        /// Login системного пользовательеля для доступа к api
        /// </summary>
        private static string _PrizmApiSystemLogon = "sysadmin";

        /// <summary>
        /// Password системного пользовательеля для доступа к api
        /// </summary>
        private static string _PrizmApiSystemPassord = "";

        /// <summary>
        /// Тайм аут жизни токена после которого требуется обновление токена
        /// </summary>
        private static int _PrizmApiTimeLiveTockenMinute = 5;

        /// <summary>
        /// Файл для лога чеков
        /// </summary>
        private static string _FileCheckLog = "AlgoritmSale.Log";

        /// <summary>
        /// Всегда требовать запрос матрикс кода
        /// </summary>
        private static bool _GetMatrixAlways = false;

        /// <summary>
        /// Класс продукта принимаем решение нужно ли выдовать запрос матрикс кода и обязательный параметр матрикс код или нет
        /// </summary>
        private static List<ProdictMatrixClass> _ProdictMatrixClassList = new List<ProdictMatrixClass>();

        /// <summary>
        /// Поведение при запросе матрикс кода если он не найтен в текущих группах товаров и не стоит галочка GetMatrixAlways=true
        /// </summary>
        private static bool _MandatoryDefault = false;
        #endregion

        #region Public Param

        /// <summary>
        /// Файл в котором мы храним конфиг
        /// </summary>
        public static string FileXml { get; private set; }

        /// <summary>
        /// Версия XML файла
        /// </summary>
        public static int Version { get { return _Version; } private set { } }

        /// <summary>
        /// Флаг трассировки
        /// </summary>
        public static bool Trace
        {
            get
            {
                return _Trace;
            }
            set
            {
                xmlRoot.SetAttribute("Trace", value.ToString());
                Save();
                _Trace = value;
            }
        }

        /// <summary>
        /// Хост на котором будет крутиться вебсервис
        /// </summary>
        public static string Host
        {
            get
            {
                return _Host;
            }
            set
            {
                xmlRoot.SetAttribute("Host", value.ToString());
                Save();
                _Host = value;
            }
        }

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        public static int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                xmlRoot.SetAttribute("Port", value.ToString());
                Save();
                _Port = value;
            }
        }

        /// <summary>
        /// Идентификатор типа оплаты за нал
        /// </summary>
        public static int TenderTypeCash
        {
            get
            {
                return _TenderTypeCash;
            }
            set
            {
                xmlRoot.SetAttribute("TenderTypeCash", value.ToString());
                Save();
                _Port = value;
            }
        }

        /// <summary>
        /// Идентификатор типа оплаты за карта
        /// </summary>
        public static int TenderTypeCredit
        {
            get
            {
                return _TenderTypeCredit;
            }
            set
            {
                xmlRoot.SetAttribute("TenderTypeCredit", value.ToString());
                Save();
                _Port = value;
            }
        }

        /// <summary>
        /// Идентификатор типа оплаты подарочный сертификат
        /// </summary>
        public static int TenderTypeGiftCert
        {
            get
            {
                return _TenderTypeGiftCert;
            }
            set
            {
                xmlRoot.SetAttribute("TenderTypeGiftCert", value.ToString());
                Save();
                _Port = value;
            }
        }

        /// <summary>
        /// Идентификатор типа оплаты подарочная карта
        /// </summary>
        public static int TenderTypeGiftCard
        {
            get
            {
                return _TenderTypeGiftCard;
            }
            set
            {
                xmlRoot.SetAttribute("TenderTypeGiftCard", value.ToString());
                Save();
                _Port = value;
            }
        }

        /// <summary>
        /// Список продавцов
        /// </summary>
        public static List<Custumer> customers
        {
            get
            {
                return _customers;
            }
            private set {}
        }

        /// <summary>
        /// Код подарочной карты
        /// </summary>
        public static string GiftCardCode
        {
            get
            {
                return _GiftCardCode;
            }
            set
            {
                xmlRoot.SetAttribute("GiftCardCode", value.ToString());
                Save();
                _GiftCardCode = value;
            }
        }

        /// <summary>
        /// Включение отключение использоания подарочных карт
        /// </summary>
        public static bool GiftCardEnable
        {
            get
            {
                return _GiftCardEnable;
            }
            set
            {
                xmlRoot.SetAttribute("GiftCardEnable", value.ToString());
                Save();
                _GiftCardEnable = value;
            }
        }

        /// <summary>
        /// Группа налогов при использовании подарочных карт
        /// </summary>
        public static int GiftCardTax
        {
            get
            {
                return _GiftCardTax;
            }
            set
            {
                xmlRoot.SetAttribute("GiftCardTax", value.ToString());
                Save();
                _GiftCardTax = value;
            }
        }

        /// <summary>
        /// Версия фискальной памяти
        /// </summary>
        public static FfdEn Ffd
        {
            get
            {
                return _Ffd;
            }
            set
            {
                FfdEn val = EventConvertor.Convert(value.ToString(), _Ffd);
                xmlRoot.SetAttribute("Ffd", val.ToString());
                Save();
                _Ffd = val;
            }
        }

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        public static int FrPort
        {
            get
            {
                return _FrPort;
            }
            set
            {
                xmlRoot.SetAttribute("FrPort", value.ToString());
                Save();
                _FrPort = value;
            }
        }

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem
        {
            get
            {
                return _FieldItem;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem", value.ToString());
                Save();
                _FieldItem = value;
            }
        }

        /// <summary>
        /// Наименование поля с номером ФД
        /// </summary>
        public static FieldDocNumEn FieldDocNum
        {
            get
            {
                return _FieldDocNum;
            }
            set
            {
                xmlRoot.SetAttribute("FieldDocNum", value.ToString());
                Save();
                _FieldDocNum = value;
            }
        }

        /// <summary>
        /// Хост с приложением Prizm на котором развёрнут API
        /// </summary>
        public static string HostPrizmApi
        {
            get
            {
                return _HostPrizmApi;
            }
            set
            {
                xmlRoot.SetAttribute("HostPrizmApi", value.ToString());
                Save();
                _HostPrizmApi = value;
            }
        }

        /// <summary>
        /// Login системного пользовательеля для доступа к api
        /// </summary>
        public static string PrizmApiSystemLogon
        {
            get
            {
                return _PrizmApiSystemLogon;
            }
            set
            {
                xmlRoot.SetAttribute("PrizmApiSystemLogon", value.ToString());
                Save();
                _PrizmApiSystemLogon = value;
            }
        }

        /// <summary>
        /// Password системного пользовательеля для доступа к api
        /// </summary>
        public static string PrizmApiSystemPassord
        {
            get
            {
                return _PrizmApiSystemPassord;
            }
            set
            {
                string pwd = Lic.InCode(value.ToString());
                xmlRoot.SetAttribute("PrizmApiSystemPassord", pwd);
                Save();
                _PrizmApiSystemPassord = pwd;
            }
        }

        /// <summary>
        /// Тайм аут жизни токена после которого требуется обновление токена
        /// </summary>
        public static int PrizmApiTimeLiveTockenMinute
        {
            get
            {
                return _PrizmApiTimeLiveTockenMinute;
            }
            set
            {
                xmlRoot.SetAttribute("PrizmApiTimeLiveTockenMinute", value.ToString());
                Save();
                _PrizmApiTimeLiveTockenMinute = value;
            }
        }

        /// <summary>
        /// Файл для лога чеков
        /// </summary>
        public static string FileCheckLog
        {
            get
            {
                return _FileCheckLog;
            }
            set
            {
                xmlRoot.SetAttribute("FileCheckLog", value.ToString());
                Save();
                _FileCheckLog = value.ToString();
            }
        }

        /// <summary>
        /// Всегда требовать запрос матрикс кода
        /// </summary>
        public static bool GetMatrixAlways
        {
            get
            {
                return _GetMatrixAlways;
            }
            set
            {
                xmlRoot.SetAttribute("GetMatrixAlways", value.ToString());
                Save();
                _GetMatrixAlways = value;
            }
        }

        /// <summary>
        /// Класс продукта принимаем решение нужно ли выдовать запрос матрикс кода и обязательный параметр матрикс код или нет
        /// </summary>
        public static List<ProdictMatrixClass> ProdictMatrixClassList
        {
            get
            {
                return _ProdictMatrixClassList;
            }
            private set {}
        }

        /// <summary>
        /// Поведение при запросе матрикс кода если он не найтен в текущих группах товаров и не стоит галочка GetMatrixAlways=true
        /// </summary>
        public static bool MandatoryDefault
        {
            get
            {
                return _MandatoryDefault;
            }
            set
            {
                xmlRoot.SetAttribute("MandatoryDefault", value.ToString());
                Save();
                _MandatoryDefault = value;
            }
        }
        #endregion

        #region Puplic Method

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="FileConfig"></param>
        public Config(string FileConfig)
        {
            try
            {
                if (obj == null) FileXml = "AlgoritmPrizm.xml";
                else FileXml = FileConfig;

                obj = this;
                Log.EventSave("Чтение конфигурационного файла", GetType().Name, EventEn.Message);

                // Читаем файл или создаём
                if (File.Exists(Environment.CurrentDirectory + @"\" + FileXml)) { Load(); }
                else { Create(); }

                // Получаем кастомизированный объект
                GetDate();

                // Подписываемся на события
                Com.Lic.onCreatedLicKey += new EventHandler<LicLib.onLicEventKey>(Lic_onCreatedLicKey);
                Com.Lic.onRegNewKey += new EventHandler<LicLib.onLicItem>(Lic_onRegNewKey);
                Com.ProviderFarm.onEventSetup += new EventHandler<EventProviderFarm>(ProviderFarm_onEventSetup);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            } 
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="FileConfig"></param>
        public Config()
            :this(null)
        {
        }

        /// <summary>
        /// Создание нового списка пользователей
        /// </summary>
        /// <param name="NewCustumers">Новый список который надо сохранить</param>
        public static void SetCustomers(List<Custumer> NewCustumers)
        {
            try
            {
                //xmlRoot.RemoveChild(xmlCustomers);
                //xmlCustomers = Document.CreateElement("Customers");
                //xmlRoot.AppendChild(xmlCustomers);
                foreach (XmlElement item in xmlCustomers.ChildNodes)
                {
                    xmlCustomers.RemoveChild(item);
                }

                foreach (Custumer item in NewCustumers)
                {
                    XmlElement xmlCustomerTest = Document.CreateElement("Customer");
                    xmlCustomerTest.SetAttribute("Login", item.login);
                    xmlCustomerTest.SetAttribute("Fio", item.fio_fo_check);
                    xmlCustomerTest.SetAttribute("INN", item.inn);
                    xmlCustomers.AppendChild(xmlCustomerTest);
                }

                Save();

                _customers = NewCustumers;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении нового списка кассиров с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SetCustomers", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Создание нового списка с классами продуктов по которым нужно запросить матрикс код
        /// </summary>
        /// <param name="NewProdictMatrixClass"></param>
        public static void SetProdictMatrixClassList(List<ProdictMatrixClass> NewProdictMatrixClass)
        {
            try
            {
                // Если не существует узла то создаём его
                if (xmlProdictMatrixClassList==null)
                {
                    xmlProdictMatrixClassList = Document.CreateElement("ProdictMatrixClassList");
                    xmlRoot.AppendChild(xmlProdictMatrixClassList);
                }
                
                foreach (XmlElement item in xmlProdictMatrixClassList.ChildNodes)
                {
                    xmlProdictMatrixClassList.RemoveChild(item);
                }

                foreach (ProdictMatrixClass item in NewProdictMatrixClass)
                {
                    XmlElement xmlProdictMatrixClass = Document.CreateElement("ProdictMatrixClass");
                    xmlProdictMatrixClass.SetAttribute("ProductClass", item.ProductClass);
                    xmlProdictMatrixClass.SetAttribute("Mandatory", item.Mandatory.ToString());
                    xmlProdictMatrixClassList.AppendChild(xmlProdictMatrixClass);
                }

                Save();

                _ProdictMatrixClassList = NewProdictMatrixClass;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении нового списка с классами продуктов по которым нужно запросить матрикс код: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SetProdictMatrixClassList", EventEn.Error);
                throw ae;
            }
        }
        #endregion

        #region Private Method

        /// <summary>
        /// Читеам файл конфигурации
        /// </summary>
        private static void Load()
        {
            try
            {
                lock(obj)
                {
                    Document.Load(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".Load()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Сохраняем конфигурацию  в файл
        /// </summary>
        private static void Save()
        {
            try
            {
                lock (obj)
                {
                    Document.Save(Environment.CurrentDirectory + @"\" + FileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации в файл с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".Save()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Создание нового файла
        /// </summary>
        private static void Create()
        {
            try
            {
                lock (obj)
                {
                    // создаём строку инициализации
                    XmlElement wbRoot = Document.DocumentElement;
                    XmlDeclaration wbxmdecl = Document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    Document.InsertBefore(wbxmdecl, wbRoot);

                    // Создаём начальное тело с которым мы будем потом работать
                    XmlElement xmlMain = Document.CreateElement("AlgoritmPrizm");
                    xmlMain.SetAttribute("Version", _Version.ToString());
                    xmlMain.SetAttribute("Trace", _Trace.ToString());
                    xmlMain.SetAttribute("PrvFullName", null);
                    xmlMain.SetAttribute("ConnectionString", "");
                    xmlMain.SetAttribute("Host", _Host);
                    xmlMain.SetAttribute("Port", _Port.ToString());
                    xmlMain.SetAttribute("Ffd", _Ffd.ToString());
                    xmlMain.SetAttribute("FrPort", _FrPort.ToString());
                    xmlMain.SetAttribute("TenderTypeCash", _TenderTypeCash.ToString());
                    xmlMain.SetAttribute("TenderTypeCredit", _TenderTypeCredit.ToString());
                    xmlMain.SetAttribute("TenderTypeGiftCert", _TenderTypeGiftCert.ToString());
                    xmlMain.SetAttribute("TenderTypeGiftCard", _TenderTypeGiftCard.ToString());
                    xmlMain.SetAttribute("GiftCardCode", _GiftCardCode);
                    xmlMain.SetAttribute("GiftCardEnable", _GiftCardEnable.ToString());
                    xmlMain.SetAttribute("GiftCardTax", _GiftCardTax.ToString());
                    xmlMain.SetAttribute("FieldItem", _FieldItem.ToString());
                    xmlMain.SetAttribute("FieldDocNum", _FieldDocNum.ToString());
                    xmlMain.SetAttribute("HostPrizmApi", _HostPrizmApi);
                    xmlMain.SetAttribute("PrizmApiSystemLogon", _PrizmApiSystemLogon);
                    xmlMain.SetAttribute("PrizmApiSystemPassord", "");
                    xmlMain.SetAttribute("PrizmApiTimeLiveTockenMinute", _PrizmApiTimeLiveTockenMinute.ToString());
                    xmlMain.SetAttribute("FileCheckLog", _FileCheckLog);
                    xmlMain.SetAttribute("GetMatrixAlways", _GetMatrixAlways.ToString());
                    xmlMain.SetAttribute("MandatoryDefault", _MandatoryDefault.ToString());
                    Document.AppendChild(xmlMain);

                    XmlElement xmlLics = Document.CreateElement("Lics");
                    xmlMain.AppendChild(xmlLics);

                    // Создаём список в который будем помещать элементы с пользователями
                    XmlElement xmlCustomers = Document.CreateElement("Customers");
                    xmlMain.AppendChild(xmlCustomers);
                    XmlElement xmlCustomerTest = Document.CreateElement("Customer");
                    xmlCustomerTest.SetAttribute("Login", "sysadmin");
                    xmlCustomerTest.SetAttribute("Fio", "Сидоров Иван Петрович");
                    xmlCustomerTest.SetAttribute("INN", "1234567890");
                    xmlCustomers.AppendChild(xmlCustomerTest);


                    // Сохраняем документ
                    Save();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при соpдании конфигурационного файла с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".Create()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получение кастомизированного запроса
        /// </summary>
        private static void GetDate()
        {
            ApplicationException appM = new ApplicationException("Неправильный настроечный файл, скорее всего не от этой программы.");
            ApplicationException appV = new ApplicationException(string.Format("Неправильная версия настроечного яайла, требуется {0} версия", _Version));
            try
            {
                lock (obj)
                {
                    xmlRoot = Document.DocumentElement;

                    // Проверяем значения заголовка
                    if (xmlRoot.Name != "AlgoritmPrizm") throw appM;
                    if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) throw appV;
                    if (Version > int.Parse(xmlRoot.GetAttribute("Version"))) UpdateVersionXml(xmlRoot, int.Parse(xmlRoot.GetAttribute("Version")));

                    string PrvFullName = null;
                    string ConnectionString = null;

                    // Получаем значения из заголовка
                    for (int i = 0; i < xmlRoot.Attributes.Count; i++)
                    {
                        if (xmlRoot.Attributes[i].Name == "Trace") try {_Trace = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception){}
                        if (xmlRoot.Attributes[i].Name == "PrvFullName") PrvFullName = xmlRoot.Attributes[i].Value.ToString();
                        try { if (xmlRoot.Attributes[i].Name == "ConnectionString") ConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Host") try { _Host = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Port") try { _Port = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Ffd") try { _Ffd = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _Ffd); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FrPort") try { _FrPort = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeCash") try { _TenderTypeCash = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeCredit") try { _TenderTypeCredit = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeGiftCert") try { _TenderTypeGiftCert = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeGiftCard") try { _TenderTypeGiftCard = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardCode") try { _GiftCardCode = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardEnable") try { _GiftCardEnable = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardTax") try { _GiftCardTax = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem") try {_FieldItem = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem);} catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldDocNum") try {_FieldDocNum = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldDocNum); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "HostPrizmApi") try { _HostPrizmApi = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiSystemLogon") try { _PrizmApiSystemLogon = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiSystemPassord") try { _PrizmApiSystemPassord = Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiTimeLiveTockenMinute") try { _PrizmApiTimeLiveTockenMinute = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FileCheckLog") try { _FileCheckLog = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GetMatrixAlways") try { _GetMatrixAlways = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "MandatoryDefault") try { _MandatoryDefault = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                    }

                    // Подгружаем провайдер
                    try
                    {
                        Com.ProviderFarm.Setup(new UProvider(PrvFullName, ConnectionString), false);
                    }
                    catch (Exception) { }

                    // Получаем список вложенных объектов
                    foreach (XmlElement iMain in xmlRoot.ChildNodes)
                    {
                        switch (iMain.Name)
                        {
                            case "Lics":
                                xmlLics = iMain;
                                foreach (XmlElement xkey in iMain.ChildNodes)
                                {
                                    try
                                    {
                                        string MachineName = null;
                                        string UserName = null;
                                        string ActivNumber = null;
                                        string LicKey = null;
                                        int ValidToYYYYMMDD = 0;
                                        string Info = null;
                                        bool HashUserOS = false;
                                        List<string> ScnFullNameList = new List<string>();

                                        //Получаем данные по параметру из файла
                                        for (int i = 0; i < xkey.Attributes.Count; i++)
                                        {
                                            if (xkey.Attributes[i].Name == "MachineName") { MachineName = xkey.Attributes[i].Value; }
                                            if (xkey.Attributes[i].Name == "UserName") { UserName = xkey.Attributes[i].Value; }
                                            if (xkey.Attributes[i].Name == "ActivNumber") { ActivNumber = xkey.Attributes[i].Value; }
                                            if (xkey.Attributes[i].Name == "LicKey") { LicKey = xkey.Attributes[i].Value; }
                                            if (xkey.Attributes[i].Name == "ValidToYYYYMMDD") { try { ValidToYYYYMMDD = int.Parse(xkey.Attributes[i].Value); } catch { } }
                                            if (xkey.Attributes[i].Name == "Info") { Info = xkey.Attributes[i].Value; }
                                            try { if (xkey.Attributes[i].Name == "HashUserOS") { HashUserOS = bool.Parse(xkey.Attributes[i].Value); } }
                                            catch (Exception) { }
                                        }
                                        if (!string.IsNullOrWhiteSpace(xkey.InnerText))
                                        {
                                            foreach (string sitem in xkey.InnerText.Split(','))
                                            {
                                                ScnFullNameList.Add(sitem);
                                            }
                                        }

                                        // Проверяем валидность подгруженного ключа
                                        if (!string.IsNullOrWhiteSpace(LicKey)) //&& Com.Lic.IsValidLicKey(LicKey)
                                        {
                                            Com.Lic.IsValidLicKey(LicKey);
                                            // Если ключь валидный то сохраняем его в списке ключей
                                            //Com.LicLib.onLicEventKey newKey = new Com.LicLib.onLicEventKey(MachineName, UserName, ActivNumber, LicKey, ValidToYYYYMMDD, Info, HashUserOS, ScnFullNameList);
                                            //Com.Lic.IsValidLicKey( .Add(newKey);
                                        }
                                    }
                                    catch { } // Если ключь прочитать не удалось или он не подходит, то исключения выдавать не нужно
                                }
                                break;
                            case "ProdictMatrixClassList":
                                xmlProdictMatrixClassList = iMain;
                                // Получаем список вложенных объектов
                                foreach (XmlElement iProdictMatrixClass in iMain.ChildNodes)
                                {
                                    switch (iProdictMatrixClass.Name)
                                    {
                                        case "ProdictMatrixClass":

                                            string ProductClass = null;
                                            bool Mandatory = false;

                                            // Получаем значения из заголовка
                                            for (int i = 0; i < iProdictMatrixClass.Attributes.Count; i++)
                                            {
                                                if (iProdictMatrixClass.Attributes[i].Name == "ProductClass") try { ProductClass = iProdictMatrixClass.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iProdictMatrixClass.Attributes[i].Name == "Mandatory") try { Mandatory = Boolean.Parse(iProdictMatrixClass.Attributes[i].Value.ToString()); } catch (Exception) { }
                                            }

                                            if (!string.IsNullOrWhiteSpace(ProductClass))
                                            {
                                                _ProdictMatrixClassList.Add(new ProdictMatrixClass(ProductClass, Mandatory));
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Customers":
                                xmlCustomers = iMain;
                                // Получаем список вложенных объектов
                                foreach (XmlElement iCustomers in iMain.ChildNodes)
                                {
                                    switch (iCustomers.Name)
                                    {
                                        case "Customer":

                                            string login = null;
                                            string fio = null;
                                            string inn = null;

                                            // Получаем значения из заголовка
                                            for (int i = 0; i < iCustomers.Attributes.Count; i++)
                                            {
                                                if (iCustomers.Attributes[i].Name == "Login") try { login = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iCustomers.Attributes[i].Name == "Fio") try { fio = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iCustomers.Attributes[i].Name == "INN") try { inn = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }

                                            }

                                            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(fio) && !string.IsNullOrWhiteSpace(inn))
                                            {
                                                _customers.Add(new Custumer(login, fio, inn));
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при разборе конфигурационного файла с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".GetDate()", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Обновление до нужной версии
        /// </summary>
        /// <param name="root">Корневой элемент</param>
        /// <param name="oldVersion">Версия файла из конфига</param>
        private static void UpdateVersionXml(XmlElement root, int oldVersion)
        {
            try
            {
                if (oldVersion <=2)
                {

                }

                root.SetAttribute("Version", _Version.ToString());
                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при обновлении конфигурационного в файла с ошибкой: {)}", ex.Message));
                Log.EventSave(ae.Message, ".UpdateVersionXml(XmlElement root, int oldVersion)", EventEn.Error);
                throw ae;
            }
        }

        // Событие регистрации нового ключа
        void Lic_onRegNewKey(object sender, LicLib.onLicItem e)
        {
            try
            {
                if (xmlLics == null)
                {
                    xmlLics = Document.CreateElement("Lics");
                    xmlRoot.AppendChild(xmlLics);
                }

                XmlElement k = Document.CreateElement("Key");
                if (e._LicEventKey.MachineName != null) k.SetAttribute("MachineName", e._LicEventKey.MachineName);
                if (e._LicEventKey.UserName != null) k.SetAttribute("UserName", e._LicEventKey.UserName);
                if (e._LicEventKey.ActivNumber != null) k.SetAttribute("ActivNumber", e._LicEventKey.ActivNumber);
                if (e._LicEventKey.LicKey != null) k.SetAttribute("LicKey", e._LicEventKey.LicKey);
                if (e._LicEventKey.ValidToYYYYMMDD != 0) k.SetAttribute("ValidToYYYYMMDD", e._LicEventKey.ValidToYYYYMMDD.ToString());
                if (e._LicEventKey.Info != null) k.SetAttribute("Info", e._LicEventKey.Info);
                k.SetAttribute("HashUserOS", e._LicEventKey.HashUserOS.ToString());
                k.InnerText = string.Join(",", e._LicEventKey.ScnFullNameList.ToArray());
                xmlLics.AppendChild(k);

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении во время создания нового ключа в файл xml: {0}", ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".Lic_onCreatedLicKey()", EventEn.Error);
                throw ae;
            }
        }
        //
        // Событие создания нового ключа
        void Lic_onCreatedLicKey(object sender, LicLib.onLicEventKey e)
        {
            try
            {
                if (xmlLics == null)
                {
                    xmlLics = Document.CreateElement("Lics");
                    xmlRoot.AppendChild(xmlLics);
                }

                XmlElement k = Document.CreateElement("Key");
                if (e.MachineName != null) k.SetAttribute("MachineName", e.MachineName);
                if (e.UserName != null) k.SetAttribute("UserName", e.UserName);
                if (e.ActivNumber != null) k.SetAttribute("ActivNumber", e.ActivNumber);
                if (e.LicKey != null) k.SetAttribute("LicKey", e.LicKey);
                if (e.ValidToYYYYMMDD != 0) k.SetAttribute("ValidToYYYYMMDD", e.ValidToYYYYMMDD.ToString());
                if (e.Info != null) k.SetAttribute("Info", e.Info);
                k.SetAttribute("HashUserOS", e.HashUserOS.ToString());
                k.InnerText = string.Join(",", e.ScnFullNameList.ToArray());
                xmlLics.AppendChild(k);

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении во время создания нового ключа в файл xml: {0}", ex.Message));
                Log.EventSave(ae.Message, obj.GetType().Name + ".Lic_onCreatedLicKey()", EventEn.Error);
                throw ae;
            }
        }

        // Событие изменения текщего провайдера
        private void ProviderFarm_onEventSetup(object sender, EventProviderFarm e)
        {
            try
            {
                XmlElement root = Document.DocumentElement;

                root.SetAttribute("PrvFullName", e.Uprv.PrvInType);
                try { root.SetAttribute("ConnectionString", Com.Lic.InCode(e.Uprv.ConnectionString)); }
                catch (Exception) { }



                // Получаем список объектов
                //foreach (XmlElement item in root.ChildNodes)
                //{
                //}

                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException("Упали при изменении файла конфигурации с ошибкой: " + ex.Message);
                Log.EventSave(ae.Message, obj.GetType().Name + ".ProviderFarm_onEventSetup()", EventEn.Error);
                throw ae;
            }
        }

        #endregion

        #region Вложенные классы
        #endregion
    }
}
