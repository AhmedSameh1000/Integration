using Integration.business.DTOs.FromDTOs;

namespace Integration.business.Services.Interfaces
{
    public interface IDataBaseService
    {
        Task<bool> AddDataBase(DbToAddDTO fromDbToAddDTO);
        Task<bool> EditFromDataBase(DbToEditDTO fromDbToEditDTO);
        Task<DbToReturn> GetFromById(int DbId);
        Task<List<DbToReturn>> GetFromList();
    }
}