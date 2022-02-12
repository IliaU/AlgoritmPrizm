using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace AlgoritmPrizm.Com
{
    public class Display:DisplayPlg.Lib.CustomerDisplayBase, DisplayPlg.Lib.CustomerDisplayI
    {

        
        public Display(int Port, int BaudRate, Parity Parity, int DataBits, StopBits StpBits):base(Port, BaudRate, Parity, DataBits, StpBits)
        {
          
        }

        protected virtual void ShowTextStart(string Text)
        {
            //throw new ApplicationException("Не реализован в прлагине метод ShowText");
        }

        /// <summary>
        /// Печать строки
        /// </summary>
        /// <param name="Text"></param>
        public void ShowText(string Text)
        {
            ShowTextStart(Text);
        }
    }
}