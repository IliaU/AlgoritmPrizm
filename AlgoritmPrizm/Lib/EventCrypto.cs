using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoritmPrizm.Lib
{
    public class EventCrypto : EventArgs
    {
        /// <summary>
        /// Текущий статус выполнения
        /// </summary>
        public bool HashExecuting { get; private set; }

        public EventCrypto(bool HashExecuting)
        {
            this.HashExecuting = HashExecuting;
        }
    }
}
