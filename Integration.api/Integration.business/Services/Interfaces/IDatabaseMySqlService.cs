namespace Integration.business.Services.Interfaces
{
    public interface IDatabaseMySqlService
    {
        Task<List<string>> GetAllTablesAsync(string connectionString);

      Task< List<string>> GetAllColumnsAsync(string connectionString, string tableName);
        Task<bool> CanConnectAsync(string connectionString);

    }
}


