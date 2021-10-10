using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com.SmtpLib
{
    /// <summary>
    /// Базовый класс для списка запросов
    /// </summary>
    public abstract class ItemSQLBase
    {
        /// <summary>
        /// Индекс элемента в списке
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Имя запроса
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Тип запроса SMTP или HTTP
        /// </summary>
        public EnSmtpTyp SmtpTyp { get; private set; }

        /// <summary>
        /// Инициализация параметров базового класса
        /// </summary>
        /// <param name="Name">Имя элемента</param>
        /// <param name="SmtpTyp">Тип запроса SMTP или HTTP</param>
        protected void InitialConfiguration(string Name, EnSmtpTyp SmtpTyp)
        {
            this.Index = -1;
            this.Name = Name;
            this.SmtpTyp = SmtpTyp;
        }

        /// <summary>
        /// Базовый класс для компонента списка эелементов конфигурации
        /// </summary>
        public abstract class SQLListBase : IEnumerable
        {
            /// <summary>
            /// Внутренний список 
            /// </summary>
            private List<ItemSQLBase> CfgL = new List<ItemSQLBase>();

            /// <summary>
            /// Индекс элемента в списке
            /// </summary>
            public int Index { get; private set; }

            /// <summary>
            /// Количчество объектов в контейнере
            /// </summary>
            public int Count
            {
                get
                {
                    int rez;
                    lock (CfgL)
                    {
                        rez = CfgL.Count;
                    }
                    return rez;
                }
                private set { }
            }

            /// <summary>
            /// Добавление нового элемента
            /// </summary>
            /// <param name="newCfg">Элемент который нужно добавить в список</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Add(ItemSQLBase newCfg, bool HashExeption)
            {
                bool rez = false;

                try
                {
                    lock (this.CfgL)
                    {
                        // Проверка на наличие этого элемента в списке
                        foreach (ItemSQLBase item in this.CfgL)
                        {
                            if (item.Name == newCfg.Name)
                            {
                                throw new ApplicationException(string.Format("Элемент с таким именем: {0} уже существует в списке.", newCfg.Name));
                            }
                        }

                        newCfg.Index = CfgL.Count;
                        this.CfgL.Add(newCfg);
                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось добавить элемент с именем {0} вс список. Произошла ошибка: {1}", newCfg.Name, ex.Message));
                }
                return rez;
            }

            /// <summary>
            /// Удаление элемента
            /// </summary>
            /// <param name="delCfg">Элемент который нужно удалить из списка</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Remove(ItemSQLBase delCfg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.CfgL)
                    {
                        int delIndex = delCfg.Index;
                        this.CfgL.RemoveAt(delIndex);

                        for (int i = delIndex; i < this.CfgL.Count; i++)
                        {
                            this.CfgL[i].Index = i;
                        }

                        rez = true;
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось удалить элемент с именем {0} из списка. Произошла ошибка: {1}", delCfg.Name, ex.Message));
                }

                return rez;
            }


            /// <summary>
            /// Обновление данных элемента конфигурации.
            /// </summary>
            /// <param name="IndexId">Индекс элемента который нужно обновить</param>
            /// <param name="updCfg">Пользователь у которого нужно изменить данные</param>
            /// <param name="HashExeption">C отображением исключений</param>
            /// <returns>Результат операции (Успех или нет)</returns>
            protected bool Update(int IndexId, ItemSQLBase updCfg, bool HashExeption)
            {
                bool rez = false;
                try
                {
                    lock (this.CfgL)
                    {

                        if (IndexId >= this.CfgL.Count)
                        {
                            if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные элемента в списке {0}. Элемента с таким индексом {1} не существует.", updCfg.Name, updCfg.ToString()));
                        }
                        else
                        {
                            updCfg.Index = IndexId;
                            this.CfgL[IndexId] = updCfg;

                            rez = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (HashExeption) throw new ApplicationException(string.Format("Не удалось обновить данные элемента в списке {0}. Произошла ошибка: {1}", updCfg.Name, ex.Message));
                }

                return rez;
            }

            /// <summary>
            /// Получение компонента по его ID
            /// </summary>
            /// <param name="i">Введите идентификатор</param>
            /// <returns></returns>
            protected ItemSQLBase getConfigurationComponent(int i)
            {
                ItemSQLBase rez = null;
                lock (CfgL)
                {
                    rez = this.CfgL[i];
                }
                return rez;
            }

            /// <summary>
            /// Для обращения по индексатору
            /// </summary>
            /// <returns>Возвращаем стандарнтый индексатор</returns>
            public IEnumerator GetEnumerator()
            {
                IEnumerator rez = null;
                lock (CfgL)
                {
                    rez = this.CfgL.GetEnumerator();
                }
                return rez;
            }
        }
    }
}
