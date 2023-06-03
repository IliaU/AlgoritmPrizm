using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using AlgoritmListener.Lib;
using AlgoritmListener.Com;

namespace AlgoritmListener.BLL
{
    /// <summary>
    /// Для работы с инструментами призма и всего что с ним сявзвно
    /// </summary>
    public static class PrizmListener
    {
        // Объект для работы с веб мордой и прослушивания всего что нужно для призма
        public static HttpClient IoIodeWebClientIsv = new HttpClient()
        {
            BaseAddress = new Uri("http://isv:5000"),
        };
        
        /// <summary>
        /// Запкскаем проверку
        /// </summary>
        public static async void Verif()
        {
            try
            {
                // Флаг который говорит о необходимости запуска нашего процесса
                bool FlagAlgoritmPrizmExistProcess = false;
                try
                {
                    using (HttpResponseMessage resp = await IoIodeWebClientIsv.GetAsync("config"))
                    {
                        // Записывает сведения о запросе в консоль.
                        HttpResponseMessage respMes = resp.EnsureSuccessStatusCode();
                        if (respMes.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Log.EventSave("Бекенд для призма работает.", "AlgoritmListener", EventEn.Message);
                            FlagAlgoritmPrizmExistProcess = true;
                        }
                        else
                        {
                            Log.EventSave("Обнаружено что наш бекенд для призма не запущен.", "AlgoritmListener", EventEn.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.EventSave(string.Format("Обнаружено что наш бекенд для призма не запущен. Ошибка: {0}", ex.Message), "AlgoritmListener", EventEn.Error);
                }
                
                // Если обнаружили что нет процесса то нужно перезапустить наш екзешник
                if (!FlagAlgoritmPrizmExistProcess)
                {
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        Log.EventSave(string.Format("Не смогли перезапустить наш процесс. Ошибка: {0}", ex.Message), "AlgoritmListener", EventEn.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке конфигурации с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "PrizmListener.Verif", EventEn.Error);
                throw ae;
            }
        }


    }
}
