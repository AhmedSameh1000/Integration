using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Interfaces
{
    public interface ILocalService
    {
        Task<ApiResponse<int>> SyncSqlToMySql(int moduleId);
        Task<ApiResponse<int>> SyncMySqlToSql(int moduleId);
        Task<ApiResponse<int>> SyncSqlToSql(int moduleId);
        Task<ApiResponse<int>> SyncMySqlToMySql(int moduleId);
    }
}
