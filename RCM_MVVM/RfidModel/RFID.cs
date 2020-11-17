using System;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

#region RFID API
using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using GDotnet.Reader.Api.Utils;
#endregion

namespace RfidModel
{
    public class RFID
    {
        bool connected = false;
        private GClient clientConn;
        private static eConnectionAttemptEventStatusType status;
        private static object waitReadSingle = new object();
        private static EncapedLogBaseEpcInfo waitTag = null;

        public enum RSSI
        {
            A1 = 17,
            A2 = 17,
            A3 = 17,
            A4 = 17,
            A5 = 17,
            A6 = 17,
            A7 = 17
        }

        public enum TargetReader
        {
            A1 = 1,
            A2 = 2,
            A3 = 3,
            A4 = 4,
            A5 = 5,
            A6 = 6,
            A7 = 7,
            All = 0
        }

        #region RFID連線相關
        public bool Connect()
        {
            clientConn = new GClient();
            if (clientConn.OpenSerial("COM6:115200", 1000, out status))
            {
                MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower();
                msgBaseSetPower.DicPower = new Dictionary<byte, byte>()
                {
                    {1, (int)RSSI.A1},
                    {2, (int)RSSI.A2},
                    {3, (int)RSSI.A3},
                    {4, (int)RSSI.A4},
                    {5, (int)RSSI.A5},
                    {6, (int)RSSI.A6},
                    {7, (int)RSSI.A7}
                };
                clientConn.SendSynMsg(msgBaseSetPower);
                if (0 == msgBaseSetPower.RtCode)
                { }
                else { }
                connected = true;
                return true;
            }
            else
            {
                connected = false;
                return false;
            }
        }
        public void Disconnect()
        {
            clientConn.Close();
        }
        #endregion

        #region RFID掃描&&取值
        /// <summary>
        /// Item1.bool Item2.Epc Item3.Tid
        /// </summary>
        /// <param name="antEnable"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> scanning(string TargetReader, string MatchEpc)
        {
            TargetReader targetReader = (TargetReader)Enum.Parse(typeof(TargetReader), TargetReader, true);

            if (connected == true)
            {
                // subscribe to event
                clientConn.OnEncapedTagEpcLog += new delegateEncapedTagEpcLog(OnEncapedTagEpcLog);
                clientConn.OnEncapedTagEpcOver += new delegateEncapedTagEpcOver(OnEncapedTagEpcOver);

                // 2 antenna read Inventory, EPC & TID
                EncapedLogBaseEpcInfo tag = ReadSingleTag(clientConn, (int)targetReader, 100, MatchEpc);
                if (null != tag)
                {
                    tag.logBaseEpcInfo.ToString();
                    return Tuple.Create(true, tag.logBaseEpcInfo.Epc, tag.logBaseEpcInfo.Tid);
                }
                else
                {
                    return Tuple.Create(false, "", "");
                }
                // do sth.
            }
            else
            {
                return Tuple.Create(false, "", "");
            }
        }


        public EncapedLogBaseEpcInfo ReadSingleTag(GClient clientConn, int TargetReader, int timeout, string MatchEpc)
        {
            uint antEnable = (uint)(eAntennaNo._1 | eAntennaNo._7 | eAntennaNo._6 | eAntennaNo._5 | eAntennaNo._4 | eAntennaNo._3 | eAntennaNo._2);
            switch (TargetReader)
            {
                case 1:
                    antEnable = (uint)(eAntennaNo._1);
                    break;
                case 2:
                    antEnable = (uint)(eAntennaNo._2);
                    break;
                case 3:
                    antEnable = (uint)(eAntennaNo._3);
                    break;
                case 4:
                    antEnable = (uint)(eAntennaNo._4);
                    break;
                case 5:
                    antEnable = (uint)(eAntennaNo._5);
                    break;
                case 6:
                    antEnable = (uint)(eAntennaNo._6);
                    break;
                case 7:
                    antEnable = (uint)(eAntennaNo._7);
                    break;
                case 0:
                    antEnable = (uint)(eAntennaNo._1 | eAntennaNo._7 | eAntennaNo._6 | eAntennaNo._5 | eAntennaNo._4 | eAntennaNo._3 | eAntennaNo._2);
                    break;
            }
            waitTag = null;
            MsgBaseStop msgBaseStop = new MsgBaseStop();
            clientConn.SendUnsynMsg(msgBaseStop);
            MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();
            msgBaseInventoryEpc.AntennaEnable = antEnable;
            msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Single;

            if (!String.IsNullOrWhiteSpace(MatchEpc))
            {
                ///* match epc */
                msgBaseInventoryEpc.Filter = new ParamEpcFilter();
                msgBaseInventoryEpc.Filter.Area = (byte)eParamFilterArea.EPC;
                msgBaseInventoryEpc.Filter.BitStart = 32;
                msgBaseInventoryEpc.Filter.HexData = MatchEpc;
                msgBaseInventoryEpc.Filter.BData = Util.ConvertHexStringToByteArray(msgBaseInventoryEpc.Filter.HexData);
                msgBaseInventoryEpc.Filter.BitLength = (byte)(msgBaseInventoryEpc.Filter.BData.Length * 8);
            }
            else
            {
                ///* search epc tid/
                msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // tid参数
                msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
                msgBaseInventoryEpc.ReadTid.Len = 6;
            }
            clientConn.SendUnsynMsg(msgBaseInventoryEpc);
            try
            {
                lock (waitReadSingle)
                {
                    if (null == waitTag)
                    {
                        Monitor.Wait(waitReadSingle, timeout);
                    }
                }
            }
            catch { }
            msgBaseStop = new MsgBaseStop();
            clientConn.SendUnsynMsg(msgBaseStop);

            return waitTag;
        }
        #endregion

        #region RFID EVENT
        public void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // Any blocking inside the callback will affect the normal use of the API !
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                waitTag = msg;
                try
                {
                    lock (waitReadSingle)
                    {
                        Monitor.Pulse(waitReadSingle);
                    }
                }
                catch { }
            }
        }

        public void OnEncapedTagEpcOver(EncapedLogBaseEpcOver msg)
        {
            if (null != msg)
            {

            }
        }
        #endregion

        #region 掃描&&取值
        /// <summary>
        /// Item1.bool Item2.Epc Item3.Tid
        /// </summary>
        /// <param name="TargetReader"></param>
        /// <param name="MatchEpc"></param>
        /// <returns></returns>
        public Tuple<bool, string, string> ScannAndRead(string TargetReader, string MatchEpc)
        {
            try
            {
                Task.Delay(500).Wait();
                var tag = scanning(TargetReader, MatchEpc);
                Task.Delay(500).Wait();
                if (tag.Item1 == true)
                {
                    return Tuple.Create(true, tag.Item2, tag.Item3);
                }
                else
                {
                    return Tuple.Create(false, "", "");
                }
            }
            catch (Exception ex)
            {
                return Tuple.Create(false, "", "");
            }
        }
        #endregion
    }
}
