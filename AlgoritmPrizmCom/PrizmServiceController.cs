using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceProcess;
using System.Threading;

namespace AlgoritmPrizmCom
{
    // контроллер управлеения сервисом
    public class PrizmServiceController
    {
        /// <summary>
        /// имя сллужбы Енисей
        /// </summary>
        private static string ControllerNameYenisey = "yenisei";

        /// <summary>
        /// имя службы Режим
        /// </summary>
        private static string ControllerNameRegime = "regime";

        /// <summary>
        /// рестарт сервисов режима
        /// </summary>
        public static void ServiceControllerRegimeYeniseyRestart()
        {
            try
            {
                ServiceController CurServiceEnisey = GetServiceController(ControllerNameYenisey);
                ServiceController CurServiceRegime = GetServiceController(ControllerNameRegime);

                CurServiceRegime.Stop();
                Thread.Sleep(5000);
                CurServiceEnisey.Stop();
                Thread.Sleep(5000);
                CurServiceEnisey.Start();
                Thread.Sleep(15000);
                CurServiceRegime.Start();
                Thread.Sleep(10000);

                CurServiceRegime.Dispose();
                CurServiceEnisey.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Найти сервис по имени
        /// </summary>
        /// <param name="ServiceName">Имя сенрвиса для поиска</param>
        /// <returns>Найденный сервис</returns>
        private static ServiceController GetServiceController(string ServiceName)
        {
            try
            {
                ServiceController rez = null;

                ServiceController[] scServices;
                scServices = ServiceController.GetServices();

                foreach (ServiceController item in scServices)
                {
                    if (item.ServiceName == ServiceName)
                    {
                        rez = item;
                        break;
                    }
                }

                return rez;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
