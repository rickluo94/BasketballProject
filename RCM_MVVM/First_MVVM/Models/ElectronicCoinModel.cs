using System;
using System.Text;
using System.IO.Ports;
using Newtonsoft.Json.Linq;
using STJ = System.Text.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Data;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace First_MVVM.Models
{
    public class ElectronicCoinModel
    {
        private int _TimeOut = 0;
        private string _writeBuffer = string.Empty;
        private SerialPort _serialPort = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);//電子投幣機;
        TaskCompletionSource<JObject> ResultDataObj;

        #region Port 通訊參數設定
        public void InitCom()
        {
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            _serialPort.ReceivedBytesThreshold = 1;
            _serialPort.RtsEnable = true;
            //Open(_serialPort);
        }
        #endregion

        private void Open(SerialPort comport)
        {
            if (!comport.IsOpen)
            {
                comport.Open();
            }
        }

        #region 收到資料觸發事件判斷是否回傳指令成功(成功後執行相關動作)
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Delay(1000).Wait();
            string response = string.Empty;
            string _is_success = string.Empty;
            string _action = string.Empty;
            string _card_id = string.Empty;
            response = _serialPort.ReadExisting();
            bool IsJson = IsJsonFormat(response);
            if (IsJson == true)
            {
                JObject _obj = JObject.Parse(response);
                _is_success = (string)_obj["is_success"];
                _action = (string)_obj["action"];
                _card_id = (string)_obj["result"]["card_id"];
                if (_is_success == "True")
                {
                    ResultDataObj.TrySetResult(_obj);
                }
            }
            else
            {
                _serialPort.Write(_writeBuffer);
                Task.Delay(1000).Wait();
            }
        }
        #endregion

        #region 讀取卡號
        public async Task<JObject> Read_card_id_request()
        {
            ResultDataObj = new TaskCompletionSource<JObject>();
            _serialPort.Write("{\"action\": \"read.card_id\",\"arg\": [],\"kwarg\": { }}");
            _writeBuffer = "{\"action\": \"read.card_id\",\"arg\": [],\"kwarg\": { }}";
            return await ResultDataObj.Task;
        }
        #endregion

        #region 連續讀取卡號
        public async Task<JObject> Read_card_id_loop_request()
        {
            ResultDataObj = new TaskCompletionSource<JObject>();
            _serialPort.Write("{\"action\": \"read.card_id.loop\",\"arg\": [],\"kwarg\": {\"search_timeout\": 300,\"search_view_text\":\"\\u6b61\\u8fce\\u5149\\u81e8\\uff0c\\u8acb\\u9760\\u5361\\uff0c\\u5361\\u7247\\u611f\\u61c9\\u4e2d\",\"success_view_text\": \"\\u8b80\\u5361\\u5b8c\\u6210\\u8acb\\u53d6\\u5361\", \"show_success_message\": false}}");
            _writeBuffer = "{\"action\": \"read.card_id.loop\",\"arg\": [],\"kwarg\": {\"search_timeout\": 300,\"search_view_text\":\"\\u6b61\\u8fce\\u5149\\u81e8\\uff0c\\u8acb\\u9760\\u5361\\uff0c\\u5361\\u7247\\u611f\\u61c9\\u4e2d\",\"success_view_text\": \"\\u8b80\\u5361\\u5b8c\\u6210\\u8acb\\u53d6\\u5361\", \"show_success_message\": false}}";
            return await ResultDataObj.Task;
        }
        #endregion

        #region 讀取卡⽚詳細資訊
        public async Task<JObject> Read_card_balance_request()
        {
            _serialPort.Write("{\"action\": \"read.card_balance\",\"arg\": [],\"kwarg\": { }}");
            _writeBuffer = "{\"action\": \"read.card_balance\",\"arg\": [],\"kwarg\": { }}";
            return await ResultDataObj.Task;
        }
        #endregion

        #region 扣款
        public async Task<JObject> Charge_request(int amount)
        {
            _serialPort.Write("{\"action\": \"charge\",\"arg\": [],\"kwarg\": {\"amount\": " + amount + "}}");
            _writeBuffer = "{\"action\": \"charge\",\"arg\": [],\"kwarg\": {\"amount\": " + amount + "}}";
            return await ResultDataObj.Task;
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
