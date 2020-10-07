using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace First_MVVM.Models
{
    public class DB_Search
    {
        DBModel dBModel = new DBModel();//使用資料庫查詢相關模組

        public DataTable User_Info(String UserName)
        {
            DataTable Table = null;
            if (!String.IsNullOrWhiteSpace(UserName))
            {
                Table = dBModel.DatabaseQuery("", "SELECT * FROM test.users where user_id = '" + UserName + "';");
            }
            return Table;
        }

        /// <summary>
        /// 使用資料庫客戶區域查詢帳戶是否已存在 true存在
        /// </summary>
        /// <param name="_phoneNumber"></param>
        /// <returns></returns>
        public bool CheckUsersExist(string _phoneNumber)
        {
            if (String.IsNullOrWhiteSpace(_phoneNumber))
                return true;
            int _count = Convert.ToInt32(dBModel.DatabaseQuery("ste_SBSCS", "SELECT COUNT(*) FROM ste_SBSCS.Customer_Address WHERE customer_user_id = '" + _phoneNumber + "';").Rows[0]["COUNT(*)"].ToString());
            if (_count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 手機簡訊驗證碼綁定號碼(讀出)
        /// </summary>
        /// <param name="_phoneNumber"></param>
        /// <returns></returns>
        public string Verify_SmPhoneBinding_Read(string _phoneNumber)
        {
            string key = dBModel.DatabaseQuery("ste_SBSCS", "SELECT Verify_SmKeyBinding FROM ste_SBSCS.Verify_SmPhoneBinding " +
                            "WHERE Verify_user_id = '" + _phoneNumber + "';").Rows[0]["Verify_SmKeyBinding"].ToString();

            return key;
        }

        /// <summary>
        /// 搜尋資料庫SmPhoneBinding比對電話驗證key
        /// </summary>
        /// <param name="_phoneNumber"></param>
        /// <param name="RandomKey"></param>
        /// <returns></returns>
        public bool Verify_SmPhoneBinding_Confirm(string _phoneNumber, string RandomKey)
        {
            int confirm = dBModel.DatabaseQuery("", "SELECT * FROM ste_SBSCS.Verify_SmPhoneBinding " +
                "WHERE Verify_user_id ='" + _phoneNumber + "' AND Verify_SmKeyBinding = SHA2('" + RandomKey + "', 256); ").Rows.Count;
            if (confirm != 0)
            { return true; }
            else { return false; }
        }

    }
}
