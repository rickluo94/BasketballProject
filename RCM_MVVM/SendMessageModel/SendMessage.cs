using System;
using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace SendMessageModel
{
    public class SendMessage
    {
        /// <summary>
        /// 帶入使用者輸入電話寫入資料庫，搜尋手機號綁定之RandomKey，發送帶有RandomKey之簡訊。
        /// </summary>
        /// <param name="_phoneNumber"></param>
        public bool SmSendSampleCode(string PhoneNumber, string RandomKeynote)
        {
            if (!string.IsNullOrWhiteSpace(PhoneNumber) && !string.IsNullOrWhiteSpace(RandomKeynote))
            {
                StringBuilder reqUrl = new StringBuilder();
                reqUrl.Append("https://smsapi.mitake.com.tw/api/mtk/SmSend?&CharsetURL=UTF-8");
                StringBuilder _params = new StringBuilder();
                _params.Append("username=42876046");
                _params.Append("&password=jyste9697");
                _params.Append($"&dstaddr={PhoneNumber}");
                _params.Append($"&smbody=智慧籃球櫃服務手機簡訊驗證碼：{RandomKeynote}，請於收到簡訊90秒內完成手機驗證。");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new
                Uri(reqUrl.ToString()));
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] bs = Encoding.UTF8.GetBytes(_params.ToString());
                request.ContentLength = bs.Length;
                request.GetRequestStream().Write(bs, 0, bs.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 隨機生成驗證碼(長度設定)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomKey(int length)
        {
            string s = "123456789ZXCVBNMASDFGHJKLQWERTYUIOP";
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            int index;
            for (int i = 0; i < length; i++)
            {
                index = rand.Next(0, s.Length);
                sb.Append(s[index]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將字串轉為Byte[]
        /// 進行SHA256加密
        /// 把加密後的字串從Byte[]轉為字串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string TakeSHA256Key(string key)
        {
            SHA256 _sha2Key = new SHA256CryptoServiceProvider();
            byte[] source = Encoding.Default.GetBytes(key);
            byte[] crypto = _sha2Key.ComputeHash(source);
            string result = Convert.ToBase64String(crypto);

            return result;
        }
    }
}
