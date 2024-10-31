using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.IO;
using System.IO.Ports;
using AlgoritmPrizmCom.Lib;

namespace AlgoritmPrizmCom.Com
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
        /// Папка для RPRO 9 запросов и работы с ЦДН
        /// </summary>
        private static string _RequestsFolder = @"Requests";

        /// <summary>
        /// Количество милесекунд для таймаута между циклами проверки шары
        /// </summary>
        private static int _TimeOutMiSec = 1000;

        /// <summary>
        /// Объект XML файла
        /// </summary>
        private static XmlDocument Document = new XmlDocument();

        /// <summary>
        /// Корневой элемент нашего документа
        /// </summary>
        private static XmlElement xmlRoot;
        #endregion

        #region Public Param

        /// <summary>
        /// Файл в котором мы храним конфиг
        /// </summary>
        public static string PathFileXml { get; private set; }

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
        /// Папка для RPRO 9 запросов и работы с ЦДН
        /// </summary>
        public static string RequestsFolder
        {
            get
            {
                return _RequestsFolder;
            }
            set
            {
                xmlRoot.SetAttribute("RequestsFolder", value.ToString());
                Save();
                _RequestsFolder = value;
            }
        }

        /// <summary>
        /// Количество милесекунд для таймаута между циклами проверки шары
        /// </summary>
        public static int TimeOutMiSec
        {
            get
            {
                return _TimeOutMiSec;
            }
            set
            {
                xmlRoot.SetAttribute("TimeOutMiSec", value.ToString());
                Save();
                _TimeOutMiSec = value;
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
                if (obj == null)
                {
                    if (!string.IsNullOrEmpty(FileConfig))
                    {
                        if (FileConfig.IndexOf(":") > 0) PathFileXml = FileConfig;
                        else PathFileXml = string.Format(@"{0}\{1}", Environment.CurrentDirectory, FileConfig);
                    }
                    else PathFileXml = "AlgoritmPrizmCom.xml";
                }

                obj = this;


                Log.EventSave("Чтение конфигурационного файла", GetType().Name, EventEn.Message);

                // Читаем файл или создаём
                if (File.Exists(PathFileXml)) { Load(); }
                else { Create(); }

                // Получаем кастомизированный объект
                GetDate();

                // Подписываемся на события
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
            : this(null)
        {
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
                lock (obj)
                {
                    Document.Load(PathFileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.Load()", EventEn.Error);
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
                    Document.Save(PathFileXml);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при сохранении конфигурации в файл с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.Save()", EventEn.Error);
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
                    XmlElement xmlMain = Document.CreateElement("AlgoritmPrizmCom");
                    xmlMain.SetAttribute("Version", _Version.ToString());
                    xmlMain.SetAttribute("Trace", _Trace.ToString());
                    xmlMain.SetAttribute("PrvFullName", null);
                    xmlMain.SetAttribute("ConnectionString", "");
                    xmlMain.SetAttribute("Host", _Host);
                    xmlMain.SetAttribute("Port", _Port.ToString());
                    xmlMain.SetAttribute("RequestsFolder", string.Format(@"{0}\{1}", Path.GetDirectoryName(PathFileXml), _RequestsFolder));
                    xmlMain.SetAttribute("TimeOutMiSec", _TimeOutMiSec.ToString());
                    Document.AppendChild(xmlMain);

                    // Сохраняем документ
                    Save();
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при соpдании конфигурационного файла с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.Create()", EventEn.Error);
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
                    if (xmlRoot.Name != "AlgoritmPrizmCom") throw appM;
                    if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) throw appV;
                    if (Version > int.Parse(xmlRoot.GetAttribute("Version"))) UpdateVersionXml(xmlRoot, int.Parse(xmlRoot.GetAttribute("Version")));

                    // Получаем значения из заголовка
                    for (int i = 0; i < xmlRoot.Attributes.Count; i++)
                    {
                        if (xmlRoot.Attributes[i].Name == "Trace") try { _Trace = bool.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Host") try { _Host = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "Port") try { _Port = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "RequestsFolder") try { _RequestsFolder = xmlRoot.Attributes[i].Value.ToString(); } catch (Exception) { }
                        if (xmlRoot.Attributes[i].Name == "TimeOutMiSec") try { _TimeOutMiSec = int.Parse(xmlRoot.Attributes[i].Value.ToString()); } catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при разборе конфигурационного файла с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.GetDate()", EventEn.Error);
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
                if (oldVersion <= 2)
                {

                }

                root.SetAttribute("Version", _Version.ToString());
                Save();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при обновлении конфигурационного в файла с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.UpdateVersionXml(XmlElement root, int oldVersion)", EventEn.Error);
                throw ae;
            }
        }
        #endregion

        #region Вложенные классы
        #endregion
    }
}
