using DbPad.Common.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data;

namespace DbPad.Adapter.MsSql
{
    public class MsSqlAdapter
    {
        public static string connectionString = "Server=.\\sqlexpress;Trusted_Connection=True;TrustServerCertificate=True;";

        public static async Task<DataTable?> ExecuteSQLAsync(string query, string database)
        {
            if (string.IsNullOrWhiteSpace(query))
                return null;

            using (var connection = new SqlConnection(MsSqlAdapter.connectionString))
            {
                await connection.OpenAsync();
                if (!string.IsNullOrWhiteSpace(database))
                    connection.ChangeDatabase(database);

                using (var cmd = new SqlCommand(query, connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);

                    return dataTable;
                }
            }
        }

        public static string Select1000Query(string tablename)
        {
            ArgumentException.ThrowIfNullOrEmpty(tablename);
            return $"SELECT TOP 1000 * FROM {tablename}";
        }

        public static async Task<List<Node>> LoadDatabasesAndTablesAsync(string connectionString)
        {
            var result = new List<Node>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                // Получаем список БД
                var databases = new List<Node>();
                using (var cmd = new SqlCommand("SELECT name FROM sys.databases", connection))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var dbName = reader.GetString(0);
                        databases.Add(new Node(dbName, dbName, new ObservableCollection<Node>(), NodeType.Database));
                    }
                }

                // Получаем таблицы для каждой БД
                foreach (var db in databases)
                {
                    using (var cmd = new SqlCommand(
                        $"USE [{db.Title}]; SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'",
                        connection))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            db.SubNodes?.Add(new Node(reader.GetString(0), db.Database, NodeType.Table));
                        }
                    }
                }

                result.AddRange(databases);
            }

            return result;
        }
    }
}
