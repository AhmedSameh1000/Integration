using System.Data.Common;
using Integration.business.DTOs.FromDTOs;
using Integration.data.Models;
namespace Integration.business.Services.Interfaces
{
    public interface IDataBaseService
    {
        Task<bool> AddDataBase(DbToAddDTO dbToAddDTO);
        Task<bool> EditDataBase(DbToEditDTO dbToEditDTO);
        Task<DbToReturn> GetById(int DbId);
        Task<List<DbToReturn>> GetList();

        DbConnection GetConnection(DataBase dataBase);
        Task<ApiResponse<bool>> DeleteDataBase(int DbId);

        Task<ApiResponse<bool>> AddColumn(ColumnToAdd columnToAdd);

    }
}