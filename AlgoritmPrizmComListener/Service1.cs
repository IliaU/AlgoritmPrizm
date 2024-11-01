using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using AlgoritmPrizmCom;
using AlgoritmPrizmCom.Lib;
using AlgoritmPrizmCom.Com;

namespace AlgoritmPrizmComListener
{
    public partial class Service1 : ServiceBase
    {

        public static string ProgramDir = @"C:\Program Files\AlgoritmPrizmCom";

        public Service1()
        {
            InitializeComponent(); 
        }

        protected override async void OnStart(string[] args)
        {
            if (!Directory.Exists(ProgramDir)) Directory.CreateDirectory(ProgramDir);
            Log lg = new Log(string.Format(@"{0}\AlgoritmPrizmCom.txt", ProgramDir));
            Config conf = new Config(string.Format(@"{0}\AlgoritmPrizmCom.xml", ProgramDir));


            if (!Directory.Exists(Config.RequestsFolder)) Directory.CreateDirectory(Config.RequestsFolder);
            string buf=null;

            // Запускаем службу чтобы работала бесконечно
            while (true)
            {
                // Обработка ошибок чтобы сервис не свалился
                try
                {
                    // Проверяем наличие файла
                    if (File.Exists(string.Format(@"{0}\request.txt", Config.RequestsFolder)))
                    {
                        // Читаем файл
                        using (StreamReader SwFile = new StreamReader(string.Format(@"{0}\request.txt", Config.RequestsFolder)))
                        {
                            buf=SwFile.ReadToEnd();

                            if (Config.Trace) Log.EventSave(string.Format("Обнаружен файл request.txt с содержимым: {0}", buf), "AlgoritmPrizmComListener", EventEn.Message);
                        }

                        // Если обнаружен файл и он не пустой
                        if (!string.IsNullOrWhiteSpace(buf))
                        {
                            string[] bufrow = buf.Split('\r');
                            buf = null;
                            if (bufrow.Length >= 2 && bufrow[1].Trim() == "Sale")
                            {
                                try
                                {
                                    // Получение настроек конфигурации с AlgoritmPrizm
                                    CdnResponceConfig cdnConf = Web.CdnForIsmpConfig();

                                    // Проверка матрикс кода через наш плагин AlgoritmPrizm
                                    CdnResponce cndResp = Web.CdnForIsmpCheck(bufrow[0].Trim());
                                    buf = CdnResponce.SerializeObject(cndResp);
                                    if (Config.Trace) Log.EventSave(string.Format("Получен ответ от ЦРПТ: {0}", buf), "AlgoritmPrizmComListener", EventEn.Message);

                                    // Проверяем на наличие ошибки
                                    if (!string.IsNullOrWhiteSpace(buf) && buf.IndexOf("ошибка") > 0)
                                    {
                                        using (StreamWriter SwFile = new StreamWriter(string.Format(@"{0}\response.txt", Config.RequestsFolder), true))
                                        {
                                            SwFile.WriteLine("False");
                                        }
                                    }
                                    else
                                    {
                                        using (StreamWriter SwFile = new StreamWriter(string.Format(@"{0}\response.txt", Config.RequestsFolder), true))
                                        {
                                            SwFile.WriteLine("True");
                                            SwFile.WriteLine(cdnConf.FrTag1262);
                                            SwFile.WriteLine(cdnConf.FrTag1263);
                                            SwFile.WriteLine(cdnConf.FrTag1264);
                                            SwFile.WriteLine(string.Format(@"UUID={0}&Time={1}", cndResp.reqId, cndResp.reqTimestamp));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    using (StreamWriter SwFile = new StreamWriter(string.Format(@"{0}\response.txt", Config.RequestsFolder), true))
                                    {
                                        SwFile.WriteLine("Error");
                                    }
                                }

                                // Удаляем файл запроса
                                if(File.Exists(string.Format(@"{0}\request.txt", Config.RequestsFolder))) File.Delete(string.Format(@"{0}\request.txt", Config.RequestsFolder));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (ex.Message.IndexOf("Unable to connect to the remote server")>=0)
                        {
                            // Наша служба AlgoritmPrizm не включена
                            using (StreamWriter SwFile = new StreamWriter(string.Format(@"{0}\response.txt", Config.RequestsFolder), true))
                            {
                                SwFile.WriteLine("Timeout");
                            }

                            // Удаляем файл запроса
                            if (File.Exists(string.Format(@"{0}\request.txt", Config.RequestsFolder))) File.Delete(string.Format(@"{0}\request.txt", Config.RequestsFolder));
                        }

                        Log.EventSave(string.Format("Ошибка: {0}", ex.Message), "AlgoritmPrizmComListener", EventEn.Error);
                    }
                    catch (Exception) { }
                }

                // Пауза между циклами
                await Task.Delay(Config.TimeOutMiSec);
            }

        }

        protected override void OnStop()
        {
            try
            {
                Log.EventSave("Cлужба остановлена.", "AlgoritmPrizmComListener", EventEn.Message);
            }
            catch (Exception) { }
        }
    }
}
