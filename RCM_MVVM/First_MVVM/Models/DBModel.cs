using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;

namespace First_MVVM.Models
{
    public class DBModel
    {
        private string DB_IP = "127.0.0.1";
        private int DB_Port = 3306;
        private string DB_Name = "ste_wms";
        private string DB_AccountNumber = "Slave_PC";
        private string DB_Password = "Ste42876046";

        //資料庫查詢
        public DataTable DatabaseQuery(string DatabaseName, string QueryString)
        {
            DataTable ReaultDataTable = new DataTable();
            string ConnectString;

            //確認有無指定資料庫
            if ((DatabaseName.Length == 0) || (DatabaseName == "null"))
            { ConnectString = "server=" + DB_IP + ";uid=" + DB_AccountNumber + ";pwd=" + DB_Password + ";Pooling=false;"; }
            else
            { ConnectString = "server=" + DB_IP + ";uid=" + DB_AccountNumber + ";pwd=" + DB_Password + ";database=" + DatabaseName + ";Pooling=true;"; }

            try
            {
                using MySqlConnection Connection = new MySqlConnection(ConnectString);
                Connection.Open();
                using (MySqlDataAdapter Adapter = new MySqlDataAdapter(QueryString, Connection))
                {
                    Adapter.Fill(ReaultDataTable);
                }
                Connection.Close();
            }
            catch //(Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString() + "(DatabaseQuery)");
            }

            return ReaultDataTable;
        }
    }
}
