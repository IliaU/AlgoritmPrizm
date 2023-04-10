using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using AlgoritmPrizm.Lib;

// file:///B:/Users/Admin/Downloads/Дисплей%20покупателя%20АТОЛ%20PD-2800%20РЭ.pdf

namespace AlgoritmPrizm.Com.DisplayPlg
{
    /// <summary>
    /// Дисплей Атол PD2800
    /// </summary>
    public sealed class DisplayPD2800 : Display
    {
        #region Внутренние параметры и классы

        /// <summary>
        /// Внутренний класс
        /// </summary>
        private static SerialPort _serialPort;
        
        /// <summary>
        /// Тип Epson
        /// </summary>
        private static readonly byte[] setTypCommand = new byte[] { 0x1F, 0x01 };

        /// <summary>
        /// Отчистка дисплея
        /// </summary>
        private static readonly byte[] setClear = new byte[] { 0x0C };

        /// <summary>
        /// Инициировать отображение
        /// </summary>
        private static readonly byte[] initShow = new byte[] { 0x1B, 0x40 };

        /// <summary>
        /// Установка курсора в начало
        /// </summary>
        private static readonly byte[] setCursorHome = new byte[] { 0x1F, 0x24, 0x01, 0x01 };

        /// <summary>
        /// Диагностика
        /// </summary>
        private static readonly byte[] setDiag = new byte[] { 0x1F, 0x01, 0x0C, 0x1F, 0x24, 0x01, 0x01, 0x20, 0x20, 0x43, 0x4F, 0x4D, 0x50, 0x4F, 0x52, 0x54, 0x20, 0x34, 0x1F, 0x24, 0x01, 0x02, 0x20, 0x42, 0x41, 0x55, 0x44, 0x52, 0x41, 0x54, 0x45, 0x20, 0x20, 0x39 };

        #endregion


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Port">Ком порт</param>
        /// <param name="BaudRate">Скорость</param>
        /// <param name="Parity">Parity</param>
        /// <param name="DataBits">DataBits</param>
        /// <param name="StpBits">StpBits</param>
        public DisplayPD2800(int Port, int BaudRate, Parity Parity, int DataBits, StopBits StpBits) : base(Port, BaudRate, Parity, DataBits, StpBits)
        {
            try
            {
                _serialPort = new SerialPort(string.Format("COM{0}", base.Port), 19200/*base.BaudRate*/, base.Parity, base.DataBits, base.StpBits);
                _serialPort.Open();
                _serialPort.DiscardOutBuffer();
                _serialPort.Write(setTypCommand, 0, setTypCommand.Length);
                _serialPort.Write(initShow, 0, initShow.Length);
                _serialPort.Write(setDiag, 0, setDiag.Length);
                _serialPort.Close();
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
                //_serialPort = new SerialPort(string.Format("COM{0}", base.Port), base.BaudRate, base.Parity, base.DataBits, base.StpBits);
                if (_serialPort == null) throw new ApplicationException("Объект не создан.");

                /*  // Cписок кодировок
                string str = "";
                foreach (EncodingInfo item in Encoding.GetEncodings())
                {
                    str += item.DisplayName + " (" + item.CodePage + ")";
                    str += "\r\n";
                }
                */

                // Перекодируем в 866 кодировку
                byte[] buf = Encoding.Convert(Encoding.Default, Encoding.GetEncoding(866), Encoding.Default.GetBytes(Text));

                // Отправляем на дисплей
                _serialPort.Open();
                _serialPort.DiscardOutBuffer();              
                _serialPort.Write(setClear, 0, setClear.Length);
                _serialPort.Write(setCursorHome, 0, setCursorHome.Length);
                _serialPort.Write(buf, 0, buf.Length);
                _serialPort.Close();
            }
            catch (Exception ex)
            {
                ApplicationException ae = new ApplicationException(string.Format("Упали с ошибкой: {0}", ex.Message));
                Log.EventSave(ae.Message, "Com.DisplayPlg.DisplayPD2800", EventEn.Error);
                throw ae;
            }
        }
    }
}
