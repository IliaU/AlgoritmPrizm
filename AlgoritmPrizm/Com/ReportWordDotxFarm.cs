using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wrd = WordDotx;
using AlgoritmPrizm.Lib;
using System.IO;
using System.Data;

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
                    /*
                    List<BLL.JsonWordDotxParams> sdl =BLL.JsonWordDotxParams.DeserializeJson(BLL.JsonWordDotxParams.SampleTest);
                    string dd = BLL.JsonWordDotxParams.SerializeJson(sdl);
                    */

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

                    this.CreateReportInf3("612235637000223014");
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
                DataTable TblBkm = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"Select name As doc_num,
  date_format(created_datetime, '%d.%m.%Y') As create_dt,
  date_format(post_date, '%d.%m.%Y') As pos_dt
From rpsods.pi_sheet
Where sid = '{0}'", DocSid));

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

                DataTable TblVal = Com.ProviderFarm.CurrentPrv.getData(string.Format(@"with T As (Select J.sid, CONCAT(P.description1, ' ', P.description2, ' ', P.attribute, ' ', P.item_size) As nxvid,
      Convert(round(J.price,2),char) As price, 
      Convert(round(J.qty,2),char) As qty,
      Convert(round(coalesce(Q.scan_qty, 0.0),2),char) As scan_qty,
      Convert(round(coalesce(Q.price, 0.0),2),char) As scan_price,
      Convert(round(J.qty*J.price,2),char) As suminv 
    From rpsods.pi_sheet D
      inner join  rpsods.pi_start J On D.sid=J.sheet_sid
      left join rpsods.pi_zone_qty Q On J.sid=Q.pi_start_sid
      inner join rpsods.invn_sbs_item  P On J.invn_sbs_item_sid =P.sid
    Where D.sid ='{0}')
Select Convert(sid - (Select Min(Sid) As Msid From T)+1,char) As np,
  nxvid, price, qty, scan_qty, scan_price, suminv
From T
Order by sid", DocSid));
                
                Wrd.TableList TblL = new Wrd.TableList();
                TblL.Add(new Wrd.Table("T1", TblVal), true);

                Wrd.TotalList Ttl = new Wrd.TotalList();

                Wrd.TaskWord Tsk = new Wrd.TaskWord(@"Унифицированная форма ИНВ-3.dotx", @"Унифицированная форма ИНВ-3.doc", BkmL, TblL);

                Wrd.FarmWordDotx.CurrentWordDotxServer.StartCreateReport(Tsk);
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
