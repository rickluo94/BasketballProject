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
    }
}
