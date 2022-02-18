using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using System.IO.Ports;
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
        /// Название компании будет учавствовать в письмах от кого и в сообщениях
        /// </summary>
        private static string _NameCompany = "";

        /// <summary>
        /// Хост на котором будет крутиться вебсервис
        /// </summary>
        private static string _Host = Environment.MachineName;

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        private static int _Port = 5000;

        /// <summary>
        /// Тип дисплея покупателя
        /// </summary>
        private static string _DisplayDspFullName = "DisplayDSP840";

        /// <summary>
        /// Порт дисплея покупателя
        /// </summary>
        private static int _DisplayPort = 6;

        /// <summary>
        /// Скорость дисплея покупателя
        /// </summary>
        private static int _DisplayBaudRate = 19200;

        /// <summary>
        /// Parity  по умолчанию = 0
        /// </summary>
        private static Parity _DisplayParity = Parity.None;

        /// <summary>
        /// DataBits  по умолчанию = 8
        /// </summary>
        private static int _DisplayDataBits = 8;

        /// <summary>
        /// StopBits  по умолчанию = StopBits.One
        /// </summary>
        private static StopBits _DisplayStpBits = StopBits.One;

        /// <summary>
        /// Список продавцов
        /// </summary>
        private static List<Custumer> _customers = new List<Custumer>();

        /// <summary>
        /// Сотрудники магазина
        /// </summary>
        private static List<Employee> _employees = new List<Employee>();

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
        /// Корневой элемент наших сотрудников в настроечном файле
        /// </summary>
        private static XmlElement xmlEmployees;

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
        /// Признак суммы которую вносили клиенты в предыдущих чеках
        /// </summary>
        private static int _TenderTypeAvans = 7;

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
        /// Какой используется провайдер для отправки SMS
        /// </summary>
        private static EnSmsTypGateway _SmsTypGateway = EnSmsTypGateway.Empty;

        /// <summary>
        /// SMTP сервер для отправки почтовых сообщений при доставке СМС
        /// </summary>
        private static string _SmsTypGatewaySmtp = "smtp.mail.ru";

        /// <summary>
        /// Порт на котором работает SMTP сервер для отправки почтовых сообщений при доставке СМС
        /// </summary>
        private static int _SmsTypGatewayPort = 25;

        /// <summary>
        /// Логин который используется для отправки сообщений например при HTTP "login" при отправки с использованием писем
        /// </summary>
        private static string _SmsTypGatewayLogin = @"login";

        /// <summary>
        /// Логин который используется для отправки сообщений например при SMTP "ilia82@mail.ru" при отправки с использованием писем
        /// </summary>
        private static string _SmsTypGatewaySmtpLogin = @"ilia82@mail.ru";

        /// <summary>
        /// Пароль который используется для отправки сообщений например при HTTP "login" при отправки с использованием писем
        /// </summary>
        private static string _SmsTypGatewayPassword = "";

        /// <summary>
        /// Пароль который используется для отправки сообщений например при SMTP "ilia82@mail.ru" при отправки с использованием писем
        /// </summary>
        private static string _SmsTypGatewaySmtpPassword = "";

        /// <summary>
        /// Порт на котором будет крутится Web сервис
        /// </summary>
        private static int _FrPort = 3;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem = FieldItemEn.Description1;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem1 = FieldItemEn.Empty;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem2 = FieldItemEn.Empty;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem3 = FieldItemEn.Empty;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem4 = FieldItemEn.Empty;

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        private static FieldItemEn _FieldItem5 = FieldItemEn.Empty;

        /// <summary>
        /// Наименование поля с номером ФД
        /// </summary>
        private static FieldDocNumEn _FieldDocNum = FieldDocNumEn.Comment2;

        /// <summary>
        /// Наименование поля которое показывает что за клиент юрик или физик и его ИНН
        /// </summary>
        private static FieldDocNumEn _FieldInnTyp = FieldDocNumEn.Comment1;

        /// <summary>
        /// Хост с приложением Prizm на котором развёрнут API
        /// </summary>
        private static string _HostPrizmApi = "http://172.16.1.102";

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
        /// Типы объекта на основе которых принимаем решения как действовать запрашивать матрикс коды или нет
        /// </summary>
        private static EnProductMatrixClassType _ProductMatrixClassType = EnProductMatrixClassType.dcs_code;

        /// <summary>
        /// Какой механизм поиска будем использовать при принятии решения о запросе матрикс кода
        /// </summary>
        private static EnMatrixParceTyp _MatrixParceTyp = EnMatrixParceTyp.Normal;

        /// <summary>
        /// Класс продукта принимаем решение нужно ли выдовать запрос матрикс кода и обязательный параметр матрикс код или нет
        /// </summary>
        private static List<ProdictMatrixClass> _ProdictMatrixClassList = new List<ProdictMatrixClass>();

        /// <summary>
        /// Поведение при запросе матрикс кода если он не найтен в текущих группах товаров и не стоит галочка GetMatrixAlways=true
        /// </summary>
        private static bool _MandatoryDefault = false;

        /// <summary>
        /// Признак окончания группы товаров по которой принимаем решение запроса матрикс кода
        /// </summary>
        private static char _ProductMatrixEndOff = ' ';

        /// <summary>
        /// Лимит для Юрлиц
        /// </summary>
        private static decimal _LimitCachForUrik = 100000;

        /// <summary>
        /// Имя папки источника с шаблонами отчётов
        /// </summary>
        private static string _WordDotxSource = "DOTX";

        /// <summary>
        /// Имя папки получателя в который будут складываться отчёты
        /// </summary>
        private static string _WordDotxTarget = "Report";

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 10%
        /// </summary>
        private static int _TaxPercent10 = 2;

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 20%
        /// </summary>
        private static int _TaxPercent20 = 1;

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 10/110%
        /// </summary>
        private static int _TaxPercent10110 = 6;

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 20/120%
        /// </summary>
        private static int _TaxPercent20120 = 5;

        /// <summary>
        /// Печать сотрудника на каждой строке чека
        /// </summary>
        private static bool _EmployeePrintingForEveryLine = false;

        /// <summary>
        /// Обрабатывать тип покупателей юр лицо в фискальном регистраторе
        /// </summary>
        private static bool _ProcessingUrikForFr = false;

        /// <summary>
        /// Выводить на печать в чек юр лицо
        /// </summary>
        private static bool _PrintingUrikForFr = false;
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
        /// Название компании будет учавствовать в письмах от кого и в сообщениях
        /// </summary>
        public static string NameCompany
        {
            get
            {
                return _NameCompany;
            }
            set
            {
                xmlRoot.SetAttribute("NameCompany", value.ToString());
                Save();
                _NameCompany = value;
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
        /// Тип дисплея покупателя
        /// </summary>
        public static string DisplayDspFullName
        {
            get
            {
                return _DisplayDspFullName;
            }
            set
            {
                xmlRoot.SetAttribute("DisplayDspFullName", value.ToString());
                Save();
                _DisplayDspFullName = value;
            }
        }

        /// <summary>
        /// Порт дисплея покупателя
        /// </summary>
        public static int DisplayPort
        {
            get
            {
                return _DisplayPort;
            }
            set
            {
                xmlRoot.SetAttribute("DisplayPort", value.ToString());
                Save();
                _DisplayPort = value;
            }
        }

        /// <summary>
        /// Скорость дисплея покупателя
        /// </summary>
        public static int DisplayBaudRate
        {
            get
            {
                return _DisplayBaudRate;
            }
            set
            {
                xmlRoot.SetAttribute("DisplayBaudRate", value.ToString());
                Save();
                _DisplayBaudRate = value;
            }
        }

        /// <summary>
        /// Parity  по умолчанию = 0
        /// </summary>
        public static Parity DisplayParity
        {
            get
            {
                return _DisplayParity;
            }
            set
            {
                Parity val = EventConvertor.Convert(value.ToString(), _DisplayParity);
                xmlRoot.SetAttribute("DisplayParity", val.ToString());
                Save();
                _DisplayParity = val;
            }
        }

        /// <summary>
        /// DataBits  по умолчанию = 8
        /// </summary>
        public static int DisplayDataBits
        {
            get
            {
                return _DisplayDataBits;
            }
            set
            {
                xmlRoot.SetAttribute("DisplayDataBits", value.ToString());
                Save();
                _DisplayDataBits = value;
            }
        }

        /// <summary>
        /// StopBits  по умолчанию = StopBits.One
        /// </summary>
        public static StopBits DisplayStpBits
        {
            get
            {
                return _DisplayStpBits;
            }
            set
            {
                StopBits val = EventConvertor.Convert(value.ToString(), _DisplayStpBits);
                xmlRoot.SetAttribute("DisplayStpBits", val.ToString());
                Save();
                _DisplayStpBits = val;
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
                _TenderTypeCash = value;
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
                _TenderTypeCredit = value;
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
                _TenderTypeGiftCert = value;
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
                _TenderTypeGiftCard = value;
            }
        }

        /// <summary>
        /// Признак суммы которую вносили клиенты в предыдущих чеках
        /// </summary>
        public static int TenderTypeAvans
        {
            get
            {
                return _TenderTypeAvans;
            }
            set
            {
                xmlRoot.SetAttribute("TenderTypeAvans", value.ToString());
                Save();
                _TenderTypeAvans = value;
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
            private set { }
        }

        /// <summary>
        /// Сотрудники магазина
        /// </summary>
        public static List<Employee> employees
        {
            get
            {
                return _employees;
            }
            private set { }
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
        /// Какой используется провайдер для отправки SMS
        /// </summary>
        public static EnSmsTypGateway SmsTypGateway
        {
            get
            {
                return _SmsTypGateway;
            }
            set
            {
                EnSmsTypGateway val = EventConvertor.Convert(value.ToString(), _SmsTypGateway);
                xmlRoot.SetAttribute("SmsTypGateway", val.ToString());
                Save();
                _SmsTypGateway = val;
            }
        }

        /// <summary>
        /// SMTP сервер для отправки почтовых сообщений при доставке СМС
        /// </summary>
        public static string SmsTypGatewaySmtp
        {
            get
            {
                return _SmsTypGatewaySmtp;
            }
            set
            {
                xmlRoot.SetAttribute("SmsTypGatewaySmtp", value.ToString());
                Save();
                _SmsTypGatewaySmtp = value;
            }
        }

        /// <summary>
        /// Порт на котором работает SMTP сервер для отправки почтовых сообщений при доставке СМС
        /// </summary>
        public static int SmsTypGatewayPort
        {
            get
            {
                return _SmsTypGatewayPort;
            }
            set
            {
                xmlRoot.SetAttribute("SmsTypGatewayPort", value.ToString());
                Save();
                _SmsTypGatewayPort = value;
            }
        }

        /// <summary>
        /// Логин который используется для отправки сообщений например при HTTP "login" при отправки с использованием писем
        /// </summary>
        public static string SmsTypGatewayLogin
        {
            get
            {
                return _SmsTypGatewayLogin;
            }
            set
            {
                xmlRoot.SetAttribute("SmsTypGatewayLogin", value.ToString());
                Save();
                _SmsTypGatewayLogin = value;
            }
        }

        /// <summary>
        /// Логин который используется для отправки сообщений например при SMTP "ilia82@mail.ru" при отправки с использованием писем
        /// </summary>
        public static string SmsTypGatewaySmtpLogin
        {
            get
            {
                return _SmsTypGatewaySmtpLogin;
            }
            set
            {
                xmlRoot.SetAttribute("SmsTypGatewaySmtpLogin", value.ToString());
                Save();
                _SmsTypGatewaySmtpLogin = value;
            }
        }

        /// <summary>
        /// Пароль который используется для отправки сообщений например при HTTP "login" или ilia82@mail.ru при отправки с использованием писем
        /// </summary>
        public static string SmsTypGatewayPassword
        {
            get
            {
                return _SmsTypGatewayPassword;
            }
            set
            {
                string pwd = Lic.InCode(value.ToString());
                xmlRoot.SetAttribute("SmsTypGatewayPassword", pwd);
                Save();
                _SmsTypGatewayPassword = value.ToString();
            }
        }

        /// <summary>
        /// Пароль который используется для отправки сообщений например при SMTP "ilia82@mail.ru" при отправки с использованием писем
        /// </summary>
        public static string SmsTypGatewaySmtpPassword
        {
            get
            {
                return _SmsTypGatewaySmtpPassword;
            }
            set
            {
                string pwd = Lic.InCode(value.ToString());
                xmlRoot.SetAttribute("SmsTypGatewaySmtpPassword", pwd);
                Save();
                _SmsTypGatewaySmtpPassword = value.ToString();
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
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem1
        {
            get
            {
                return _FieldItem1;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem1", value.ToString());
                Save();
                _FieldItem1 = value;
            }
        }

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem2
        {
            get
            {
                return _FieldItem2;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem2", value.ToString());
                Save();
                _FieldItem2 = value;
            }
        }

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem3
        {
            get
            {
                return _FieldItem3;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem3", value.ToString());
                Save();
                _FieldItem3 = value;
            }
        }

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem4
        {
            get
            {
                return _FieldItem4;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem4", value.ToString());
                Save();
                _FieldItem4 = value;
            }
        }

        /// <summary>
        /// Наименование поля с именем товара
        /// </summary>
        public static FieldItemEn FieldItem5
        {
            get
            {
                return _FieldItem5;
            }
            set
            {
                xmlRoot.SetAttribute("FieldItem5", value.ToString());
                Save();
                _FieldItem5 = value;
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
        /// Наименование поля которое показывает что за клиент юрик или физик и его ИНН
        /// </summary>
        public static FieldDocNumEn FieldInnTyp
        {
            get
            {
                return _FieldInnTyp;
            }
            set
            {
                xmlRoot.SetAttribute("FieldInnTyp", value.ToString());
                Save();
                _FieldInnTyp = value;
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
                _PrizmApiSystemPassord = value.ToString();
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
        /// Типы объекта на основе которых принимаем решения как действовать запрашивать матрикс коды или нет
        /// </summary>
        public static EnProductMatrixClassType ProductMatrixClassType
        {
            get
            {
                return _ProductMatrixClassType;
            }
            set
            {
                xmlRoot.SetAttribute("ProductMatrixClassType", value.ToString());
                Save();
                _ProductMatrixClassType = value;
            }
        }

        /// <summary>
        /// Какой механизм поиска будем использовать при принятии решения о запросе матрикс кода
        /// </summary>
        public static EnMatrixParceTyp MatrixParceTyp
        {
            get
            {
                return _MatrixParceTyp;
            }
            set
            {
                xmlRoot.SetAttribute("MatrixParceTyp", value.ToString());
                Save();
                _MatrixParceTyp = value;
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

        /// <summary>
        /// Признак окончания группы товаров по которой принимаем решение запроса матрикс кода
        /// </summary>
        public static char ProductMatrixEndOff
        {
            get
            {
                return _ProductMatrixEndOff;
            }
            set
            {
                xmlRoot.SetAttribute("ProductMatrixEndOff", value.ToString());
                Save();
                _ProductMatrixEndOff = value;
            }
        }

        /// <summary>
        /// Лимит для Юрлиц
        /// </summary>
        public static decimal LimitCachForUrik
        {
            get
            {
                return _LimitCachForUrik;
            }
            set
            {
                xmlRoot.SetAttribute("LimitCachForUrik", value.ToString());
                Save();
                _LimitCachForUrik = value;
            }
        }

        /// <summary>
        /// Имя папки источника с шаблонами отчётов
        /// </summary>
        public static string WordDotxSource
        {
            get
            {
                return _WordDotxSource;
            }
            set
            {
                xmlRoot.SetAttribute("WordDotxSource", value.ToString());
                Save();
                _WordDotxSource = value.ToString();
            }
        }

        /// <summary>
        /// Имя папки получателя в который будут складываться отчёты
        /// </summary>
        public static string WordDotxTarget
        {
            get
            {
                return _WordDotxTarget;
            }
            set
            {
                xmlRoot.SetAttribute("WordDotxTarget", value.ToString());
                Save();
                _WordDotxTarget = value.ToString();
            }
        }

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 10%
        /// </summary>
        public static int TaxPercent10
        {
            get
            {
                return _TaxPercent10;
            }
            set
            {
                xmlRoot.SetAttribute("TaxPercent10", value.ToString());
                Save();
                _TaxPercent10 = value;
            }
        }

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 20%
        /// </summary>
        public static int TaxPercent20
        {
            get
            {
                return _TaxPercent20;
            }
            set
            {
                xmlRoot.SetAttribute("TaxPercent20", value.ToString());
                Save();
                _TaxPercent20 = value;
            }
        }

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 10/110%
        /// </summary>
        public static int TaxPercent10110
        {
            get
            {
                return _TaxPercent10110;
            }
            set
            {
                xmlRoot.SetAttribute("TaxPercent10110", value.ToString());
                Save();
                _TaxPercent10110 = value;
            }
        }

        /// <summary>
        /// Идентификатор налоговой ставки в фискальнике 20/120%
        /// </summary>
        public static int TaxPercent20120
        {
            get
            {
                return _TaxPercent20120;
            }
            set
            {
                xmlRoot.SetAttribute("TaxPercent20120", value.ToString());
                Save();
                _TaxPercent20120 = value;
            }
        }


        /// <summary>
        /// Печать сотрудника на каждой строке чека
        /// </summary>
        public static bool EmployeePrintingForEveryLine
        {
            get
            {
                return _EmployeePrintingForEveryLine;
            }
            set
            {
                xmlRoot.SetAttribute("EmployeePrintingForEveryLine", value.ToString());
                Save();
                _EmployeePrintingForEveryLine = value;
            }
        }

        /// <summary>
        /// Обрабатывать тип покупателей юр лицо в фискальном регистраторе
        /// </summary>
        public static bool ProcessingUrikForFr
        {
            get
            {
                return _ProcessingUrikForFr;
            }
            set
            {
                xmlRoot.SetAttribute("ProcessingUrikForFr", value.ToString());
                Save();
                _ProcessingUrikForFr = value;
            }
        }

        /// <summary>
        /// Выводить на печать в чек юр лицо
        /// </summary>
        public static bool PrintingUrikForFr
        {
            get
            {
                return _PrintingUrikForFr;
            }
            set
            {
                xmlRoot.SetAttribute("PrintingUrikForFr", value.ToString());
                Save();
                _PrintingUrikForFr = value;
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
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
                // Если корневого элемента нет создаём его
                if (xmlCustomers == null)
                {
                    //xmlRoot.RemoveChild(xmlCustomers);
                    xmlCustomers = Document.CreateElement("Customers");
                    xmlRoot.AppendChild(xmlCustomers);
                }

                XmlNodeList itemList = xmlCustomers.ChildNodes;
                int itemListCount = itemList.Count-1;
                for (int i = itemListCount; i >=0 ; i--)
                {
                    xmlCustomers.RemoveChild(itemList[i]);
                }

                foreach (Custumer item in NewCustumers)
                {
                    XmlElement xmlCustomerTest = Document.CreateElement("Customer");
                    xmlCustomerTest.SetAttribute("Login", item.login);
                    xmlCustomerTest.SetAttribute("Fio", item.fio_fo_check);
                    xmlCustomerTest.SetAttribute("Job", item.Job);
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
        /// Создание нового списка сотрудников
        /// </summary>
        /// <param name="NewEmployees">Новый список который надо сохранить</param>
        public static void SetEmployees(List<Employee> NewEmployees)
        {
            try
            {
                // Если корневого элемента нет создаём его
                if (xmlEmployees==null)
                {
                    //xmlRoot.RemoveChild(xmlCustomers);
                    xmlEmployees = Document.CreateElement("Employees");
                    xmlRoot.AppendChild(xmlEmployees);
                }

                XmlNodeList itemList = xmlEmployees.ChildNodes;
                int itemListCount = itemList.Count - 1;
                for (int i = itemListCount; i >= 0; i--)
                {
                    xmlEmployees.RemoveChild(itemList[i]);
                }

                foreach (Employee item in NewEmployees)
                {
                    XmlElement xmlEmployeeTest = Document.CreateElement("Employee");
                    xmlEmployeeTest.SetAttribute("PrizmLogin", item.PrizmLogin);
                    xmlEmployeeTest.SetAttribute("Fio", item.fio_fo_check);
                    xmlEmployees.AppendChild(xmlEmployeeTest);
                }

                Save();

                _employees = NewEmployees;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении нового списка сотрудников с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, ".SetEmployees", EventEn.Error);
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

                XmlNodeList itemList = xmlProdictMatrixClassList.ChildNodes;
                int itemListCount = itemList.Count - 1;
                for (int i = itemListCount; i >= 0; i--)
                {
                    xmlProdictMatrixClassList.RemoveChild(itemList[i]);
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации в файл с ошибкой: {0}", ex.Message));
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
                    xmlMain.SetAttribute("NameCompany", _NameCompany);
                    xmlMain.SetAttribute("PrvFullName", null);
                    xmlMain.SetAttribute("ConnectionString", "");
                    xmlMain.SetAttribute("Host", _Host);
                    xmlMain.SetAttribute("Port", _Port.ToString());
                    xmlMain.SetAttribute("DisplayDspFullName", _DisplayDspFullName);
                    xmlMain.SetAttribute("DisplayPort", _DisplayPort.ToString());
                    xmlMain.SetAttribute("DisplayBaudRate", _DisplayBaudRate.ToString());
                    xmlMain.SetAttribute("DisplayParity", _DisplayParity.ToString());
                    xmlMain.SetAttribute("DisplayDataBits", _DisplayDataBits.ToString());
                    xmlMain.SetAttribute("DisplayStpBits", _DisplayStpBits.ToString());
                    xmlMain.SetAttribute("Ffd", _Ffd.ToString());
                    xmlMain.SetAttribute("FrPort", _FrPort.ToString());
                    xmlMain.SetAttribute("TenderTypeCash", _TenderTypeCash.ToString());
                    xmlMain.SetAttribute("TenderTypeCredit", _TenderTypeCredit.ToString());
                    xmlMain.SetAttribute("TenderTypeGiftCert", _TenderTypeGiftCert.ToString());
                    xmlMain.SetAttribute("TenderTypeGiftCard", _TenderTypeGiftCard.ToString());
                    xmlMain.SetAttribute("TenderTypeAvans", _TenderTypeAvans.ToString());
                    xmlMain.SetAttribute("GiftCardCode", _GiftCardCode);
                    xmlMain.SetAttribute("GiftCardEnable", _GiftCardEnable.ToString());
                    xmlMain.SetAttribute("GiftCardTax", _GiftCardTax.ToString());
                    xmlMain.SetAttribute("FieldItem", _FieldItem.ToString());
                    xmlMain.SetAttribute("FieldItem1", _FieldItem1.ToString());
                    xmlMain.SetAttribute("FieldItem2", _FieldItem2.ToString());
                    xmlMain.SetAttribute("FieldItem3", _FieldItem3.ToString());
                    xmlMain.SetAttribute("FieldItem4", _FieldItem4.ToString());
                    xmlMain.SetAttribute("FieldItem5", _FieldItem5.ToString());
                    xmlMain.SetAttribute("FieldDocNum", _FieldDocNum.ToString());
                    xmlMain.SetAttribute("FieldInnTyp", _FieldInnTyp.ToString());
                    xmlMain.SetAttribute("HostPrizmApi", _HostPrizmApi);
                    xmlMain.SetAttribute("PrizmApiSystemLogon", _PrizmApiSystemLogon);
                    xmlMain.SetAttribute("PrizmApiSystemPassord", "");
                    xmlMain.SetAttribute("PrizmApiTimeLiveTockenMinute", _PrizmApiTimeLiveTockenMinute.ToString());
                    xmlMain.SetAttribute("FileCheckLog", _FileCheckLog);
                    xmlMain.SetAttribute("GetMatrixAlways", _GetMatrixAlways.ToString());
                    xmlMain.SetAttribute("ProductMatrixClassType", _ProductMatrixClassType.ToString());
                    xmlMain.SetAttribute("MatrixParceTyp", _MatrixParceTyp.ToString());
                    xmlMain.SetAttribute("MandatoryDefault", _MandatoryDefault.ToString());
                    xmlMain.SetAttribute("LimitCachForUrik", _LimitCachForUrik.ToString());
                    xmlMain.SetAttribute("SmsTypGateway", _SmsTypGateway.ToString());
                    xmlMain.SetAttribute("SmsTypGatewaySmtp", _SmsTypGatewaySmtp);
                    xmlMain.SetAttribute("SmsTypGatewayPort", _SmsTypGatewayPort.ToString());
                    xmlMain.SetAttribute("SmsTypGatewayLogin", _SmsTypGatewayLogin);
                    xmlMain.SetAttribute("SmsTypGatewaySmtpLogin", _SmsTypGatewaySmtpLogin);
                    xmlMain.SetAttribute("SmsTypGatewayPassword", _SmsTypGatewayPassword);
                    xmlMain.SetAttribute("SmsTypGatewaySmtpPassword", _SmsTypGatewaySmtpPassword);
                    xmlMain.SetAttribute("WordDotxSource", _WordDotxSource);
                    xmlMain.SetAttribute("WordDotxTarget", _WordDotxTarget);
                    xmlMain.SetAttribute("ProductMatrixEndOff", _ProductMatrixEndOff.ToString());
                    xmlMain.SetAttribute("TaxPercent10", _TaxPercent10.ToString());
                    xmlMain.SetAttribute("TaxPercent20", _TaxPercent20.ToString());
                    xmlMain.SetAttribute("TaxPercent10110", _TaxPercent10110.ToString());
                    xmlMain.SetAttribute("TaxPercent20120", _TaxPercent20120.ToString());
                    xmlMain.SetAttribute("EmployeePrintingForEveryLine", _EmployeePrintingForEveryLine.ToString());
                    xmlMain.SetAttribute("ProcessingUrikForFr", _ProcessingUrikForFr.ToString());
                    xmlMain.SetAttribute("PrintingUrikForFr", _PrintingUrikForFr.ToString());
                    Document.AppendChild(xmlMain);

                    // Для работы с лицензиями
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


                    // Создаём список в который будем помещать элементы с сотрудниками
                    XmlElement xmlEmployees = Document.CreateElement("Employees");
                    xmlMain.AppendChild(xmlEmployees);
                    XmlElement xmlEmployeeTest = Document.CreateElement("Employee");
                    xmlEmployeeTest.SetAttribute("PrizmLogin", "sysadmin");
                    xmlEmployeeTest.SetAttribute("Fio", "Иванов Иван Иванович");
                    xmlEmployees.AppendChild(xmlEmployeeTest);
                    

                    // Сохраняем документ
                    Save();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при соpдании конфигурационного файла с ошибкой: {0}", ex.Message));
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
                        if (xmlRoot.Attributes[i].Name == "NameCompany") try { _NameCompany = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrvFullName") PrvFullName = xmlRoot.Attributes[i].Value.ToString();
                        try { if (xmlRoot.Attributes[i].Name == "ConnectionString") ConnectionString = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Host") try { _Host = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Port") try { _Port = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayDspFullName") try { _DisplayDspFullName = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayPort") try { _DisplayPort = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayBaudRate") try { _DisplayBaudRate = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayParity") try { _DisplayParity = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _DisplayParity); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayDataBits") try { _DisplayDataBits = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "DisplayStpBits") try { _DisplayStpBits = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _DisplayStpBits); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Ffd") try { _Ffd = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _Ffd); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FrPort") try { _FrPort = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeCash") try { _TenderTypeCash = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeCredit") try { _TenderTypeCredit = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeGiftCert") try { _TenderTypeGiftCert = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeGiftCard") try { _TenderTypeGiftCard = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TenderTypeAvans") try { _TenderTypeAvans = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardCode") try { _GiftCardCode = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardEnable") try { _GiftCardEnable = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GiftCardTax") try { _GiftCardTax = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem") try {_FieldItem = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem);} catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem1") try { _FieldItem1 = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem1); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem2") try { _FieldItem2 = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem2); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem3") try { _FieldItem3 = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem3); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem4") try { _FieldItem4 = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem4); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldItem5") try { _FieldItem5 = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldItem5); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldDocNum") try {_FieldDocNum = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldDocNum); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FieldInnTyp") try {_FieldInnTyp = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _FieldInnTyp); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "HostPrizmApi") try { _HostPrizmApi = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiSystemLogon") try { _PrizmApiSystemLogon = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiSystemPassord") try { _PrizmApiSystemPassord = Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrizmApiTimeLiveTockenMinute") try { _PrizmApiTimeLiveTockenMinute = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "FileCheckLog") try { _FileCheckLog = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "GetMatrixAlways") try { _GetMatrixAlways = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "ProductMatrixClassType") try { _ProductMatrixClassType = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), EnProductMatrixClassType.dcs_code); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "MatrixParceTyp") try { _MatrixParceTyp = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), EnMatrixParceTyp.Normal); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "MandatoryDefault") try { _MandatoryDefault = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "LimitCachForUrik") try { _LimitCachForUrik = decimal.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "SmsTypGateway") try { _SmsTypGateway = EventConvertor.Convert(xmlRoot.Attributes[i].Value.ToString(), _SmsTypGateway); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "SmsTypGatewaySmtp") _SmsTypGatewaySmtp = xmlRoot.Attributes[i].Value.ToString();
                        if (xmlRoot.Attributes[i].Name == "SmsTypGatewayPort") try { _SmsTypGatewayPort = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "SmsTypGatewayLogin") _SmsTypGatewayLogin = xmlRoot.Attributes[i].Value.ToString();
                        if (xmlRoot.Attributes[i].Name == "SmsTypGatewaySmtpLogin") _SmsTypGatewaySmtpLogin = xmlRoot.Attributes[i].Value.ToString();
                        try { if (xmlRoot.Attributes[i].Name == "SmsTypGatewayPassword") _SmsTypGatewayPassword = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                        try { if (xmlRoot.Attributes[i].Name == "SmsTypGatewaySmtpPassword") _SmsTypGatewaySmtpPassword = Com.Lic.DeCode(xmlRoot.Attributes[i].Value.ToString()); }
                        catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "WordDotxSource") _WordDotxSource = xmlRoot.Attributes[i].Value.ToString();
                        if (xmlRoot.Attributes[i].Name == "WordDotxTarget") _WordDotxTarget = xmlRoot.Attributes[i].Value.ToString();
                        if (xmlRoot.Attributes[i].Name == "ProductMatrixEndOff")  try { _ProductMatrixEndOff = Char.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TaxPercent10") try { _TaxPercent10 = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TaxPercent20") try { _TaxPercent20 = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TaxPercent10110") try { _TaxPercent10110 = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TaxPercent20120") try { _TaxPercent20120 = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "EmployeePrintingForEveryLine") try { _EmployeePrintingForEveryLine = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "ProcessingUrikForFr") try { _ProcessingUrikForFr = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "PrintingUrikForFr") try { _PrintingUrikForFr = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
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
                                                bool HashFlagProductMatrinxClass = true;
                                                foreach (ProdictMatrixClass itemProdMatrixClF in _ProdictMatrixClassList)
                                                {
                                                    if(itemProdMatrixClF.ProductClass== ProductClass)
                                                    {
                                                        HashFlagProductMatrinxClass = false;
                                                        break;
                                                    }
                                                }

                                                if (HashFlagProductMatrinxClass) _ProdictMatrixClassList.Add(new ProdictMatrixClass(ProductClass, Mandatory));
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
                                            string Job = null;
                                            string inn = null;

                                            // Получаем значения из заголовка
                                            for (int i = 0; i < iCustomers.Attributes.Count; i++)
                                            {
                                                if (iCustomers.Attributes[i].Name == "Login") try { login = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iCustomers.Attributes[i].Name == "Fio") try { fio = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iCustomers.Attributes[i].Name == "Job") try { Job = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iCustomers.Attributes[i].Name == "INN") try { inn = iCustomers.Attributes[i].Value.ToString(); } catch (Exception) { }

                                            }

                                            if (!string.IsNullOrWhiteSpace(login) && !string.IsNullOrWhiteSpace(fio) && !string.IsNullOrWhiteSpace(inn))
                                            {
                                                bool HashFlagCustomer = true;
                                                foreach (Custumer itemCustumerF in _customers)
                                                {
                                                    if (itemCustumerF.login == login)
                                                    {
                                                        HashFlagCustomer = false;
                                                        break;
                                                    }
                                                }

                                                if (HashFlagCustomer) _customers.Add(new Custumer(login, fio, Job, inn));
                                            }

                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            case "Employees":
                                xmlEmployees = iMain;
                                // Получаем список вложенных объектов
                                foreach (XmlElement iEmployees in iMain.ChildNodes)
                                {
                                    switch (iEmployees.Name)
                                    {
                                        case "Employee":

                                            string PrizmLogin = null;
                                            string fio = null;

                                            // Получаем значения из заголовка
                                            for (int i = 0; i < iEmployees.Attributes.Count; i++)
                                            {
                                                if (iEmployees.Attributes[i].Name == "PrizmLogin") try { PrizmLogin = iEmployees.Attributes[i].Value.ToString(); } catch (Exception) { }
                                                if (iEmployees.Attributes[i].Name == "Fio") try { fio = iEmployees.Attributes[i].Value.ToString(); } catch (Exception) { }
                                            }

                                            if (!string.IsNullOrWhiteSpace(PrizmLogin) && !string.IsNullOrWhiteSpace(fio))
                                            {
                                                bool HashFlagEmployee = true;
                                                foreach (Employee itemEmployeeF in _employees)
                                                {
                                                    if (itemEmployeeF.PrizmLogin == PrizmLogin)
                                                    {
                                                        HashFlagEmployee = false;
                                                        break;
                                                    }
                                                }

                                                if (HashFlagEmployee) _employees.Add(new Employee(PrizmLogin, fio));
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при разборе конфигурационного файла с ошибкой: {0}", ex.Message));
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при обновлении конфигурационного в файла с ошибкой: {0}}", ex.Message));
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
