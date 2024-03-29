﻿using System;
using System.Threading.Tasks;
using LattePanda.Firmata;

namespace IOModel
{
    public static class IO
    {
        public static readonly bool IsTest = false;
        public static Arduino _IO;
        public static readonly byte Lock = 1;
        public static readonly byte UnLock = 0;
        public static readonly int DoorOpen = 1;
        public static readonly int DoorLock = 0;
        public static readonly int PumpON = 0;
        public static readonly int PumpOFF = 1;

        #region DoorCheck signal
        public enum IN
        {
            A1 = 0,
            A2 = 1,
            A3 = 2,
            A4 = 3,
            A5 = 4,
            A6 = 5,
            A7 = 6,
            A8 = 18,
            Pump = 19
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
            A6 = 12,
            A7 = 20,
            A8 = 21,
            Buzz= 22,
            Pump = 23
        }
        #endregion

        public static bool SetDevicePort(string PortName,int BaudRate)
        {
            try
            {
                if (IsTest == false)
                {
                    _IO = new Arduino(PortName, BaudRate);
                    return true;
                }
                else
                {
                    return false;
                }
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
                if (IsTest == false)
                {
                    #region digital INPUT
                    foreach (int Value in Enum.GetValues(typeof(IN)))
                    {
                        Task.Delay(500).Wait();
                        _IO.pinMode(Value, Arduino.INPUT);
                    }
                    #endregion

                    #region digital UTPUT
                    foreach (int Value in Enum.GetValues(typeof(Out)))
                    {
                        Task.Delay(500).Wait();
                        _IO.pinMode(Value, Arduino.OUTPUT);
                    }
                    #endregion

                    #region SET digital OUTPUT
                    foreach (int Value in Enum.GetValues(typeof(Out)))
                    {
                        Task.Delay(500).Wait();
                        _IO.digitalWrite(Value, Lock);
                    }
                    #endregion
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 必須符合<DoorCheck>規範內點位名稱
        /// </summary>
        /// <param name="PinName"></param>
        /// <returns></returns>
        public static int Read(string PinName)
        {
            try
            {
                if (IsTest ==false)
                {
                    IN iN = (IN)Enum.Parse(typeof(IN), PinName, true);
                    return _IO.digitalRead((int)iN);
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        /// <summary>
        /// 必須符合<DoorLocker>規範內點位名稱
        /// </summary>
        /// <param name="PinName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool Write(string PinName, byte Value)
        {
            try
            {
                if (IsTest == false)
                {
                    Out _out = (Out)Enum.Parse(typeof(Out), PinName, true);
                    _IO.digitalWrite((int)_out, Value);
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
