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
        enum IN
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
        enum Out
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
                _IO.pinMode((int)IN.A1, Arduino.INPUT);
                _IO.pinMode((int)IN.A2, Arduino.INPUT);
                _IO.pinMode((int)IN.A3, Arduino.INPUT);
                _IO.pinMode((int)IN.A4, Arduino.INPUT);
                _IO.pinMode((int)IN.A5, Arduino.INPUT);
                _IO.pinMode((int)IN.A6, Arduino.INPUT);
                _IO.pinMode((int)IN.A7, Arduino.INPUT);
                _IO.pinMode((int)IN.A8, Arduino.INPUT);
                _IO.pinMode((int)IN.Pump, Arduino.INPUT);
                #endregion

                #region digital OUTPUT
                _IO.pinMode((int)Out.A1, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A2, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A3, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A4, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A5, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A6, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A7, Arduino.OUTPUT);
                _IO.pinMode((int)Out.A8, Arduino.OUTPUT);
                _IO.pinMode((int)Out.Pump, Arduino.OUTPUT);
                _IO.pinMode((int)Out.Buzz, Arduino.OUTPUT);
                #endregion

                #region OUTPUT 預設關

                _IO.digitalWrite((int)Out.A1, Lock);
                _IO.digitalWrite((int)Out.A2, Lock);
                _IO.digitalWrite((int)Out.A3, Lock);
                _IO.digitalWrite((int)Out.A4, Lock);
                _IO.digitalWrite((int)Out.A5, Lock);
                _IO.digitalWrite((int)Out.A6, Lock);
                _IO.digitalWrite((int)Out.A7, Lock);
                _IO.digitalWrite((int)Out.A8, Lock);
                _IO.digitalWrite((int)Out.Pump, Lock);
                _IO.digitalWrite((int)Out.Buzz, Lock);
                #endregion
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static int Read(string PinName)
        {
            try
            {
                int result = -1;
                switch (PinName)
                {
                    case "A1":
                        result = _IO.digitalRead((int)IN.A1);
                        break;
                    case "A2":
                        result = _IO.digitalRead((int)IN.A2);
                        break;
                    case "A3":
                        result = _IO.digitalRead((int)IN.A3);
                        break;
                    case "A4":
                        result = _IO.digitalRead((int)IN.A4);
                        break;
                    case "A5":
                        result = _IO.digitalRead((int)IN.A5);
                        break;
                    case "A6":
                        result = _IO.digitalRead((int)IN.A6);
                        break;
                    case "A7":
                        result = _IO.digitalRead((int)IN.A7);
                        break;
                    case "A8":
                        result = _IO.digitalRead((int)IN.A8);
                        break;
                    case "Pump":
                        result = _IO.digitalRead((int)IN.Pump);
                        break;
                }
                return result;
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public static bool Write(string PinName, byte Value)
        {
            try
            {
                switch (PinName)
                {
                    case "A1":
                        _IO.digitalWrite((int)Out.A1, Value);
                        break;
                    case "A2":
                        _IO.digitalWrite((int)Out.A2, Value);
                        break;
                    case "A3":
                        _IO.digitalWrite((int)Out.A3, Value);
                        break;
                    case "A4":
                        _IO.digitalWrite((int)Out.A4, Value);
                        break;
                    case "A5":
                        _IO.digitalWrite((int)Out.A5, Value);
                        break;
                    case "A6":
                        _IO.digitalWrite((int)Out.A6, Value);
                        break;
                    case "A7":
                        _IO.digitalWrite((int)Out.A7, Value);
                        break;
                    case "A8":
                        _IO.digitalWrite((int)Out.A8, Value);
                        break;
                    case "Pump":
                        _IO.digitalWrite((int)Out.Pump, Value);
                        break;
                    case "Buzz":
                        _IO.digitalWrite((int)Out.Buzz, Value);
                        break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
