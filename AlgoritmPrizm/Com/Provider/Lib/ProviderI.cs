using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using AlgoritmPrizm.Lib;

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
    }
}
