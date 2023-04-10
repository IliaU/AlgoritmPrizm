using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using AlgoritmPrizm.Lib;

namespace AlgoritmPrizm.Com.DisplayPlg
{
    /// <summary>
    /// DSP 840
    /// </summary>
    public sealed class  DisplayDSP840:Display
    {
        #region Внутренние параметры и классы

        /// <summary>
        /// Внутренний класс
        /// </summary>
        private static SerialPort _serialPort;

        /// <summary>
        /// Инициализация строки
        /// </summary>
        private static readonly byte[] setCursorBeginning = new byte[] { 0x04, 0x01, 0x50, 0x31, 0x17 };

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Port">Ком порт</param>
        /// <param name="BaudRate">Скорость</param>
        /// <param name="Parity">Parity</param>
        /// <param name="DataBits">DataBits</param>
        /// <param name="StpBits">StpBits</param>
        public DisplayDSP840(int Port, int BaudRate, Parity Parity, int DataBits, StopBits StpBits) : base(Port, BaudRate, Parity, DataBits, StpBits)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали при загрузке драйвера для работы с фискальным регистратором с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, GetType().Name, EventEn.Error);
                throw ae;
            }
        }

        /// <summary>
        /// Печать строки
        /// </summary>
        /// <param name="Text"></param>
        protected override void ShowTextStart(string Text)
        {
            try
            {
                _serialPort = new SerialPort(string.Format("COM{0}", base.Port), base.BaudRate, base.Parity, base.DataBits, base.StpBits);
                _serialPort.Open();
                _serialPort.DiscardOutBuffer();

                byte[] buf = Encoding.UTF8.GetBytes(Text);

                _serialPort.Write(setCursorBeginning, 0, 5);
                _serialPort.Write(buf, 0, buf.Length);

                _serialPort.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.DisplayPlg.DisplayDSP840", EventEn.Error);
                throw ae;
            }
        }
    }
}
