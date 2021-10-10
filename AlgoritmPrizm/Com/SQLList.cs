using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AlgoritmPrizm.Com.SmtpLib;
using AlgoritmPrizm.BLL;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com
{
    public sealed class SQLList : ItemSQLBase.SQLListBase
    {
        private object MyObj = new object();
        private static SQLList obj;

        /// <summary>
        /// Вытаскивает элемент по его индексу в этом контернере
        /// </summary>
        /// <param name="i">Индекс</param>
        /// <returns>Елемент</returns>
        public ItemSQL this[int i]
        {
            get
            {
                ItemSQL rez = null;
                lock (this.MyObj)
                {
                    rez = ((ItemSQL)base.getConfigurationComponent(i));
                }

                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Вытаскивает элемент по его имени в этом контернере
        /// </summary>
        /// <param name="Name">Имя</param>
        /// <returns>Елемент</returns>
        public ItemSQL this[string Name]
        {
            get
            {
                ItemSQL rez = null;
                lock (this.MyObj)
                {
                    for (int i = 0; i < base.Count; i++)
                    {
                        if (base.getConfigurationComponent(i).Name == Name) rez = (ItemSQL)base.getConfigurationComponent(i);
                    }
                }

                return rez;
            }
            private set { }
        }

        /// <summary>
        /// Событие добавления запроса к базе данных
        /// </summary>
        public static event EventHandler<EventSQLList> onEventAddItemSql;

        /// <summary>
        /// Событие изменения запроса к базе данных
        /// </summary>
        public static event EventHandler<EventSQLList> onEventUpdateItemSql;

        /// <summary>
        /// Событие удаления запроса
        /// </summary>
        public static event EventHandler<EventSQLList> onEventRemoveItemSql;

        /// <summary>
        /// Конструктор
        /// </summary>
        public SQLList()
        {
            if (obj == null)
            {
                obj = this;
            }
        }

        public static SQLList GetInstanse()
        {
            if (obj == null) obj = new SQLList();
            return obj;
        }

        /// <summary>
        /// Добавление запроса
        /// </summary>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <returns>Результат выполнения операции</returns>
        public static bool Add(ItemSQL ItmSql, bool HashExeption)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                return obj.MyAdd(ItmSql, HashExeption, true);
            }
        }

        /// <summary>
        /// Добавление запроса
        /// </summary>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <param name="HashLog">Писать в лог?</param>
        /// <returns>Результат выполнения операции</returns>
        public static bool Add(ItemSQL ItmSql, bool HashExeption, bool HashLog)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                return obj.MyAdd(ItmSql, HashExeption, HashLog);
            }
        }

        /// <summary>
        /// Обновление запроса
        /// </summary>
        /// <param name="IndexSqlItem">Индекс позиции которую нужно заменить</param>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <returns>Результат выполнения операции</returns>
        public static bool Update(int IndexSqlItem, ItemSQL ItmSql, bool HashExeption)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                return obj.MyUpdate(IndexSqlItem, ItmSql, HashExeption);
            }
        }

        /// <summary>
        /// Удаление запроса
        /// </summary>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <returns>Результат выполнения операции</returns>
        public static bool Remove(ItemSQL ItmSql, bool HashExeption)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                return obj.MyRemove(ItmSql, HashExeption);
            }
        }

        /// <summary>
        /// Добавление запроса
        /// </summary>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <param name="HashLog">Писать в лог?</param>
        /// <returns>Результат выполнения операции</returns>
        private bool MyAdd(ItemSQL ItmSql, bool HashExeption, bool HashLog)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                try
                {
                    // Собственно обработка события
                    EventSQLList myArg = new EventSQLList(ItmSql);
                    if (onEventAddItemSql != null)
                    {
                        onEventAddItemSql.Invoke(this, myArg);
                    }

                    // Логируем изменение подключения
                    if (HashLog) Log.EventSave(string.Format("Пользователь добавил новый запрос: {0}", ItmSql.Name), "SQLList", EventEn.Message);

                    return base.Add(ItmSql, HashExeption);
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Произожла ошибка при сохранении нового запроса {0}: {1}", ItmSql.Name, ex.Message), GetType().Name + ".MyAdd", EventEn.Error);
                    throw;
                }

            }
        }

        /// <summary>
        /// Обновление запроса
        /// </summary>
        /// <param name="IndexSqlItem">Индекс позиции которую нужно заменить</param>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <returns>Результат выполнения операции</returns>
        private bool MyUpdate(int IndexSqlItem, ItemSQL ItmSql, bool HashExeption)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                try
                {
                    // Собственно обработка события
                    EventSQLList myArg = new EventSQLList(ItmSql);
                    if (onEventUpdateItemSql != null)
                    {
                        onEventUpdateItemSql.Invoke(this, myArg);
                    }

                    // Логируем изменение подключения
                    Log.EventSave(string.Format("Пользователь обновляет запрос: {0}", ItmSql.Name), "SQLList", EventEn.Message);

                    return base.Update(IndexSqlItem, ItmSql, HashExeption);
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Произожла ошибка при обновлении запроса {0}: {1}", ItmSql.Name, ex.Message), GetType().Name + ".MyUpdate", EventEn.Error);
                    throw;
                }

            }
        }

        /// <summary>
        /// Удаление запроса
        /// </summary>
        /// <param name="ItmSql">Запрос</param>
        /// <param name="HashExeption">Отображать ошибки пользователю?</param>
        /// <returns>Результат выполнения операции</returns>
        private bool MyRemove(ItemSQL ItmSql, bool HashExeption)
        {
            if (obj == null) throw new ApplicationException("Класс SQLList ещё не инициирован. Сначала запустите констроуктор а потом используейте методы");
            lock (obj)
            {
                try
                {
                    // Собственно обработка события
                    EventSQLList myArg = new EventSQLList(ItmSql);
                    if (onEventRemoveItemSql != null)
                    {
                        onEventRemoveItemSql.Invoke(this, myArg);
                    }

                    // Логируем изменение подключения
                    Log.EventSave(string.Format("Пользователь удаляет запрос: {0}", ItmSql.Name), "SQLList", EventEn.Message);

                    return base.Remove(ItmSql, HashExeption);
                }
                catch (Exception ex)
                {
                    Com.Log.EventSave(string.Format("Произожла ошибка при удалении запроса {0}: {1}", ItmSql.Name, ex.Message), GetType().Name + ".MyRemove", EventEn.Error);
                    throw;
                }

            }
        }

    }
}
