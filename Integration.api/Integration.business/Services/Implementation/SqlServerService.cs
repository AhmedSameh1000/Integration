using Integration.business.Services.Interfaces;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;


public class SqlServerService : IDatabaseSqlService
{
    public async Task<List<string>> GetAllTablesAsync(string connectionString)
    {
        var tables = new List<string>();

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);

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

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();
            var command = new SqlCommand($"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", connection);
            command.Parameters.AddWithValue("@TableName", tableName);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    columns.Add(reader.GetString(0));
                }
            }
        }

        return columns;
    }

    public async Task<bool> CanConnectAsync(string connectionString)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
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

