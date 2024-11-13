using Azure;

namespace Integration.business.Services.Interfaces
{
    public interface ISyncMySqlToSqlService
    {
        Task<ApiResponse<int>> SyncFromMySqlToSql(int moduleId);
    }
}

