﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com.Provider.Lib
{
    /// <summary>
    /// Инитерфейс для всех провайдеров
    /// </summary>
    public interface ProviderI
    {
        /// <summary>
        /// Получение версии базы данных
        /// </summary>
        /// <returns>Возвращает версию базы данных в виде строки</returns>
        //string VersionDB();

        /// <summary>
        /// Провекрка конекта
        /// </summary>
        /// <returns></returns>
        bool testConnection();

        /// <summary>
        /// Процедура вызывающая настройку подключения
        /// </summary>
        /// <returns>Возвращает значение требуется ли сохранить подключение как основное или нет</returns>
        bool SetupConnectDB();

        /// <summary>
        /// Печать строки подключения с маскировкой секретных данных
        /// </summary>
        /// <returns>Строка подклюения с замасированной секретной информацией</returns>
        string PrintConnectionString();

        /// <summary>
        /// Устанавливаем факт по чеку
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="InvcNo">Сид докумнета</param>
        /// <param name="PosDate">Дата документа</param>
        /// <param name="TotalCashSum">Сумма по документу уплаченная налом</param>
        void SetPrizmCustPorog(string CustInn, string InvcNo, DateTime PosDate, decimal TotalCashSum);

        /// <summary>
        /// Необходимо изменить количество в товаре так как произошла ошибка при печати
        /// </summary>
        /// <param name="ProductSid">Идентификатор товара</param>
        /// <param name="qty">Количетсов товара которое необходимо добавить к текущему количеству</param>
        /// <param name="AddQty"></param>
        void SetQtyRollbackItem(string ProductSid, double qty);

        /// <summary>
        /// Установка признака отложенности докумнета
        /// </summary>
        /// <param name="DocumentSid">Идентификатор документа</param>
        void SetIsHelpRollbackDoc(string DocumentSid);

        /// <summary>
        /// Установка признака отложенного чека в документе
        /// </summary>
        /// <param name="DocumentSid">Идентификатор документа</param>
        /// <param name="IsHeld">Признак отложенного чека (0 активный | 1 отложенный)</param>
        void SetIsHeldForDocements(string DocumentSid, int IsHeld);

        /// <summary>
        /// Получить сумму по клиенту за дату
        /// </summary>
        /// <param name="CustInn">Инн покупателя</param>
        /// <param name="Dt">Дата смены</param>
        /// <returns>Сумму по клиенту за выбранную дату</returns>
        decimal GetTotalCashSum(string CustInn, DateTime Dt);

        /// <summary>
        /// Получить сумууму бонусов клиента
        /// </summary>
        /// <param name="CustSid">Идентификатор клиента</param>
        /// <returns>Бонусы доступные клиенту</returns>
        decimal GetCustBon(string CustSid);

        /// <summary>
        /// Возврат строк от документа сид которого мы указали
        /// </summary>
        /// <param name="referDocSid">Сид документа у которого надо получить данные из базы</param>
        /// <returns>сами строки документа</returns>
        List<BLL.JsonPrintFiscDocItem> GetItemsForReturnOrder(string referDocSid);

        /// <summary>
        /// Для получения номера карточки товара по её сиду
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        string GetInvnSbsItemNo(string InvnSbsItemSid);

        /// <summary>
        /// Получаем номер документа из базы данных
        /// </summary>
        /// <param name="sid"></param>
        /// <returns>Получаем номер документа</returns>
        Int64 GetDocNoFromDocument(string sid);

        /// <summary>
        /// Для получения содержимого полей text1-10 из карточки товаров
        /// </summary>
        /// <param name="InvnSbsItemSid">Сид карточки товара</param>
        /// <returns>Возвращаем номер карточки товара</returns>
        InvnSbsItemText GetInvnSbsItemText(string InvnSbsItemSid);

        /// <summary>
        /// Возврат тендера из ссылки на документ указанного в линке массива tenders
        /// </summary>
        /// <param name="itemTender">линк на строку из массива tenders</param>
        /// <returns>сама строка тендера</returns>
        BLL.JsonPrintFiscDocTender GetTenderForReturnOrder(BLL.JsonPrintFiscDocTender itemTender);

        /// <summary>
        /// Возврат строки тендера по номеру документа
        /// </summary>
        /// <param name="docsid">Номер документа</param>
        /// <returns>строки тендера из документа</returns>
        List<BLL.JsonPrintFiscDocTender> GetTendersForDocument(string docsid);

        /// <summary>
        /// Полусение логона пользователя по полному имени из таблицы rpsods.employee
        /// </summary>
        /// <param name="FullName">Полное имя</param>
        /// <returns>Логин</returns>
        string GetLoginFromEmplName(string FullName);
    }
}
