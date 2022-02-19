using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wrd = WordDotx;
using AlgoritmPrizm.Lib;
using System.IO;
using System.Data;
using System.Drawing;

namespace AlgoritmPrizm.Com
{
    public class ReportWordDotxFarm
    {
        /// <summary>
        /// Класс который будет у нас формировать наши отчёты
        /// </summary>
        public static ReportWordDotxFarm WordDotxFrm;

        /// <summary>
        /// Последняя ошибка в пуле формирования отчётов
        /// </summary>
        public static Wrd.EvTaskWordError LastErroServerWord;

        /// <summary>
        /// Последняя ошибка в пуле формирования отчётов
        /// </summary>
        public static Wrd.EvTaskExcelError LastErroServerExcel;

        /// <summary>
        /// При добавлении задания туда складываем результат чтобы потом за ним следить и чтобы потом оценивать что в каком статусе
        /// </summary>
        public static List<Wrd.TaskWord> RezStatCachWord = new List<Wrd.TaskWord>();

        /// <summary>
        /// При добавлении задания туда складываем результат чтобы потом за ним следить и чтобы потом оценивать что в каком статусе
        /// </summary>
        public static List<Wrd.TaskExcel> RezStatCachExcel = new List<Wrd.TaskExcel>();

        /// <summary>
        /// Конструктор
        /// </summary>
        public ReportWordDotxFarm()
        {
            int Vers = 1;
            try
            {
                // Инициализация наших классов
                if (WordDotxFrm == null)
                {
                    // Проверка валидности версии нашей dll
                    if (Wrd.FarmWordDotx.VersionDll[0] != Vers) throw new ApplicationException(string.Format("Версия файла WordDotx (Word) не поддерживается нужна версия {0}", Vers));
                    if (Wrd.FarmExcel.VersionDll[0] != Vers) throw new ApplicationException(string.Format("Версия файла WordDotx (Excel) не поддерживается нужна версия {0}", Vers));

                    // Логируем запуск модуля
                    Log.EventSave("Загрузка модуля построителя отчётов.", string.Format("{0}", this.GetType().Name), EventEn.Message);

                    // Проверяем наличие папочек
                    string Sorce = string.Format(@"{0}\{1}", Environment.CurrentDirectory, Config.WordDotxSource);
                    string Target = string.Format(@"{0}\{1}", Environment.CurrentDirectory, Config.WordDotxTarget);
                    if (!Directory.Exists(Sorce)) Directory.CreateDirectory(Sorce);
                    if (!Directory.Exists(Target)) Directory.CreateDirectory(Target);

                    // Подписываемся на события в пуле
                    Wrd.FarmWordDotx.PoolWorkerList.onEvTaskWordStart += PoolWorkerList_onEvTaskWordStart;
                    Wrd.FarmWordDotx.PoolWorkerList.onEvTaskWordEnd += PoolWorkerList_onEvTaskWordEnd;
                    Wrd.FarmWordDotx.PoolWorkerList.onEvTaskWordError += PoolWorkerList_onEvTaskWordError;
                    //
                    Wrd.FarmExcel.PoolWorkerList.onEvTaskExcelStart += PoolWorkerList_onEvTaskExcelStart;
                    Wrd.FarmExcel.PoolWorkerList.onEvTaskExcelEnd += PoolWorkerList_onEvTaskExcelEnd;
                    Wrd.FarmExcel.PoolWorkerList.onEvTaskExcelError += PoolWorkerList_onEvTaskExcelError;

                    // Создаём сервер которы будет формировать отчёты даже если мы им пользоваться не будем он выставит нашему фарму пути по умолчанию
                    Wrd.FarmWordDotx.CreateWordDotxServer(Sorce, Target);
                    //
                    Wrd.FarmExcel.CreateExcelServer(Sorce, Target);

                    // Запускаем пул с несколькими потоками по усолчанию с количеством как физическое кол-во CPU
                    Wrd.FarmWordDotx.PoolWorkerList.Start();
                    //
                    Wrd.FarmExcel.PoolWorkerList.Start();

                    // Делаем класс статичным чтобынельзя было инициировать несколько раз
                    WordDotxFrm = this;

                    // Для тестирования отчёта по статистики
                    //this.CreateReportInf3("612235637000223014");
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
        /// Отчёт отображает статистику по формированию отчётов в пуле
        /// </summary>
        /// <returns>HTML код который представляет из себя статистику</returns>
        public static string AksRepStat()
        {
            try
            {

                // for (int i = 0; i < Wrd.FarmWordDotx.PoolWorkerList.Count; i++)
                //{
                //    Wrd.EnStatusWorkercs ddd = Wrd.FarmWordDotx.PoolWorkerList[i].StatusWorker;
                //}
                //foreach (Wrd.Worker item in Wrd.FarmWordDotx.PoolWorkerList)
                //{
                //    Wrd.EnStatusWorkercs ddd = item.StatusWorker;
                //}


                // Палитра
                Color ColorDefFond = ColorTranslator.FromHtml("#02014f");   // Цвет шрифта по умолчанию
                Color ColorDefBack = ColorTranslator.FromHtml("#bebdff");   // Цвет фона по умолчанию
                Color ColorDefBackBody = ColorTranslator.FromHtml("#fff");   // Цвет фона по умолчанию
                Color ColorErrFond = ColorTranslator.FromHtml("#91060f");   // Цвет шрифта при ошибке
                Color ColorErrBack = ColorTranslator.FromHtml("#fcc5c9");   // Цвет фона при ошибке
                Color ColorWarFond = ColorTranslator.FromHtml("#ffc803");   // Цвет шрифта при предупреждении
                Color ColorWarBack = ColorTranslator.FromHtml("#faf1cf");   // Цвет фона при предупреждении
                Color ColorSucFond = ColorTranslator.FromHtml("#017d16");   // Цвет шрифта при успехе
                Color ColorSucBack = ColorTranslator.FromHtml("#e3fce7");   // Цвет фона при успехе

                string rez = null;
                /*rez += string.Format(@"<script>");
                rez += string.Format(@" async function pushReport (design) {{");
                rez += string.Format(@"  let response = await fetch(");
                rez += string.Format(@"   ""http://{0}:{1}/AksRepStat"",",Web.Host, Web.Port);
                rez += string.Format(@"   {{");
                rez += string.Format(@"    method: 'POST',");
                rez += string.Format(@"    headers: {{");
                rez += string.Format(@"     'Content-Type': 'application/json, version=2',");
                rez += string.Format(@"     'Accept': 'application/json, version=2',");
                rez += string.Format(@"     'Accept-Encoding': 'gzip, deflate, br'");
                //rez += string.Format(@"     'Auth-Session': session.token");
                rez += string.Format(@"    }},");
                rez += string.Format(@"    body: JSON.stringify([{{");
                rez += string.Format(@"     valueString: design");
                rez += string.Format(@"    }}])");
                rez += string.Format(@"   }}");
                rez += string.Format(@"  );");
                rez += string.Format(@"  await response.arrayBuffer();");
                //rez += string.Format(@"  let result = await response.json();");
                //rez += string.Format(@"  if (result[""Message""])");
                //rez += string.Format(@"  {{");
                //rez += string.Format(@"  alert(result[""Message""]);");
                //rez += string.Format(@"  }}");
                rez += string.Format(@" }}");
                rez += string.Format(@"</script>");

                rez += string.Format(@"        <p onclick=""pushReport('ggg')"">Скачать файл</p>");
*/
                rez += string.Format(@"<table border=""1"" style=""width: 100%; text-align: center; color: {0}; border-collapse: collapse; border: 1px solid black; "">", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@" <caption style=""color: {0}; font-size: x-large; font-weight: bold"">Статистика модуля формирования отчётов на основе шаблонов Word</caption>", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@" <thead style=""color: {0}; background: {1}; font-size: large; border: 1px {0};"">", ColorTranslator.ToHtml(ColorDefFond), ColorTranslator.ToHtml(ColorDefBack));
                rez += string.Format(@"  <tr>");
                rez += string.Format(@"   <th>Всего заданий в ожидании Word - Excel</th>");
                rez += string.Format(@"   <th>Всего потоков в пуле Word - Excel</th>");
                rez += string.Format(@"   <th>Максимум потоков в пуле Word - Excel</th>");
                rez += string.Format(@"  </tr>");
                rez += string.Format(@" </thead>");
                rez += string.Format(@" <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@"  <tr>");
                rez += string.Format(@"   <td>{0}-{1}</td>", Wrd.FarmWordDotx.QueTaskWordCount, Wrd.FarmExcel.QueTaskExcelCount);
                rez += string.Format(@"   <td>{0}-{1}</td>", Wrd.FarmWordDotx.PoolWorkerList.Count, Wrd.FarmExcel.PoolWorkerList.Count);
                rez += string.Format(@"   <td>{0}-{1}</td>", Wrd.FarmWordDotx.PoolWorkerList.MaxCountThreadOfPull, Wrd.FarmExcel.PoolWorkerList.MaxCountThreadOfPull);
                rez += string.Format(@"  </tr>");
                rez += string.Format(@" <tbody>");
                rez += string.Format(@"</table>");

                rez += string.Format(@"</br>");

                rez += string.Format(@"<table border=""1"" style=""width: 100%; text-align: center; color: {0}; border-collapse: collapse; border: 1px solid black; "">", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@" <caption style=""color: {0}; font-size: x-large; font-weight: bold"">Детализация по заданиям за последние 10 часов.</caption>", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@" <thead style=""color: {0}; background: {1}; font-size: large; border: 1px {0};"">", ColorTranslator.ToHtml(ColorDefFond), ColorTranslator.ToHtml(ColorDefBack));
                rez += string.Format(@"  <tr>");
                rez += string.Format(@"   <th>Имя отчёта</th>");
                rez += string.Format(@"   <th>Статус</th>");
                rez += string.Format(@"   <th>Детализация</th>");
                rez += string.Format(@"  </tr>");
                rez += string.Format(@" </thead>");
                rez += string.Format(@" <tbody style=""border: 1px {0}; text-align: left;"">", ColorTranslator.ToHtml(ColorDefFond));
                // Получаем список заданий которые сейчас обрабатывает пул
                //List<Wrd.TaskWord> TskL = Wrd.FarmWordDotx.PoolWorkerList.GetTaskWordList();
                //Чистим  от старья
                RemoveOldTaskWordInCach();
                // Пробекаем по этому списку
                foreach (Wrd.TaskWord item in RezStatCachWord)
                {
                    // определяем стиль для строки чтобы подсветить нужным цветом наши задания
                    Color ColorCurFond = ColorDefFond;
                    Color ColorCurBack = ColorDefBack;
                    Color ColorCurBackBody = ColorDefBackBody;
                    string StyleForRol = "";
                    string StyleForRolBody = "";
                    switch (item.StatusTask)
                    {
                        case Wrd.EnStatusTask.Success:
                            ColorCurFond = ColorSucFond;
                            ColorCurBack = ColorSucBack;
                            ColorCurBackBody = ColorSucBack;
                            break;
                        case Wrd.EnStatusTask.WARNING:
                            ColorCurFond = ColorWarFond;
                            ColorCurBack = ColorWarBack;
                            ColorCurBackBody = ColorWarBack;
                            break;
                        case Wrd.EnStatusTask.ERROR:
                            ColorCurFond = ColorErrFond;
                            ColorCurBack = ColorErrBack;
                            ColorCurBackBody = ColorErrBack;
                            break;
                        default:
                            break;
                    }
                    StyleForRol =string.Format(@" style=""color: {0}; background: {1}; font-size: large; border: 1px {0};""", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBack));
                    StyleForRolBody = string.Format(@" style=""color: {0}; background: {1}; font-size: large; border: 1px {0};""", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody));

                    rez += string.Format(@"  <tr{0}>", " " + StyleForRolBody);
                    rez += string.Format(@"   <td>{0}</td>", Path.GetFileName(item.Target));
                    rez += string.Format(@"   <td style=""text-align: center;"">{0}</td>", item.StatusTask.ToString());
                    rez += string.Format(@"   <td>");
                   

                    // Если статус с ошибкой то отображаем суть ошибки
                    if (item.StatusTask == Wrd.EnStatusTask.ERROR)
                    {
                        string tmpErrMes="";
                        foreach (string itemErrMes in item.StatusMessage)
                        {
                            if (tmpErrMes != "") tmpErrMes += "</br>";
                            tmpErrMes += itemErrMes;
                        }
                        rez += string.Format(@"    {0}", tmpErrMes);
                    }
                    else
                    {
                        rez += string.Format(@"    <table border=""1"" style=""width: 100%; text-align: center; color: {0}; border-collapse: collapse; border: 1px solid black; "">", ColorTranslator.ToHtml(ColorCurFond));
                        rez += string.Format(@"     <caption style=""color: {0}; font-size: large; font-weight: bold"">Статус формирования документа</caption>", ColorTranslator.ToHtml(ColorCurFond));
                        rez += string.Format(@"     <thead style=""color: {0}; background: {1}; font-size: medium; border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBack));
                        rez += string.Format(@"      <tr>");
                        rez += string.Format(@"       <th>Таблица в документе</th>");
                        rez += string.Format(@"       <th>Обработано строк</th>");
                        rez += string.Format(@"       <th>Всего строк</th>");
                        rez += string.Format(@"      </tr>");
                        rez += string.Format(@"     </thead>");
                        rez += string.Format(@"     <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond));
                        for (int iAfRowRez = 0; iAfRowRez < item.RezTsk.TableInWordAffectedRowList.Count; iAfRowRez++)
                        {
                            rez += string.Format(@"      <tr style=""border: 1px {0}; {1}"">", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody));
                            rez += string.Format(@"       <td>{0}</td>", iAfRowRez+1);
                            rez += string.Format(@"       <td>{0}</td>", item.RezTsk.TableInWordAffectedRowList[iAfRowRez].AffectedRow);
                            rez += string.Format(@"       <td>{0}</td>", item.RezTsk.TableInWordAffectedRowList[iAfRowRez].Tbl.TableValue.Rows.Count);
                            rez += string.Format(@"      </tr>");
                        }
                        rez += string.Format(@"     <tbody>");
                        rez += string.Format(@"    </table>");
                    }

                    rez += string.Format(@"   </br>");

                    // Время старта окончания итд 
                    rez += string.Format(@"    <table border=""0"" style=""text-align: left; color: {0}; border-collapse: collapse;"">", ColorTranslator.ToHtml(ColorCurFond));
                    rez += string.Format(@"     <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond));
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Создание задания</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), item.CraeteDt.ToString());
                    rez += string.Format(@"       <td rowapan=""3"">");
                    if (item.StatusTask == Wrd.EnStatusTask.Success)
                    {
                        rez += string.Format(@"        <a href=""http://{0}:{1}/AksRepStat?sid={2}"" target=""_blank""><p onclick=""pushReport('{0}')"">Скачать файл</p></a>", Web.Host, Web.Port, item.Sid.ToString());
                    }
                    rez += string.Format(@"       </td>");
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Реальное время начала построения отчёта</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), (item.StartProcessing != null ? ((DateTime)item.StartProcessing).ToString() : ""));
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Реальное время кокнчания процесса построения отчёта</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), (item.EndProcessing != null ? ((DateTime)item.EndProcessing).ToString() : ""));
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"     <tbody>");
                    rez += string.Format(@"    </table>");

                    rez += string.Format(@"   </td>");
                    rez += string.Format(@"  </tr>");
                }

                //Чистим  от старья
                RemoveOldTaskWordInCach();
                // Пробекаем по этому списку
                foreach (Wrd.TaskExcel item in RezStatCachExcel)
                {
                    // определяем стиль для строки чтобы подсветить нужным цветом наши задания
                    Color ColorCurFond = ColorDefFond;
                    Color ColorCurBack = ColorDefBack;
                    Color ColorCurBackBody = ColorDefBackBody;
                    string StyleForRol = "";
                    string StyleForRolBody = "";
                    switch (item.StatusTask)
                    {
                        case Wrd.EnStatusTask.Success:
                            ColorCurFond = ColorSucFond;
                            ColorCurBack = ColorSucBack;
                            ColorCurBackBody = ColorSucBack;
                            break;
                        case Wrd.EnStatusTask.WARNING:
                            ColorCurFond = ColorWarFond;
                            ColorCurBack = ColorWarBack;
                            ColorCurBackBody = ColorWarBack;
                            break;
                        case Wrd.EnStatusTask.ERROR:
                            ColorCurFond = ColorErrFond;
                            ColorCurBack = ColorErrBack;
                            ColorCurBackBody = ColorErrBack;
                            break;
                        default:
                            break;
                    }
                    StyleForRol = string.Format(@" style=""color: {0}; background: {1}; font-size: large; border: 1px {0};""", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBack));
                    StyleForRolBody = string.Format(@" style=""color: {0}; background: {1}; font-size: large; border: 1px {0};""", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody));

                    rez += string.Format(@"  <tr{0}>", " " + StyleForRolBody);
                    rez += string.Format(@"   <td>{0}</td>", Path.GetFileName(item.Target));
                    rez += string.Format(@"   <td style=""text-align: center;"">{0}</td>", item.StatusTask.ToString());
                    rez += string.Format(@"   <td>");


                    // Если статус с ошибкой то отображаем суть ошибки
                    if (item.StatusTask == Wrd.EnStatusTask.ERROR)
                    {
                        string tmpErrMes = "";
                        foreach (string itemErrMes in item.StatusMessage)
                        {
                            if (tmpErrMes != "") tmpErrMes += "</br>";
                            tmpErrMes += itemErrMes;
                        }
                        rez += string.Format(@"    {0}", tmpErrMes);
                    }
                    else
                    {
                        rez += string.Format(@"    <table border=""1"" style=""width: 100%; text-align: center; color: {0}; border-collapse: collapse; border: 1px solid black; "">", ColorTranslator.ToHtml(ColorCurFond));
                        rez += string.Format(@"     <caption style=""color: {0}; font-size: large; font-weight: bold"">Статус формирования документа</caption>", ColorTranslator.ToHtml(ColorCurFond));
                        rez += string.Format(@"     <thead style=""color: {0}; background: {1}; font-size: medium; border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBack));
                        rez += string.Format(@"      <tr>");
                        rez += string.Format(@"       <th>Таблица в документе</th>");
                        rez += string.Format(@"       <th>Обработано строк</th>");
                        rez += string.Format(@"       <th>Всего строк</th>");
                        rez += string.Format(@"      </tr>");
                        rez += string.Format(@"     </thead>");
                        rez += string.Format(@"     <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond));
                        for (int iAfRowRez = 0; iAfRowRez < item.RezTsk.TableInExcelAffectedRowList.Count; iAfRowRez++)
                        {
                            rez += string.Format(@"      <tr style=""border: 1px {0}; {1}"">", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody));
                            rez += string.Format(@"       <td>{0}</td>", iAfRowRez + 1);
                            rez += string.Format(@"       <td>{0}</td>", item.RezTsk.TableInExcelAffectedRowList[iAfRowRez].AffectedRow);
                            rez += string.Format(@"       <td>{0}</td>", item.RezTsk.TableInExcelAffectedRowList[iAfRowRez].Tbl.TableValue.Rows.Count);
                            rez += string.Format(@"      </tr>");
                        }
                        rez += string.Format(@"     <tbody>");
                        rez += string.Format(@"    </table>");
                    }

                    rez += string.Format(@"   </br>");

                    // Время старта окончания итд 
                    rez += string.Format(@"    <table border=""0"" style=""text-align: left; color: {0}; border-collapse: collapse;"">", ColorTranslator.ToHtml(ColorCurFond));
                    rez += string.Format(@"     <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorCurFond));
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Создание задания</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), item.CraeteDt.ToString());
                    rez += string.Format(@"       <td rowapan=""3"">");
                    if (item.StatusTask == Wrd.EnStatusTask.Success)
                    {
                        rez += string.Format(@"        <a href=""http://{0}:{1}/AksRepStat?sid={2}"" target=""_blank""><p onclick=""pushReport('{0}')"">Скачать файл</p></a>", Web.Host, Web.Port, item.Sid.ToString());
                    }
                    rez += string.Format(@"       </td>");
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Реальное время начала построения отчёта</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), (item.StartProcessing != null ? ((DateTime)item.StartProcessing).ToString() : ""));
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"      <tr>");
                    rez += string.Format(@"       <td>Реальное время кокнчания процесса построения отчёта</td>");
                    rez += string.Format(@"       <td style=""border: 1px {0}; {1}"">{2}</td>", ColorTranslator.ToHtml(ColorCurFond), ColorTranslator.ToHtml(ColorCurBackBody), (item.EndProcessing != null ? ((DateTime)item.EndProcessing).ToString() : ""));
                    rez += string.Format(@"      </tr>");
                    rez += string.Format(@"     <tbody>");
                    rez += string.Format(@"    </table>");

                    rez += string.Format(@"   </td>");
                    rez += string.Format(@"  </tr>");
                }

                rez += string.Format(@" <tbody>");
                rez += string.Format(@"</table>");

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при получении статистики от модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.AksRepStat", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Oстановка потоков и завершение их работы
        /// </summary>
        public void Join()
        {
            try
            {
                // Если класс инициирован тогда есть что останавливать
                if (WordDotxFrm != null)
                {
                    // остановка потоков и завершение их работы
                    Wrd.FarmWordDotx.PoolWorkerList.Stop();
                    Wrd.FarmWordDotx.PoolWorkerList.Join();

                    Wrd.FarmExcel.PoolWorkerList.Stop();
                    Wrd.FarmExcel.PoolWorkerList.Join();
                }
            }
            catch(Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.Join", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Получение пути к файлу по его сиду
        /// </summary>
        /// <param name="sid">Сид документа по которому хотим получить путь к готовому файлу</param>
        /// <returns>Путь к готовому файлу</returns>
        public static string GetPathReport(string sid)
        {
            try
            {
                string rez=null;

                // Пробегаем по всем заданиям и вытаскиваем руть если нашли нужное задание
                foreach (Wrd.TaskWord item in RezStatCachWord)
                {
                    if(item.Sid.ToString()==sid)
                    {
                        if (item.Target.IndexOf('\\')>0) rez = item.Target;
                        else rez = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, Config.WordDotxTarget, item.Target);
                        break;
                    }
                }

                // Пробегаем по всем заданиям и вытаскиваем руть если нашли нужное задание
                foreach (Wrd.TaskExcel item in RezStatCachExcel)
                {
                    if (item.Sid.ToString() == sid)
                    {
                        if (item.Target.IndexOf('\\') > 0) rez = item.Target;
                        else rez = string.Format(@"{0}\{1}\{2}", Environment.CurrentDirectory, Config.WordDotxTarget, item.Target);
                        break;
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при попытке получить путь к готовому отчёту с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.GetPathReport", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /*
                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();
                
                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Шаблон.dotx", @"Результат.doc", BkmL, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskWordInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTask RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);
         */

        /// <summary>
        /// Прайслист
        /// </summary>
        public static string CreateReportPlWrd()
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"PriceList.xlsx");
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по формированию списка товаров уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select name As doc_num,
  date_format(created_datetime, '%d.%m.%Y') As create_dt,
  date_format(post_date, '%d.%m.%Y') As pos_dt
From rpsods.pi_sheet"));

                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();
                if (TblBkm.Rows.Count == 1)
                {
                    BkmL.Add(new Wrd.Bookmark("doc_num0", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("doc_num1", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("create_dt", TblBkm.Rows[0]["create_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt0", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt1", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt2", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                }

                // Создаём запрос для получения таблицы
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"with T As (Select J.sid, P.description1 As Aip, P.description2 As Name,
      P.attribute As Attr, P.item_size As Size,
      Convert(round(J.price,2),char) As price, 
      Convert(round(J.qty,2),char) As qty,
      Convert(round(coalesce(Q.scan_qty, 0.0),2),char) As scan_qty,
      Convert(round(coalesce(Q.price, 0.0),2),char) As scan_price,
      Convert(round(J.qty*J.price,2),char) As suminv 
    From rpsods.pi_sheet D
      inner join  rpsods.pi_start J On D.sid=J.sheet_sid and J.Active=1
      left join rpsods.pi_zone_qty Q On J.sid=Q.pi_start_sid
      inner join rpsods.invn_sbs_item  P On J.invn_sbs_item_sid =P.sid and P.Active=1)
Select Convert(sid - (Select Min(Sid) As Msid From T)+1,char) As np,
  Aip, Name, Attr, Size, price, qty, scan_qty, scan_price, suminv
From T
Order by sid"));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                // Создаём список итогов
                //Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"PriceList.xlsx", TargetFile, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportPlWrd", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Прайслист
        /// </summary>
        public static string CreateReportPl()
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"PriceList.xlsx");
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по формированию списка товаров уже сущестаует", TargetFile));
       
                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                
                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"PriceList.xlsx", TargetFile, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);

            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportPl", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }
        
        /// <summary>
        /// Формирование отчёта по ИНВ-19
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public static string CreateReportInf19Wrd(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-19 ({0}).doc", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select name As doc_num,
  date_format(created_datetime, '%d.%m.%Y') As create_dt,
  date_format(post_date, '%d.%m.%Y') As pos_dt
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();
                if (TblBkm.Rows.Count == 1)
                {
                    BkmL.Add(new Wrd.Bookmark("doc_num0", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("doc_num1", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("create_dt", TblBkm.Rows[0]["create_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt0", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt1", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt2", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                }

                // Создаём запрос для получения таблицы
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"with T As (Select J.sid, CONCAT(P.description1, ' ', P.description2, ' ', P.attribute, ' ', P.item_size) As nxvid,
      Convert(round(J.price,2),char) As price, 
      Convert(round(J.qty,2),char) As qty,
      Convert(round(coalesce(Q.scan_qty, 0.0),2),char) As scan_qty,
      Convert(round(coalesce(Q.price, 0.0),2),char) As scan_price,
      Convert(round(J.qty*J.price,2),char) As suminv 
    From rpsods.pi_sheet D
      inner join  rpsods.pi_start J On D.sid=J.sheet_sid and J.Active=1
      left join rpsods.pi_zone_qty Q On J.sid=Q.pi_start_sid
      inner join rpsods.invn_sbs_item  P On J.invn_sbs_item_sid =P.sid and P.Active=1
    Where D.sid ='{0}')
Select Convert(sid - (Select Min(Sid) As Msid From T)+1,char) As np,
  nxvid, price, qty, scan_qty, scan_price, suminv
From T
Order by sid", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Унифицированная форма ИНВ-19.dotx", TargetFile, BkmL, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskWordInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskWord RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf19", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-19
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public static string CreateReportInf19(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-19 ({0}).xlsx", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select sid
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("3|B4", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"Унифицированная форма ИНВ-19.xlsx", TargetFile, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);

                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf19", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-8
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public static string CreateReportInf8aWrd(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-8а ({0}).doc", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select name As doc_num,
  date_format(created_datetime, '%d.%m.%Y') As create_dt,
  date_format(post_date, '%d.%m.%Y') As pos_dt
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();
                if (TblBkm.Rows.Count == 1)
                {
                    BkmL.Add(new Wrd.Bookmark("doc_num0", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("doc_num1", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("create_dt", TblBkm.Rows[0]["create_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt0", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt1", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt2", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                }

                // Создаём запрос для получения таблицы
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"with T As (Select J.sid, CONCAT(P.description1, ' ', P.description2, ' ', P.attribute, ' ', P.item_size) As nxvid,
      Convert(round(J.price,2),char) As price, 
      Convert(round(J.qty,2),char) As qty,
      Convert(round(coalesce(Q.scan_qty, 0.0),2),char) As scan_qty,
      Convert(round(coalesce(Q.price, 0.0),2),char) As scan_price,
      Convert(round(J.qty*J.price,2),char) As suminv 
    From rpsods.pi_sheet D
      inner join  rpsods.pi_start J On D.sid=J.sheet_sid and J.Active=1
      left join rpsods.pi_zone_qty Q On J.sid=Q.pi_start_sid
      inner join rpsods.invn_sbs_item  P On J.invn_sbs_item_sid =P.sid and P.Active=1
    Where D.sid ='{0}')
Select Convert(sid - (Select Min(Sid) As Msid From T)+1,char) As np,
  nxvid, price, qty, scan_qty, scan_price, suminv
From T
Order by sid", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Унифицированная форма ИНВ-8a.dotx", TargetFile, BkmL, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskWordInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskWord RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf8", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-8
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public static string CreateReportInf8a(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-8а ({0}).xlsx", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select sid
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("3|B4", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"Унифицированная форма ИНВ-8а.xlsx", TargetFile, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);

                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf8", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-3
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации (610444449000210592)</param>
        public static string CreateReportInf3Wrd(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-3 ({0}).doc", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select name As doc_num,
  date_format(created_datetime, '%d.%m.%Y') As create_dt,
  date_format(post_date, '%d.%m.%Y') As pos_dt
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();
                if (TblBkm.Rows.Count==1)
                {
                    BkmL.Add(new Wrd.Bookmark("doc_num0", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("doc_num1", TblBkm.Rows[0]["doc_num"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("create_dt", TblBkm.Rows[0]["create_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt0", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt1", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("pos_dt2", TblBkm.Rows[0]["pos_dt"].ToString()), true);
                }

                // Создаём запрос для получения таблицы
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"with T As (Select J.sid, CONCAT(P.description1, ' ', P.description2, ' ', P.attribute, ' ', P.item_size) As nxvid,
      Convert(round(J.price,2),char) As price, 
      Convert(round(J.qty,2),char) As qty,
      Convert(round(coalesce(Q.scan_qty, 0.0),2),char) As scan_qty,
      Convert(round(coalesce(Q.price, 0.0),2),char) As scan_price,
      Convert(round(J.qty*J.price,2),char) As suminv 
    From rpsods.pi_sheet D
      inner join  rpsods.pi_start J On D.sid=J.sheet_sid and J.Active=1
      left join rpsods.pi_zone_qty Q On J.sid=Q.pi_start_sid
      inner join rpsods.invn_sbs_item  P On J.invn_sbs_item_sid =P.sid and P.Active=1
    Where D.sid ='{0}')
Select Convert(sid - (Select Min(Sid) As Msid From T)+1,char) As np,
  nxvid, price, qty, scan_qty, scan_price, suminv
From T
Order by sid", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Унифицированная форма ИНВ-3.dotx", TargetFile, BkmL, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskWordInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskWord RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf3", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-3
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации (610444449000210592)</param>
        public static string CreateReportInf3(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Унифицированная форма ИНВ-3 ({0}).xlsx", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select sid
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("3|B4", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"Унифицированная форма ИНВ-3.xlsx", TargetFile, TblL);
                
                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);
                
                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf3", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Формирование отчёта по ИНВ-3
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации (610444449000210592)</param>
        public static string CreateReportUdp(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"УПД ({0}).xlsx", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"With T AS (Select D.sid, D.doc_no, 
        Date_Format(D.created_datetime,'%d.%m.%Y') As dt, 
        A.address_3 As inn,
        '54ff8a8584631c0f68cef121eff8777217128bc1' As token
    From rpsods.document D
      inner join rpsods.customer C On D.bt_cuid = C.sid
      inner join rpsods.customer_address A On C.sid = A.cust_sid
    Where D.sid = '{0}'
    )
Select sid From T
union 
Select doc_no From T
union 
Select dt From T
union 
Select inn From T
union 
Select token From T", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("2|B4", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskExcel Tsk = new Wrd.TaskExcel(@"УПД.xlsx", TargetFile, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskExcelInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskExcel RTsk = Wrd.FarmExcel.QueTaskExcelAdd(Tsk);

                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportUdp", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Формирование отчёта по ИНВ-3
        /// </summary>
        /// <param name="DocSid">Сид документа инвентаризации (610444449000210592)</param>
        public static string CreateReportReturnBlankWrd(string DocSid)
        {
            try
            {
                // Строим имя файла в которое заливать будем отчёт и проверяем есть такое задание уже в работе или нет
                string TargetFile = string.Format(@"Заявления на возврат ({0}).doc", DocSid);
                if (HashFileProcessing(TargetFile)) throw new ApplicationException(string.Format("Такое задание по этому документу {0} уже сущестаует", TargetFile));

                // Создаём запрос для получения списка закладок
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"select date_format(d.post_date, '%d') as fd,
    case when date_format(d.post_date, '%m') = '01' then 'января'
		when date_format(d.post_date, '%m') = '02' then 'февраля'
        when date_format(d.post_date, '%m') = '03' then 'марта'
        when date_format(d.post_date, '%m') = '04' then 'апреля'
        when date_format(d.post_date, '%m') = '05' then 'мая'
        when date_format(d.post_date, '%m') = '06' then 'июня'
        when date_format(d.post_date, '%m') = '07' then 'июля'
        when date_format(d.post_date, '%m') = '08' then 'августа'
        when date_format(d.post_date, '%m') = '09' then 'сентября'
        when date_format(d.post_date, '%m') = '10' then 'октября'
        when date_format(d.post_date, '%m') = '11' then 'ноября'
        else 'декабря' end as fm, 
	substr(date_format(d.post_date, '%Y'),1,3) as fy, 
    substr(date_format(d.post_date, '%Y'),4,1)  as y,
    coalesce(convert(d.{1}, char),'--') as fr_no, 
    date_format(d.post_date,'%d.%m.%Y') fr_data, 
    convert(round(Sum(t.given+t.taken),2), char) As fr_summa, 
    convert(round(Sum(case when t.tender_type={2} then t.given+t.taken else 0 end),2), char) As fr_nal, 
    convert(round(Sum(case when t.tender_type<>{2} then t.given+t.taken else 0 end),2), char) fr_bnal,
    date_format(sysdate(),'%d.%m.%Y') As curdata
From `rpsods`.`document` d
	inner join `rpsods`.`tender` t on d.sid = t.doc_sid
where d.`sid` = {0}
group by d.`sid`, d.post_date",  DocSid, Config.FieldDocNum.ToString(), Config.TenderTypeCash));

                if (TblBkm == null) throw new ApplicationException(string.Format("Документ  с сидом {0} не найден.", DocSid));

                // Создаём список закладок
                Wrd.BookmarkList BkmL = new Wrd.BookmarkList();
                if (TblBkm.Rows.Count == 1)
                {
                    BkmL.Add(new Wrd.Bookmark("fd", TblBkm.Rows[0]["fd"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fm", TblBkm.Rows[0]["fm"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fy", TblBkm.Rows[0]["fy"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("y", TblBkm.Rows[0]["y"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fr_no", TblBkm.Rows[0]["fr_no"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fr_data", TblBkm.Rows[0]["fr_data"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fr_summa", TblBkm.Rows[0]["fr_summa"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fr_nal", TblBkm.Rows[0]["fr_nal"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("fr_bnal", TblBkm.Rows[0]["fr_bnal"].ToString()), true);
                    BkmL.Add(new Wrd.Bookmark("curdata", TblBkm.Rows[0]["curdata"].ToString()), true);
                }

                // Создаём запрос для получения таблицы
                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"select p.description2 as C0, p.attribute as C1, p.item_size as C2,
	case when row_number() over() = count(*) over() Then'' else ',' end As C3
From `rpsods`.`document` d
	inner join `rpsods`.`document_item` i on d.sid=i.doc_sid
    inner join `rpsods`.invn_sbs_item p on i.invn_sbs_item_sid=p.sid
where d.`sid` = {0}
order by i.item_pos", DocSid));

                // Создаём список таблиц
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                // Создаём список итогов
                Wrd.TotalList Ttl = new Wrd.TotalList();

                // Создаём задание и получаем объект которым будем смотреть результат
                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Заявления на возврат.docx", TargetFile, BkmL, TblL);

                // Добавляем в кешь чтобы потом следить за отчётом
                AddTaskWordInCach(Tsk);

                // передаём в очередь наше задание
                Wrd.RezultTaskWord RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


                return string.Format("Создание отчёта запущено. Отчёт будет создан с именем {0}", TargetFile);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при формировании отчёта с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.CreateReportInf3", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        #region Private method

        // Произошла ошибка при отрисовки отчёта
        private void PoolWorkerList_onEvTaskWordError(object sender, Wrd.EvTaskWordError e)
        {
            try
            {
                LastErroServerWord = e;
                Log.EventSave(string.Format("Произошла ошибка при отрисовки отчёта {0}: ({1})", e.Tsk.Source, e.ErrorMessage), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Error);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        // Произошло успешная отрисовка отчёта
        private void PoolWorkerList_onEvTaskWordEnd(object sender, Wrd.EvTaskWordEnd e)
        {
            try
            {
                Log.EventSave(string.Format("Произошло успешная отрисовка отчёта {0}", e.Tsk.Source), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Message);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        // Произошёл старт процесса формирования отчёта
        private void PoolWorkerList_onEvTaskWordStart(object sender, Wrd.EvTaskWordStart e)
        {
            try
            {
                Log.EventSave(string.Format("Произошёл старт процесса формирования отчёта {0}", e.Tsk.Source), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Message);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskWordStart", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        // Произошла ошибка при отрисовки отчёта
        private void PoolWorkerList_onEvTaskExcelError(object sender, Wrd.EvTaskExcelError e)
        {
            try
            {
                LastErroServerExcel = e;
                Log.EventSave(string.Format("Произошла ошибка при отрисовки отчёта {0}: ({1})", e.Tsk.Source, e.ErrorMessage), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Error);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskExcelEnd", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        // Произошло успешная отрисовка отчёта
        private void PoolWorkerList_onEvTaskExcelEnd(object sender, Wrd.EvTaskExcelEnd e)
        {
            try
            {
                Log.EventSave(string.Format("Произошло успешная отрисовка отчёта {0}", e.Tsk.Source), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Message);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskExcelEnd", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        // Произошёл старт процесса формирования отчёта
        private void PoolWorkerList_onEvTaskExcelStart(object sender, Wrd.EvTaskExcelStart e)
        {
            try
            {
                Log.EventSave(string.Format("Произошёл старт процесса формирования отчёта {0}", e.Tsk.Source), string.Format("{0}.PoolWorkerList_onEvTaskWordEnd", this.GetType().Name), EventEn.Message);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске модуля отрисовки отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.PoolWorkerList_onEvTaskExcelStart", this.GetType().Name), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Проверяем есть ли задание с таким же файлом или нет
        /// </summary>
        /// <param name="Filename">Имя файла которое надо проверить</param>
        /// <returns>true если файл уже создаётся</returns>
        private static bool HashFileProcessing(string Filename)
        {
            try
            {
                bool rez = false;

                // Пробегаем по списку
                foreach (Wrd.TaskWord item in RezStatCachWord)
                {
                    // Если в таргете указан полный путь то вытаскиваем имя файла
                    string FName = item.Target;
                    if (FName.IndexOf(@"\") > 0) FName = Path.GetFileName(FName);

                    if (Filename == FName && item.StatusTask != Wrd.EnStatusTask.Success && item.StatusTask != Wrd.EnStatusTask.ERROR)
                    {
                        rez = true;
                        break;
                    }
                }

                // Пробегаем по списку
                foreach (Wrd.TaskExcel item in RezStatCachExcel)
                {
                    // Если в таргете указан полный путь то вытаскиваем имя файла
                    string FName = item.Target;
                    if (FName.IndexOf(@"\") > 0) FName = Path.GetFileName(FName);

                    if (Filename == FName && item.StatusTask != Wrd.EnStatusTask.Success && item.StatusTask != Wrd.EnStatusTask.ERROR)
                    {
                        rez = true;
                        break;
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при проверке в очереди объекта с именем {0} с ошибкой: {0}", Filename, ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.HashFileProcessing", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        // Добавление заданий в наш кешь для того чтобы потом отобразить их в статистике
        private static void AddTaskWordInCach(Wrd.TaskWord Tsk)
        {
            try
            {
                RemoveOldTaskWordInCach();

                if (Tsk!=null)
                {
                    RezStatCachWord.Add(Tsk);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при добавлении обхекта задания в кешь который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.AddTaskWordInCach", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        // Добавление заданий в наш кешь для того чтобы потом отобразить их в статистике
        private static void AddTaskExcelInCach(Wrd.TaskExcel Tsk)
        {
            try
            {
                RemoveOldTaskExcelInCach();

                if (Tsk != null)
                {
                    RezStatCachExcel.Add(Tsk);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при добавлении обхекта задания в кешь который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.AddTaskExcelInCach", "ReportExcelFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Чистка от заданий которые уже давно выполнились и нам не интересны
        /// </summary>
        private static void RemoveOldTaskWordInCach()
        {
            try
            {
                for (int i = RezStatCachWord.Count-1; i >= 0; i--)
                {
                    if (RezStatCachWord[i].CraeteDt.AddHours(10) < DateTime.Now) RezStatCachWord.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чистки старых заданий из кеша который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.RemoveOldTaskWordInCach", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Чистка от заданий которые уже давно выполнились и нам не интересны
        /// </summary>
        private static void RemoveOldTaskExcelInCach()
        {
            try
            {
                for (int i = RezStatCachExcel.Count - 1; i >= 0; i--)
                {
                    if (RezStatCachExcel[i].CraeteDt.AddHours(10) < DateTime.Now) RezStatCachExcel.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чистки старых заданий из кеша который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.RemoveOldTaskExcelInCach", "ReportExcelFarm"), EventEn.Error);
                throw ae;
            }
        }

        #endregion
    }
}
