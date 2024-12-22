namespace Integration.business.Services.Interfaces
{
    public interface IDatabaseSqlService
    {
        Task<List<string>> GetAllTablesAsync(string connectionString);

       Task< List<string>> GetAllColumnsAsync(string connectionString, string tableName);
        Task<bool> CanConnectAsync(string connectionString);

    }  
}


