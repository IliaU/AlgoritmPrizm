using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace AlgoritmPrizm.Com.Display.Lib
{
    public abstract class CustomerDisplayBase
    {
        /// <summary>
        /// Ком порт
        /// </summary>
        public int Port { get; protected set; } = 6;

        /// <summary>
        /// Скорость
        /// </summary>
        public int BaudRate { get; protected set; } = 19200;

        public Parity Parity { get; protected set; } = Parity.None;

        public int DataBits { get; protected set; } = 8;

        public StopBits StpBits { get; protected set; } = StopBits.One;

        public CustomerDisplayBase(int Port, int BaudRate, Parity Parity, int DataBits, StopBits StpBits)
        {
            this.Port = Port;
            this.BaudRate = BaudRate;
            this.Parity = Parity;
            this.DataBits = DataBits;
            this.StpBits = StpBits;
        }
    }
}
