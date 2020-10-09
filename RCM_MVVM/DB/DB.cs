using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace DBModel
{
    public class DB
    {
        
        public async Task<DataTable> Read(string Account)
        {
            DataTable table = new DataTable();
            DataColumn column;
            DataRow row;
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "customer_user_id";
            table.Columns.Add(column);
            string buffer = string.Empty;
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "35.201.200.86",
                Database = "ste_SBSCS",
                UserID = "root",
                Password = "Jyste42876046",
                //SslMode = MySqlSslMode.Required,
            };

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

    }
}
