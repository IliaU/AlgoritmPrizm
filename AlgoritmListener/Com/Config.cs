using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;
using AlgoritmListener.Lib;
using AlgoritmListener.Com;

namespace AlgoritmListener.Com
{
    /// <summary>
    /// Класс для работы с конфигурационным фалом
    /// </summary>
    public class Config
    {
        #region Private Param
        /// <summary>
        /// Версия XML файла
        /// </summary>
        private static int _Version = 1;

        /// <summary>
        /// Тайм аут между циклами проверки
        /// </summary>
        private static int _TimeOutSec = 10;

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
        #endregion

        #region Public Param

        /// <summary>
        /// Файл в котором мы храним конфиг
        /// </summary>
        public static string FileXml { get; private set; }

        /// <summary>
        /// Директория в котором мы храним конфиг
        /// </summary>
        public static string DirXml { get; private set; }

        /// <summary>
        /// Версия XML файла
        /// </summary>
        public static int Version { get { return _Version; } private set { } }


        /// <summary>
        /// Тайм аут между циклами проверки
        /// </summary>
        public static int TimeOutSec { get { return _TimeOutSec; } private set { } }

        /// <summary>
        /// Место хранения конфига нешего плагина к призму
        /// </summary>
        public static string PrizmListener_FileConf = "";

        #endregion

        #region Puplic Method

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="FileConfig"></param>
        public static void SetupConfig(string DirConfig, string FileConfig)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FileConfig)) FileXml = "AlgoritmListener.xml";
                else FileXml = FileConfig;

                if (string.IsNullOrWhiteSpace(DirConfig)) DirXml = @"C:\Program Files\AlgoritmListener";
                else DirXml = DirConfig;

                Log.EventSave("Чтение конфигурационного файла", "Config", EventEn.Message);

                // Читаем файл или создаём
                if (File.Exists(DirXml + @"\" + FileXml)) { Load(); }
                else { Create(); }

                // Получаем кастомизированный объект
                GetDate();

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Config.SetupConfig", EventEn.Error);
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
                Document.Load(DirXml + @"\" + FileXml);
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
                Document.Save(DirXml + @"\" + FileXml);
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
                // создаём строку инициализации
                XmlElement wbRoot = Document.DocumentElement;
                XmlDeclaration wbxmdecl = Document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                Document.InsertBefore(wbxmdecl, wbRoot);

                // Создаём начальное тело с которым мы будем потом работать
                XmlElement xmlMain = Document.CreateElement("AlgoritmListener");
                xmlMain.SetAttribute("Version", _Version.ToString());
                xmlMain.SetAttribute("TimeOutSec", _TimeOutSec.ToString());
                Document.AppendChild(xmlMain);

                // Создаём список в который будем помещать элементы с пользователями
                XmlElement xmlPrizmListener = Document.CreateElement("PrizmListener");
                xmlPrizmListener.SetAttribute("FileConf", @"C:\Users\Admin\source\GIT\AlgoritmPrizm\AlgoritmPrizm\bin\Debug\AlgoritmPrizm.xml");
                xmlMain.AppendChild(xmlPrizmListener);

                // Сохраняем документ
                Save();
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
                xmlRoot = Document.DocumentElement;

                // Проверяем значения заголовка
                if (xmlRoot.Name != "AlgoritmListener") throw appM;
                if (Version < int.Parse(xmlRoot.GetAttribute("Version"))) throw appV;
                if (Version > int.Parse(xmlRoot.GetAttribute("Version"))) UpdateVersionXml(xmlRoot, int.Parse(xmlRoot.GetAttribute("Version")));

                // Получаем значения из заголовка
                for (int i = 0; i < xmlRoot.Attributes.Count; i++)
                {
                    if (xmlRoot.Attributes[i].Name == "TimeOutSec") try {_TimeOutSec = int.Parse(xmlRoot.Attributes[i].Value.ToString());}catch (Exception) {}
                }

                // Получаем список вложенных объектов
                foreach (XmlElement iMain in xmlRoot.ChildNodes)
                {
                    switch (iMain.Name)
                    {
                        case "PrizmListener":

                            // Получаем значения из заголовка
                            for (int i = 0; i < iMain.Attributes.Count; i++)
                            {
                                if (iMain.Attributes[i].Name == "FileConf") try { PrizmListener_FileConf = iMain.Attributes[i].Value.ToString(); } catch (Exception) { }
                            }
                            
                            break;
                        default:
                            break;
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
                ApplicationException ae = new ApplicationException(string.Format("Упали при обновлении конфигурационного в файла с ошибкой: {0}}", ex.Message));
                Log.EventSave(ae.Message, "Config.UpdateVersionXml(XmlElement root, int oldVersion)", EventEn.Error);
                throw ae;
            }
        }

        #endregion
    }
}
