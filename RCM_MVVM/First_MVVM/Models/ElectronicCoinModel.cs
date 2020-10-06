using System;
using System.Text;
using System.IO.Ports;
using Newtonsoft.Json.Linq;
using STJ = System.Text.Json;
using System.Collections.Generic;
using System.Reflection;

namespace First_MVVM.Models
{
    public class ElectronicCoinModel
    {
        public static SerialPort SerialPort;
        public static Read_card_id_response read_Card_Id_Response = new Read_card_id_response();
        public static Read_card_id_loop_response read_Card_Id_Loop_Response = new Read_card_id_loop_response();
        public static Read_card_balance_response read_Card_Balance_Response = new Read_card_balance_response();
        public static Charge_response charge_Response = new Charge_response();

        #region Port 通訊參數設定
        public static bool InitCom()
        {
            SerialPort = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);//電子投幣機
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);//DataReceived事件委托
            SerialPort.ReceivedBytesThreshold = 1;
            SerialPort.RtsEnable = true;
            return OpenPort();
        }
        #endregion

        #region 打開連接埠
        public static bool OpenPort()
        {
            try
            {
                SerialPort.Open();
            }
            catch { }
            if (SerialPort.IsOpen)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 收到資料觸發事件判斷是否回傳指令成功(成功後執行相關動作)
        public static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string response = SerialPort.ReadExisting();
            if (IsJsonFormat(response) == true)
            {
                JObject obj = JObject.Parse(response);
                string _is_success = (string)obj["is_success"];
                string _action = (string)obj["action"];
                if (_is_success == "True")
                {
                    switch (_action)
                    {
                        case "ping":
                            
                            break;
                        case "read.card_id":
                            read_Card_Id_Response = STJ.JsonSerializer.Deserialize<Read_card_id_response>(response);
                            break;
                        case "read.card_id.loop":
                            read_Card_Id_Loop_Response = STJ.JsonSerializer.Deserialize<Read_card_id_loop_response>(response);
                            break;
                        case "read.card_balance":
                            read_Card_Balance_Response = STJ.JsonSerializer.Deserialize<Read_card_balance_response>(response);
                            break;
                        case "charge":
                            charge_Response = STJ.JsonSerializer.Deserialize<Charge_response>(response);
                            break;
                    }
                }
                else if (_is_success == "False")
                {
                    switch (_action)
                    {
                        case "ping":

                            break;
                        case "read.card_id":

                            break;
                        case "read.card_id.loop":

                            break;
                        case "read.card_balance":
                            
                            break;
                        case "charge":

                            break;
                    }
                }
            }
        }
        #endregion

        #region Read_card_id_request()讀取卡號
        public static void Read_card_id_request()
        {
            SerialPort.Write("{\"action\": \"read.card_id\",\"arg\": [],\"kwarg\": { }}");
        }
        public class Read_card_id_response
        {
            public string device_mac { get; set; }
            public int timestamp { get; set; }
            public string version { get; set; }
            public bool is_success { get; set; }
            public string action { get; set; }
            public read_card_result result { get; set; }
        }
        public class read_card_result
        {
            public string card_id { get; set; }
        }
        #endregion

        #region Read_card_id_loop_request()連續讀取卡號
        public void Read_card_id_loop_request()
        {
            SerialPort.Write("{\"action\": \"read.card_id.loop\",\"arg\": [],\"kwarg\": {\"search_timeout\": 300,\"search_view_text\":\"\\u6b61\\u8fce\\u5149\\u81e8\\uff0c\\u8acb\\u9760\\u5361\\uff0c\\u5361\\u7247\\u611f\\u61c9\\u4e2d\",\"success_view_text\": \"\\u8b80\\u5361\\u5b8c\\u6210\\u8acb\\u53d6\\u5361\", \"show_success_message\": false}}");
        }
        public class Read_card_id_loop_response
        {
            public string device_mac { get; set; }
            public int timestamp { get; set; }
            public string version { get; set; }
            public bool? is_success { get; set; }
            public string action { get; set; }
            public Read_card_id_loop_result result { get; set; }
        }
        public class Read_card_id_loop_result
        {
            public string card_id { get; set; }
            public string card_type { get; set; }
            public int search_timeout { get; set; }
        }
        #endregion

        #region Read_card_balance_request()讀取卡⽚詳細資訊
        public void Read_card_balance_request()
        {
            SerialPort.Write("{\"action\": \"read.card_balance\",\"arg\": [],\"kwarg\": { }}");
        }
        public class Read_card_balance_response
        {
            public string device_mac { get; set; }
            public int timestamp { get; set; }
            public string version { get; set; }
            public bool? is_success { get; set; }
            public string action { get; set; }
            public Read_card_balance_result result { get; set; }
            public string ticket_type { get; set; }
        }
        public class Read_card_balance_result
        {
            public string card_id { get; set; }
            public string card_purse_id { get; set; }
            public string invoice_vehicle { get; set; }
            public int balance { get; set; }
        }
        #endregion

        #region Charge_request(int amount)扣款
        public void Charge_request(int amount)
        {
            SerialPort.Write("{\"action\": \"charge\",\"arg\": [],\"kwarg\": {\"amount\": " + amount + "}}");
        }
        public class Charge_response
        {
            public string device_mac { get; set; }
            public int timestamp { get; set; }
            public string version { get; set; }
            public bool? is_success { get; set; }
            public string action { get; set; }
            public Charge_result result { get; set; }
        }
        public class Charge_result
        {
            public string card_id { get; set; }
            public string card_type { get; set; }
            public string ticket_type { get; set; }
            public string card_purse_id { get; set; }
            public string tran_card_id { get; set; }
            public string invoice_vehicle { get; set; }
            public int balance { get; set; }
            public int before_tran_balance { get; set; }
            public int auto_prepaid_amount { get; set; }
            public int amount { get; set; }
            public string ecc_device_no { get; set; }
            public string batch_number { get; set; }
            public string rrn { get; set; }
            public string cpu_pruse_ver_num { get; set; }
            public string bank_code { get; set; }
            public string area_code { get; set; }
            public string cpu_area_code { get; set; }
            public int deposit_value { get; set; }
        }
        #endregion

        #region 檢查回傳是否符合JSON格式
        private static bool IsJsonFormat(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;
            if ((value.StartsWith("{") && value.EndsWith("}")) ||
                (value.StartsWith("[") && value.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        #endregion
    }
}
