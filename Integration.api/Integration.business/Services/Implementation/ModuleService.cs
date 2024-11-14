using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.business.DTOs.ModuleDTOs;
using Integration.business.Services.Interfaces;
using Integration.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration.business.Services.Implementation
{
    public class ModuleService : IModuleService
    {
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly ILocalService _localService;

        public ModuleService(IGenericRepository<Module> ModuleRepository,ILocalService localService)
        {
            _moduleRepository = ModuleRepository;
            _localService = localService;
        }
        public async Task<List<ModuleForReturnDTO>> GetModules()
        {
            var Modules=await _moduleRepository.GetAllAsNoTracking();

            return Modules.Select(c=>new ModuleForReturnDTO()
            {
                Id = c.Id,
                Name = c.Name,
                SyncType=c.SyncType.ToString(),
                TableFromName=c.TableFromName,
                TableToName=c.TableToName,
            }).ToList();
        }

        public async Task<ApiResponse<int>> Sync(int ModuleId, SyncType syncType)
        {
            ApiResponse<int> response;

            switch (syncType)
            {
                case SyncType.LocalSqlToSql:
                    response = await _localService.SyncSqlToSql(ModuleId);
                    break;
                case SyncType.LocalSqlToMySql:
                    response = await _localService.SyncSqlToMySql(ModuleId);
                    break;
                case SyncType.LocalMySqlToSql:
                    response = await _localService.SyncMySqlToSql(ModuleId);
                    break;
                case SyncType.LocalMySqlToMySql:
                    response = await _localService.SyncMySqlToMySql(ModuleId);
                    break;
                default:
                    response = new ApiResponse<int>(false, "Invalid sync type", 0);
                    break;
            }

            return response;
        }

    }
}
