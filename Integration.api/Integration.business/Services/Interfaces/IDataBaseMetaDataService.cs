using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Interfaces
{

    public interface IDataBaseMetaDataService
    {
        Task<List<string>> GetAllTablesAsync(int databaseId);

        Task<  List<string>> GetAllColumnsAsync(int databaseId, string tableName);
        Task<bool> CanConnectAsync(int databaseId);
    }
}


