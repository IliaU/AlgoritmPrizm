using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using CryptoPro.Sharpei;
using CryptoPro.Sharpei.Xml;
using AlgoritmPrizm.Lib;
using System.IO;
using System.Threading;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Класс для работы с сертификатом
    /// </summary>
    public static class Crypto
    {
        /// <summary>
        /// Хранилище доступных сертификатов
        /// </summary>
        private static X509Store store;

        /// <summary>
        /// Коллекция доступных сертификатов
        /// </summary>
        private static X509Certificate2Collection certCollections;

        /// <summary>
        /// Текущий сертификат
        /// </summary>
        public static X509Certificate2 curCert { get; private set; }

        public static DateTime GetExpirationDate
        {
            get
            {
                try
                {
                    if (curCert == null) throw new ArgumentNullException("Не установлен Текущий сертификат.");
                    return DateTime.Parse(curCert.GetExpirationDateString());
                }
                catch (Exception ex)
                {
                    ApplicationException ae = new ApplicationException(string.Format("Не смогли получить дату валидности сертификата ({0})", ex.Message));
                    Log.EventSave(ae.Message, "Crypto. GetExpirationDate", EventEn.Error);
                    throw ae;
                }
            }
            private set { }
        }

        /// <summary>
        /// Количество дней за которое нужно считать статус сертификата как Warning
        /// </summary>
        private static int WarningCountExpirationDate = 10;

        /// <summary>
        /// тата после которой нужно проверить валидность сертификата на предмент исчерпания по времени
        /// </summary>
        private static DateTime NestVerifValidateCertificate = DateTime.Now;

        /// <summary>
        /// Наличие подключения к токену
        /// </summary>
        /// <returns></returns>
        public static bool HashConnect()
        {
            if (Com.Config.EnableServiceCrypto)
            {
                // Делаем проверку валидности ттолько если мы не проверяли какое то время
                if (NestVerifValidateCertificate < DateTime.Now)
                {
                    NestVerifValidateCertificate = DateTime.Now.AddSeconds(30);
                    ValidateCertificate(false);
                }

                if (Status == CryptoStatusEn.Valid || Status == CryptoStatusEn.Warning) return true;
                else return false;
            }
            return false;
        }

        /// <summary>
        /// Статус Токена
        /// </summary>
        public static CryptoStatusEn Status
        {
            get; private set;
        }

        /// <summary>
        /// Глубина которая внутри при вызове
        /// </summary>
        private static int HashExecuCount;
        //
        /// <summary>
        /// Статус выполнения какой-то задачи
        /// </summary>
        public static bool HashExecuting { get; private set; }
        //
        /// <summary>
        /// Событие изменения текущего универсального провайдера
        /// </summary>
        public static event EventHandler<EventCrypto> onEventChangeHashExecuting;
        //
        /// <summary>
        /// Флаг для реализации при котором мы будем выставлять статус до начала работ и после
        /// </summary>
        /// <param name="FlsgExecuting">Перед началом передаём True. В конце передаём False</param>
        private static void setHashExecuting(bool flag)
        {
            if (Com.Config.EnableServiceCrypto)
            {
                // Смотрим глубину вложения по этому событию иначе возникает зависание в интерфейсе из за блокировки
                if (flag) HashExecuCount++;
                else
                {
                    if (HashExecuCount > 0) HashExecuCount--;
                }

                if ((flag && HashExecuCount == 1)
                        || (!flag && HashExecuCount == 0))
                {
                    HashExecuting = flag;

                    // Собственно обработка события
                    if (onEventChangeHashExecuting != null)
                    {
                        EventCrypto myArg = new EventCrypto(HashExecuting);

                        // Асинхронный запуск процесса
                        //thrTestConnect = new Thread(setHashExecutingRunEvent);
                        Thread thrEventChangeHashExecuting = new Thread(new ParameterizedThreadStart(setHashExecutingRunEvent)); //Запуск с параметрами   
                        thrEventChangeHashExecuting.Name = "A_Thr_EventChangeHashExecuting";
                        thrEventChangeHashExecuting.IsBackground = true;
                        thrEventChangeHashExecuting.Start(myArg);
                    }
                }
            }
        }
        //
        /// <summary>
        /// Асинронный запуск обработки нашего события
        /// </summary>
        private static void setHashExecutingRunEvent(object myArg)
        {
            try
            {
                if (onEventChangeHashExecuting != null)
                {
                    onEventChangeHashExecuting.Invoke(null, (EventCrypto)myArg);
                }
            }
            catch (Exception) { }
        }


        /// <summary>
        /// Получаем список доступных сертификатов
        /// </summary>
        /// <returns>Список доступных сертификатов</returns>
        public static List<string> GetListSert()
        {
            List<string> rez = new List<string>();

            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                // Если коллекцию уже получили то повторно этого делать не нужно
                if (certCollections == null)
                {
                    //store = new X509Store(StoreLocation.LocalMachine);
                    store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    //store.Open(OpenFlags.ReadOnly);
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    //certCollections = store.Certificates;
                    // Фильтруем только валидные по времени сертификаты
                    certCollections = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);
                }

                // Получаем список сертификатов
                foreach (X509Certificate2 item in certCollections)
                {
                    rez.Add(item.Subject);
                }

                // Тест выбора сертификата
                //X509Certificate SeleSert = SelectedSert();
                //byte[] fff = CreateSignature(SeleSert, "Привет мужик");
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли получить список сертификатов ({0})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.GetListSert", Lib.EventEn.Error, true, true);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }


            return rez;
        }

        /// <summary>
        /// Нужно взвать окновыбора у пользователя, чтобы он выбрал сертификат для настройки в качестве текущего
        /// </summary>
        /// <returns></returns>
        public static X509Certificate SelectedSert()
        {
            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                // Открываем хранилище сертификатов для поиска и выбора сертификата подписи
                X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Фильтруем только валидные по времени сертификаты
                var collection = store.Certificates.Find(X509FindType.FindByTimeValid, DateTime.Now, true);

                // Для отображения отфильтрованной коллекции используем стандартное окно windows. (работает очень медленно)
                // Другой способ - итерировать по переменной collection и выводить информацию о сертификате в своем интерфейсе (будет быстрее)
                X509Certificate2Collection signerCertificatesCollection = X509Certificate2UI.SelectFromCollection(collection, "Выберите сертификат", "Выберите сертификат из списка для автоматического заполнения полей заявки.", X509SelectionFlag.SingleSelection);

                // Возвращает выбранный сертификат для последующего использования.
                return signerCertificatesCollection.Count != 0 ? signerCertificatesCollection[0] : null;

            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли вызвать выбор сертификатов ({0})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.SelectedSert(t", Lib.EventEn.Error, true, true);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }
        }

        /// <summary>
        /// Подписываем сертификатом
        /// </summary>
        /// <param name="signerCertificate">Сертификат которым делаем подпись</param>
        /// <param name="dataStr">Данные которые хотим подписать</param>
        static byte[] CreateSignature(X509Certificate signerCertificate, string dataStr, bool Detached)
        {
            byte[] rez = null;

            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                // Создаем объект для подписи
                var contentInfo = new ContentInfo(Encoding.UTF8.GetBytes(dataStr));
                // Инициализируем класс для работы с Cms и создания открепленной подписи (без самих данных) последний параметр это прикреплённая или не прикреплённая подпись
                var cms = new SignedCms(contentInfo, Detached);
                // Создаем подписанта из сертификата, который был выбран ранее
                var signer = new CmsSigner((X509Certificate2)signerCertificate);
                // Вычисляем подпись
                cms.ComputeSignature(signer, true);
                // Кодируем подпись
                rez = cms.Encode();
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли подписать набор данных выбранным сертификатом ({0})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.CreateSignature(t", Lib.EventEn.Error, true, true);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }

            return rez;
        }

        /// <summary>
        /// Доступный для других методов метод подписания данных
        /// </summary>
        /// <param name="dataStr">Данные которые нужно подписать</param>
        /// <returns>Цифровая подпись</returns>
        public static byte[] CreateSignature(string dataStr, bool Detached)
        {
            try
            {
                if (curCert == null) throw new ApplicationException("Сертификат пока не установлен");
                return CreateSignature(curCert, dataStr, Detached);
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли подписать набор данных выбранным сертификатом ({0})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.CreateSignature(string dataStr)", Lib.EventEn.Error, true, true);
                throw exp;
            }


        }

        /// <summary>
        /// Доступный для других методов метод подписания данных
        /// </summary>
        /// <param name="dataStr">Данные которые нужно подписать</param>
        /// <returns>Цифровая подпись</returns>
        public static byte[] CreateSignature(string dataStr)
        {
            try
            {
                return CreateSignature(dataStr, false);
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли подписать набор данных выбранным сертификатом ({0})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.CreateSignature(string dataStr)", Lib.EventEn.Error, true, true);
                throw exp;
            }


        }


        /// <summary>
        /// Устанавливаем текущий сертификат, который будем использовать
        /// </summary>
        /// <param name="SubjectName">Имя сертификата, которое нужно найти и установить в качестве текущего</param>
        public static void OpenSert(string SubjectName)
        {
            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                X509Certificate2 crt = null;

                if (store == null || certCollections == null) GetListSert();

                if (certCollections == null) throw new ApplicationException(string.Format("Не смогли получить список доступных сертификатов."));

                // Перебор доступных сертификатов
                foreach (X509Certificate2 item in certCollections)
                {
                    if (item.Subject.Contains(SubjectName))
                    {
                        crt = item;
                        break;
                    }
                }

                // Если сертификат найден
                if (crt != null)
                {
                    if (ValidateCertificate(crt, true))
                    {
                        // Настраиваем открытый сертификат
                        curCert = crt;
                        DateTime timeSert = DateTime.Parse(crt.GetExpirationDateString());

                        // Устанавливаем статус нашего открытого сертификата
                        if (timeSert.AddDays(WarningCountExpirationDate) > DateTime.Now)
                        {
                            Status = CryptoStatusEn.Valid;
                        }
                        else Status = CryptoStatusEn.Warning;
                    }
                    else throw new ApplicationException(string.Format(@"Cертификат ""{0}"" не валиден. Дата валидности истекла.", SubjectName));
                }
                else
                {
                    throw new ApplicationException(string.Format("Не смогли найти сертификат: {0}", SubjectName));
                }


            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли открыть сертификат: {0} ({1})", SubjectName, ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.OpenSert", Lib.EventEn.Error, true, false);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }

        }

        /// <summary>
        /// Проверка валидности сертификата
        /// </summary>
        /// <param name="Crt">Сертификат, который нужно проверить.</param>
        /// <param name="isNotLogEvent">Не логировать</param>
        /// <returns>Статус Валидности сртификата на текущий момент времени.</returns>
        public static bool ValidateCertificate(X509Certificate Crt, bool isNotLogEvent)
        {
            bool FlagValid = false;
            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                // Получаем дату до которой сертификат считается валидным
                DateTime timeSert = DateTime.Parse(Crt.GetExpirationDateString());

                // Считаем валидность сертификата
                if (DateTime.Now.CompareTo(timeSert) < 0)
                {
                    if (isNotLogEvent) Com.Log.EventSave(string.Format("Сертификат валиден. Текущая дата: {0} Дата сертификата: {1}", DateTime.Now.ToString(), timeSert.ToString()), "Crypto.ValidateCertificate", Lib.EventEn.Message, true, false);
                    FlagValid = true;
                }
                else Com.Log.EventSave(string.Format("Сертификат не валиден. Текущая дата: {0} Дата сертификата: {1}", DateTime.Now.ToString(), timeSert.ToString()), "Crypto.ValidateCertificate", Lib.EventEn.Message, true, false);

                // Устанавливаем статус нашего открытого сертификата
                if (FlagValid)
                {
                    if (timeSert.AddDays(WarningCountExpirationDate) > DateTime.Now)
                    {
                        Status = CryptoStatusEn.Valid;
                    }
                    else Status = CryptoStatusEn.Warning;
                }
                else Status = CryptoStatusEn.NotValid;

            }
            catch (Exception ex)
            {
                if (isNotLogEvent) Com.Log.EventSave(string.Format("Не смогли проверить валидность сертификата: {0} ({1})", Crt.Subject, ex.Message), "Crypto.ValidateCertificate", Lib.EventEn.Error, true, true);
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }
            return FlagValid;
        }
        //
        /// <summary>
        /// Проверка валидности открытого сертификата
        /// </summary>
        /// <param name="isNotLogEvent">Не логировать</param>
        /// <returns>Статус Валидности сртификата на текущий момент времени.</returns>
        public static bool ValidateCertificate(bool isNotLogEvent)
        {
            bool rez = false;
            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                // Можно проверить валидность только если сертификат установлен и доступен
                if (curCert != null) rez = ValidateCertificate(curCert, isNotLogEvent);
                else Status = CryptoStatusEn.Empty;
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли открыть текущий сертификат ({1})", ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.ValidateCertificate(string SubjectName)", Lib.EventEn.Error, true, false);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }

            return rez;
        }
        //
        /// <summary>
        /// Проверка валидности открытого сертификата
        /// </summary>
        /// <returns>Статус Валидности сртификата на текущий момент времени.</returns>
        public static bool ValidateCertificate(string SubjectName)
        {
            bool rez = false;
            try
            {
                // Отмечаем начало работы
                setHashExecuting(true);

                X509Certificate2 crt = null;

                if (store == null || certCollections == null) GetListSert();

                if (certCollections == null) throw new ApplicationException(string.Format("Не смогли получить список доступных сертификатов."));

                // Перебор доступных сертификатов
                foreach (X509Certificate2 item in certCollections)
                {
                    if (item.Subject.Contains(SubjectName))
                    {
                        crt = item;
                        break;
                    }
                }

                // Если сертификат найден
                if (crt != null)
                {
                    rez = ValidateCertificate(crt, true);
                    if (!rez) throw new ApplicationException(string.Format(@"Cертификат ""{0}"" не валиден. Дата валидности истекла.", SubjectName));
                }
                else
                {
                    throw new ApplicationException(string.Format("Не смогли найти сертификат: {0}", SubjectName));
                }
            }
            catch (Exception ex)
            {
                ApplicationException exp = new ApplicationException(string.Format("Не смогли открыть сертификат: {0} ({1})", SubjectName, ex.Message));
                Com.Log.EventSave(exp.Message, "Crypto.ValidateCertificate(string SubjectName)", Lib.EventEn.Error, true, false);
                throw exp;
            }
            finally
            {
                // Отмечаем окончание работы
                setHashExecuting(false);
            }
            return rez;
        }


        static void Encrypt(string inFileName, string outFileName, SymmetricAlgorithm gost)
        {
            //Создаем потоки для входного и выходного файлов.
            byte[] bin = new byte[100];  // Промежуточный буфер.
            long rdlen = 0;              // Общее число записанных байт.

            try
            {
                FileStream fin = File.Open(inFileName, FileMode.OpenOrCreate);//  new FileStream(inFileName, FileMode.Open, FileAccess.Read);
                FileStream fout = File.Open(outFileName, FileMode.OpenOrCreate);  //new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
                fout.SetLength(0);

                long totlen = fin.Length;    // Общий размер входного файла.
                int len;                     // Число считанных за один раз байт.

                // Создаем криптопоток для записи
                CryptoStream encStream = new CryptoStream(fout, gost.CreateEncryptor(),
                    CryptoStreamMode.Write);

                Console.WriteLine("Зашифрование...");

                // Читаем из входного файла, шифруем и пишем в выходной.
                while (rdlen < totlen)
                {
                    len = fin.Read(bin, 0, 100);
                    encStream.Write(bin, 0, len);
                    rdlen = rdlen + len;
                    Console.WriteLine("обработано {0} байтов", rdlen);
                }

                encStream.Close();
                fout.Close();
                fin.Close();
            }
            catch (IOException ex)
            {

                throw ex;
            }
        }

        static void Decrypt(string inFileName, string outFileName, SymmetricAlgorithm gost)
        {
            // Создаем потоки для входного и выходного файлов.
            byte[] bin = new byte[100];  // Промежуточный буфер.
            long rdlen = 0;              // Общее число записанных байт.

            FileStream fin = new FileStream(inFileName, FileMode.Open, FileAccess.Read);
            FileStream fout = new FileStream(outFileName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            long totlen = fin.Length;    // Общий размер входного файла.
            int len;                     // Число считанных за один раз байт.

            // Создаем криптопоток для записи
            CryptoStream encStream = new CryptoStream(fout, gost.CreateDecryptor(),
                CryptoStreamMode.Write);

            Console.WriteLine("Расшифрование...");

            // Читаем из входного файла, расшифровываем и пишем в выходной.
            while (rdlen < totlen)
            {
                len = fin.Read(bin, 0, 100);
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
                Console.WriteLine("обработано {0} байтов", rdlen);
            }

            encStream.Close();
            fout.Close();
            fin.Close();
        }



        static byte[] GostSignHash(byte[] HashToSign, Gost3410CryptoServiceProvider key, string HashAlg)
        {
            try
            {
                //Создаем форматтер подписи с закрытым ключом из переданного 
                //функции криптопровайдера.
                GostSignatureFormatter Formatter = new GostSignatureFormatter((Gost3410CryptoServiceProvider)key);

                //Устанавливаем хэш-алгоритм.
                Formatter.SetHashAlgorithm(HashAlg);

                //Создаем подпись для HashValue и возвращаем ее.
                return Formatter.CreateSignature(HashToSign);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static bool GostVerifyHash(byte[] HashValue, byte[] SignedHashValue, AsymmetricAlgorithm key, string HashAlg)
        {
            try
            {
                //Создаем форматтер подписи с закрытым ключом из переданного 
                //функции криптопровайдера.
                GostSignatureDeformatter Deformatter = new GostSignatureDeformatter(key);

                //Устанавливаем хэш-алгоритм.
                Deformatter.SetHashAlgorithm(HashAlg);

                //Проверяем подпись и возвращаем результат. 
                return Deformatter.VerifySignature(HashValue, SignedHashValue);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
