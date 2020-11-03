using System;
using LattePanda.Firmata;
using System.Threading;

namespace IOModel
{
    public static class IO
    {
        public static Arduino _IO;
        public static readonly byte Lock = 1;
        public static readonly byte UnLock = 0;
        public static readonly int DoorOpen = 1;
        public static readonly int DoorLock = 0;

        #region DoorCheck signal
        public enum IN
        {
            A1 = 0,
            A2 = 1,
            A3 = 2,
            A4 = 3,
            A5 = 4,
            A6 = 0,
            A7 = 0,
            A8 = 6,
            Pump = 5
        }
        #endregion

        #region DoorLocker
        public enum Out
        {
            A1 = 7,
            A2 = 8,
            A3 = 9,
            A4 = 10,
            A5 = 11,
            A6 = 7,
            A7 = 7,
            A8 = 21,
            Pump = 22,
            Buzz = 23
        }
        #endregion

        public static bool SetDevicePort(string PortName,int BaudRate)
        {
            try
            {
                _IO = new Arduino(PortName, BaudRate);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool SetIOParameter()
        {
            try
            {
                #region digital INPUT
                foreach (int Value in Enum.GetValues(typeof(IN)))
                {
                    _IO.pinMode(Value, Arduino.INPUT);
                }
                #endregion

                #region digital UTPUT
                foreach (int Value in Enum.GetValues(typeof(Out)))
                {
                    _IO.pinMode(Value, Arduino.OUTPUT);
                }
                #endregion

                #region SET digital OUTPUT
                foreach (int Value in Enum.GetValues(typeof(Out)))
                {
                    _IO.digitalWrite(Value, Lock);
                }
                #endregion
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 必須符合規範內點位名稱
        /// </summary>
        /// <param name="PinName"></param>
        /// <returns></returns>
        public static int Read(string PinName)
        {
            IN iN = (IN) Enum.Parse(typeof(IN), PinName, true);
            try
            {
                return _IO.digitalRead((int)iN);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /// <summary>
        /// 必須符合規範內點位名稱
        /// </summary>
        /// <param name="PinName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Write(string PinName, byte Value)
        {
            Out _out = (Out) Enum.Parse(typeof(Out), PinName, true);
            try
            {
                _IO.digitalWrite((int)_out, Value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
