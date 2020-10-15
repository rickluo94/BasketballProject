using System;
using System.IO.Ports;
using System.Threading;

namespace RfidModel
{
    public class RFID
    {
        private SerialPort _serialPort;
        private int _timeOut, _timeOutDefault;
        private AutoResetEvent _receiveNow;
        public void SetDevicePort(string portName, int baudRate, int timeOut)
        {
            try
            {
                _timeOut = timeOut;
                _timeOutDefault = timeOut;
                _serialPort = new SerialPort(portName, baudRate);
                _serialPort.Parity = Parity.None;
                _serialPort.Handshake = Handshake.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.RtsEnable = true;
                _serialPort.DtrEnable = true;
                _serialPort.WriteTimeout = _timeOut;
                _serialPort.ReadTimeout = _timeOut;
            }
            catch (Exception ex)
            {

            }
        }

        public bool Open()
        {
            try
            {
                if (_serialPort != null && !_serialPort.IsOpen)
                {
                    _receiveNow = new System.Threading.AutoResetEvent(false);
                    _serialPort.Open();
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPort_DataReceived);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (e.EventType == SerialData.Chars)
                {
                    _receiveNow.Set();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Close()
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
