using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using AlgoritmPrizm.Lib;
using DrvFRLib;
using AlgoritmPrizm.BLL;


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
                    throw new ApplicationException(string.Format("Упали с ошибкой: {0}", rez.Description));
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
        /// <returns>Возвращаем номердокумента и поле в которое нужно его сохранять</returns>
        public static JsonPrintFiscDocReturn PrintCheck(JsonPrintFiscDoc Doc, int OperatorNumber, string DocName)
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

                // Открываем чек
                OpenReceipt(Doc, TekCustomer, DocCustTyp);

                // Печатаем заголовок
                PrintCheckHead(false, Doc, TekCustomer);

                /*Печать штрихкода с номером чека. На будущее
    if Result = 0 then begin
      ShowMessage('Barcode');
                FR.BarCode := IntToStr(ReceiptNumber);
            Result:= CheckErr(FR.PrintBarcode);
                end;
                */

                //************** ПЕЧАТЬ ПОЗИЦИЙ ЧЕКА **************************************

                int TekDocStavkiNDS1 = 0;      // Нал
                int TekDocStavkiNDS2 = 0;      // Безнал
                int TekDocStavkiNDS3 = 0;      // Смешанный
                int TekDocStavkiNDS4 = 0;      // Депозит

                // Если это возврат денег с заказа клиента то лезем в ссылку на документ источник и получаем от туда нужные строки из базы данных
                if (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.receipt_type == 0 && Doc.given_amt != 0 && Doc.items.Count==0)
                {
                    string referDocSid = Doc.ref_order_sid;
                    foreach (BLL.JsonPrintFiscDocItem nitemRefer in Com.ProviderFarm.CurrentPrv.GetItemsForReturnOrder(referDocSid))
                    {
                        Doc.items.Add(nitemRefer);
                    }
                }

                // Сумма чека для предоплаты в момент просчёта строк
                decimal SumChekForPredoplata = 0;
                decimal SumChekFoCustomer = GetSummCheck(Doc, false); // Получаем сумму чека из фискальника чтобы потом этого не делать при просчёте позиций во время предоплаты
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


                    // В зависимости от типа докумена разный вариант ставки
                    switch (Doc.receipt_type)
                    {
                        case 0:
                        case 1:

                            switch (Doc.items[itm].tax_percent)
                            {
                                case 10:
                                    TekStavkiNDS1 = Config.TaxPercent10;
                                    break;
                                case 20:
                                    TekStavkiNDS1 = Config.TaxPercent20;
                                    break;
                                default:
                                    break;
                            }
                            /*
                            for (int i = 0; i < StavkiNDS.Count; i++)
                            {
                                if (Doc.items[itm].tax_percent == StavkiNDS[i])
                                {
                                    // если способ расчёта нал
                                    TekStavkiNDS1 = i + 1;
                                    if (TekDocStavkiNDS1 == 0) TekDocStavkiNDS1 = TekStavkiNDS1;
                                }
                                break;
                            }*/
                            break;
                        case 2:
                            switch (Doc.items[itm].tax_percent)
                            {
                                case 10:
                                    TekStavkiNDS1 = Config.TaxPercent10110;
                                    break;
                                case 20:
                                    TekStavkiNDS1 = Config.TaxPercent20120;
                                    break;
                                default:
                                    break;
                            }

                            /*for (int i = 0; i < StavkiNDS_Dep.Count; i++)
                            {
                                if (Doc.items[itm].tax_percent == StavkiNDS_Dep[i])
                                {
                                    // если способ расчёта нал
                                    TekStavkiNDS1 = i + 5;
                                    if (TekDocStavkiNDS1 == 0) TekDocStavkiNDS1 = TekStavkiNDS1;
                                }
                                break;
                            }*/
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
                                break;
                            case 2:// 10%
                                //TekStavkiNDS1 = 6; // 10/110
                                TekStavkiNDS1 = Config.TaxPercent10110;
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
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note1, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note2
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note2))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note2, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note3
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note3))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note3, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note4
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note4))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note4, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note5
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note5))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note5, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note6
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note6))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note6, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note7
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note7))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note7, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note8
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note8))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note8, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note9
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note9))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note9, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Проверяем есть кодмаркировки или нет по полю note10
                    if (!string.IsNullOrWhiteSpace(Doc.items[itm].note10))
                    {
                        flagmarkink = true;
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note10, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }

                    // Если эта строка не содержала товаров с маркировкой
                    if (!flagmarkink)
                    {
                        PrintCheckItem(Doc, Doc.items[itm], Doc.items[itm].note10, 1, TekStavkiNDS1, TekStavkiNDS2, TekStavkiNDS3, TekStavkiNDS4, DocCustTyp, ref SumChekForPredoplata, itm, Doc.items.Count, SumChekFoCustomer, SumChekFoPrice);
                    }
                }

                //************** СКИДКА/НАЦЕНКА НА ЧЕК ************************************
                PrinCheckDiscount(false, Doc);

                //************** ОПЛАТЫ ПО ЧЕКУ ******************************************

                //Web.UpdateFiskDocNum(Doc, Fr.OpenDocumentNumber);

                // Печать концовки чека
                CloseReceipt(Doc, TekDocStavkiNDS1, TekDocStavkiNDS2, TekDocStavkiNDS3, TekDocStavkiNDS4);


                // Обновляем статус и опрашиваем фискальник на предмет получения последнего номера документа
                Fr.FNGetStatus();

                //Web.UpdateFiskDocNum(Doc, Fr.DocumentNumber);
                rezWeb.fiscDocNum = Fr.DocumentNumber;

                // Открываем денежный ящик
                OpenDrawer();

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
        /// <param name="Doc">Сам документ</param>
        /// <param name="item">строка которую берём за образец</param>
        /// <param name="note">код маркировки который нужно отправить</param>
        /// <param name="Department">подозреваю что тут он не нужен надо проверить ??????</param>
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
        
        private static void PrintCheckItem(JsonPrintFiscDoc Doc, JsonPrintFiscDocItem item, string note, int Department, int Tax1, int Tax2, int Tax3, int Tax4, EnFrTyp DocCustTyp, ref decimal SumChekForPredoplata, int IndexItemForPredoplata, int CountForPredoplata, decimal SumChekFoCustomer, decimal SumChekFoPrice)
        {
            try
            {
                // Печать штрих кода
                if (!string.IsNullOrWhiteSpace(item.scan_upc)) PrintLine(item.scan_upc);

                //Описание товара
                string StringForPrinting= item.item_description1;
                switch (Config.FieldItem)
                {
                    case FieldItemEn.Description1:
                        StringForPrinting = item.item_description1;
                        break;
                    case FieldItemEn.Description2:
                        StringForPrinting = item.item_description2;
                        break;
                    default:
                        StringForPrinting = item.item_description1;
                        break;
                }
                Fr.StringForPrinting = StringForPrinting;


                // Если есть матрих код то количество не может быть более 1
                if (string.IsNullOrWhiteSpace(note)) Fr.Quantity = item.quantity;
                else
                {  // Матрикс код обнаружен Логируем инфу
                    Fr.Quantity = 1;                            // Код маркировки есть значит количество 1
                    FileCheckLog.EventPrintSave(note, Doc.sid, (decimal)item.price, DocCustTyp, Doc.receipt_type, item.sid);
                }

                // Есдли похоже на предоплату и общая сумма по документу не равна сумме что заплатил покупатель то будем рассчитывать пропорционально цену
                if ((Doc.receipt_type == 2 && SumChekFoCustomer != SumChekFoPrice)
                    // Если это возврат денег с заказа клиента то лезем в ссылку на документ источник и получаем от туда нужные строки из базы данных
                    || (Doc.tenders.Count(t => t.tender_type == 7) > 0 && Doc.receipt_type == 0 && Doc.given_amt != 0)
                    )
                {
                    // Получаем на сколько стандартно домножаем сумму для того чтобы пропарционально позиции в чеке поменять
                    decimal PrcForPrice = SumChekFoCustomer / SumChekFoPrice;

                    // если это последняя позиция в чеке то нужно накзначить так сумму чтобы она сошлась с тем что заплотил пользователь
                    if (IndexItemForPredoplata + 1 == CountForPredoplata)
                    {
                        Fr.Price = (SumChekFoCustomer - SumChekForPredoplata) / (decimal)item.quantity;
                        SumChekForPredoplata += (SumChekFoCustomer - SumChekForPredoplata);
                    }
                    else
                    {
                        Fr.Price = (decimal)item.price * PrcForPrice;
                        SumChekForPredoplata += (decimal)item.price * PrcForPrice * (decimal)item.quantity;
                    }
                    //SumChekForPredoplata
                }
                else Fr.Price = (decimal)item.price;  // Цена в строке  (decimal)1.56;          

                // В зависимости от типа документа
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



                //
                // Продолжаем наритать строку
                Fr.Department = Department;       // Надо проверить возможно не нужно передавать при печати строк

                // В зависимости от типа документа разная группа налогов
                switch (DocCustTyp)
                {
                    case EnFrTyp.GiftCard:
                        Fr.Tax1 = Com.Config.GiftCardTax;
                        break;
                    default:
                        Fr.Tax1 = Tax1;
                        Fr.Tax2 = Tax2;
                        Fr.Tax3 = Tax3;
                        Fr.Tax4 = Tax4;
                        break;
                }



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

                // Если есть матрихс код то добавляем его
                if (!string.IsNullOrWhiteSpace(note))
                {
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

                    /*
                    
                    Fr.MarkingType = 17485;                     //Тип товара маркированный 17485, 5408
                    Fr.GTIN = note.Substring(1, 14);            // парсим GTIN
                    Fr.SerialNumber = note.Substring(15, 13);   // парсим серийный номер
                    if (Fr.FNSendItemCodeData() != 0)           // Отправляем матрикс код
                    {
                        Verification(Fr);
                        throw new ApplicationException(string.Format("Упали с ошибкой при отправке матрикс кода в строке {1}: {0}", Status.Description, item.item_pos));
                    }*/

                    // Печать инфа по скидке по позиции
                    if (item.discount_amt != 0)
                    {
                        PrintLine(string.Format("Скидка {0} руб.", item.discount_amt), true);
                    }
                }

                // Печатаем сотрудника продавшего товар
                if (!string.IsNullOrWhiteSpace(item.employee1_name)) PrintLine(item.employee1_name, true);
                if (!string.IsNullOrWhiteSpace(item.employee2_name)) PrintLine(item.employee2_name, true);
                if (!string.IsNullOrWhiteSpace(item.employee3_name)) PrintLine(item.employee3_name, true);
                if (!string.IsNullOrWhiteSpace(item.employee4_name)) PrintLine(item.employee4_name, true);
                if (!string.IsNullOrWhiteSpace(item.employee5_name)) PrintLine(item.employee5_name, true);
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
        private static void OpenReceipt(JsonPrintFiscDoc Doc, Custumer TekCustomer, EnFrTyp DocCustTyp)
        {
            try
            {
                // Указываем продовца
                Fr.TableNumber = 2;
                Fr.RowNumber = 30;
                Fr.FieldNumber = 2;
                Fr.ValueOfFieldString = TekCustomer.fio_fo_check;  // "Жирникова Анастасия";     // Имя кассира
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

                // Открываем чек
                if (Fr.OpenCheck() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Не смогли открыть чек: {0}", Status.Description));
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
                if (IsCopy)
                {
                    // Печать информациипо чеку
                    Print2in1Line(string.Format("ККТ {0}" + Fr.SerialNumber),
                                string.Format("{0} {1}", Fr.Date.ToShortDateString(), Fr.Time.ToShortTimeString()));

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
                if (string.IsNullOrWhiteSpace(Doc.store_name))
                {
                    Print2in1Line(string.Format("КОД МАГАЗИНА: {0}", Doc.store_number),
                                string.Format("ЧЕК {0}", Doc.document_number));
                }
                else
                {
                    Print2in1Line(string.Format("КОД МАГАЗИНА: {0}", Doc.store_name),
                                string.Format("ЧЕК {0}", Doc.document_number));
                }


                // ПЕЧАТЬ ШТРИХ-КОДА
                Fr.BarCode = Doc.document_number.ToString();
                Fr.BarWidth = 2;
                Fr.LineNumber = 50;
                Fr.BarcodeType = 0;
                Fr.BarcodeAlignment = TBarcodeAlignment.baCenter;
                if (Fr.PrintBarcodeGraph() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Не смогли напечатать ПЕЧАТЬ ШТРИХ-КОДА: {0}", Status.Description));
                }


                // КАССИР - СОТРУДНИК
                //if Associater <> '' then
                //  S := Format('СОТРУДНИК: %s',[Associater]) //пока не знаю как взять это поле в депозитах.. ClerkName
                //else
                //S:= '';
                Print2in1Line(@"КАССИР:", TekCustomer.fio_fo_check);
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

                // Отчеркиваем заголовок
                PrintSeparator();

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

                        Print2in1Line(string.Format("Скидка на чек   {0}", Doc.discount_perc),
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
                    Print2in1Line(string.Format("Скидка на чек   {0}", Doc.total_discount_amt),
                                string.Format("={0}", Doc.total_discount_amt));
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
        /// <param name="CrocessSummToFR">Заполнить поля в фискальнике по нашим правилам или нет Fr.Summ1-Fr.Summ16 по умолчанию нет</param>
        /// <returns>Итоговая сумма по всем типам оплат</returns>
        public static decimal GetSummCheck(JsonPrintFiscDoc Doc, bool CrocessSummToFR)
        {
            try
            {
                decimal rez = 0;

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
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                            }

                            // Если тип оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.taken;
                            }

                            // Если тип оплаты подарочный сертификат
                            if (item.tender_type == Com.Config.TenderTypeGiftCert && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                            }

                            // Если тип оплаты подарочная карта
                            if (item.tender_type == Com.Config.TenderTypeGiftCard && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                            }

                            // Если тип оплаты подарочная карта
                            if (item.tender_type == Com.Config.TenderTypeAvans && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ14 += (decimal)item.taken;
                            }

                            // Заказ клиента когда он вносит сумму
                            if (item.tender_type == 7 && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                            }

                            // КОгда клиент делает возврат денег пока не выполнен заказ
                            if (item.tender_type == 7 && Doc.given_amt != 0)
                            {
                                rez += (decimal)Doc.given_amt;
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)Doc.given_amt;
                                flagexit = true;  // Делается по всему документу нет смысла проходить по позициям
                            }

                            break;
                        case 1:
                            // Если тип оплаты нал
                            if (item.tender_type == Com.Config.TenderTypeCash && item.given != 0)
                            {
                                rez += (decimal)item.given;
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.given;
                            }

                            // Если тип оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.given != 0)
                            {
                                rez += (decimal)item.given;
                                if (CrocessSummToFR) Fr.Summ4 += (decimal)item.given;
                            }

                            break;
                        case 2:
                            // Депозит наликом
                            if (item.tender_type == Com.Config.TenderTypeCash && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ1 += (decimal)item.taken;
                            }

                            // Депозит оплаты карта
                            if (item.tender_type == Com.Config.TenderTypeCredit && item.taken != 0)
                            {
                                rez += (decimal)item.taken;
                                if (CrocessSummToFR) Fr.Summ2 += (decimal)item.taken;
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
        private static void CloseReceipt(JsonPrintFiscDoc Doc, int Tax1, int Tax2, int Tax3, int Tax4)
        {
            try
            {
                // Отчеркивание оплат
                PrintSeparator();

                // Печатаем Емайл покупателя
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

                /*
                  // Если печатается строка по НДС
                  if (Result = 0) AND(bUseNDSString = 1) then
                   Result := PrintLine(StrAlignCenter('Сумма чека содержит НДС', nLenLine));
                */

                // Получение и печать Подитога
                //if Result = 0 then Result := CheckErr(
                if (Fr.CheckSubTotal() != 0)
                {
                    Verification(Fr);
                    throw new ApplicationException(string.Format("Не смогли отправить подитог ошибка: {0}", Status.Description));
                }
                Thread.Sleep(500);

                // Заполняем сумму в фискальнике
                GetSummCheck(Doc, true);


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
                Fr.OpenDrawer();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой при открытии денежного ящика: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.FR.OpenDrawer", EventEn.Error);
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

