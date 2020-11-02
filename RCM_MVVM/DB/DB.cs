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
            Server = "35.201.200.86",
            Database = "ste_SBSCS",
            UserID = "root",
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

        public async Task<DataTable> Verify_TheMember(string CardID)
        {
            DataTable table = new DataTable();
            return table;
        }

        public async Task<DataTable> Outstanding_Amount(string UserID)
        {
            DataTable table = new DataTable();
            return table;
        }
    }

    public class DBWrite
    {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "35.201.200.86",
            Database = "ste_SBSCS",
            UserID = "root",
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



    }

}
