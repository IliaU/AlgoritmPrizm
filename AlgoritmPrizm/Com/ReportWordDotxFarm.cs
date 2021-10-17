using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wrd = WordDotx;
using AlgoritmPrizm.Lib;
using System.IO;

namespace AlgoritmPrizm.Com
{
    public class ReportWordDotxFarm
    {
        /// <summary>
        /// Класс который будет у нас формировать наши отчёты
        /// </summary>
        public static Wrd.FarmWordDotx WordDotx;

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReportWordDotxFarm()
        {
            int Vers = 1;
            try
            {
                // Инициализация наших классов
                if (WordDotx==null)
                {
                    // Проверка валидности версии нашей dll
                    if (Wrd.FarmWordDotx.VersionDll[0] != Vers) throw new ApplicationException(string.Format("Версия файла WordDotx не поддерживается нужна версия {0}", Vers));

                    // Логируем запуск модуля
                    Log.EventSave("Загрузка модуля построителя отчётов.", string.Format("{0}", this.GetType().Name), EventEn.Message);

                    // Проверяем наличие папочек
                    string Sorce = string.Format(@"{0}\{1}", Environment.CurrentDirectory, Config.WordDotxSource);
                    string Target = string.Format(@"{0}\{1}", Environment.CurrentDirectory, Config.WordDotxTarget);
                    if (!Directory.Exists(Sorce)) Directory.CreateDirectory(Sorce);
                    if (!Directory.Exists(Target)) Directory.CreateDirectory(Target);

                    // Создаём сервер которы будет формировать отчёты
                    Wrd.FarmWordDotx.CreateWordDotxServer(Sorce, Target);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}", this.GetType().Name),  EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-19
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public void CreateReportInf19(string DocSid)
        {
            try
            {
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();

                Wrd.TableList TblL = new Wrd.TableList();

                Wrd.TotalList Ttl = new Wrd.TotalList();

                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Шаблон.dotx", @"Результат.doc", BkmL, TblL);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf19", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-8
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public void CreateReportInf8(string DocSid)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf8", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-3
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public void CreateReportInf3(string DocSid)
        {
            try
            {

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf3", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }
    }
}
