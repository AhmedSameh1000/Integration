using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.business.Services.Interfaces;
using Integration.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Implementation
{
    public class DataBaseMetaDataService : IDataBaseMetaDataService
    {
        private readonly IGenericRepository<DataBase> _databaseRepo;
        private readonly IDatabaseSqlService _databaseSqlService;
        private readonly IDatabaseMySqlService _databaseMySqlService;
        private readonly IDataBaseService _dataBaseService;

        public DataBaseMetaDataService(
            IGenericRepository<DataBase> databaseRepo,
            IDatabaseSqlService  databaseSqlService,
            IDatabaseMySqlService databaseMySqlService,
            IDataBaseService dataBaseService)
        {
            _databaseRepo = databaseRepo;
            _databaseSqlService = databaseSqlService;
            _databaseMySqlService = databaseMySqlService;
            _dataBaseService = dataBaseService;
        }
        public async Task<bool> CanConnectAsync(int databaseId)
        {
            var DataBase=await _databaseRepo.GetFirstOrDefault(c=>c.Id==databaseId);

            if(DataBase==null)
                return false;

            if (DataBase.dataBaseType == DataBaseType.SqlServer)
            {
                return await _databaseSqlService.CanConnectAsync(DataBase.ConnectionString);
            }
            else
            {
                return await _databaseMySqlService.CanConnectAsync(DataBase.ConnectionString);
            }


        }

        public async Task<List<string>> GetAllColumnsAsync(int databaseId, string tableName)
        {
            var DataBase = await _databaseRepo.GetFirstOrDefault(c => c.Id == databaseId);

            if (DataBase == null)
                return new List<string>();

            if (DataBase.dataBaseType == DataBaseType.SqlServer)
            {
                return await  _databaseSqlService.GetAllColumnsAsync(DataBase.ConnectionString, tableName);
            }
            else
            {
                return await  _databaseMySqlService.GetAllColumnsAsync(DataBase.ConnectionString,tableName);
            }
        }

        public async Task<List<string>> GetAllTablesAsync(int databaseId)
        {
            var DataBase = await _databaseRepo.GetFirstOrDefault(c => c.Id == databaseId);

            if (DataBase == null)
                return new List<string>();

            if (DataBase.dataBaseType == DataBaseType.SqlServer)
            {
                return await _databaseSqlService.GetAllTablesAsync(DataBase.ConnectionString);
            }
            else
            {
                return await _databaseMySqlService.GetAllTablesAsync(DataBase.ConnectionString);
            }
        }
    }
}
