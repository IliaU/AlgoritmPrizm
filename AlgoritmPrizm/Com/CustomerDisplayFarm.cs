using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace AlgoritmPrizm.Com
{
    /// <summary>
    /// Работа с дисплеем
    /// </summary>
    public static class CustomerDisplayFarm
    {
        /// <summary>
        /// Текущий дисплей
        /// </summary>
        public static CustomerDisplay CurDisplay = new Display.DisplayDSP840(6, 19200, 0, 8, StopBits.One);
    }
}
