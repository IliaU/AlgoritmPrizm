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
        public static Wrd.EvTaskWordError LastErroServer;

        /// <summary>
        /// При добавлении задания туда складываем результат чтобы потом за ним следить и чтобы потом оценивать что в каком статусе
        /// </summary>
        public static List<Wrd.TaskWord> RezStatCach = new List<Wrd.TaskWord>();

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
                    if (Wrd.FarmWordDotx.VersionDll[0] != Vers) throw new ApplicationException(string.Format("Версия файла WordDotx не поддерживается нужна версия {0}", Vers));

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

                    // Создаём сервер которы будет формировать отчёты даже если мы им пользоваться не будем он выставит нашему фарму пути по умолчанию
                    Wrd.FarmWordDotx.CreateWordDotxServer(Sorce, Target);

                    // Запускаем пул с несколькими потоками по усолчанию с количеством как физическое кол-во CPU
                    Wrd.FarmWordDotx.PoolWorkerList.Start();

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
                rez += string.Format(@"   <th>Всего заданий в ожидании</th>");
                rez += string.Format(@"   <th>Всего потоков в пуле</th>");
                rez += string.Format(@"   <th>Максимум потоков в пуле</th>");
                rez += string.Format(@"  </tr>");
                rez += string.Format(@" </thead>");
                rez += string.Format(@" <tbody style=""border: 1px {0};"">", ColorTranslator.ToHtml(ColorDefFond));
                rez += string.Format(@"  <tr>");
                rez += string.Format(@"   <td>{0}</td>", Wrd.FarmWordDotx.QueTaskWordCount);
                rez += string.Format(@"   <td>{0}</td>", Wrd.FarmWordDotx.PoolWorkerList.Count);
                rez += string.Format(@"   <td>{0}</td>", Wrd.FarmWordDotx.PoolWorkerList.MaxCountThreadOfPull);
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
                foreach (Wrd.TaskWord item in RezStatCach)
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
                foreach (Wrd.TaskWord item in RezStatCach)
                {
                    if(item.Sid.ToString()==sid)
                    {
                        if (item.Target.IndexOf('\\')>0) rez = item.Target;
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
        public static void CreateReportPl()
        {
            try
            {
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
                Wrd.RezultTask RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


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
        public static string CreateReportInf8a(string DocSid)
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
                Wrd.RezultTask RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


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
        /// <param name="DocSid">Сид документа инвентаризации</param>
        public static string CreateReportInf3(string DocSid)
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
                Wrd.RezultTask RTsk = Wrd.FarmWordDotx.QueTaskWordAdd(Tsk);


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
                LastErroServer = e;
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
                foreach (Wrd.TaskWord item in RezStatCach)
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
                    RezStatCach.Add(Tsk);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при добавлении обхекта задания в кешь который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.AddTaskWordInCach", "ReportWordDotxFarm"), EventEn.Error);
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
                for (int i = RezStatCach.Count-1; i >= 0; i--)
                {
                    if (RezStatCach[i].CraeteDt.AddHours(10) < DateTime.Now) RezStatCach.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при чистки старых заданий из кеша который помогает следить за результатом отчётов с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.RemoveOldTaskWordInCach", "ReportWordDotxFarm"), EventEn.Error);
                throw ae;
            }
        }

        #endregion
    }
}
