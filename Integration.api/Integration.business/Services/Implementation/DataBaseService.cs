using AutoRepairPro.Data.Models;
using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.business.DTOs.FromDTOs;
using Integration.business.Services.Interfaces;

namespace Integration.business.Services.Implementation
{
    public class DataBaseService : IDataBaseService
    {
        private readonly IGenericRepository<DataBase> _fromDataBase;

        public DataBaseService(IGenericRepository<DataBase> FromDataBase)
        {
            _fromDataBase = FromDataBase;
        }


        public async Task<bool> AddDataBase(DbToAddDTO fromDbToAddDTO)
        {
            var FromDB = new DataBase()
            {
                DbName = fromDbToAddDTO.Name,
                ConnectionString = fromDbToAddDTO.Connection
            };
            await _fromDataBase.Add(FromDB);
            return await _fromDataBase.SaveChanges();
        }

        public async Task<bool> EditFromDataBase(DbToEditDTO fromDbToEditDTO)
        {
            var FromDb = await _fromDataBase.GetFirstOrDefault(c => c.Id == fromDbToEditDTO.Id);

            if (FromDb == null)
                return false;

            FromDb.DbName = fromDbToEditDTO.Name;
            FromDb.ConnectionString = fromDbToEditDTO.Connection;
            _fromDataBase.Update(FromDb);
            return await _fromDataBase.SaveChanges();
        }

        public async Task<DbToReturn> GetFromById(int DbId)
        {
            var FromDb = await _fromDataBase.GetFirstOrDefault(c => c.Id == DbId);

            if (FromDb is null)
                return null;

            var Result = new DbToReturn()
            {
                Id = DbId,
                Connection = FromDb.ConnectionString,
                Name = FromDb.DbName,
            };

            return Result;
        }

        public async Task<List<DbToReturn>> GetFromList()
        {
            var FromDbs = await _fromDataBase.GetAllAsNoTracking();

            return FromDbs.Select(c => new DbToReturn()
            {
                Connection = c.ConnectionString,
                Id = c.Id,
                Name = c.DbName,
            }).ToList();
        }
    }
}
