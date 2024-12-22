using System.Data.Common;
using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.business.DTOs.FromDTOs;
using Integration.business.Services.Interfaces;
using Integration.data.Models;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Integration.business.Services.Implementation
{
    public class DataBaseService : IDataBaseService
    {
        private readonly IGenericRepository<DataBase> _DataBaseRepo;

        public DataBaseService(IGenericRepository<DataBase>  databaseService)
        {
            _DataBaseRepo = databaseService;
        }


        public async Task<bool> AddDataBase(DbToAddDTO DataBaseToAddDTO)
        {
            if (!Enum.TryParse(DataBaseToAddDTO.DataBaseType, out DataBaseType dbType))
            {
                return false;
            }
            var DataBase = new DataBase()
            {
                DbName = DataBaseToAddDTO.Name,
                ConnectionString = DataBaseToAddDTO.Connection,
                dataBaseType = dbType, // Assign the enum value
            };

            await _DataBaseRepo.Add(DataBase);
            return await _DataBaseRepo.SaveChanges();
        }

        public async Task<bool> EditDataBase(DbToEditDTO DataBaseToEditDTO)
        {
            var DataBase = await _DataBaseRepo.GetFirstOrDefault(c => c.Id == DataBaseToEditDTO.Id);

            if (DataBase == null)
                return false;

            if (!Enum.TryParse(DataBaseToEditDTO.DataBaseType, out DataBaseType dbType))
            {
                return false;
            }

            DataBase.DbName = DataBaseToEditDTO.Name;
            DataBase.ConnectionString = DataBaseToEditDTO.Connection;
            DataBase.dataBaseType= dbType;
            _DataBaseRepo.Update(DataBase);
            return await _DataBaseRepo.SaveChanges();
        }

        public DbConnection GetConnection(DataBase dataBase)
        {
            switch (dataBase.dataBaseType)
            {
                case DataBaseType.SqlServer:
                    return new SqlConnection(dataBase.ConnectionString);
                case DataBaseType.MySql:
                    return new MySqlConnection(dataBase.ConnectionString);
                default:
                    throw new NotSupportedException("Unsupported database type.");
            }
        }


        public async Task<DbToReturn> GetById(int DbId)
        {
            var DataBase = await _DataBaseRepo.GetFirstOrDefault(c => c.Id == DbId);

            if (DataBase is null)
                return null;

            var Result = new DbToReturn()
            {
                Id = DbId,
                Name = DataBase.DbName,
                DataBaseType=DataBase.dataBaseType.ToString(),
                Connection=DataBase.ConnectionString
            };

            return Result;
        }

        public async Task<List<DbToReturn>> GetList()
        {
            var DataBases = await _DataBaseRepo.GetAllAsNoTracking();

            return DataBases.Select(c => new DbToReturn()
            {
                Id = c.Id,
                Name = c.DbName,
                DataBaseType = c.dataBaseType.ToString(),
            }).ToList();
        }

        public async Task<ApiResponse<bool>> DeleteDataBase(int DbId)
        {
            var DataBase = await _DataBaseRepo.GetFirstOrDefault(c => c.Id == DbId);

            if (DataBase is null)
                return new ApiResponse<bool>(false, "Data Base Not Found");

            _DataBaseRepo.Remove(DataBase);

            var isDeleted = await _DataBaseRepo.SaveChanges();
        
            if(!isDeleted)
                return new ApiResponse<bool>(false, "Error When Delete Database");


            return new ApiResponse<bool>(true, "Data Base Deleted Succesfuly");

        }

        public async Task<ApiResponse<bool>> AddColumn(ColumnToAdd columnToAdd)
        {
            var dataBase = await _DataBaseRepo.GetFirstOrDefault(c => c.Id == columnToAdd.dbId);

            if (dataBase is null)
                return new ApiResponse<bool>(false, "Data Base Not Found");

            var query = columnToAdd.query;

            using (var connection = GetConnection(dataBase))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        var checkResult = await command.ExecuteScalarAsync();
                       
                        return new ApiResponse<bool>(true, "Column added successfully");
                    }
                }
                catch (Exception ex)
                {
                    return new ApiResponse<bool>(false, $"Error executing query: {ex.Message}");
                }
            }
        }

    }
}
