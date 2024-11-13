using Integration.business.Services.Interfaces;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

public class MySqlService : IDatabaseMySqlService
{
    public async Task<List<string>> GetAllTablesAsync(string connectionString)
    {
        var tables = new List<string>();

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand("SHOW TABLES", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    tables.Add(reader.GetString(0));
                }
            }
        }

        return tables;
    }

    public async Task<List<string>> GetAllColumnsAsync(string connectionString, string tableName)
    {
        var columns = new List<string>();

        using (var connection = new MySqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new MySqlCommand($"DESCRIBE {tableName}", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    columns.Add(reader.GetString(0));  // Column names are in the first column
                }
            }
        }

        return columns;
    }

    public async Task<bool> CanConnectAsync(string connectionString)
    {
        try
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return connection.State == System.Data.ConnectionState.Open;
            }
        }
        catch (Exception)
        {
            return false; // Connection failed
        }
    }
}
