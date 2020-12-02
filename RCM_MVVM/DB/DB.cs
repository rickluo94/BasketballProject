using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace DBModel
{
    public class DBRead
    {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "34.80.22.21",
            Database = "ste_SBSCS",
            UserID = "LockerUsers",
            Password = "Jyste42876046",
            //SslMode = MySqlSslMode.Required,
        };


        public async Task<DataTable> Customer_Address(string Account)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "customer_user_id";
            table.Columns.Add(column);
            string buffer = string.Empty;
            

            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM ste_SBSCS.Customer_Address WHERE customer_user_id = '{Account}';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["customer_user_id"] = reader.GetString(0);
                            table.Rows.Add(row);
                        }
                    }
                }
                
            }

            return table;
        }

        public async Task<DataTable> Verify_SmPhoneBinding(string PhoneNumber,string RandomKey)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = {"Verify_user_id", "ModifyDate", "Verify_SmKeyBinding"};
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT* FROM ste_SBSCS.Verify_SmPhoneBinding WHERE Verify_user_id ='{PhoneNumber}' AND Verify_SmKeyBinding = SHA2('{RandomKey}', 256);";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["Verify_user_id"] = reader.GetString(0);
                            row["ModifyDate"] = reader.GetString(0);
                            row["Verify_SmKeyBinding"] = reader.GetString(0);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<DataTable> RFID_Users(string CardID)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = { "RFID_user_id","RFID_Card_ID","RFID_Last_Active","RFID_Ticket_Type","ModifyDate","RFID_Card_Purse_ID" };
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM ste_SBSCS.RFID_Users Where RFID_Card_ID = '{CardID}';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["RFID_user_id"] = reader.GetString(0);
                            row["RFID_Card_ID"] = reader.GetString(1);
                            row["RFID_Last_Active"] = reader.GetDateTime(2);
                            row["RFID_Ticket_Type"] = reader.GetString(3);
                            row["ModifyDate"] = reader.GetDateTime(4);
                            row["RFID_Card_Purse_ID"] = reader.GetString(5);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<DataTable> Take_History(string UserID)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = { "Take_SN", "Take_Items_EPC", "Take_Items_TID", "Take_ModifyUser", "Take_Date", "Take_CheckIn", "Take_BoxName" };
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM ste_SBSCS.Take_History Where Take_ModifyUser ='{UserID}' And Take_CheckIn = '未歸還';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["Take_SN"] = reader.GetInt32(0);
                            row["Take_Items_EPC"] = reader.GetString(1);
                            row["Take_Items_TID"] = reader.GetString(2);
                            row["Take_ModifyUser"] = reader.GetString(3);
                            row["Take_Date"] = reader.GetDateTime(4);
                            row["Take_CheckIn"] = reader.GetString(5);
                            row["Take_BoxName"] = reader.GetString(6);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<DataTable> Inventory(string CabinetLoc)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = { "Inventory_Items_EPC", "Inventory_Items_TID", "Inventory_Amount", "Inventory_CabinetLoc", "ModifyDate", "CreateDate" };
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM ste_SBSCS.Inventory Where Inventory_CabinetLoc = '{CabinetLoc}';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["Inventory_Items_EPC"] = reader.GetString(0);
                            row["Inventory_Items_TID"] = reader.GetString(1);
                            row["Inventory_Amount"] = reader.GetInt16(2);
                            row["Inventory_CabinetLoc"] = reader.GetString(3);
                            row["ModifyDate"] = reader.GetDateTime(4);
                            row["CreateDate"] = reader.GetDateTime(5);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<DataTable> InventoryX()
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = { "Inventory_Amount", "Inventory_CabinetLoc"};
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT Inventory_Amount, Inventory_CabinetLoc FROM ste_SBSCS.Inventory;";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["Inventory_Amount"] = reader.GetInt16(0);
                            row["Inventory_CabinetLoc"] = reader.GetString(1);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<DataTable> Charge_History(string ID)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            string[] _columnName = { "Charge_SN", "Charge_id", "Charge_amount", "Charge_Hours_use", "RFID_Card_ID", "CreateDate", "ModifyDate", "Charge_Status" };
            foreach (string _name in _columnName)
            {
                column = new DataColumn();
                column.DataType = Type.GetType("System.String");
                column.ColumnName = _name;
                table.Columns.Add(column);
            }

            string buffer = string.Empty;


            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM ste_SBSCS.Charge_History Where Charge_id = '{ID}' And Charge_Status = '未付款';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            row = table.NewRow();
                            row["Charge_SN"] = reader.GetInt32(0);
                            row["Charge_id"] = reader.GetString(1);
                            row["Charge_amount"] = reader.GetInt16(2);
                            row["Charge_Hours_use"] = reader.GetString(3);
                            row["RFID_Card_ID"] = reader.GetString(4);
                            row["CreateDate"] = reader.GetDateTime(5);
                            row["ModifyDate"] = reader.GetDateTime(6);
                            row["Charge_Status"] = reader.GetString(7);
                            table.Rows.Add(row);
                        }
                    }
                }

            }
            return table;
        }

        public async Task<int> NotReturnedCheckOut(string ID)
        {
            int Amount = 0;
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"SELECT count(Take_CheckIn) FROM ste_SBSCS.Take_History Where Take_ModifyUser = '{ID}' And Take_CheckIn = '未歸還';";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Amount = reader.GetInt32(0);
                        }
                    }
                }

            }
            return Amount;
        }

    }

    public class DBWrite
    {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "34.80.22.21",
            Database = "ste_SBSCS",
            UserID = "LockerUsers",
            Password = "Jyste42876046",
            //SslMode = MySqlSslMode.Required,
        };

        public async Task<bool> Customer_info(string ID, string UserName, string Email)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`Customer_info` (`customer_user_id`, `username`, `email`) VALUES ('{ID}', '{UserName}', '{Email}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Users(string ID, string UserName, string Password)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`users` (`user_id`, `username`, `password`) VALUES ('{ID}', '{UserName}', '{Password}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Customer_Address(string ID, string City, string Area)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`Customer_Address` (`customer_user_id`, `city`, `area`) VALUES ('{ID}', '{City}', '{Area}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> RFID_Users(string ID, string RFID_Card_ID, string RFID_Card_Purse_ID)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`RFID_Users` (`RFID_user_id`, `RFID_Card_ID`, `RFID_Card_Purse_ID`) VALUES ('{ID}', '{RFID_Card_ID}', '{RFID_Card_Purse_ID}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Verify_SmPhoneBinding(string PhoneNumber, string RandomKey)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO ste_SBSCS.Verify_SmPhoneBinding(Verify_user_id, Verify_SmKeyBinding) " +
                    "VALUES ('" + PhoneNumber + "',SHA2('" + RandomKey + "',256)) ON DUPLICATE KEY UPDATE Verify_SmKeyBinding = SHA2('" + RandomKey + "',256);";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Verify_SmPhoneBinding_UPDATE(string PhoneNumber,string RandomKey)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"UPDATE `ste_SBSCS`.`Verify_SmPhoneBinding` SET `Verify_SmKeyBinding` = SHA2('{RandomKey}',256) WHERE (`Verify_user_id` = '{PhoneNumber}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Inventory(string CabinetLoc, int Amount)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"UPDATE `ste_SBSCS`.`Inventory` SET `Inventory_Amount` = '{Amount}' WHERE (`Inventory_CabinetLoc` = '{CabinetLoc}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Take_History(string ID,string BoxName,string Object_EPC, string Object_TID)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`Take_History` (`Take_Items_EPC`, `Take_Items_TID`, `Take_ModifyUser`, `Take_BoxName`) VALUES ('{Object_EPC}', '{Object_TID}', '{ID}','{BoxName}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Take_History_UPDATE(string SN,string CheckInStatus)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"UPDATE `ste_SBSCS`.`Take_History` SET `Take_CheckIn` = '{CheckInStatus}' WHERE (`Take_SN` = '{SN}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Charge_History(string ID,int Amount,string HoursUse,string Card_ID)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"INSERT INTO `ste_SBSCS`.`Charge_History` (`Charge_id`, `Charge_amount`, `Charge_Hours_use`, `RFID_Card_ID`) VALUES('{ID}', '{Amount}', '{HoursUse}', '{Card_ID}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public async Task<bool> Charge_History_UPDATE(string Charge_SN)
        {
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {
                    command.CommandText = $"UPDATE `ste_SBSCS`.`Charge_History` SET `Charge_Status` = '已付款' WHERE(`Charge_SN` = '{Charge_SN}');";

                    int index = command.ExecuteNonQuery();
                    if (index == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }

}
