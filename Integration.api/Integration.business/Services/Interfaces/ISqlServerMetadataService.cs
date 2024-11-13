using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Interfaces
{
    public interface IDatabaseSqlService
    {
        Task<List<string>> GetAllTablesAsync(string connectionString);

        Task<List<string>> GetAllColumnsAsync(string connectionString, string tableName);
        Task<bool> CanConnectAsync(string connectionString);

    }  

    public interface IDatabaseMySqlService
    {
        Task<List<string>> GetAllTablesAsync(string connectionString);

        Task<List<string>> GetAllColumnsAsync(string connectionString, string tableName);
        Task<bool> CanConnectAsync(string connectionString);

    }
}


