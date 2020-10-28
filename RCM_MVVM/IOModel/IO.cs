using System;
using LattePanda.Firmata;
using System.Threading;

namespace IOModel
{
    public class IO
    {
        public const byte Lock = 1;
        public const byte Unlock = 0;

        #region DoorCheck signal
        public readonly int A1_IN = 0;
        public readonly int A2_IN = 1;
        public readonly int A3_IN = 2;
        public readonly int A4_IN = 3;
        public readonly int A5_IN = 4;
        public readonly int A6_IN = 0;
        public readonly int A7_IN = 0;
        public readonly int A8_IN = 6;
        #endregion

        #region DoorLocker
        public readonly int A1_OUT = 7;
        public readonly int A2_OUT = 8;
        public readonly int A3_OUT = 9;
        public readonly int A4_OUT = 10;
        public readonly int A5_OUT = 11;
        public readonly int A6_OUT = 7;
        public readonly int A7_OUT = 7;
        public readonly int A8_OUT = 21;
        #endregion

        #region pumpBtnIn
        public static readonly int pump_IN = 5;
        #endregion

        #region pump
        public readonly int pump_OUT = 22;
        #endregion

        #region buzz
        public readonly int buzz_OUT = 23;
        #endregion

        public Arduino _IO;

        public bool SetDevicePort(string PortName,int BaudRate)
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

        public bool SetIOParameter()
        {
            try
            {
                #region digital INPUT
                _IO.pinMode(A1_IN, Arduino.INPUT);
                _IO.pinMode(A2_IN, Arduino.INPUT);
                _IO.pinMode(A3_IN, Arduino.INPUT);
                _IO.pinMode(A4_IN, Arduino.INPUT);
                _IO.pinMode(A5_IN, Arduino.INPUT);
                _IO.pinMode(A6_IN, Arduino.INPUT);
                _IO.pinMode(A7_IN, Arduino.INPUT);
                _IO.pinMode(A8_IN, Arduino.INPUT);
                _IO.pinMode(pump_IN, Arduino.INPUT);
                #endregion

                #region digital OUTPUT
                _IO.pinMode(A1_OUT, Arduino.OUTPUT);
                _IO.pinMode(A2_OUT, Arduino.OUTPUT);
                _IO.pinMode(A3_OUT, Arduino.OUTPUT);
                _IO.pinMode(A4_OUT, Arduino.OUTPUT);
                _IO.pinMode(A5_OUT, Arduino.OUTPUT);
                _IO.pinMode(A6_OUT, Arduino.OUTPUT);
                _IO.pinMode(A7_OUT, Arduino.OUTPUT);
                _IO.pinMode(A8_OUT, Arduino.OUTPUT);
                _IO.pinMode(pump_OUT, Arduino.OUTPUT);
                _IO.pinMode(buzz_OUT, Arduino.OUTPUT);
                #endregion

                #region OUTPUT 預設關
                _IO.digitalWrite(A1_OUT, Lock);
                _IO.digitalWrite(A2_OUT, Lock);
                _IO.digitalWrite(A3_OUT, Lock);
                _IO.digitalWrite(A4_OUT, Lock);
                _IO.digitalWrite(A5_OUT, Lock);
                _IO.digitalWrite(A6_OUT, Lock);
                _IO.digitalWrite(A7_OUT, Lock);
                _IO.digitalWrite(A8_OUT, Lock);
                _IO.digitalWrite(pump_OUT, Lock);
                _IO.digitalWrite(buzz_OUT, Lock);
                #endregion
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public int Read(int Pin)
        {
            try
            {
                int result = _IO.digitalRead(Pin);
                return result;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public bool Write(int Pin, byte Value)
        {
            try
            {
                _IO.digitalWrite(Pin, Value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
