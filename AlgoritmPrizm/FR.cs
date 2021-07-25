using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using DrvFRLib;


namespace AlgoritmPrizm
{
    /// <summary>
    /// Для работы с фискальным регистратором
    /// </summary>
    public static class FR
    {

        public static void testc()
        {
            string DocName = "Имя документа"; // Win1251    30 len
            int OperatorNumber = 1;             // 30 мах

            DrvFRLib.DrvFR Fr = new DrvFR();
            Fr.Password = 30;
            Fr.PortNumber = 3; // Com port
            Fr.ComputerName = Environment.MachineName;
            //Fr.BaudRate скорость от 0..6
            //Fr.Timeout  тайм аут в приёме байт от 0..255
            if (Fr.Connect() != 0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }


            // Проверяем статус фискальника если смена закрыта то надо открыть
            if (FRstatECRMode(Fr) == 4)
            {
                if (OperatorNumber > 0 && OperatorNumber < 31)
                {
                    // Не понятно как указать оператора
                    //Кажется это задаётся паролем который ввели при подключении к устройству
                    //Fr.OperatorNumber = OperatorNumber;
                    Fr.FNOpenSession();
                    FRstatECRMode(Fr);
                }
                else throw new ApplicationException("Не выбран оператор в фискальном регистраторе.");
            }
            if (statECRMode != 2)
            {
                switch (statECRMode)
                {
                    case 8:
                        int uuu=Fr.CloseCheck();
                        FRstatECRMode(Fr);
                        break;
                    default:
                        throw new ApplicationException(string.Format(@"Статус фискальника:""{0}""", statECRModeDescription));
                }
            }




            int hhh = FRstatECRMode(Fr);


            int ttn = Fr.TableNumber;

            // Открываем для настройки параметра
            if (Fr.FNOperation()!=0)
            {
                Fr.Annulment();
                Fr.Disconnect();
            }
            Fr.TagNumber = 1021;
            Fr.TagType = 7;
            Fr.TagValueStr = "Погодин";
            if (Fr.FNSendTag() != 0)
            {
                int ResultCode = Fr.ResultCode;
                string ResultCodeDescription = Fr.ResultCodeDescription;
                string ss = "";
                if (Fr.FNGetTagDescription() == 0) ss=Fr.TagDescription;

                MessageBox.Show(string.Format("{0} {1} ({2})", ResultCode, ResultCodeDescription, ss));
                //return;
            }
            // Открываем для настройки параметра
            if (Fr.FNOperation() != 0)
            {
                Fr.Annulment();
                Fr.Disconnect();
            }

            int uu =Fr.FNBeginSTLVTag();
            Fr.TagNumber = 1203;        // ИНН кассира   1227 ИНН покупателя, 1228 имя покупателя, 1230код страны, 1230 Номер ГТД
            Fr.TagType = 7;             // string
            Fr.TagValueStr = "123456789012";
            if (Fr.FNAddTag() != 0)
            {
                int ResultCode = Fr.ResultCode;
                string ResultCodeDescription = Fr.ResultCodeDescription;
                string ss = "";
                if (Fr.FNGetTagDescription() == 0) ss = Fr.TagDescription;

                MessageBox.Show(string.Format("{0} {1} ({2})", ResultCode, ResultCodeDescription, ss));
                //return;
            }

            
          

            // Печатаем заголовок документа
            if (!string.IsNullOrWhiteSpace(DocName))
            {
                if (DocName.Length > 30) throw new ApplicationException("Длина заголовка не может быть более 30 символов.");
                else
                {
                    Fr.DocumentName = DocName;
                    Fr.PrintDocumentTitle();
                }
            }


            // Fr.CheckType = 1; //Приход
            Fr.Quantity = 1000;
            Fr.Price = (decimal)1.5;// (decimal)1.56;
            Fr.Department = 1;
            //Fr.Summ1 = (decimal)177.61;
            //Fr.TaxValueEnabled = false;
            Fr.Tax1 = 1;
            Fr.Tax2 = 2;
            Fr.Tax3 = 0;
            Fr.Tax4 = 0;
            
            //Fr.PaymentTypeSign = 4;
            //Fr.PaymentItemSign = 1;
            Fr.StringForPrinting = "Tovar";
            //int i1= Fr.FNOperation();


           // Fr.FNSendItemCodeData();
            if (Fr.Sale() != 0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }




            Fr.Password = 30;
            Fr.PortNumber = 3; // Com port
            //if (Fr.Connect() != 0)
            //{
            //    MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
            //    return;
            //}
            Fr.Summ1 = (decimal)1500;
            Fr.Summ2 = 0;// 100;
            Fr.Summ3 = 0; // 200;
            Fr.Summ4 = 0; // 300;
            //Fr.Summ5 = 0;
            //Fr.Summ6 = 0;
            //Fr.Summ7 = 0;
            //Fr.Summ8 = 0;
            //Fr.Summ9 = 0;
            //Fr.Summ10 = 0;
            //Fr.Summ11 = 0;
            //Fr.Summ12 = 0;
            //Fr.Summ13 = 0;
            //Fr.Summ14 = 0;
            //Fr.Summ15 = 0;
            //Fr.Summ16 = 0;
//            Fr.DiscountOnCheck = 5;
            //Fr.RoundingSumm = 0;
            //Fr.TaxValue1 = 0;
            //Fr.TaxValue2 = 0;
            //Fr.TaxValue3 = 0;
            //Fr.TaxValue4 = 0;
            //Fr.TaxValue5 = 0;
            //Fr.TaxValue6 = 0;
            //Fr.TaxType = 1;
            Fr.Tax1 = 1;
            Fr.Tax2 = 2;
            Fr.Tax3 = 0;
            Fr.Tax4 = 0;
            Fr.StringForPrinting = "========";
            if (Fr.CloseCheck()!=0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }
            MessageBox.Show(string.Format("Ваша сдача {0}", Fr.Change));
            //Fr.Disconnect();

            /*
            Fr.BarCode = "rrrr";
            Fr.LineNumber = 50;
            Fr.BarcodeType = 0; // Code 128 a
            Fr.BarWidth = 3;
            Fr.BarcodeAlignment = 0;
            Fr.PrintBarcodeText = 0;
            Fr.TagNumber = 1203;
            Fr.TagType = 7;
            Fr.TagValueStr = "TagValueStr";
            int Err = Fr.FNSendTag();
            Fr.PrintString();
            Fr.PrintBarcodeLine();
            Fr.Disconnect();
            */
        }


        public static void testcM()
        {
            string DocName = "Имя документа"; // Win1251    30 len
            int OperatorNumber = 1;             // 30 мах

            DrvFRLib.DrvFR Fr = new DrvFR();
            Fr.Password = 30;
            Fr.PortNumber = 3; // Com port
            Fr.ComputerName = Environment.MachineName;

            string Matrix = @"010290000066650421Jsid2E""4oh2>T91002a92/1tPzrragHUbOA+cq0FIp54OZZF6GcVCJhA9W6Mnb7W6LvZEn9r9thrj+HsBFpqyH/zl5Ri6pXxF3HTwjuWeKG== 007R";

            //Fr.BaudRate скорость от 0..6
            //Fr.Timeout  тайм аут в приёме байт от 0..255
            if (Fr.Connect() != 0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }


            // Проверяем статус фискальника если смена закрыта то надо открыть
            if (FRstatECRMode(Fr) == 4)
            {
                if (OperatorNumber > 0 && OperatorNumber < 31)
                {
                    // Не понятно как указать оператора
                    //Кажется это задаётся паролем который ввели при подключении к устройству
                    //Fr.OperatorNumber = OperatorNumber;
                    if (Fr.FNOpenSession() != 0)
                    {
                        MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription)); 
                    }
                    Fr.Disconnect();
                    Fr = new DrvFR();
                    Fr.Password = 30;
                    Fr.PortNumber = 3; // Com port
                    Fr.ComputerName = Environment.MachineName;
                    if (Fr.Connect() != 0)
                    {
                        MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                        return;
                    }
                    FRstatECRMode(Fr);

                }
                else throw new ApplicationException("Не выбран оператор в фискальном регистраторе.");
            }
            if (statECRMode != 2)
            {
                switch (statECRMode)
                {
                    // Если кончились 24 часа то закрываем смену сняв Z отчёт
                    case 3:
                        if (Fr.FNCloseSession() != 0)
                        {
                            MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                        }
                        Fr.Disconnect();
                        return;
                    case 8:
                        int uuu=Fr.CloseCheck();
                        FRstatECRMode(Fr);
                        break;
                    default:
                        throw new ApplicationException(string.Format(@"Статус фискальника:""{0}""", statECRModeDescription));
                }
            }



            // Печатаем заголовок документа
            if (!string.IsNullOrWhiteSpace(DocName))
            {
                if (DocName.Length > 30) throw new ApplicationException("Длина заголовка не может быть более 30 символов.");
                else
                {
                    Fr.DocumentName = DocName;
                    if (Fr.PrintDocumentTitle() != 0)
                    {
                        MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                    }
                }
            }

            int hhh = FRstatECRMode(Fr);


            int ttn = Fr.TableNumber;

            // Открываем для настройки параметра
            if (Fr.FNOperation()!=0)
            {
                Fr.Annulment();
                Fr.Disconnect();
            }
            Fr.TagNumber = 1021;
            Fr.TagType = 7;
            Fr.TagValueStr = "Жирникова Анастасия";
            if (Fr.FNSendTag() != 0)
            {
                int ResultCode = Fr.ResultCode;
                string ResultCodeDescription = Fr.ResultCodeDescription;
                string ss = "";
                if (Fr.FNGetTagDescription() == 0) ss=Fr.TagDescription;

                MessageBox.Show(string.Format("{0} {1} ({2})", ResultCode, ResultCodeDescription, ss));
                //return;
            }
            // Открываем для настройки параметра
            if (Fr.FNOperation() != 0)
            {
                Fr.Annulment();
                Fr.Disconnect();
            }

            int uu =Fr.FNBeginSTLVTag();
            Fr.TagNumber = 1203;        // ИНН кассира   1227 ИНН покупателя, 1228 имя покупателя, 1230код страны, 1230 Номер ГТД
            Fr.TagType = 7;             // string
            Fr.TagValueStr = "561142439622";
            if (Fr.FNAddTag() != 0)
            {
                int ResultCode = Fr.ResultCode;
                string ResultCodeDescription = Fr.ResultCodeDescription;
                string ss = "";
                if (Fr.FNGetTagDescription() == 0) ss = Fr.TagDescription;

                MessageBox.Show(string.Format("{0} {1} ({2})", ResultCode, ResultCodeDescription, ss));
                //return;
            }

            /*
          

            // Печатаем заголовок документа
            if (!string.IsNullOrWhiteSpace(DocName))
            {
                if (DocName.Length > 30) throw new ApplicationException("Длина заголовка не может быть более 30 символов.");
                else
                {
                    Fr.DocumentName = DocName;
                    if (Fr.PrintDocumentTitle()!=0)
                    {
                        MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                    }
                }
            }
            */

            // Fr.CheckType = 1; //Приход
            Fr.Quantity = 1;
            Fr.Price = (decimal)1.5;// (decimal)1.56;
            Fr.MarkingType = 17485;///Тип товара маркированный
            Fr.GTIN = Matrix.Substring(1, 14); // парсим GTIN
            Fr.SerialNumber = Matrix.Substring(15, 13); // парсим серийный номер
            if (Fr.FNSendItemCodeData() != 0)                    // Отправляем матрикс код
            {

            }
            Fr.Department = 1;
            //Fr.Summ1 = (decimal)177.61;
            //Fr.TaxValueEnabled = false;
            Fr.Tax1 = 1;
            Fr.Tax2 = 2;
            Fr.Tax3 = 0;
            Fr.Tax4 = 0;
            
            //Fr.PaymentTypeSign = 4;
            //Fr.PaymentItemSign = 1;
            Fr.StringForPrinting = "Tovar";
            //int i1= Fr.FNOperation();


           // Fr.FNSendItemCodeData();
            if (Fr.Sale() != 0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }




            Fr.Password = 30;
            Fr.PortNumber = 3; // Com port
            //if (Fr.Connect() != 0)
            //{
            //    MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
            //    return;
            //}
            Fr.Summ1 = (decimal)1.5;
            Fr.Summ2 = 0;// 100;
            Fr.Summ3 = 0; // 200;
            Fr.Summ4 = 0; // 300;
            //Fr.Summ5 = 0;
            //Fr.Summ6 = 0;
            //Fr.Summ7 = 0;
            //Fr.Summ8 = 0;
            //Fr.Summ9 = 0;
            //Fr.Summ10 = 0;
            //Fr.Summ11 = 0;
            //Fr.Summ12 = 0;
            //Fr.Summ13 = 0;
            //Fr.Summ14 = 0;
            //Fr.Summ15 = 0;
            //Fr.Summ16 = 0;
//            Fr.DiscountOnCheck = 5;
            //Fr.RoundingSumm = 0;
            //Fr.TaxValue1 = 0;
            //Fr.TaxValue2 = 0;
            //Fr.TaxValue3 = 0;
            //Fr.TaxValue4 = 0;
            //Fr.TaxValue5 = 0;
            //Fr.TaxValue6 = 0;
            //Fr.TaxType = 1;
            Fr.Tax1 = 1;
            Fr.Tax2 = 2;
            Fr.Tax3 = 0;
            Fr.Tax4 = 0;
            Fr.StringForPrinting = "========";
            if (Fr.FNCloseCheckEx()!=0)
            {
                MessageBox.Show(string.Format("{0} {1}", Fr.ResultCode, Fr.ResultCodeDescription));
                return;
            }
            MessageBox.Show(string.Format("Ваша сдача {0}", Fr.Change));
            //Fr.Disconnect();

            /*
            Fr.BarCode = "rrrr";
            Fr.LineNumber = 50;
            Fr.BarcodeType = 0; // Code 128 a
            Fr.BarWidth = 3;
            Fr.BarcodeAlignment = 0;
            Fr.PrintBarcodeText = 0;
            Fr.TagNumber = 1203;
            Fr.TagType = 7;
            Fr.TagValueStr = "TagValueStr";
            int Err = Fr.FNSendTag();
            Fr.PrintString();
            Fr.PrintBarcodeLine();
            Fr.Disconnect();
            */
        }


        /// <summary>
        /// Код состояния фискального регистратора
        /// </summary>
        public static int statECRMode;

        /// <summary>
        /// Описание состояния фискального регистратора
        /// </summary>
        public static string statECRModeDescription;

        /// <summary>
        /// Проверка статуса фискального регистратора
        /// </summary>
        /// <param name="Fr">Фискальный регистратор с которым работаем</param>
        /// <returns>Код состояния</returns>
        public static int FRstatECRMode(DrvFR Fr)
        {
            try
            {
                // Проверка в каком режиме наодится фискальник
                statECRMode = Fr.ECRMode;
                if (statECRMode != 0)
                {
                    switch (statECRMode)
                    {
                        case 0:
                            statECRModeDescription = "Принтер в рабочем режиме";
                            break;
                        case 1:
                            statECRModeDescription="Выдача данных";
                            break;
                        case 2:
                            statECRModeDescription="Открытая смена, 24 часа не кончились";
                            break;
                        case 3:
                            statECRModeDescription="Открытая смена, 24 часа кончились";
                            break;
                        case 4:
                            statECRModeDescription = "Закрытая смена";
                            break;
                        case 5:
                            statECRModeDescription = "Блокировка по неправильному паролю налогового инспектора";
                            break;
                        case 6:
                            statECRModeDescription = "Ожидание подтверждения ввода даты";
                            break;
                        case 7:
                            statECRModeDescription = "разрешение изменения положения десятичной точки";
                            break;
                        case 8:
                            statECRModeDescription = "Открытый документ";
                            break;
                        case 9:
                            statECRModeDescription = "Режим разрешения технологического обнуления";
                            break;
                        case 10:
                            statECRModeDescription = "Тестовый прогон";
                            break;
                        case 11:
                            statECRModeDescription = "Печать полного фискального отчёта";
                            break;
                        case 12:
                            statECRModeDescription = "Печать длинного отчёта ЭКЛЗ";
                            break;
                        case 13:
                            statECRModeDescription = "Работа с фискальным подкладным документом";
                            break;
                        case 14:
                            statECRModeDescription = "Печать докладного документа";
                            break;
                        case 15:
                            statECRModeDescription = "Фискальный подкладной документ сформирован";
                            break;
                        default:
                            statECRModeDescription = "Не известная ошибка";
                            break;
                    }
                }


                return statECRMode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}