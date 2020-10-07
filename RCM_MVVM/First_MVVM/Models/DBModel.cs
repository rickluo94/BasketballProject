using System;
using System.Data;
using System.Windows;
using MySql.Data.MySqlClient;

namespace First_MVVM.Models
{
    public class DBModel
    {
        private string DB_IP = "35.201.200.86";
        private int DB_Port = 3306;
        private string DB_AccountNumber = "root";
        private string DB_Password = "Jyste42876046";

        //資料庫查詢
        public DataTable DatabaseQuery(string DatabaseName, string QueryString)
        {
            DataTable ReaultDataTable = new DataTable();
            string ConnectString = $"server={DB_IP};uid={DB_AccountNumber};pwd={DB_Password};Pooling=false;";
            

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString() + "(DatabaseQuery)");
            }

            return ReaultDataTable;
        }

        public bool DatabaseCommand(string DatabaseName, string CommandString)
        {
            string ConnectString = $"server={DB_IP};uid={DB_AccountNumber};pwd={DB_Password};Pooling=false;";

            try
            {
                using (MySqlConnection Connection = new MySqlConnection(ConnectString))
                {
                    Connection.Open();
                    using (MySqlCommand SQL_Command = new MySqlCommand(CommandString, Connection))
                    { SQL_Command.ExecuteNonQuery(); }
                }
            }
            catch
            { 
                return false; 
            }

            return true;
        }
    }
}
