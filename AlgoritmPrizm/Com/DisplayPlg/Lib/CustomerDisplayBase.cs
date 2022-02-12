using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

namespace AlgoritmPrizm.Com.DisplayPlg.Lib
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

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Port">Ком порт</param>
        /// <param name="BaudRate">Скорость</param>
        /// <param name="Parity">Parity</param>
        /// <param name="DataBits">DataBits</param>
        /// <param name="StpBits">StpBits</param>
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
