using AutoRepairPro.Data.Repositories.Interfaces;
using Integration.business.DTOs.ModuleDTOs;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Integration.business.Services.Implementation
{
    public class ModuleService : IModuleService
    {
        private readonly IGenericRepository<Module> _moduleRepository;
        private readonly ISyncService _localService;
        private readonly IGenericRepository<TableReference> _tableReference;
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHost;

        public ModuleService(
            IGenericRepository<Module> ModuleRepository,
            ISyncService localService,
            IGenericRepository<TableReference> TableReference,
            AppDbContext appDbContext,
            IWebHostEnvironment webHost
        )
        {
            _moduleRepository = ModuleRepository;
            _localService = localService;
            _tableReference = TableReference;
            _appDbContext = appDbContext;
            _webHost = webHost;
        }

        public async Task<ApiResponse<bool>> CreateModule(ModuleForCreateDTO moduleForCreateDTO)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync();
            try
            {
                var existingModule = await _moduleRepository.GetFirstOrDefault(c => c.priority == moduleForCreateDTO.priority);

                if (existingModule != null)
                    return new ApiResponse<bool>(false, "duplicate priority");

                // Map the DTO to the entity model
                var module = new Module()
                {
                    Name = moduleForCreateDTO.ModuleName,
                    TableFromName = moduleForCreateDTO.TableFromName,
                    TableToName = moduleForCreateDTO.TableToName,
                    ToPrimaryKeyName = moduleForCreateDTO.ToPrimaryKeyName,
                    fromPrimaryKeyName = moduleForCreateDTO.FromPrimaryKeyName,
                    LocalIdName = moduleForCreateDTO.LocalIdName,
                    CloudIdName = moduleForCreateDTO.CloudIdName,
                    SyncType = (SyncType)moduleForCreateDTO.SyncType,
                    ToDbId = moduleForCreateDTO.ToDbId,
                    FromDbId = moduleForCreateDTO.FromDbId,
                    ToInsertFlagName = moduleForCreateDTO.ToInsertFlagName,
                    ToUpdateFlagName = moduleForCreateDTO.ToUpdateFlagName,
                    FromInsertFlagName = moduleForCreateDTO.FromInsertFlagName,
                    FromUpdateFlagName = moduleForCreateDTO.FromUpdateFlagName,
                    condition = moduleForCreateDTO.condition,
                    FromDeleteFlagName = moduleForCreateDTO.FromDeleteFlagName,
                    ToDeleteFlagName = moduleForCreateDTO.ToDeleteFlagName,
                    priority = moduleForCreateDTO.priority,
                    columnFroms = moduleForCreateDTO.Columns.Select(c => new ColumnFrom()
                    {
                        ColumnFromName = c.ColumnFrom,
                        ColumnToName = c.ColumnTo,
                        Key = c.Key,
                        isReference=c.isReference
                    }).ToList()
                };

                // Add the module and save changes
                bool isModuleCreated = await AddModuleAsync(module);
                if (!isModuleCreated)
                    return new ApiResponse<bool>(false, "Error When Creating Module.");

                // Add table references
                if (moduleForCreateDTO.References.Count > 0)
                {
                    var references = moduleForCreateDTO.References.Select(c => new TableReference()
                    {
                        LocalName = c.LocalName,
                        PrimaryName = c.PrimaryName,
                        TableFromName = c.TableFromName,
                        ModuleId = module.Id,
                        Alter=c.Alter,
                        Key=c.Key,
                    }).ToList();

                    bool areReferencesAdded = await AddReferencesAsync(references);
                    if (!areReferencesAdded)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<bool>(false, "Error on adding references.");
                    }

                }

                // Add operations
                if (moduleForCreateDTO.Operations.Count > 0)
                {
                    var operations = moduleForCreateDTO.Operations.Select(c => new Operation()
                    {
                        ModuleId = module.Id,
                        TableFrom = c.OPTableFromName,
                        TableTo = c.OPTableToName,
                        CloudId = c.CloudIdName,
                        LocalId = c.OPToItemLocalIdName,
                        fromPriceInsertDate = c.OPFromInsertDate,
                        ItemId = c.OPFromItemIdName,
                        PriceFrom = c.OPFromItemPrice,
                        PriceTo = c.OPToItemPrice,
                        storeId = c.StoreIdName,
                        customerId = c.CustomerIdName,
                        OPToItemPrimary = c.OPToPrimary,
                        operationType = (OperationType)c.Type,
                        fromInsertFlag = c.OPFromInsertFlag,
                        fromDeleteFlag = c.OPFromDeleteFlag,
                        fromUpdateFlag = c.OPFromUpdateFlag,
                        ToDeleteFlag = c.OPToDeleteFlag,
                        ToInsertFlag = c.OPToInsertFlag,
                        ToUpdateFlag = c.OPToUpdateFlag,
                        Condition = c.OPCondition,
                        OpCustomerReference=c.OpCustomerReference,
                        OPProductReference=c.OPProductReference,
                        OpToCustomerId=c.OpToCustomerId,
                        OpToProductId=c.OpToProductId,
                        OpFromPrimary = c.OpFromPrimary,
                        FromItemParent=c.ItemParent,
                        OpFromDeleteDate=c.OpDeleteDate,
                        OpFromInsertDate= c.OpInsertDate,
                        OpFromUpdateDate=c.OpUpdateDate
                    }).ToList();

                    await _appDbContext.Operations.AddRangeAsync(operations);
                    var operationsAdded = await _appDbContext.SaveChangesAsync();
                    if (operationsAdded <= 0)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<bool>(false, "Error on adding operations.");

                    }
                }

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync();
                return new ApiResponse<bool>(true, "Module created successfully.", true);
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of any failure
                await transaction.RollbackAsync();
                return new ApiResponse<bool>(false, "Error: " + ex.Message, false);
            }
        }

        private async Task<bool> AddModuleAsync(Module module)
        {
            try
            {
                await _moduleRepository.Add(module);
                return await _moduleRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log exception (if needed)
                throw new Exception("Failed to add module: " + ex.Message);
            }
        }

        private async Task<bool> AddReferencesAsync(List<TableReference> references)
        {
            try
            {
                await _tableReference.AddRange(references);
                return await _tableReference.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log exception (if needed)
                throw new Exception("Failed to add references: " + ex.Message);
            }
        }

        public async Task<List<ModuleForReturnDTO>> GetModules()
        {
            var Modules = await _moduleRepository.GetAllAsNoTracking( );

            return Modules
                .Select(c => new ModuleForReturnDTO()
                {
                    Id = c.Id,
                    Name = c.Name,
                    SyncType = c.SyncType.ToString(),
                    TableFromName = c.TableFromName,
                    TableToName = c.TableToName,
                    priorty=c.priority,
                    isdisabled=c.isDisabled,
                }).OrderBy(c => c.priorty).ToList();
        }


        public async Task<ApiResponse<int>> Sync(int ModuleId, SyncType syncType)
        {
            try
            {
                ApiResponse<int> apiResponse = null;
                switch (syncType)
                {
                    case SyncType.Operation:
                        apiResponse = await _localService.SyncOperation(ModuleId);
                        break;
                    case SyncType.Normal:
                        apiResponse = await _localService.SyncNormal(ModuleId);
                        break;
                    default:
                        break;
                }


                if (apiResponse is null)
                    return new ApiResponse<int>(false, "An Error when Sync Module");

                return apiResponse;

            }
            catch (Exception ex)
            {
                return new ApiResponse<int>(false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ModuleFullDataForReturnDTO>> GetModuleById(int Id)
        {
            var Module=await _moduleRepository.GetFirstOrDefault(c => c.Id == Id, new[] { "columnFroms", "Operations" } );

            if (Module is null)
                return new ApiResponse<ModuleFullDataForReturnDTO>(false, "Module Not Found");


            var Referances=await _tableReference.GetAllAsNoTracking(c=>c.ModuleId==Id);

            var ModuleForReturn = new ModuleFullDataForReturnDTO()
            {
                Id = Id,
                Name = Module.Name,
                CloudIdName = Module.CloudIdName,
                LocalIdName = Module.LocalIdName,
                FromDbId = Module.FromDbId,
                ToDbId = Module.ToDbId,
                FromInsertFlagName = Module.FromInsertFlagName,
                FromUpdateFlagName = Module.FromUpdateFlagName,
                ToInsertFlagName = Module.ToInsertFlagName,
                ToUpdateFlagName = Module.ToUpdateFlagName,
                SyncType = Module.SyncType.ToString(),
                fromPrimaryKeyName = Module.fromPrimaryKeyName,
                TableFromName = Module.TableFromName,
                TableToName = Module.TableToName,
                ToPrimaryKeyName = Module.ToPrimaryKeyName,
                ToDeleteFlagName=Module.ToDeleteFlagName,
                FromDeleteFlagName=Module.FromDeleteFlagName,
                condition=Module.condition,
                priority=Module.priority,
                columnsFromDTOs = Module.columnFroms.Select(c => new ColumnFromDTO()
                {
                    Id = c.Id,
                    ColumnFromName = c.ColumnFromName,
                    ColumnToName = c.ColumnToName,
                    isReference = c.isReference,
                    ModuleId = c.ModuleId,
                    TableReferenceName = c.TableReferenceName,
                    IsAlter=c.isAlter,
                    key=c.Key,
                }).ToList(),
                referancesForReturnDTOs = Referances.Select(c => new ReferancesForReturnDTO()
                {
                    Id = c.Id,
                    ModuleId = c.ModuleId,
                    LocalName = c.LocalName,
                    PrimaryName = c.PrimaryName,
                    TableFromName = c.TableFromName,
                    Alter = c.Alter,
                    key = c.Key
                }).ToList(),
                operationForReturnDTOs= Module.Operations.Select(c=>new DTOs.OperationDTOs.OperationForReturnDTO()
                {
                    CloudId = c.CloudId,
                    customerId=c.customerId,
                    fromInsertDate= c.fromPriceInsertDate,
                    fromInsertFlag = c.fromInsertFlag,
                    Id = c.Id,
                    ItemId = c.ItemId,
                    LocalId = c.LocalId,
                    ModuleId= c.ModuleId,
                    operationType=c.operationType,
                    OPToPrimary= c.OPToItemPrimary,
                    PriceFrom=c.PriceFrom,
                    PriceTo=c.PriceTo,
                    storeId=c.storeId,
                    TableFrom=c.TableFrom,
                    TableTo= c.TableTo,
                    Condition=c.Condition,
                    fromDeleteFlag=c.fromDeleteFlag,
                    fromUpdateFlag= c.fromUpdateFlag,
                    ToDeleteFlag= c.ToDeleteFlag,
                    ToInsertFlag= c.ToInsertFlag,
                    ToUpdateFlag = c.ToUpdateFlag,
                    OPProductReference=c.OPProductReference,
                    OpCustomerReference = c.OpCustomerReference,
                    OpToCustomerId=c.OpToCustomerId,
                    OpToProductId=c.OpToProductId,
                    OpFromPrimary = c.OpFromPrimary,
                    ItemParent = c.FromItemParent,
                    OpDeleteDate = c.OpFromDeleteDate,
                    OpInsertDate = c.OpFromInsertDate,
                    OpUpdateDate = c.OpFromUpdateDate
                }).ToList()
            };
            return new ApiResponse<ModuleFullDataForReturnDTO>(true,"Module Found",ModuleForReturn);
         
        }

        public async Task<ApiResponse<bool>> DeleteModule(int id)
        {
           var Module=await _moduleRepository.GetFirstOrDefault(c=>c.Id==id);

            if (Module is null)
                return new ApiResponse<bool>(false, "Module not found");
            var Referance=await _tableReference.GetAllAsTracking(c=>c.ModuleId==id);

            if (Referance.Any())
            {
                _tableReference.RemoveRange(Referance);
                var deleted = await _tableReference.SaveChanges();

                if (!deleted)
                    return new ApiResponse<bool>(false, "En error whene delete module");
            }

            _moduleRepository.Remove(Module);
            var IsDeleted= await _moduleRepository.SaveChanges();

            if (!IsDeleted)
                return new ApiResponse<bool>(false, "En error whene delete module");

            return new ApiResponse<bool>(true, "Module deleted successfully");
        }

        public async Task<ApiResponse<bool>> EditModule(ModuleForEditDTO moduleForEdit)
        {
            using var transaction = await _appDbContext.Database.BeginTransactionAsync(); // بدء المعاملة

            try
            {
                // 1. جلب الـ Module الحالي من قاعدة البيانات باستخدام الـ ID
                var module = await _moduleRepository.GetFirstOrDefault(c => c.Id == moduleForEdit.Id);
                if (module is null)
                {
                    return new ApiResponse<bool>(false, "Module not found.", false);
                }


                var ModulePriority = await _moduleRepository.GetFirstOrDefault(c => c.priority == moduleForEdit.priority &&c.Id!=moduleForEdit.Id);

                if (ModulePriority != null)
                    return new ApiResponse<bool>(false, "duplicate priority",false);


               

                // 2. تحديث خصائص الـ Module بناءً على الـ DTO الجديد
                module.Name = moduleForEdit.ModuleName;
                module.TableFromName = moduleForEdit.TableFromName;
                module.TableToName = moduleForEdit.TableToName;
                module.ToPrimaryKeyName = moduleForEdit.ToPrimaryKeyName;
                module.fromPrimaryKeyName = moduleForEdit.FromPrimaryKeyName;
                module.LocalIdName = moduleForEdit.LocalIdName;
                module.CloudIdName = moduleForEdit.CloudIdName;
                module.ToDbId = moduleForEdit.ToDbId;
                module.FromDbId = moduleForEdit.FromDbId;
                module.ToInsertFlagName = moduleForEdit.ToInsertFlagName;
                module.ToUpdateFlagName = moduleForEdit.ToUpdateFlagName;
                module.FromInsertFlagName = moduleForEdit.FromInsertFlagName;
                module.FromUpdateFlagName = moduleForEdit.FromUpdateFlagName;
                module.condition = moduleForEdit.condition;
                module.FromDeleteFlagName = moduleForEdit.FromDeleteFlagName;
                module.ToDeleteFlagName = moduleForEdit.ToDeleteFlagName;
                module.priority= moduleForEdit.priority;
                // 3. إزالة الأعمدة القديمة (إذا كانت موجودة)
                var existingColumns = await _appDbContext.columnFroms.Where(c => c.ModuleId == moduleForEdit.Id).ToListAsync();
                _appDbContext.columnFroms.RemoveRange(existingColumns);

                // 4. إضافة الأعمدة الجديدة
                var newColumns = moduleForEdit.Columns.Select(c => new ColumnFrom()
                {
                    ColumnFromName = c.ColumnFrom,
                    ColumnToName = c.ColumnTo,
                    ModuleId = moduleForEdit.Id,
                    Key = c.Key,
                    isReference=c.isReference,
                }).ToList();

                await _appDbContext.columnFroms.AddRangeAsync(newColumns);

                // 5. إزالة المراجع القديمة
                var existingReferences = await _appDbContext.References.Where(r => r.ModuleId == moduleForEdit.Id).ToListAsync();
                _appDbContext.References.RemoveRange(existingReferences);

                // 6. إضافة المراجع الجديدة
                if (moduleForEdit.References.Any())
                {
                    var newReferences = moduleForEdit.References.Select(r => new TableReference()
                    {
                        LocalName = r.LocalName,
                        PrimaryName = r.PrimaryName,
                        TableFromName = r.TableFromName,
                        ModuleId = moduleForEdit.Id,
                        Alter=r.Alter,
                        Key=r.Key,
                        
                        
                    }).ToList();

                    await _appDbContext.References.AddRangeAsync(newReferences);
                }


                var AllOperation = await _appDbContext.Operations.Where(c => c.ModuleId == module.Id).ToListAsync();

                 _appDbContext.RemoveRange(AllOperation);

                if (moduleForEdit.Operations.Any())
                {
                    var Operations = moduleForEdit.Operations.Select(c => new Operation()
                    {
                        ModuleId = moduleForEdit.Id,
                        CloudId= c.CloudIdName,
                        Condition= c.OPCondition,
                        customerId=c.CustomerIdName,
                        fromDeleteFlag=c.OPFromDeleteFlag,
                        fromInsertFlag=c.OPFromInsertFlag,
                        fromUpdateFlag=c.OPFromUpdateFlag,
                        fromPriceInsertDate=c.OPFromInsertDate,
                        ItemId=c.OPFromItemIdName,
                        LocalId=c.OPToItemLocalIdName,
                        operationType=(OperationType)c.Type,
                        OPToItemPrimary=c.OPToPrimary,
                        PriceFrom=c.OPFromItemPrice,
                        PriceTo=c.OPToItemPrice,
                        storeId=c.StoreIdName,
                        TableFrom=c.OPTableFromName,
                        TableTo=c.OPTableToName,
                        ToDeleteFlag=c.OPToDeleteFlag,
                        ToInsertFlag=c.OPToInsertFlag,
                        ToUpdateFlag=c.OPToUpdateFlag,
                        OpCustomerReference=c.OpCustomerReference,
                        OPProductReference=c.OPProductReference,
                        OpToCustomerId=c.OpToCustomerId,
                        OpToProductId=c.OpToProductId,
                        OpFromPrimary = c.OpFromPrimary,
                        FromItemParent = c.ItemParent,
                        OpFromDeleteDate = c.OpDeleteDate,
                        OpFromInsertDate = c.OpInsertDate,
                        OpFromUpdateDate = c.OpUpdateDate
                    }).ToList();

                    await _appDbContext.Operations.AddRangeAsync(Operations);
                }




                // 7. حفظ التغييرات في قاعدة البيانات
               var Result= await _appDbContext.SaveChangesAsync();

                if(Result<=0)
                    await transaction.RollbackAsync();


                // 8. تأكيد المعاملة
                await transaction.CommitAsync();

                return new ApiResponse<bool>(true, "Module updated successfully.", true);
            }
            catch (Exception ex)
            {
                // في حال حدوث خطأ، نقوم بإلغاء المعاملة
                await transaction.RollbackAsync();
                return new ApiResponse<bool>(false, "Error: " + ex.Message, false);
            }
        }

        public async Task<ApiResponse<bool>> DisableModule(int id)
        {
            var Module = await _moduleRepository.GetFirstOrDefault(c => c.Id == id);

            if (Module is null)
                return new ApiResponse<bool>(false, "Module not found.");


            if(Module.isDisabled)   
                return new ApiResponse<bool>(true, "Module is already disabled.");


            Module.isDisabled = true;
            _moduleRepository.Update(Module);
            var IsDisabled = await _moduleRepository.SaveChanges();

            if (!IsDisabled)
                return new ApiResponse<bool>(false, "Error occurred when disabling the module.");

            return new ApiResponse<bool>(true, "Module Disabled Successfully.");

        }

        public async Task<ApiResponse<bool>> EnableModule(int id)
        {
            var Module = await _moduleRepository.GetFirstOrDefault(c => c.Id == id);

            if (Module is null)
                return new ApiResponse<bool>(false, "Module not found.");


            if (!Module.isDisabled)
                return new ApiResponse<bool>(true, "Module is already Enabled.");


            Module.isDisabled = false;
            _moduleRepository.Update(Module);
            var IsEnabled = await _moduleRepository.SaveChanges();


            if (!IsEnabled)
                return new ApiResponse<bool>(false, "Error occurred when Enabling the module.");

            return new ApiResponse<bool>(true, "Module Enabled Successfully.");
        }
        public async Task<bool?> GetAutoValueAsync()
        {
            try
            {
                var path = Path.Combine(_webHost.WebRootPath, "Auto.json");
                if (!System.IO.File.Exists(path))
                    return null;

                // Read and deserialize the JSON file
                var jsonContent = await System.IO.File.ReadAllTextAsync(path);
                var jsonObject = JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonContent);

                if (jsonObject != null && jsonObject.ContainsKey("Auto"))
                {
                    return jsonObject["Auto"];
                }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                Console.WriteLine($"Error reading JSON file: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAutoValueAsync(bool newValue)
        {
            try
            {
                var path = Path.Combine(_webHost.WebRootPath, "Auto.json");
                // Create a new JSON object
                var updatedObject = new { Auto = newValue };

                // Serialize and write to the file
                var updatedJson = JsonSerializer.Serialize(updatedObject, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(path, updatedJson);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                Console.WriteLine($"Error updating JSON file: {ex.Message}");
                return false;
            }
        }
    }
}
