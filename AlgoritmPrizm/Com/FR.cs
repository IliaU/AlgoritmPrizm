using AlgoritmPrizm.BLL;
using AlgoritmPrizm.Lib;
using DrvFRLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Класс для работы с фискальным регистратором
    /// </summary>
    public class FR
    {
        #region Private Param
        /// <summary>
        /// Для блокировки чтобы не шли запросы парралельно а только последовательно
        /// </summary>
        private static object obj = null;

        /// <summary>
        /// Представляет из себя объект для работы с фикальным регистратором
        /// </summary>
        private static DrvFRLib.DrvFR Fr = null;

        /// <summary>
        /// Ширина ленты
        /// </summary>
        private static int nLenLine;

        /// <summary>
        /// Текущий статус фискальника
        /// </summary>
        private static FrStatError Status = new FrStatError();

        /// <summary>
        /// Текущие ставки налогов установленные в фискальнике
        /// </summary>
        private static List<double> StavkiNDS = new List<double>();

        /// <summary>
        /// Текущие ставки налогов установленные в фискальнике для депозитов
        /// </summary>
        private static List<double> StavkiNDS_Dep = new List<double>();
        #endregion

        #region Public Param
        /// <summary>
        /// Сом порт на котором висит фискальник
        /// </summary>
        public static int ComPort { get; private set; }
        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="FRComPort">Порт который будет использоваться при работе с FR</param>
        public FR(int? FRComPort)
        {
            try
            {
                // если объект ещё не создан то создаём егои поверяем статус
                if (obj == null)
                {
                    Com.Log.EventSave("Запуск драйвера для работы с фискальным регистратором", GetType().Name, EventEn.Message);

                    // Создаём временный объект для работы с драйвером
                    int TmpComPort = FRComPort ?? 3;
                    DrvFRLib.DrvFR TmpFr = new DrvFR();
                    TmpFr.Password = 30;
                    TmpFr.PortNumber = TmpComPort;
                    TmpFr.ComputerName = Environment.MachineName;
                    //TmpFr.BaudRate скорость от 0..6
                    //TmpFr.Timeout  тайм аут в приёме байт от 0..255

                    // Если конект успешен
                    if (TestConnect(TmpFr))
                    {
                        // Сохраняем в случае успеха объект для работы других методов
                        ComPort = TmpComPort;
                        Fr = TmpFr;
                        obj = this;
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке драйвера для работы с фискальным регистратором с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                //throw ae;
            }
        }

        /// <summary>
        /// Проверка подключения и установка пареметров
        /// </summary>
        /// <param name="TmpFr"></param>
        /// <returns></returns>
        private static bool TestConnect(DrvFRLib.DrvFR TmpFr)
        {
            try
            {

                Lib.FrStatError ErrDetail;

                // Проверяем подключение
                if (TmpFr.Connect() != 0)
                {
                    bool FlagError = false;
                    // если ест ошибка пробуем найти устройство
                    try
                    {
                        TmpFr.FindDevice();
                        if (TmpFr.Connect() == 0) Config.FrPort = TmpFr.PortNumber;
                        else
                        {
                            FlagError = true;
                            ErrDetail = Verification(TmpFr);
                            throw new ApplicationException(string.Format("Отсутствует подключение к FR: {0} (Port - {1})", ErrDetail.Description, TmpFr.PortNumber));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (FlagError) throw ex;
                        else
                        {
                            ErrDetail = Verification(TmpFr);
                            throw new ApplicationException(string.Format("Несмогли обнаружить устройство FR: {0} - {1} (Port - {2})", ErrDetail.Description, ex.Message, TmpFr.PortNumber));
                        }
                    }
                }

                // Запрашивает технические параметры устройства и модифицирует свойства UMajorProtocolVersion, UMinorProtocolVersion, UMajorType, UMinorType, UModel, UCodePage, UDescription, CapGetShortECRStatus.
                if (TmpFr.GetDeviceMetrics() != 0)
                {
                    ErrDetail = Verification(TmpFr);
                    throw new ApplicationException(string.Format("Не смогли выполнить метод GetDeviceMetrics: {0}", ErrDetail.Description));
                }

                // Метод возвращает в свойства LDEscapeIP, LDEscapePort, LDEscapeTimeout ,LDName, LDComNumber, LDBaudrate, LDComputerName и LDTimeout параметры логического устройства с номером из свойства LDNumber.
                if (TmpFr.GetParamLD() != 0)
                {
                    ErrDetail = Verification(TmpFr);
                    throw new ApplicationException(string.Format("Не смогли выполнить метод GetParamLD: {0}", ErrDetail.Description));
                }

                // Выставляем шрифт и подсчитывем ширину ленты
                TmpFr.FontType = 1;         //Тип шрифта при печати строки.
                                            //Метод запрашивает параметры шрифта FontType и модифицирует свойства PrintWidth, CharWidth, CharHeight, FontCount.
                                            //Перед вызовом метода в свойстве Password указать пароль системного администратора.
                                            //В свойстве OperatorNumber возвращается порядковый номер оператора, чей пароль был введен.
                                            //Метод может вызываться в любом режиме.
                if (TmpFr.GetFontMetrics() != 0)
                {
                    ErrDetail = Verification(TmpFr);
                    throw new ApplicationException(string.Format("Не смогли выполнить метод GetFontMetrics: {0}", ErrDetail.Description));
                }
                if (TmpFr.CharWidth != 0) nLenLine = TmpFr.PrintWidth / TmpFr.CharWidth;
                else nLenLine = 36;

                // Заполняем ставками которые установленны в фискальнике
                StavkiNDS.Clear();
                double tmpNalogStavka;
                TmpFr.TableNumber = 6;
                TmpFr.RowNumber = 1;
                TmpFr.FieldNumber = 1;
                // Читаем таблицу для того чтобы всё записалось
                if (TmpFr.ReadTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой выполнении команды ReadTable при попытке прочитать ставки налога: {0}", Status.Description));
                }
                StavkiNDS.Add(Math.Round(double.Parse(TmpFr.ValueOfFieldString) / 100, 2));
                //
                TmpFr.TableNumber = 6;
                TmpFr.RowNumber = 2;
                TmpFr.FieldNumber = 1;
                // Читаем таблицу для того чтобы всё записалось
                if (TmpFr.ReadTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой выполнении команды ReadTable при попытке прочитать ставки налога: {0}", Status.Description));
                }
                tmpNalogStavka = Math.Round(double.Parse(TmpFr.ValueOfFieldString) / 100, 2);
                if (StavkiNDS.Where(a => a == tmpNalogStavka).Count() == 0)
                    StavkiNDS.Add(tmpNalogStavka);


                // Заполняем ставками которые установленны в фискальнике Для депозитов
                StavkiNDS_Dep.Clear();
                double tmpNalogStavkaDep;
                TmpFr.TableNumber = 6;
                TmpFr.RowNumber = 5;
                TmpFr.FieldNumber = 1;
                // Читаем таблицу для того чтобы всё записалось
                if (TmpFr.ReadTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой выполнении команды ReadTable при попытке прочитать ставки налога: {0}", Status.Description));
                }
                StavkiNDS_Dep.Add(Math.Round(double.Parse(TmpFr.ValueOfFieldString) / 100, 2));
                //
                TmpFr.TableNumber = 6;
                TmpFr.RowNumber = 6;
                TmpFr.FieldNumber = 1;
                // Читаем таблицу для того чтобы всё записалось
                if (TmpFr.ReadTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой выполнении команды ReadTable при попытке прочитать ставки налога: {0}", Status.Description));
                }
                tmpNalogStavkaDep = Math.Round(double.Parse(TmpFr.ValueOfFieldString) / 100, 2);
                if (StavkiNDS_Dep.Where(a => a == tmpNalogStavkaDep).Count() == 0)
                    StavkiNDS_Dep.Add(tmpNalogStavkaDep);


                return true;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке драйвера для работы с фискальным регистратором с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.TestConnect", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Снятие  Z отчёта
        /// </summary>
        /// <returns>Статус</returns>
        public static Lib.FrStatError ZREport()
        {
            Lib.FrStatError rez = new FrStatError();
            try
            {
                if (Fr.FNCloseSession() != 0)
                {
                    rez = Verification(Fr);

                    //При ошибке с бумагой печать продолжается после верификации но маса верификация выдаёт код ошибки 2 которую мы игнорируем так как печать продолжилась сам если вдруг печать не продолжится воткнём Fr.ContinuePrint() чтобы продолжить печать
                    if (rez.CodeECRMode != 2)
                    {
                        rez = Verification(Fr);
                        throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.ZREport", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Снятие  X отчёта
        /// </summary>
        /// <returns>Статус</returns>
        public static void XREport()
        {
            Lib.FrStatError rez = new FrStatError();
            try
            {
                if (Fr.PrintReportWithoutCleaning() != 0)
                {
                    rez = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.XREport", EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Открытие смены
        /// </summary>
        /// <returns></returns>
        public static Lib.FrStatError OpenShift()
        {
            Lib.FrStatError rez = new FrStatError();
            try
            {
                if (Fr.FNOpenSession() != 0)
                {
                    rez = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.XReport", EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Печать чека
        /// </summary>
        /// <param name="Doc">Документ который представляет из себя чек</param>
        /// <param name="OperatorNumber"> номер оператора который указан в фискальнике</param>
        /// <param name="DocName">Заголовок документа</param>
        /// <param name="IsCopy">Заголовок документа</param>
        /// <returns>Возвращаем номердокумента и поле в которое нужно его сохранять</returns>
        public static JsonPrintFiscDocReturn PrintCheck(JsonPrintFiscDoc Doc, int OperatorNumber, string DocName, bool IsCopy)
        {
            JsonPrintFiscDocReturn rezWeb = new JsonPrintFiscDocReturn(Com.Config.FieldDocNum);

            //string Matrix = @"010290000066650421Jsid2E""4oh2 > T91002a92 / 1tPzrragHUbOA + cq0FIp54OZZF6GcVCJhA9W6Mnb7W6LvZEn9r9thrj + HsBFpqyH / zl5Ri6pXxF3HTwjuWeKG == 007R";
            Lib.FrStatError rez = new FrStatError();
            try
            {
                /// Тип документа
                EnFrTyp DocCustTyp = EnFrTyp.Default;

                // Поиск  сотрудника
                Custumer TekCustomer = Com.Config.customers.Find(x => x.login.ToUpper() == Doc.cashier_login_name.ToUpper());
                if (TekCustomer == null) throw new ApplicationException(string.Format("Не можем сопоставить пользователя в RPro ({0}) с логином в конфиге", Doc.cashier_login_name));

                // Проверка включена опция с подаочными картами
                if (Com.Config.GiftCardEnable)
                {
                    // Пробегаем по строкам документа
                    foreach (JsonPrintFiscDocItem item in Doc.items)
                    {
                        if (item.dcs_code.IndexOf(Com.Config.GiftCardCode) >= 0)
                        {
                            DocCustTyp = EnFrTyp.GiftCard;
                            break;
                        }
                    }
                    if (DocCustTyp == EnFrTyp.GiftCard && Doc.items.Count != 1) throw new ApplicationException("Данный чек не может быть закрыт, т.к более 1-й подарочной карты");
                }

                //************** ОТКРЫТИЕ ЧЕКА ********************************************

                // Провеяем статус фискальника и если чего не хватает правим. например есть чек не завершонный, смена не открыта итд
                if (ConfigStatusFR(Doc, OperatorNumber, DocName)) return rezWeb;

                // Проверка строк документа если они пустые то получаем их из базы давнных и обогощаем тем что имеем
                if (Doc.items.Count(t => string.IsNullOrWhiteSpace(t.invn_sbs_item_sid) &&
                    !string.IsNullOrWhiteSpace(t.link)) > 0)
                {
                    List<JsonPrintFiscDocItem> DocTmp = Com.ProviderFarm.CurrentPrv.GetItemsForReturnOrder(Doc.sid);
                    for (int i = 0; i < Doc.items.Count(); i++)
                    {
                        DocTmp[i].link = Doc.items[i].link;
                        Doc.items[i] = DocTmp[i];
                    }
                }

                // Если нет строк то надо получить инфу от ссылки на докумен для специального документа (возврат депозита)
                if (Doc.items.Count == 0)
                {
                    List<JsonPrintFiscDocItem> DocTmp = Com.ProviderFarm.CurrentPrv.GetItemsForReturnOrder(Doc.ref_order_sid);
                    foreach (JsonPrintFiscDocItem itemDocFtmp in DocTmp)
                    {
                        Doc.items.Add(itemDocFtmp);
                    }

                    List<BLL.JsonPrintFiscDocTender> TenTmp = Com.ProviderFarm.CurrentPrv.GetTendersForDocument(Doc.ref_order_sid);
                    foreach (BLL.JsonPrintFiscDocTender itemTenTmp in TenTmp)
                    {
                        if (itemTenTmp.given == 0 && itemTenTmp.taken != 0)
                        {
                            itemTenTmp.given = itemTenTmp.taken;
                            itemTenTmp.taken = 0;
                            Doc.tenders.Add(itemTenTmp);
                        }
                    }

                    DocCustTyp = EnFrTyp.ReturnDeposit;
                }
                else
                {
                    // Если скидка есть а процент в Json нет явно косяк, идём в API и получаем строчку от туда
                    if (Doc.items.Count(t => t.discount_perc == 0 && t.discount_amt != 0) > 0)
                    {
                        for (int i = 0; i < Doc.items.Count; i++)
                        {
                            List<JsonPrintFiscDocItem> JournalRowTmp = Web.GetCopyDocumentsJournalRestSharp(Doc.items[i].link);
                            Doc.items[i].discount_perc = JournalRowTmp[0].discount_perc;
                        }
                    }
                }

                // Если это копия то нужно инфу подтянуть по API  та как падла не передаёт в доке :-(
                if (IsCopy)
                {
                    for (int i = 0; i < Doc.items.Count; i++)
                    {
                        List<JsonPrintFiscDocItem> JournalRowTmp = Web.GetCopyDocumentsJournalRestSharp(Doc.items[i].link);
                        if (JournalRowTmp.Count > 0) Doc.items[i] = JournalRowTmp[0];
                    }

                    for (int i = 0; i < Doc.tenders.Count; i++)
                    {
                        List<JsonPrintFiscDocTender> TenderRowTmp = Web.GetCopyDocumentsTenderRestSharp(Doc.tenders[i].link);
                        if (TenderRowTmp.Count > 0) Doc.tenders[i] = TenderRowTmp[0];
                    }
                }
                else
                {
                    for (int i = 0; i < Doc.tenders.Count; i++)
                    {
                        List<JsonPrintFiscDocTender> TenderRowTmp = Web.GetCopyDocumentsTenderRestSharp(Doc.tenders[i].link);
                        if (TenderRowTmp.Count > 0)
                        {
                            Doc.tenders[i] = TenderRowTmp[0];
                        }
                    }
                }

                // Открываем чек
                OpenReceipt(Doc, TekCustomer, DocCustTyp, IsCopy);

                // Печатаем заголовок
                PrintCheckHead(IsCopy, Doc, TekCustomer);

                //************** ПЕЧАТЬ ПОЗИЦИЙ ЧЕКА **************************************

                int TekDocStavkiNDS1 = 0;      // Нал
                int TekDocStavkiNDS2 = 0;      // Безнал
                int TekDocStavkiNDS3 = 0;      // Смешанный
                int TekDocStavkiNDS4 = 0;      // Депозит

                // Если это возврат денег с заказа клиента то лезем в ссылку на документ источник и получаем от туда нужные строки из базы данных
                if (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.receipt_type == 0 && Doc.given_amt != 0 && Doc.items.Count == 0)
                {
                    string referDocSid = Doc.ref_order_sid;
                    foreach (BLL.JsonPrintFiscDocItem nitemRefer in Com.ProviderFarm.CurrentPrv.GetItemsForReturnOrder(referDocSid))
                    {
                        Doc.items.Add(nitemRefer);
                    }
                }

                // Возврат депозита воркраунд для подтягивания строк из tenders
                if (Doc.receipt_type == 0 && Doc.tenders.Count > 0
                    && Doc.tenders.Count(t => t.taken == 0) == Doc.tenders.Count)
                {
                    DocCustTyp = EnFrTyp.ReturnDeposit;

                    // Проходим по позициям тендера и тажим инфук в чек
                    for (int i = 0; i < Doc.tenders.Count; i++)
                    {
                        Doc.tenders[i] = Com.ProviderFarm.CurrentPrv.GetTenderForReturnOrder(Doc.tenders[i]);
                    }
                }

                // Сумма чека для предоплаты в момент просчёта строк
                decimal SumChekForPredoplata = 0;
                FrGetSummCheckRez SumChekFoCustomer = GetSummCheck(Doc, DocCustTyp, false); // Получаем сумму чека из фискальника чтобы потом этого не делать при просчёте позиций во время предоплаты
                decimal SumChekFoPrice = 0;
                foreach (JsonPrintFiscDocItem item in Doc.items)
                {
                    SumChekFoPrice += (decimal)(item.quantity * item.price);
                }

                // Пробегаем по строкам докумнета
                for (int itm = 0; itm < Doc.items.Count; itm++)
                {
                    int TekStavkiNDS1 = 0;      // Нал
                    int TekStavkiNDS2 = 0;      // Безнал
                    int TekStavkiNDS3 = 0;      // Смешанный
                    int TekStavkiNDS4 = 0;      // Депозит
                    string TekStavkiNdsDescription = "";

                    // В зависимости от типа докумена разный вариант ставки
                    switch (Doc.receipt_type)
                    {
                        case 0:
                        case 1:

                            switch (Doc.items[itm].tax_percent.ToString())
                            {
                                case "10":
                                    TekStavkiNDS1 = Config.TaxPercent10;
                                    TekStavkiNdsDescription = "10";
                                    break;
                                case "20":
                                    TekStavkiNDS1 = Config.TaxPercent20;
                                    TekStavkiNdsDescription = "20";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            switch (Doc.items[itm].tax_percent.ToString())
                            {
                                case "10":
                                    TekStavkiNDS1 = Config.TaxPercent10110;
                                    TekStavkiNdsDescription = "10";
                                    break;
                                case "20":
                                    TekStavkiNDS1 = Config.TaxPercent20120;
                                    TekStavkiNdsDescription = "20";
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                    }

                    // Исключение для заказов 10/110  20/120
                    if (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.receipt_type == 0 && Doc.given_amt != 0)
                    {
                        switch (TekStavkiNDS1)
                        {
                            case 1:// 20%
                                //TekStavkiNDS1 = 5; // 20/120
                                TekStavkiNDS1 = Config.TaxPercent20120;
                                TekStavkiNdsDescription = "20/120";
                                break;
                            case 2:// 10%
                                //TekStavkiNDS1 = 6; // 10/110
                                TekStavkiNDS1 = Config.TaxPercent10110;
                                TekStavkiNdsDescription = "10/110";
                                break;
                            default:
                                break;
                        }
                    }


                    // По умолчанию считам что строки с маркиовкой нет
                    bool flagmarkink = false;


                    // Проверяем есть кодмаркировки или нет по полю note1
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note1))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note1, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note2
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note2))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note2, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note3
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note3))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note3, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note4
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note4))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note4, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note5
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note5))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note5, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note6
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note6))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note6, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note7
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note7))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note7, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note8
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note8))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note8, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note9
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note9))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note9, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note10
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note10))
                    {
                        flagmarkink = true;
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note10, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }

                    // Если эта строка не содержала товаров с маркировкой
                    if (!flagmarkink)
                    {
                        PrintCheckItem(IsCopy, Doc, Doc.items[itm], itm, Doc.items[itm].note10, 1, TekStavkiNdsDescription, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer.itog, SumChekFoPrice);
                    }
                }

                //************** СКИДКА/НАЦЕНКА НА ЧЕК ************************************
                PrinCheckDiscount(IsCopy, Doc);

                //************** ОПЛАТЫ ПО ЧЕКУ ******************************************

                // Печать концовки чека
                CloseReceipt(IsCopy, Doc, DocCustTyp, TekDocStavkiNDS1, TekDocStavkiNDS2, TekDocStavkiNDS3, TekDocStavkiNDS4);

                // Обновляем статус и опрашиваем фискальник на предмет получения последнего номера документа
                if (!IsCopy) Fr.FNGetStatus();

                //Web.UpdateFiskDocNum(Doc, Fr.DocumentNumber);
                if (!IsCopy) rezWeb.fiscDocNum = Fr.DocumentNumber;

                Cut();

                // Открываем денежный ящик
                //if (!IsCopy) OpenDrawer();

                return rezWeb;
            }
            catch (Exception ex)
            {
                // Сохраняем ошибку
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintCheck", EventEn.Error);

                // Отменяем чек
                try
                {
                    if (Fr.CancelCheck() != 0)
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Не смогли отменить чек который упал с ошибой: ({0}) так как словили ошибку при отмене чека: {1}", ae.Message, Status.Description));
                    }
                }
                catch (Exception) { }

                throw ae;
            }
        }

        /// <summary>
        /// Печать чека
        /// </summary>
        /// <param name="Doc">Документ который представляет из себя чек</param>
        /// <param name="OperatorNumber"> номер оператора который указан в фискальнике</param>
        /// <param name="DocName">Заголовок документа</param>
        /// <returns>Возвращаем номердокумента и поле в которое нужно его сохранять</returns>
        public static JsonPrintFiscDocReturn PrintCheck(JsonPrintFiscDoc Doc, int OperatorNumber, string DocName)
        {
            try
            {
                return PrintCheck(Doc, OperatorNumber, DocName, false);
            }
            catch (Exception ex)
            {
                // Сохраняем ошибку
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintCheck", EventEn.Error);

                // Отменяем чек
                try
                {
                    if (Fr.CancelCheck() != 0)
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Не смогли отменить чек который упал с ошибой: ({0}) так как словили ошибку при отмене чека: {1}", ae.Message, Status.Description));
                    }
                }
                catch (Exception) { }

                throw ae;
            }
        }

        /// <summary>
        /// Печать двух строк в одной строке
        /// </summary>
        /// <param name="S1">Первая строка</param>
        /// <param name="S2">Втрая строка</param>
        private static void Print2in1Line(string S1, string S2)
        {
            try
            {

                if ((S1.Length + S2.Length) < nLenLine)
                {
                    PrintLine(string.Format("{0}{1}{2}", S1, new string(' ', nLenLine - (S1.Length + S2.Length)), S2));
                }
                else
                {
                    if (!Status.HashError)
                    {
                        PrintLine(string.Format("{0}{1}", new string(' ', nLenLine - S2.Length), S2));
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати двух строк в чеке: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.Print2in1Line", EventEn.Error);
                throw ae;
            }
        }
        //
        /// <summary>
        /// Печать обычной строки
        /// </summary>
        /// <param name="S">Строка которую надо напечатать</param>
        /// <param name="AlingRirht">Выравнивание справа</param>
        /// <returns></returns>
        private static void PrintLineCenter(string S)
        {
            try
            {
                Fr.StringForPrinting = string.Format("{0}{1}", new string(' ', (nLenLine - S.Length) / 2), S);

                if (Fr.PrintString() != 0)
                {
                    FrStatError rez = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой при печати строки ({0}) в чеке: {1}", S, rez.Description));
                }
                Fr.StringForPrinting = "";
                //                Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintLineCenter", EventEn.Error);
                throw ae;
            }
        }
        //
        /// <summary>
        /// Печать обычной строки
        /// </summary>
        /// <param name="S">Строка которую надо напечатать</param>
        /// <param name="AlingRirht">Выравнивание справа</param>
        /// <returns></returns>
        private static void PrintLine(string S, bool AlingRirht)
        {
            try
            {
                if (AlingRirht) Fr.StringForPrinting = string.Format("{0}{1}", new string(' ', nLenLine - S.Length), S);
                else Fr.StringForPrinting = S;

                if (Fr.PrintString() != 0)
                {
                    FrStatError rez = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой при печати строки ({0}) в чеке: {1}", S, rez.Description));
                }
                Fr.StringForPrinting = "";
                //                Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintLine", EventEn.Error);
                throw ae;
            }
        }
        //
        /// <summary>
        /// Печать обычной строки
        /// </summary>
        /// <param name="S">Строка которую надо напечатать</param>
        /// <returns></returns>
        private static void PrintLine(string S)
        {
            try
            {
                PrintLine(S, false);

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintLine", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Отчеркиваем заголовок или ещё какую часть чека
        /// </summary>
        /// <returns>просто печатает чтобы отжелить часть чека</returns>
        private static void PrintSeparator()
        {
            try
            {
                PrintLine(new string('-', nLenLine));
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печате разделителя внутри чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintSeparator", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Печать маркорованной строки в чеке 
        /// </summary>
        /// <param name="IsCopy">Копия чека</param>
        /// <param name="Doc">Сам документ</param>
        /// <param name="item">строка которую берём за образец</param>
        /// <param name="IndexPos">Индекс позиции в чеке</param>
        /// <param name="note">код маркировки который нужно отправить</param>
        /// <param name="Department">подозреваю что тут он не нужен надо проверить ??????</param>
        /// <param name="TekStavkiNdsDescription">НДС в виде строки</param>
        /// <param name="Tax1">Группа налогов не понятно что передавать передавал 1</param>
        /// <param name="Tax2">Группа налогов не понятно что передавать передавал 2</param>
        /// <param name="Tax3">Группа налогов не понятно что передавать передавал 0</param>
        /// <param name="Tax4">Группа налогов не понятно что передавать передавал 0</param>
        /// <param name="DocCustTyp">Тип документа</param>
        /// <param name="SumChekForPredoplata">Сумма чека для предоплаты в момент просчёта строк</param>
        /// <param name="IndexItemForPredoplata">Индекс позиции в чеке</param>
        /// <param name="CountForPredoplata">Количество позиций в чеке</param>
        /// <param name="SumChekFoCustomer">Итоговая сколько заплачено покупателем по всему чеку нужно для выявления пропорции на сколько уменьшать этот чек чтобы сумма сошлась</param>
        /// <param name="SumChekFoPrice">Итоговая сумма по чеку по ценам магазина а не по тому что внёс покупатель</param>
        private static void PrintCheckItem(bool IsCopy, JsonPrintFiscDoc Doc, JsonPrintFiscDocItem item, int IndexPos, string note, int Department, string TekStavkiNdsDescription, int Tax1, int Tax2, int Tax3, int Tax4, EnFrTyp DocCustTyp, ref decimal SumChekForPredoplata, int IndexItemForPredoplata, int CountForPredoplata, decimal SumChekFoCustomer, decimal SumChekFoPrice)
        {
            try
            {
                // Печать сотрудника если такая настройка включена
                if (Config.EmployeePrintingForEveryLine)
                {
                    // Печатаем сотрудника продавшего товар
                    string employee = null;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee1_name)) employee = item.employee1_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee2_name)) employee = item.employee2_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee3_name)) employee = item.employee3_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee4_name)) employee = item.employee4_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee5_name)) employee = item.employee5_name;
                    //
                    if (!string.IsNullOrWhiteSpace(employee))
                    {
                        Employee TekEmployees = Config.employees.Find(t => t.PrizmLogin.ToUpper() == employee.ToUpper());
                        if (TekEmployees != null && !string.IsNullOrWhiteSpace(TekEmployees.fio_fo_check)) employee = TekEmployees.fio_fo_check.Trim();
                        Print2in1Line("Сотрудник:", employee);
                    }
                }

                // Печать штрих кода
                //if (!string.IsNullOrWhiteSpace(item.scan_upc)) PrintLine(item.scan_upc);

                //Описание товара
                InvnSbsItemText TmpTextInfo = ProviderFarm.CurrentPrv.GetInvnSbsItemText(item.invn_sbs_item_sid);
                if (string.IsNullOrWhiteSpace(item.invn_sbs_item_sid)) item.invn_sbs_item_sid = TmpTextInfo.item_sid;
                if (string.IsNullOrWhiteSpace(item.item_description1)) item.item_description1 = TmpTextInfo.description1;
                if (string.IsNullOrWhiteSpace(item.item_description2)) item.item_description2 = TmpTextInfo.description2;
                if (string.IsNullOrWhiteSpace(item.attribute)) item.attribute = TmpTextInfo.attribute;
                if (string.IsNullOrWhiteSpace(item.scan_upc)) item.scan_upc = TmpTextInfo.upc;
                if (string.IsNullOrWhiteSpace(item.item_size)) item.item_size = TmpTextInfo.item_size;
                //
                string StringForPrinting = item.item_description1;
                switch (Config.FieldItem)
                {
                    case FieldItemEn.Description1:
                        StringForPrinting = string.Format("{0}", item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        StringForPrinting = string.Format("{0}", item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0}", tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        StringForPrinting = string.Format("{0}", item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        StringForPrinting = string.Format("{0}", item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        StringForPrinting = string.Format("{0}", item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0}", TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        StringForPrinting = string.Format("{0}", item.item_description1).Trim();
                        break;
                }
                switch (Config.FieldItem1)
                {
                    case FieldItemEn.Description1:
                        if (!string.IsNullOrWhiteSpace(item.item_description1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        if (!string.IsNullOrWhiteSpace(item.item_description2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        if (!string.IsNullOrWhiteSpace(item.attribute)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        if (!string.IsNullOrWhiteSpace(item.item_size)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        if (!string.IsNullOrWhiteSpace(item.scan_upc)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        break;
                }
                switch (Config.FieldItem2)
                {
                    case FieldItemEn.Description1:
                        if (!string.IsNullOrWhiteSpace(item.item_description1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        if (!string.IsNullOrWhiteSpace(item.item_description2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        if (!string.IsNullOrWhiteSpace(item.attribute)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        if (!string.IsNullOrWhiteSpace(item.item_size)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        if (!string.IsNullOrWhiteSpace(item.scan_upc)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        break;
                }
                switch (Config.FieldItem3)
                {
                    case FieldItemEn.Description1:
                        if (!string.IsNullOrWhiteSpace(item.item_description1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        if (!string.IsNullOrWhiteSpace(item.item_description2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        if (!string.IsNullOrWhiteSpace(item.attribute)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        if (!string.IsNullOrWhiteSpace(item.item_size)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        if (!string.IsNullOrWhiteSpace(item.scan_upc)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        break;
                }
                switch (Config.FieldItem4)
                {
                    case FieldItemEn.Description1:
                        if (!string.IsNullOrWhiteSpace(item.item_description1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        if (!string.IsNullOrWhiteSpace(item.item_description2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        if (!string.IsNullOrWhiteSpace(item.attribute)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        if (!string.IsNullOrWhiteSpace(item.item_size)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        if (!string.IsNullOrWhiteSpace(item.scan_upc)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        break;
                }
                switch (Config.FieldItem5)
                {
                    case FieldItemEn.Description1:
                        if (!string.IsNullOrWhiteSpace(item.item_description1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description1).Trim();
                        break;
                    case FieldItemEn.Description2:
                        if (!string.IsNullOrWhiteSpace(item.item_description2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_description2).Trim();
                        break;
                    case FieldItemEn.InvnSbsItemNo:
                        string tmp = Com.ProviderFarm.CurrentPrv.GetInvnSbsItemNo(item.invn_sbs_item_sid);
                        if (!string.IsNullOrWhiteSpace(tmp)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, tmp).Trim();
                        break;
                    case FieldItemEn.Attribute:
                        if (!string.IsNullOrWhiteSpace(item.attribute)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.attribute).Trim();
                        break;
                    case FieldItemEn.ItemSize:
                        if (!string.IsNullOrWhiteSpace(item.item_size)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.item_size).Trim();
                        break;
                    case FieldItemEn.ScanUpc:
                        if (!string.IsNullOrWhiteSpace(item.scan_upc)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, item.scan_upc).Trim();
                        break;
                    case FieldItemEn.Text1:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text1)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text1).Trim();
                        break;
                    case FieldItemEn.Text2:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text2)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text2).Trim();
                        break;
                    case FieldItemEn.Text3:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text3)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text3).Trim();
                        break;
                    case FieldItemEn.Text4:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text4)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text4).Trim();
                        break;
                    case FieldItemEn.Text5:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text5)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text5).Trim();
                        break;
                    case FieldItemEn.Text6:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text6)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text6).Trim();
                        break;
                    case FieldItemEn.Text7:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text7)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text7).Trim();
                        break;
                    case FieldItemEn.Text8:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text8)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text8).Trim();
                        break;
                    case FieldItemEn.Text9:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text9)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text9).Trim();
                        break;
                    case FieldItemEn.Text10:
                        if (TmpTextInfo != null && !string.IsNullOrWhiteSpace(TmpTextInfo.Text10)) StringForPrinting = string.Format("{0} {1}", StringForPrinting, TmpTextInfo.Text10).Trim();
                        break;
                    default:
                        break;
                }

                if (!IsCopy) Fr.StringForPrinting = StringForPrinting;


                // Если есть матрих код то количество не может быть более 1
                double QtyForPrinting;
                if (string.IsNullOrWhiteSpace(note)) QtyForPrinting = item.quantity;
                else
                {  // Матрикс код обнаружен Логируем инфу
                    QtyForPrinting = 1;                            // Код маркировки есть значит количество 1
                    FileCheckLog.EventPrintSave(note, Doc.sid, (decimal)item.price, DocCustTyp, Doc.receipt_type, item.sid);
                }
                if (!IsCopy) Fr.Quantity = QtyForPrinting;

                // Если похоже на предоплату и общая сумма по документу не равна сумме что заплатил покупатель то будем рассчитывать пропорционально цену
                decimal PriceForPrinting;
                if ((Doc.receipt_type == 2 && SumChekFoCustomer != SumChekFoPrice)
                    // Если это возврат денег с заказа клиента то лезем в ссылку на документ источник и получаем от туда нужные строки из базы данных
                    || (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.receipt_type == 0 && Doc.given_amt != 0 && SumChekFoCustomer < SumChekFoPrice)
                    || DocCustTyp == EnFrTyp.ReturnDeposit
                    )
                {
                    // Получаем на сколько стандартно домножаем сумму для того чтобы пропарционально позиции в чеке поменять
                    decimal PrcForPrice = SumChekFoCustomer / SumChekFoPrice;

                    // если это последняя позиция в чеке то нужно накзначить так сумму чтобы она сошлась с тем что заплотил пользователь
                    if (IndexItemForPredoplata + 1 == CountForPredoplata)
                    {
                        PriceForPrinting = (SumChekFoCustomer - SumChekForPredoplata) / (decimal)item.quantity;
                        if (!IsCopy) Fr.Price = PriceForPrinting;
                        SumChekForPredoplata += (SumChekFoCustomer - SumChekForPredoplata);
                    }
                    else
                    {
                        PriceForPrinting = (decimal)item.price * PrcForPrice;
                        if (!IsCopy) Fr.Price = PriceForPrinting;
                        SumChekForPredoplata += (decimal)item.price * PrcForPrice * (decimal)item.quantity;
                    }
                }
                else
                {
                    PriceForPrinting = (decimal)item.price;  // Цена в строке  (decimal)1.56;  
                    if (!IsCopy) Fr.Price = PriceForPrinting;
                }

                // В зависимости от типа документа
                if (!IsCopy)
                {
                    switch (DocCustTyp)
                    {
                        case EnFrTyp.GiftCard:
                            Fr.PaymentTypeSign = 3;             // Типа расчета - аванс
                            Fr.PaymentItemSign = 10;            // Признак предмета расчета (платёж)
                            break;
                        default:
                            // В зависимости от типа докумена разный вариант ставки
                            switch (Doc.receipt_type)
                            {
                                case 0:
                                case 1:
                                    Fr.PaymentTypeSign = 4;     // Типа расчета - полный расчет
                                    Fr.PaymentItemSign = 1;     // Признак предмета расчета (Товар)
                                    break;
                                case 2:
                                    // Если это аванс при депозите
                                    if (StringForPrinting.ToUpper().IndexOf("АВАНС") >= 0)
                                    {
                                        Fr.PaymentTypeSign = 3;             // Типа расчета - аванс
                                        Fr.PaymentItemSign = 10;            // Признак предмета расчета (платёж)
                                    }
                                    else
                                    {
                                        if (SumChekFoCustomer != SumChekFoPrice) Fr.PaymentTypeSign = 2;     // Типа расчета - частичная предоплата
                                        else Fr.PaymentTypeSign = 1;        // Типа расчета - предоплата 100%

                                        Fr.PaymentItemSign = 10;            //Признак предмета расчета(Платеж)
                                    }
                                    break;
                                default:
                                    throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                            }
                            break;
                    }
                }

                //
                // Продолжаем наритать строку
                if (!IsCopy) Fr.Department = Department;       // Надо проверить возможно не нужно передавать при печати строк

                // В зависимости от типа документа разная группа налогов
                int Tax1ForPrinting;
                int Tax2ForPrinting;
                int Tax3ForPrinting;
                int Tax4ForPrinting;
                switch (DocCustTyp)
                {
                    case EnFrTyp.GiftCard:
                        Tax1ForPrinting = Com.Config.GiftCardTax;
                        if (!IsCopy) Fr.Tax1 = Tax1ForPrinting;
                        break;
                    default:
                        Tax1ForPrinting = Tax1;
                        if (!IsCopy) Fr.Tax1 = Tax1ForPrinting;
                        Tax2ForPrinting = Tax2;
                        if (!IsCopy) Fr.Tax2 = Tax2ForPrinting;
                        Tax3ForPrinting = Tax3;
                        if (!IsCopy) Fr.Tax3 = Tax3ForPrinting;
                        Tax4ForPrinting = Tax4;
                        if (!IsCopy) Fr.Tax4 = Tax4ForPrinting;
                        break;
                }


                if (IsCopy)
                {
                    PrintLine(StringForPrinting);
                    PrintLine(string.Format("{0} X {1}", QtyForPrinting.ToString(), PriceForPrinting.ToString()), true);
                    Print2in1Line(QtyForPrinting.ToString(), string.Format("={0}", (QtyForPrinting * (double)PriceForPrinting).ToString()));
                }
                else
                {
                    //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                    switch (Doc.receipt_type)
                    {
                        case 0:  // Покупка
                        case 2:  // Депозит
                                 // Печать строки в чеке
                            if (Fr.FNOperation() != 0)
                            {
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Упали с ошибкой при печати позиции в чеке {1}: {0}", Status.Description, item.item_pos));
                            }
                            break;
                        case 1:  // Возврат
                            if (Fr.FNOperation() != 0)    //ReturnBuy
                            {
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Упали с ошибкой при печати строки чека {1}: {0}", Status.Description, item.item_pos));
                            }
                            break;
                        default:
                            throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                    }
                }

                // Если это не копия чека
                if (!IsCopy)
                {
                    // Если есть матрихс код то добавляем его
                    if (!string.IsNullOrWhiteSpace(note))
                    {
                        // Если это меховой товар то делаем просто отправку бар кода
                        if (Com.Config.MexSendItemBarcode && note.IndexOf("RU-") == 0)
                        {
                            Fr.BarCode = note;
                            //Fr.ItemStatus = 1;

                            if (Fr.FNSendItemBarcode() != 0)             // Отправляем матрикс код
                            {
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Упали с ошибкой при отправке матрикс кода в строке {1}: {0}", Status.Description, item.item_pos));
                            }
                        }
                        else
                        {
                            if (note.IndexOf("RU-") == 0) Log.EventSave("Обнаружена продажа меха при выключенной поддержки реализации меха.", "Com.FR.PrintCheckItem", EventEn.Warning);

                            // парсинг относительно ТЗ (Доработка КМ перед передачей в ККТ)
                            // 01
                            string PrefA = null;
                            if (note.Length > 1) PrefA = note.Substring(0, 2);
                            // Gtin
                            string PrefBgtin = null;
                            if (note.Length > 15) PrefBgtin = note.Substring(2, 14);
                            // 21
                            string PrefC = null;
                            if (note.Length > 17) PrefC = note.Substring(16, 2);
                            // Идентификатор экземпляра
                            string PrefD = null;
                            if (note.Length > 30) PrefD = note.Substring(18, 13);
                            // 91
                            string PrefE = null;
                            if (note.Length > 32) PrefE = note.Substring(31, 2);
                            //
                            string PrefF = null;
                            if (note.Length > 36) PrefF = note.Substring(33, 4);
                            // 92
                            string PrefG = null;
                            if (note.Length > 38) PrefG = note.Substring(37, 2);
                            // Крипто хвост
                            string PrefH = null;
                            if (note.Length > 40) PrefH = note.Substring(39);

                            // Вставляем тег матрикс кода
                            Fr.MarkingType = 5408;

                            // Проверяем версию FFD
                            switch (Config.Ffd)
                            {
                                case FfdEn.v1_05:
                                    Fr.BarCode = note;
                                    break;
                                case FfdEn.v1_2:
                                    // Если хоть один параметр не найден то работаем по логике предыдущей версии
                                    if (string.IsNullOrEmpty(PrefA) || string.IsNullOrEmpty(PrefBgtin)
                                        || string.IsNullOrEmpty(PrefC) || string.IsNullOrEmpty(PrefD)
                                        || string.IsNullOrEmpty(PrefE) || string.IsNullOrEmpty(PrefF)
                                        || string.IsNullOrEmpty(PrefG) || string.IsNullOrEmpty(PrefH))
                                    {
                                        Fr.BarCode = note;
                                        Log.EventSave(string.Format("Не смогли преобразовать матрикс код к формату ФФД 1.2 ({0})", note), "Com.FR.PrintCheckItem", EventEn.Error);
                                    }
                                    else
                                    {
                                        string BarCodeTmp = string.Format("{1}{2}{3}{4}{0}{5}{6}{0}{7}{8}", (char)29, PrefA, PrefBgtin, PrefC, PrefD, PrefE, PrefF, PrefG, PrefH);

                                        Fr.BarCode = BarCodeTmp;
                                        Fr.ItemStatus = 1;
                                        Fr.TLVDataHex = "";     // На текущий момент стало поступать достаточно много вопросов связанных с причинами возникновения ошибки 11: "Неразрешенные реквизиты" в ответ на команду проверки кода маркировки (метод FNCheckItemBarcode, реализующий команду FF61h). Не смотря на то, что данная ошибки не декларирована в протоколе ФН под ФФД1.2, но она возникает. Причина ошибки в том, что при заполнении реквизитов, необходимых для проверки кода маркировки, пользователь (разработчик ПО) не заполняет все необходимые поля, а именно свойство TLVDataHex. Если не реализуется дробное кол-во предмета расчета, то в данное свойство нужно в явном виде передавать "пустую строку". Если этого не сделать, то в него будет внесен ответ от сервера ОИСМ от предыдущей проверки кода маркировки.
                                    }

                                    break;
                                default:
                                    throw new ApplicationException(string.Format("Упали с ошибкой при выборе версии ФФД Пока не описано как с этой версией работать ({0})", Config.Ffd.ToString()));
                            }

                            if (Config.Ffd != FfdEn.v1_05)
                            {
                                if (Fr.FNCheckItemBarcode() != 0)             // Отправляем матрикс код
                                {
                                    Verification(Fr);
                                    throw new ApplicationException(string.Format("Упали с ошибкой при проверке матрикс кода в строке {1}: {0}", Status.Description, item.item_pos));
                                }

                                if (Fr.FNAcceptMakringCode() != 0)             // Отправляем матрикс код
                                {
                                    Verification(Fr);
                                    throw new ApplicationException(string.Format("Упали с ошибкой при проверке матрикс кода в строке {1}: {0}", Status.Description, item.item_pos));
                                }
                            }

                            if (Fr.FNSendItemBarcode() != 0)             // Отправляем матрикс код
                            {
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Упали с ошибкой при отправке матрикс кода в строке {1}: {0}", Status.Description, item.item_pos));
                            }
                        }
                    }
                }

                // Печать инфа по скидке по позиции
                if (item.discount_amt != 0)
                {
                    if (item.discount_amt > 0)
                    {
                        Print2in1Line(string.Format("Вкл. скидку {0}%", Math.Round(item.discount_perc, 2).ToString("0.00")), string.Format("{0} руб.", item.discount_amt));
                    }
                    else
                    {
                        Print2in1Line(string.Format("Вкл. наценку {0}%", Math.Round(item.discount_perc * -1, 2).ToString("0.00")), string.Format("{0} руб.", item.discount_amt * -1));
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати строчки матрикс кода: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintCheckItem", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Проверка статуса фискального регистратора
        /// </summary>
        /// <param name="TmpFr">Передаёмобъект драйвера который надо проверить</param>
        /// <returns>Получаем результат в виде кода ошибки и описания</returns>
        private static Lib.FrStatError Verification(DrvFR TmpFr)
        {
            Lib.FrStatError rez = new Lib.FrStatError();
            rez.CodeECRMode = TmpFr.ECRMode;


            rez.DescriptionECRMode = "";
            rez.CodeResult = TmpFr.ResultCode;
            rez.DescriptionResult = "";

            try
            {
                // Проверка в каком режиме наодится фискальник
                if (rez.CodeECRMode != 0)
                {
                    switch (rez.CodeECRMode)
                    {
                        case 0:
                            rez.DescriptionECRMode = "Принтер в рабочем режиме";
                            break;
                        case 1:
                            rez.DescriptionECRMode = "Выдача данных";
                            break;
                        case 2:
                            rez.DescriptionECRMode = "Открытая смена, 24 часа не кончились";
                            break;
                        case 3:
                            rez.DescriptionECRMode = "Открытая смена, 24 часа кончились";
                            break;
                        case 4:
                            rez.DescriptionECRMode = "Закрытая смена";
                            break;
                        case 5:
                            rez.DescriptionECRMode = "Блокировка по неправильному паролю налогового инспектора";
                            break;
                        case 6:
                            rez.DescriptionECRMode = "Ожидание подтверждения ввода даты";
                            break;
                        case 7:
                            rez.DescriptionECRMode = "разрешение изменения положения десятичной точки";
                            break;
                        case 8:
                            rez.DescriptionECRMode = "Открытый документ";
                            break;
                        case 9:
                            rez.DescriptionECRMode = "Режим разрешения технологического обнуления";
                            break;
                        case 10:
                            rez.DescriptionECRMode = "Тестовый прогон";
                            break;
                        case 11:
                            rez.DescriptionECRMode = "Печать полного фискального отчёта";
                            break;
                        case 12:
                            rez.DescriptionECRMode = "Печать длинного отчёта ЭКЛЗ";
                            break;
                        case 13:
                            rez.DescriptionECRMode = "Работа с фискальным подкладным документом";
                            break;
                        case 14:
                            rez.DescriptionECRMode = "Печать докладного документа";
                            break;
                        case 15:
                            rez.DescriptionECRMode = "Фискальный подкладной документ сформирован";
                            break;
                        default:
                            rez.DescriptionECRMode = "Не известная ошибка";
                            break;
                    }
                }

                // Если есть ошибка в операции
                if (rez.CodeResult != 0)
                {
                    switch (rez.CodeResult)
                    {
                        case 88:
                            rez.CodeResult = TmpFr.ResultCode;
                            rez.DescriptionResult = TmpFr.ResultCodeDescription;
                            if (TmpFr.ContinuePrint() != 0)
                            {
                                rez = Verification(TmpFr);
                                ApplicationException ae = new ApplicationException(string.Format("Получена ошибка 88 попытались обработать методом Fr.ContinuePrint() но вылезла ошибка {0}", rez.Description));
                                Log.EventSave(ae.Message, "Com.FR.Verification", EventEn.Error);
                                throw ae;
                            }
                            break;
                        default:
                            rez.DescriptionResult = TmpFr.ResultCodeDescription;
                            break;
                    }
                }

                // Выставляем статус фискальника текущий в нашем класе
                Status = rez;

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при парсинге ошибки: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.Verification", EventEn.Error);
                throw ae;
            }

        }

        /// <summary>
        /// Проверяем статус фискальника
        /// </summary>
        /// <param name="Doc">Документ который представляет из себя чек</param>
        /// <param name="OperatorNumber"> номер оператора который указан в фискальнике</param>
        /// <param name="DocName">Заголовок документа</param>
        /// <returns>Идти на выход</returns>
        private static bool ConfigStatusFR(JsonPrintFiscDoc Doc, int OperatorNumber, string DocName)
        {
            try
            {
                Fr.GetECRStatus();

                Lib.FrStatError Err = Verification(Fr);

                // Проверяем подключение к FR
                if (Fr.Connect() != 0)
                {
                    Err = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой: {0}", Err.Description));
                }

                // Проверяем статус фискальника если смена закрыта то надо открыть
                if (Err.CodeECRMode == 4)
                {
                    OpenShift();
                    /*
                    if (Fr.Disconnect() != 0)
                    {
                        Err = Verification(Fr);
                        throw new ApplicationException(string.Format("Упали с ошибкой: {0}", Err.Description));
                    }
                    */

                    /*
                    if (Fr.CancelCheck() != 0)           // Отправляем матрикс код
                    {
                        Err = Verification(Fr);
                        throw new ApplicationException(string.Format("Упали с ошибкой при выполнении командв CancelCheck(): {0}", Err.Description));
                    }
                    */

                    // Если конект успешен
                    //TestConnect(Fr);

                    //Fr.GetECRStatus();
                    //Verification(Fr);

                    Thread.Sleep(3000);

                    // Если это печать чека товыполняем
                    if (Doc != null) PrintCheck(Doc, OperatorNumber, DocName);

                    return true;
                }

                if (Err.CodeECRMode != 2)
                {
                    switch (Err.CodeECRMode)
                    {
                        // Если кончились 24 часа то закрываем смену сняв Z отчёт
                        case 3:
                            if (Fr.FNCloseSession() != 0)
                            {
                                Err = Verification(Fr);
                                throw new ApplicationException(string.Format(@"Статус фискальника:""{0}""", Err.Description));
                            }
                            //Fr.Disconnect();
                            return true;
                        case 8: // Если ест незавершонный чек то закрываем его
                            if (Fr.CancelCheck() != 0)           // Отправляем матрикс код
                            {
                                Err = Verification(Fr);
                                throw new ApplicationException(string.Format("Упали с ошибкой при выполнении командв CancelCheck(): {0}", Err.Description));
                            }
                            /*
                            if (Fr.CloseCheck() != 0)
                            {
                                Err = Verification(Fr);
                            }*/
                            break;
                        default:
                            throw new ApplicationException(string.Format(@"Статус фискальника:""{0}""", Err.Description));
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при проверки текущего статуса и настройке фискальника: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.ConfigStatusFR", EventEn.Error);
                throw ae;
            }

        }

        /// <summary>
        /// Открытие чека
        /// </summary>
        /// <param name="Doc">Сам документ</param>
        /// <param name="TekCustomer">текущий покупатель</param>
        /// <param name="DocCustTyp">Тип документа</param>
        /// <param name="IsCopy">Копия чека</param>
        private static void OpenReceipt(JsonPrintFiscDoc Doc, Custumer TekCustomer, EnFrTyp DocCustTyp, bool IsCopy)
        {
            try
            {
                // Указываем продовца
                Fr.TableNumber = 2;
                Fr.RowNumber = 30;
                Fr.FieldNumber = 2;
                string TekCustomerTmp = TekCustomer.fio_fo_check.Trim();
                if (!string.IsNullOrWhiteSpace(TekCustomer.Job)) TekCustomerTmp = string.Format("{0} {1}", TekCustomerTmp, TekCustomer.Job).Trim();
                Fr.ValueOfFieldString = TekCustomerTmp;  // "Жирникова Анастасия";     // Имя кассира
                if (Fr.WriteTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой при добавлении тега Имя кассира: {0}", Status.Description));
                }
                // Читаем таблицу для того чтобы всё записалось
                if (Fr.ReadTable() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой выполнении команды ReadTable после установки кассира: {0}", Status.Description));
                }

                //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                switch (Doc.receipt_type)
                {
                    case 0:
                    case 2:
                        if (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.given_amt != 0)
                        {
                            Fr.CheckType = 2;
                        }
                        else Fr.CheckType = 0;
                        break;
                    case 1:
                        Fr.CheckType = 2;
                        break;
                    default:
                        throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                }

                // Если жто копия то делаем не открытие а печать заголовка
                if (IsCopy)
                {
                    if (Fr.PrintDocumentTitle() != 0)
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Не смогли напечатать заголовок: {0}", Status.Description));
                    }
                }
                else
                {
                    // Открываем чек
                    if (Fr.OpenCheck() != 0)
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Не смогли открыть чек: {0}", Status.Description));
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при открытии чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.OpenReceipt", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Печать заголовка чека
        /// </summary>
        /// <param name="IsCopy"> Печатать в заголовке информацию по типу чека и фискальнику</param>
        /// <param name="Doc">Сам документ</param>
        /// <param name="TekCustomer">Информация по продовцу</param>
        private static void PrintCheckHead(bool IsCopy, JsonPrintFiscDoc Doc, Custumer TekCustomer)
        {
            try
            {
                if (IsCopy) Thread.Sleep(500);  // Иногда тупит говорит что чтото уже печатает
                if (IsCopy)
                {
                    // Печать информациипо чеку
                    //Print2in1Line(string.Format("ККТ {0}" + Fr.SerialNumber),
                    //            string.Format("{0} {1}", Fr.Date.ToShortDateString(), Fr.Time.ToShortTimeString()));

                    //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                    switch (Fr.CheckType)
                    {
                        case 0:
                            PrintLine(@"КАССОВЫЙ ЧЕК/ПРИХОД");
                            break;
                        case 2:
                            PrintLine(@"КАССОВЫЙ ЧЕК/ВОЗВРАТ ПРИХОДА");
                            break;
                        default:
                            break;
                    }
                }

                // Номер магазина и чека
                string DocNum = (Doc.document_number != null ? Doc.document_number.ToString() : null);
                if (string.IsNullOrWhiteSpace(DocNum)) DocNum = Com.ProviderFarm.CurrentPrv.GetDocNoFromDocument(Doc.sid).ToString();
                //
                if (string.IsNullOrWhiteSpace(Doc.store_name))
                {
                    Print2in1Line(string.Format("КОД МАГАЗИНА: {0}", Doc.store_number),
                                string.Format("ЧЕК {0}", DocNum));
                }
                else
                {
                    Print2in1Line(string.Format("КОД МАГАЗИНА: {0}", Doc.store_name),
                                string.Format("ЧЕК {0}", DocNum));
                }

                //                if (IsCopy) Thread.Sleep(300);

                // ПЕЧАТЬ ШТРИХ-КОДА
                Fr.BarCode = DocNum.ToString();
                Fr.BarWidth = 2;
                Fr.LineNumber = 50;
                Fr.BarcodeType = 0;
                Fr.BarcodeAlignment = TBarcodeAlignment.baCenter;
                if (Fr.PrintBarcodeGraph() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Не смогли напечатать ПЕЧАТЬ ШТРИХ-КОДА: {0}", Status.Description));
                }

                //                if (IsCopy) Thread.Sleep(300);

                // КАССИР - СОТРУДНИК
                Print2in1Line(@"КАССИР:", TekCustomer.fio_fo_check);
                if (!IsCopy)
                {
                    if (!string.IsNullOrWhiteSpace(TekCustomer.inn))
                    {
                        Fr.TagNumber = 1203;
                        Fr.TagType = 7;
                        Fr.TagValueStr = TekCustomer.inn;
                        switch (Fr.FNSendTag())
                        {
                            case 51:
                                throw new ApplicationException(string.Format("Кассир {0} имеет неправильный ИНН {1}", TekCustomer.fio_fo_check, TekCustomer.inn));
                            case 0:
                                break;
                            default:
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли напечатать ПЕЧАТЬ ШТРИХ-КОДА: {0}", Status.Description));
                        }
                    }
                }

                //                if (IsCopy) Thread.Sleep(300);

                // Печать информации про юрлицо
                if ((Config.ProcessingUrikForFr
                    && !string.IsNullOrWhiteSpace(Doc.bt_last_name)
                    && !string.IsNullOrWhiteSpace(Doc.bt_address_line3))
                    && (
                        (Config.PrintingUrikForFr && Doc.bt_address_line3.Trim().Length == 10)   // У юриков ИНН 10 символов а у физиков 12
                        || (Config.PrintingIpForFr && Doc.bt_address_line3.Trim().Length == 12)   // У юриков ИНН 10 символов а у физиков 12
                       )
                    )
                {
                    if (!IsCopy)
                    {
                        Fr.TagNumber = 1256;
                        switch (Fr.FNBeginSTLVTag())
                        {
                            case 0:
                                break;
                            default:
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли сделать составной тег для  покупателя: {0}", Status.Description));
                        }

                        // Заполняем наименование Юр лица
                        Fr.TagID = 0;
                        Fr.TagNumber = 1227;
                        Fr.TagType = 7;
                        Fr.TagValueStr = Doc.bt_last_name.Trim();
                        switch (Fr.FNAddTag())
                        {
                            case 0:
                                break;
                            default:
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли добавить наименование юр лица: {0}", Status.Description));
                        }

                        // Заполняем ИНН Юр лица
                        Fr.TagID = 0;
                        Fr.TagNumber = 1228;
                        Fr.TagType = 7;
                        Fr.TagValueStr = Doc.bt_address_line3.Trim();
                        switch (Fr.FNAddTag())
                        {
                            case 0:
                                break;
                            default:
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли добавить ИНН юр лица: {0}", Status.Description));
                        }

                        // отправка составного тега
                        switch (Fr.FNSendSTLVTag())
                        {
                            case 0:
                                break;
                            default:
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли отправить составной тег по юр лицу: {0}", Status.Description));
                        }
                    }

                    //                    if (IsCopy) Thread.Sleep(300);

                    if (Config.PrintingUrikForFr || Config.PrintingIpForFr)
                    {
                        string FieldInnTyp;
                        switch (Config.FieldInnTyp)
                        {
                            case FieldDocNumEn.Comment1:
                                FieldInnTyp = Doc.comment1;
                                break;
                            case FieldDocNumEn.Comment2:
                                FieldInnTyp = Doc.comment2;
                                break;
                            case FieldDocNumEn.bt_address_line3:
                                FieldInnTyp = Doc.bt_address_line3;
                                break;
                            default:
                                FieldInnTyp = null;
                                break;
                        }
                        PrintLine(string.Format("ПОКУПАТЕЛЬ:{0}", Doc.bt_last_name.Trim()));
                        PrintLine(string.Format("ИНН ПОКУПАТЕЛЯ:{0}", FieldInnTyp.Trim()));
                    }
                }


                // Информация по заказу, если нужно
                int ItemCount = 0;
                if (!string.IsNullOrWhiteSpace(Doc.order_document_number) && Doc.order_document_number != "0")
                {
                    if (ItemCount == 0)
                    {
                        PrintLine(string.Format("Оплата по ЗК №{0}", Doc.order_document_number));
                    }
                    else
                    {
                        PrintLine(string.Format("Чек по ЗК №{0}", Doc.order_document_number));
                    }
                }

                //                if (IsCopy) Thread.Sleep(300);

                // Печатаем сотрудника продавшего товар
                string employee = null;
                foreach (JsonPrintFiscDocItem item in Doc.items)
                {
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee1_name)) employee = item.employee1_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee2_name)) employee = item.employee2_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee3_name)) employee = item.employee3_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee4_name)) employee = item.employee4_name;
                    if (employee == null && !string.IsNullOrWhiteSpace(item.employee5_name)) employee = item.employee5_name;
                    if (!string.IsNullOrWhiteSpace(employee)) break;
                }
                if (!string.IsNullOrWhiteSpace(employee))
                {
                    Employee TekEmployees = Config.employees.Find(t => t.PrizmLogin.ToUpper() == employee.ToUpper());
                    if (TekEmployees != null && !string.IsNullOrWhiteSpace(TekEmployees.fio_fo_check)) employee = TekEmployees.fio_fo_check.Trim();
                    Print2in1Line("Сотрудник:", employee);
                }

                // Отчеркиваем заголовок
                PrintSeparator();

                //                if (IsCopy) Thread.Sleep(300);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати заголовка чека чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrintCheckHead", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// СКИДКА/НАЦЕНКА НА ЧЕК
        /// </summary>
        /// <param name="AsCopy">Печатать скидку или нет</param>
        public static void PrinCheckDiscount(bool AsCopy, JsonPrintFiscDoc Doc)
        {
            try
            {

                // Если скдки нет то печатать нечего
                if ((Doc.discount_perc == null || Doc.discount_perc == 0)
                    || (Doc.total_discount_amt == null || Doc.total_discount_amt == 0)) return;

                //LogMsg(Format('Скидка/Наценка на чек: %.2f', [DiscPerc]));
                //AssertMsg(Format('Скидка/Наценка на чек: %.2f', [DiscPerc]));

                // Отчёркиваем линию
                PrintSeparator();

                // Скидка в процентах на чек
                if (Doc.discount_perc != null && Doc.discount_perc != 0)
                {

                    // Печатаем скидку
                    if (AsCopy)
                    {
                        PrintSeparator();       // Отчёркиваем линию

                        Print2in1Line(string.Format("Скидка на чек   {0}", ((double)Doc.discount_perc).ToString("0.00")),
                                string.Format("={0}", Doc.discount_amount));
                    }
                    else
                    {
                        if (Doc.discount_perc > 0)
                        {
                            Fr.DiscountOnCheck = (double)Doc.discount_perc;
                        }
                        else
                        {
                            // Пробегаем по типу оплаты
                            foreach (JsonPrintFiscDocTender item in Doc.tenders)
                            {
                                //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                                switch (Doc.receipt_type)
                                {
                                    case 0:
                                        Fr.Summ1 = (decimal)(Doc.discount_amount < 0 ? Doc.discount_amount * -1 : Doc.discount_amount);
                                        break;
                                    case 1:
                                        Fr.Summ4 = (decimal)(Doc.discount_amount < 0 ? Doc.discount_amount * -1 : Doc.discount_amount);
                                        break;
                                    case 2:
                                        Fr.Summ2 = (decimal)(Doc.discount_amount < 0 ? Doc.discount_amount * -1 : Doc.discount_amount);
                                        break;
                                    default:
                                        throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                                }
                            }


                            //Метод регистрирует надбавку на сумму, задаваемую в свойстве Summ1, с вычислением налогов.
                            if (Fr.Charge() != 0)
                            {
                                Verification(Fr);
                                throw new ApplicationException(string.Format("Не смогли напечатать надбавку при регистрации чека: {0}", Status.Description));
                            }
                        }
                    }
                }

                if (Doc.total_discount_amt != null || Doc.total_discount_amt != 0)
                {
                    Print2in1Line(string.Format("Скидка на чек   {0}", ((double)Doc.total_discount_amt).ToString("0.00")),
                                string.Format("={0}", ((double)Doc.total_discount_amt).ToString("0.00")));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при печати скидкинаценки на чек: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.PrinCheckDiscount", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получение итоговой суммы по всем типам оплат по документу
        /// </summary>
        /// <param name="Doc">Сам документ</param>
        /// <param name="DocTyp">Тип документа</param>
        /// <param name="CrocessSummToFR">Заполнить поля в фискальнике по нашим правилам или нет Fr.Summ1-Fr.Summ16 по умолчанию нет</param>
        /// <returns>Итоговая сумма по всем типам оплат</returns>
        public static FrGetSummCheckRez GetSummCheck(JsonPrintFiscDoc Doc, EnFrTyp DocTyp, bool CrocessSummToFR)
        {
            try
            {
                FrGetSummCheckRez rez = new FrGetSummCheckRez();

                if (CrocessSummToFR)
                {
                    // Сброс значений
                    Fr.Summ1 = 0;
                    Fr.Summ2 = 0;
                    Fr.Summ3 = 0;
                    Fr.Summ4 = 0;
                    Fr.Summ5 = 0;
                    Fr.Summ6 = 0;
                    Fr.Summ7 = 0;
                    Fr.Summ8 = 0;
                    Fr.Summ9 = 0;
                    Fr.Summ10 = 0;
                    Fr.Summ11 = 0;
                    Fr.Summ12 = 0;
                    Fr.Summ13 = 0;
                    Fr.Summ14 = 0;
                    Fr.Summ15 = 0;
                    Fr.Summ16 = 0;
                }

                // Пробегаем по типу оплаты
                bool flagexit = false;
                foreach (JsonPrintFiscDocTender item in Doc.tenders)
                {
                    //«0» - продажа, «1» - покупка, «2» - возврат продажи, «3» - возврат покупки.
                    switch (Doc.receipt_type)
                    {
                        case 0:

                            // Если тип оплаты нал
                            if (item.tender_type == Com.Config.TenderTypeCash && item.taken != 0)
                            {
                                //rez.itog += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                                continue;
                            }

                            // Если тип оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.BezNal, item.tender_name, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.taken;
                                continue;
                            }

                            // Если тип оплаты подарочный сертификат
                            if (item.tender_type == Com.Config.TenderTypeGiftCert && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Predoplata, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                                continue;
                            }

                            // Если тип оплаты подарочная карта
                            if (item.tender_type == Com.Config.TenderTypeGiftCard && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Predoplata, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                                continue;
                            }

                            // Если тип оплаты подарочная карта
                            if (DocTyp != EnFrTyp.ReturnDeposit && item.tender_type == Com.Config.TenderTypeAvans && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Predoplata, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                                continue;
                            }

                            // Заказ клиента когда он вносит сумму
                            if (item.tender_type == 7 && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                                continue;
                            }

                            // КОгда клиент делает возврат денег пока не выполнен заказ
                            if (item.tender_type == 7 && Doc.given_amt != 0)
                            {
                                //rez += (decimal)Doc.given_amt;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)Doc.given_amt);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)Doc.given_amt;
                                flagexit = true;  // Делается по всему документу нет смысла проходить по позициям
                            }

                            // Если это возврат депозита и тип оплаты нал
                            if (DocTyp == EnFrTyp.ReturnDeposit && item.tender_type == Com.Config.TenderTypeCash && item.given != 0)
                            {
                                //rez.itog += (decimal)item.given;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)item.given);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.given;
                                continue;
                            }

                            // Если тип оплаты карта
                            if (DocTyp == EnFrTyp.ReturnDeposit && item.tender_type == Com.Config.TenderTypeCredit && item.given != 0)
                            {
                                //rez += (decimal)item.given;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.BezNal, item.tender_name, (decimal)item.given);
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.given;
                                continue;
                            }

                            break;
                        case 1:
                            // Если тип оплаты нал
                            if (item.tender_type == Com.Config.TenderTypeCash && item.given != 0)
                            {
                                //rez += (decimal)item.given;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)item.given);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.given;
                                continue;
                            }

                            // Если тип оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.given != 0)
                            {
                                //rez += (decimal)item.given;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.BezNal, item.tender_name, (decimal)item.given);
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.given;
                                continue;
                            }

                            break;
                        case 2:
                            // Депозит наликом
                            if (item.tender_type == Com.Config.TenderTypeCash && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.Nal, null, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                                continue;
                            }

                            // Депозит оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.taken != 0)
                            {
                                //rez += (decimal)item.taken;
                                rez.SetSumm(FrGetSummCheckRezTypeEn.BezNal, item.tender_name, (decimal)item.taken);
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.taken; //Fr.Summ2
                                continue;
                            }

                            break;
                        default:
                            throw new ApplicationException(string.Format("В токументе появился тип поля receipt_typ={0}, который мы не знаем как обрабатывать", Doc.receipt_type));
                    }

                    // выход без перепора позиций
                    if (flagexit) break;
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при закрытии чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.GetSummCheck", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Закрытие чека
        /// </summary>
        /// <param name="Doc">Сам документ</param>
        private static void CloseReceipt(bool IsCopy, JsonPrintFiscDoc Doc, EnFrTyp DocTyp, int Tax1, int Tax2, int Tax3, int Tax4)
        {
            try
            {
                // Отчеркивание оплат
                PrintSeparator();

                // Печатаем Емайл покупателя
                if (!IsCopy)
                {
                    if (!string.IsNullOrWhiteSpace(Doc.bt_email))
                    {
                        Fr.CustomerEmail = Doc.bt_email;
                        if (Fr.FNSendCustomerEmail() != 0)
                        {
                            Verification(Fr);
                            throw new ApplicationException(string.Format("Не смогли отправить емейл покупателя ошибка: {0}", Status.Description));
                        }
                    }

                    // Отчеркивание оплат
                    PrintSeparator();
                }

                // Получение и печать Подитога
                if (!IsCopy)
                {
                    if (Fr.CheckSubTotal() != 0)
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Не смогли отправить подитог ошибка: {0}", Status.Description));
                    }
                    //500                    Thread.Sleep(300);
                }

                // Заполняем сумму в фискальнике
                FrGetSummCheckRez Itog = GetSummCheck(Doc, DocTyp, true);

                // Печать итога на дисплей                
                Com.DisplayFarm.CurDisplay.ShowText(string.Format("Итого: {0} руб.", (Fr.Summ1 + Fr.Summ2 + Fr.Summ3 + Fr.Summ4 + Fr.Summ5
                    + Fr.Summ6 + Fr.Summ7 + Fr.Summ8 + Fr.Summ9 + Fr.Summ10 + Fr.Summ11 + Fr.Summ12 + Fr.Summ13 + Fr.Summ14 + Fr.Summ15 + Fr.Summ16).ToString("0.00")));

                // Печатеам концовку для копии чека
                //                if (IsCopy) Thread.Sleep(300);
                PrintSeparator();
                if (IsCopy)
                {
                    //                    if (IsCopy) Thread.Sleep(300);
                    PrintLineCenter(string.Format("КОПИЯ ЧЕКА ЗА {0}", ((DateTime)Doc.invoice_posted_date).ToShortDateString()));
                    Print2in1Line(string.Format("ИТОГО К ОПЛАТЕ"), string.Format("={0}", Math.Round(Itog.itog, 2).ToString("0.00")));
                    //                    if (IsCopy) Thread.Sleep(300);
                }

                // Печатем под итог
                foreach (FrGetSummCheckRezType itemTyp in Itog.TenderTyp)
                {
                    switch (itemTyp.TenderTyp)
                    {
                        case FrGetSummCheckRezTypeEn.Nal:
                            Print2in1Line("НАЛИЧНЫМИ", string.Format("={0}", Math.Round((decimal)itemTyp.PromItog, 2).ToString("0.00")));
                            break;
                        case FrGetSummCheckRezTypeEn.BezNal:
                            foreach (FrGetSummCheckRezTypeTenderName itemName in itemTyp.TenderName)
                            {
                                Print2in1Line((string.IsNullOrWhiteSpace(itemName.Name) ? "vvvv" : itemName.Name.Replace("UDF1", "MIR").ToUpper()), string.Format("={0}", Math.Round(itemName.Value, 2).ToString("0.00")));
                            }
                            break;
                        case FrGetSummCheckRezTypeEn.Credit:
                            Print2in1Line("КРЕДИТОМ", Math.Round(itemTyp.PromItog, 2).ToString("0.00"));
                            break;
                        case FrGetSummCheckRezTypeEn.Predoplata:
                            Print2in1Line("ВКЛЮЧАЯ ПРЕДОПЛАТУ", Math.Round(itemTyp.PromItog, 2).ToString("0.00"));
                            break;
                        default:
                            break;
                    }
                }


                if (IsCopy)
                {
                    // Сумма чека и подсчёит итога
                    decimal SumChekFoPrice = 0;
                    foreach (JsonPrintFiscDocItem item in Doc.items)
                    {
                        SumChekFoPrice += (decimal)(item.quantity * item.price);
                    }
                    if (Itog.itog > SumChekFoPrice) Print2in1Line("Сдача", string.Format("={0}", Math.Round(Itog.itog - SumChekFoPrice, 2).ToString("0.00")));

                    PrintLineCenter("Сумма чека содержит НДС");
                    PrintLine("ВСЕ СУММЫ УКАЗАНЫ В РУБЛЯХ", true);

                    Fr.FeedDocument();
                }
                else
                {

                    // Сумма чека и подсчёит итога
                    decimal SumChekFoPrice = 0;
                    foreach (JsonPrintFiscDocItem item in Doc.items)
                    {
                        SumChekFoPrice += (decimal)(item.quantity * item.price);
                    }
                    if (Itog.itog > SumChekFoPrice) Print2in1Line("Сдача", string.Format("={0}", Math.Round(Itog.itog - SumChekFoPrice, 2).ToString("0.00")));


                    // Вставка подитога для гучи
                    /*if (Fr.Summ1 != 0) Print2in1Line("НАЛИЧНЫМИ", Fr.Summ1.ToString());
                    if (Fr.Summ2 != 0) Print2in1Line("КРЕДИТОМ", Fr.Summ2.ToString());  //
                    if (Fr.Summ4 != 0) Print2in1Line("БЕЗНАЛИЧНЫМИ", Fr.Summ4.ToString());
                    if (Fr.Summ14 != 0) Print2in1Line("ВКЛЮЧАЯ ПРЕДОПЛАТУ", Fr.Summ14.ToString());  //
                    */
                    PrintSeparator();


                    // Проверяем версию FFD
                    switch (Config.Ffd)
                    {
                        case FfdEn.v1_05:

                            // Закрываем чек
                            if (Fr.CloseCheckEx() != 0)
                            {
                                // Сохраняем ошибку
                                Verification(Fr);
                                ApplicationException ErrCloseCheck = new ApplicationException(string.Format("Не смогли закрыть чек ошибка: {0}", Status.Description));

                                // Отменяем чек
                                if (Fr.CancelCheck() != 0)
                                {
                                    Verification(Fr);
                                    throw new ApplicationException(string.Format("Не смогли отменить чек который упал с ошибой: ({0}) так как словили ошибку при отмене чека: {1}", ErrCloseCheck.Message, Status.Description));
                                }

                                // Выкидывем предыдужую ошибку если прошла отмена
                                throw ErrCloseCheck;
                            }

                            break;
                        case FfdEn.v1_2:

                            // Закрываем чек
                            if (Fr.FNCloseCheckEx() != 0)
                            {
                                // Сохраняем ошибку
                                Verification(Fr);
                                ApplicationException ErrCloseCheck = new ApplicationException(string.Format("Не смогли закрыть чек ошибка: {0}", Status.Description));

                                // Отменяем чек
                                if (Fr.CancelCheck() != 0)
                                {
                                    Verification(Fr);
                                    throw new ApplicationException(string.Format("Не смогли отменить чек который упал с ошибой: ({0}) так как словили ошибку при отмене чека: {1}", ErrCloseCheck.Message, Status.Description));
                                }

                                // Выкидывем предыдужую ошибку если прошла отмена
                                throw ErrCloseCheck;
                            }

                            break;
                        default:
                            throw new ApplicationException(string.Format("Упали с ошибкой при выборе версии ФФД Пока не описано как с этой версией работать ({0})", Config.Ffd.ToString()));
                    }

                    // Открытие денежного ящика
                    OpenDrawer();
                }

                // Отрезаем чек
                Cut();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при закрытии чека: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.CloseReceipt", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Открытие денежного ящика
        /// </summary>
        public static void OpenDrawer()
        {
            try
            {
                if (Fr.OpenDrawer() != 0)
                {
                    // Сохраняем ошибку
                    Verification(Fr);
                    ApplicationException ErrCloseCheck = new ApplicationException(string.Format("Не смогли закрыть чек ошибка: {0}", Status.Description));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при открытии денежного ящика: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.OpenDrawer", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// отрезка чека
        /// </summary>
        public static void Cut()
        {
            try
            {
                Fr.CutType = false;
                if (Fr.CutCheck() != 0)
                {
                    // Сохраняем ошибку
                    Verification(Fr);
                    ApplicationException ErrCloseCheck = new ApplicationException(string.Format("Не смогли отрезать чек ошибка: {0}", Status.Description));
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при открытии денежного ящика: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.Cut", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Внесение наличных
        /// </summary>
        /// <param name="Sum">Сумма которую внесли</param>
        /// <returns></returns>
        public static Lib.FrStatError CashIncome(decimal Sum)
        {
            Lib.FrStatError rez = new FrStatError();
            try
            {
                try
                {
                    if (!ConfigStatusFR(null, 0, null)) OpenShift();
                }
                catch (Exception ex)
                {
                    Log.EventSave(ex.Message, "Com.FR.CachIncome", EventEn.Warning);
                }
                finally
                {
                    Fr.Summ1 = Sum;

                    if (Fr.CashIncome() != 0)
                    {
                        rez = Verification(Fr);
                        throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.CachIncome", EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Изьятие наличных
        /// </summary>
        /// <param name="Sum">Сумма которую изъяли</param>
        /// <returns></returns>
        public static Lib.FrStatError CashOutcome(decimal Sum)
        {
            Lib.FrStatError rez = new FrStatError();
            try
            {
                Fr.Summ1 = Sum;

                if (Fr.CashOutcome() != 0)
                {
                    rez = Verification(Fr);
                    throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.CashOutcome", EventEn.Error);
                throw ae;
            }
        }
    }
}

