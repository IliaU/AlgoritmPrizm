using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.IO.Ports;
using AlgoritmPrizm.Lib;


namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Работа с дисплеем
    /// </summary>
    public class DisplayFarm
    {
        /// <summary>
        /// Получаем список доступных дисплеев
        /// </summary>
        /// <returns>Список имён доступных дисплеев</returns>
        public static List<string> ListDisplayName;

        /// <summary>
        /// Текущий дисплей
        /// </summary>
        public static Display CurDisplay;

        /// <summary>
        /// Конструктор
        /// </summary>
        public DisplayFarm()
        {
            try
            {
                if (ListDisplayName == null)
                {
                    Com.Log.EventSave("Инициализация классов облуживания дисплее покупателя", GetType().Name, EventEn.Message);
                    ListCustomerDisplayName();

                    // Установка текущего дисплея
                    string dd = ListDisplayName.Find(x => x == Config.DisplayDspFullName);
                    if (!string.IsNullOrWhiteSpace(dd)) CurDisplay = CreateNewDisplay("DisplayDSP840");//, 6, 19200, 0, 8, StopBits.One);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                //throw ae;
            }
        }

        /// <summary>
        /// Установка текущего дисплея для пользователя
        /// </summary>
        /// <param name="NewDsp"></param>
        public static void SetCurrentDisplay(Display NewDsp)
        {
            try
            {
                CurDisplay = NewDsp;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при инициализации конструктора с ошибкой: ({0})", ex.Message));
                Com.Log.EventSave(ae.Message, "DisplayFarm.SetCurrentDisplay", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получаем список допустимых дисплеев
        /// </summary>
        /// <returns>Список доступных документов</returns>
        public static List<string> ListCustomerDisplayName()
        {

            try
            {
                // Если список ещё не получали то получаем его
                if (ListDisplayName == null)
                {
                    ListDisplayName = new List<string>();

                    Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "AlgoritmPrizm.Com.DisplayPlg").ToArray();

                    foreach (Type item in typelist)
                    {
                        // Проверяем реализовывает ли класс наш интерфейс если да то это провайдер который можно подкрузить
                        bool flagI = false;
                        foreach (Type i in item.GetInterfaces())
                        {
                            if (i.FullName == "AlgoritmPrizm.Com.DisplayPlg.Lib.CustomerDisplayI")
                            {
                                flagI = true;
                                break;
                            }
                        }
                        if (!flagI) continue;

                        // Проверяем что наш клас наследует PlugInBase 
                        bool flagB = false;
                        foreach (MemberInfo mi in item.GetMembers())
                        {
                            if (mi.DeclaringType.FullName == "AlgoritmPrizm.Com.Display")
                            {
                                flagB = true;
                                break;
                            }
                        }
                        if (!flagB) continue;

                        // Проверяем конструктор нашего класса  
                        bool flag = false;
                        //bool flag0 = false;
                        string nameConstructor;
                        foreach (ConstructorInfo ctor in item.GetConstructors())
                        {
                            nameConstructor = item.Name;

                            // получаем параметры конструктора  
                            ParameterInfo[] parameters = ctor.GetParameters();

                            // если в этом конструктаре 11 параметров то проверяем тип и имя параметра 
                            if (parameters.Length == 5)
                            {
                                bool flag2 = true;
                                if (parameters[0].ParameterType.Name != "Int32" || parameters[0].Name != "Port") flag = false;
                                if (parameters[1].ParameterType.Name != "Int32" || parameters[1].Name != "BaudRate") flag = false;
                                if (parameters[2].ParameterType.Name != "Parity" || parameters[2].Name != "Parity") flag = false;
                                if (parameters[3].ParameterType.Name != "Int32" || parameters[3].Name != "DataBits") flag = false;
                                if (parameters[4].ParameterType.Name != "StopBits" || parameters[4].Name != "StpBits") flag = false;
                                flag = flag2;
                            }

                            // Проверяем конструктор для создания документа пустого по умолчанию
                            //if (parameters.Length == 0) flag0 = true;
                        }
                        if (!flag) continue;
                        //if (!flag0) continue;

                        ListDisplayName.Add(item.Name);
                    }
                }
            }
            catch (Exception ex )
            {
                Com.Log.EventSave(ex.Message, "DisplayFarm.ListCustomerDisplayName", EventEn.Error);
            }

            

            return ListDisplayName;
        }

        /// <summary>
        /// Создание диспле определённого типа с параметрами порта из конфига
        /// </summary>
        /// <param name="PrvFullName">Имя плагина определяющего тип дисплея который создаём</param>
        /// <returns>Возвращаем дисплей</returns>
        public static Display CreateNewDisplay(string PrvFullName)
        {
            // Если списка дисплеев ещё нет то создаём его
            ListCustomerDisplayName();

            Display rez = null;

            // Проверяем наличие существование этого типа документа
            foreach (string item in ListDisplayName)
            {
                if (item == PrvFullName.Trim())
                {
                    Type myType = Type.GetType("AlgoritmPrizm.Com.DisplayPlg." + PrvFullName.Trim(), false, true);

                    // Создаём экземпляр объекта
                    object[] targ = { Config.DisplayPort, Config.DisplayBaudRate, Config.DisplayParity, Config.DisplayDataBits, Config.DisplayStpBits };
                    rez = (Display)Activator.CreateInstance(myType, targ);

                    break;
                }
            }

            return rez;
        }

        /// <summary>
        /// Создание диспле определённого типа
        /// </summary>
        /// <param name="DspFullName">Имя плагина определяющего тип дисплея который создаём</param>
        /// <param name="Port">Ком порт</param>
        /// <param name="BaudRate">Скорость</param>
        /// <param name="Parity">Parity</param>
        /// <param name="DataBits">DataBits</param>
        /// <param name="StpBits">StpBits</param>
        /// <returns>Возвращаем дисплей</returns>
        public static Display CreateNewDisplay(string DspFullName, int Port, int BaudRate, Parity Parity, int DataBits, StopBits StpBits)
        {
            // Если списка дисплеев ещё нет то создаём его
            ListCustomerDisplayName();

            Display rez = null;

            // Проверяем наличие существование этого типа документа
            foreach (string item in ListDisplayName)
            {
                if (item == DspFullName.Trim())
                {
                    Type myType = Type.GetType("AlgoritmPrizm.Com.DisplayPlg." + DspFullName.Trim(), false, true);

                    // Создаём экземпляр объекта  
                    object[] targ = { Port, BaudRate, Parity, DataBits, StpBits };
                    rez = (Display)Activator.CreateInstance(myType, targ);
                    break;
                }
            }

            return rez;
        }
    }
}
