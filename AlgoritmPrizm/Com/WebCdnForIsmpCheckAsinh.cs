using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Threading;

using AlgoritmPrizm.Lib;
using AlgoritmPrizm.BLL;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Проверка матрикс кода на площадке СДН
    /// </summary>
    public class WebCdnForIsmpCheckAsinh
    {
        string MatrixCode;
        public JsonCdnForIsmpResponce rez = null;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="MatrixCode">Матрикс код который нужно проверить</param>
        public WebCdnForIsmpCheckAsinh(string MatrixCode)
        {
            try
            {
                this.MatrixCode = MatrixCode;

                // Асинхронный запуск процесса
                Thread Thr = new Thread(ACdnForIsmpCheck); //Запуск с параметрами   
                Thr.Name = "ACdnForIsmpCheck";
                Thr.IsBackground = true;
                Thr.Start();

                // Проверяем наличие ответа в течении заданного количества времении в конфиге
                int waittime = Config.CdnRequestTimeout;
                while (waittime>0)
                {
                    waittime = waittime - 100;

                    if (rez != null) break;
                }

                
                if (rez == null)
                {
                    // если ответа нет то рубим процесс и не передаём ошибку пользователю
                    try
                    {
                        Thr.Abort();
                    }
                    catch (Exception) {  }
                    throw new ApplicationException("Не дождались ответа а течении заданнго времени");
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.WebCdnForIsmpCheckAsinh", GetType().Name), EventEn.Error);
                throw ae;
            }
        }


        /// <summary>
        /// Проверка матрикс кода на площадке СДН
        /// </summary>
        private void ACdnForIsmpCheck()
        {
            try
            {
                this.rez = Web.CdnForIsmpCheck(this.MatrixCode);
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при запуске метода с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, string.Format("{0}.ACdnForIsmpCheck", this.GetType().Name), EventEn.Error);
                //throw ae;
            }
        }

    }
}
