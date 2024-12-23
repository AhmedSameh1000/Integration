using Google.Protobuf.WellKnownTypes;
using Integration.business.DTOs.ModuleDTOs;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Text;

namespace Integration.business.Services.Implementation
{
    public class SyncService : ISyncService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IDataBaseService _dataBaseService;

        public SyncService(
            AppDbContext appDbContext,
            IDataBaseService dataBaseService)
        {
            _appDbContext = appDbContext;
            _dataBaseService = dataBaseService;
        }

        #region SyncNormal

        public async Task<ApiResponse<int>> SyncNormal(int moduleId)
        {
            var module = await _appDbContext.modules
           .Include(c => c.ToDb)
           .Include(c => c.FromDb)
           .Include(c => c.columnFroms)
           .FirstOrDefaultAsync(c => c.Id == moduleId);

            if (module is null)
                return new ApiResponse<int>(false, "Module Not Found");

            if(string.IsNullOrEmpty(module.fromPrimaryKeyName) ||string.IsNullOrEmpty(module.ToPrimaryKeyName))
                return new ApiResponse<int>(false, "Primary Keys Not Selected");



            if (module.columnFroms.Count==0)
                return new ApiResponse<int>(false, "Not Found Any Columns To Sync");

            var references = await _appDbContext.References
                  .Where(c => c.ModuleId == moduleId)
                  .ToListAsync();
            var ReferencesIds = await GetReferenceIds(references, module,false);
            var columnFrom = module.columnFroms.Select(c=>new ColumnFromDTOOOOO
            {
                Id=c.Id,
                ColumnFromName=c.ColumnFromName,
                ColumnToName=c.ColumnToName,
                ModuleId=c.ModuleId,
                key=c.Key
            });
            var UsedColumns = new Dictionary<int, bool>();

            var queryFrom = $"SELECT {string.Join(',', columnFrom.Select(c => c.ColumnFromName))} FROM {module.TableFromName} {module.condition}";
            var updateQueries = new StringBuilder();
            var allValues = new List<Dictionary<string, string>>();
            var AllIdsAndLocalIdsOnCloud = await FetchIdsAsync(module.ToPrimaryKeyName, module.LocalIdName, module.TableToName, module);
            var SelectedFrom = module.FromDb;
            if (SelectedFrom is null)
                return new ApiResponse<int>(false, "No FromDataBase Selected");

            var listTODelete = new StringBuilder();
            var listFromDelete = new StringBuilder();
            var listFromUpdateFlags = new StringBuilder();

            var deleleteflag = 0;

            using (var connection = _dataBaseService.GetConnection(SelectedFrom))
            {
                await connection.OpenAsync();
                var UpdateQueryGenerated = new Dictionary<string,string>();
                var InsertQueryGenerated = new Dictionary<string, string>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryFrom;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                    
                        while (await reader.ReadAsync())
                        {
                            var idTo = -1;
                            var idFrom = -1;


                         
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var Name = reader.GetName(i);
                                var d = columnFrom.FirstOrDefault(c => c.ColumnFromName == Name && !UsedColumns.ContainsKey(c.Id));
                                
                                if (d != null)
                                {
                                    d.isUsed = true;
                                    UsedColumns.TryAdd(d.Id, true);
                                }

                                string value;
                                if (reader.IsDBNull(i))
                                {
                                    value = "NULL"; 
                                }
                                else
                                {
                                    if (Name == module.fromPrimaryKeyName)
                                    {
                                        int key = Convert.ToInt32(reader.GetValue(i).ToString());
                                        idFrom = key;
                                        listFromUpdateFlags.Append($"UPDATE {module.TableFromName} SET {module.FromInsertFlagName}=0,{module.FromUpdateFlagName}=0 where {module.fromPrimaryKeyName}={key};");

                                        if (AllIdsAndLocalIdsOnCloud.TryGetValue(key.ToString(), out var newId))
                                        {
                                            idTo = Convert.ToInt32(newId);
                                        }
                                        else
                                        {
                                            idTo = -1;
                                        }
                                    }
                                    if (Name == module.FromDeleteFlagName)
                                    {
                                        int key = Convert.ToInt32(reader.GetValue(i).ToString());
                                        deleleteflag = key;
                                        if (key == 1 &&idFrom!=-1)
                                        {
                                            listFromDelete.Append($"Delete from {module.TableFromName} where {module.fromPrimaryKeyName} = {idFrom};");
                                            listTODelete.Append($"Delete from {module.TableToName} where {module.ToPrimaryKeyName} = {idTo};");
                                            continue;
                                        }
                                    }

                                    // إذا كان النوع DateTime، قم بتنسيق القيمة
                                    if (reader.GetFieldType(i) == typeof(DateTime))
                                    {
                                        DateTime dateTimeValue = reader.GetDateTime(i);
                                        value = $"{dateTimeValue:yyyy-MM-dd HH:mm:ss}"; // تنسيق التاريخ
                                    }
                                    else
                                    {
                                        value = reader.GetValue(i).ToString().Trim();
                                    }
                                    // تخصيص قيم ToInsertFlagName و ToUpdateFlagName
                                    if (Name == module.ToInsertFlagName || Name == module.ToUpdateFlagName)
                                    {
                                        value = "0";
                                    }


                                    if (!string.IsNullOrEmpty(d.key))
                                    {
                                        var key = d.key.Trim();
                                        if (ReferencesIds.ContainsKey(key))
                                        {

                                            ReferencesIds[key].TryGetValue(value, out string referenceId);
                                            if (referenceId is null)
                                                return new ApiResponse<int>(false, $"The provided reference value ({value}) for {d.ColumnFromName} to {d.ColumnToName} does not exist in the corresponding table.");
                                            else
                                                value = referenceId;

                                        }
                                        else
                                            return new ApiResponse<int>(false, $"The key '{key}' does not exist in the references mapping.");
                                    }
                                       

                                    UpdateQueryGenerated.Add(d.ColumnToName,value.Trim());
                                    InsertQueryGenerated.Add(d.ColumnToName,value.Trim());
                                }
                            }
                            if (idTo != -1)
                            {
                                // تحديث (Update) في حالة وجود idTo
                                var setClauses = UpdateQueryGenerated
                                    .Select(kvp => $"{kvp.Key} = '{kvp.Value}'")
                                    .ToList();

                                var UpdateQuery = $"Update {module.TableToName} set {string.Join(", ", setClauses)} where {module.ToPrimaryKeyName} = {idTo}; ";

                                updateQueries.Append(UpdateQuery);
                            }
                            else
                            {
                                // إدخال (Insert) في حالة عدم وجود idTo
                                var columns = InsertQueryGenerated.Keys;
                                var values = InsertQueryGenerated.Values
                                    .Select(value => $"'{value}'")
                                    .ToList();

                                var InsertQuery = $"INSERT INTO {module.TableToName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)}); ";

                                updateQueries.Append(InsertQuery);
                            }

                            UsedColumns.Clear();
                            InsertQueryGenerated.Clear();
                            UpdateQueryGenerated.Clear();
                        }

                    }
                }
            }
            var AllQueriesTo=  updateQueries.Append(listTODelete.ToString());
            if (AllQueriesTo.Length <= 0)
            {
                return new ApiResponse<int>(false, "No data available for synchronization.");
            }

            int RowsEffected = 0;
            try
            {
                var res= await UpdateRowsAsync(AllQueriesTo, module);
                if (!res.Success)
                    return new ApiResponse<int>(false, res.Message);

                RowsEffected = res.Data;
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>(false, ex.Message);
            }

            var SyncIdBetweenTables = await FetchIdsAsync(module.ToPrimaryKeyName, module.LocalIdName, module.TableToName, module);
            using (var connection = _dataBaseService.GetConnection(SelectedFrom))
            {
                await connection.OpenAsync();
                // Start a transaction
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var IdsSynced = new StringBuilder();
                        foreach (var item in SyncIdBetweenTables)
                        {
                            if (!string.IsNullOrEmpty(item.Key))
                            {
                                IdsSynced.Append($"Update {module.TableFromName} set {module.CloudIdName}={item.Value} where {module.fromPrimaryKeyName}={item.Key};");
                            }
                        }
                        var FromUpdateQuery = IdsSynced.ToString();
                        FromUpdateQuery += listFromUpdateFlags.ToString();
                        FromUpdateQuery += listFromDelete.ToString();

                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction; // Associate command with transaction
                            command.CommandText = FromUpdateQuery;
                            int RowsEfected2 = await command.ExecuteNonQueryAsync();
                        }
                        // Commit the transaction if everything is successful
                        await transaction.CommitAsync();

                        if(RowsEffected==0)
                            return new ApiResponse<int>(true, "No data available for synchronization.");

                        return new ApiResponse<int>(true, "Sync Successfully", RowsEffected);

                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an error
                        await transaction.RollbackAsync();
                        return new ApiResponse<int>(false, $"Sync Failed: {ex.Message}");
                    }
                }
            }
        }


        private async Task<ApiResponse<int>> UpdateRowsAsync(StringBuilder updateQueries, Integration.data.Models.Module module)
        {
            var DataBaseSelected = module.ToDb;
            if (DataBaseSelected is null)
                return new ApiResponse<int>(false,"No ToDB Selected");

            var Connectionstr = DataBaseSelected.ConnectionString;
            int rowsAffected = 0;

            try
            {
                // الحصول على الاتصال بناءً على قاعدة البيانات المحددة
                using (var connection = _dataBaseService.GetConnection(DataBaseSelected))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        // إنشاء الأمر وتنفيذ الاستعلامات
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = updateQueries.ToString();

                            // استخدام ExecuteNonQueryAsync لتنفيد الاستعلامات
                            rowsAffected = await command.ExecuteNonQueryAsync();
                        }

                        // إذا نجحت جميع الاستعلامات، يتم تأكيد المعاملة
                        await transaction.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>(false, ex.Message);
            }

            return new ApiResponse<int>(true, $"Sync Success",rowsAffected);
        }


        private async Task<Dictionary<string, string>> FetchIdsAsync(string CloudIdName, string CloudLocalIdName, string CloudTable, Module module)
        {
            var SelectedTO = module.ToDb;
            if (SelectedTO is null)
                return null;

            var dataDictionary = new Dictionary<string, string>();

            try
            {
                // الحصول على الاتصال بناءً على نوع قاعدة البيانات
                using (var connection = _dataBaseService.GetConnection(SelectedTO))
                {
                    await connection.OpenAsync();

                    // إنشاء أمر قاعدة البيانات العام
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = $"SELECT {CloudIdName}, {CloudLocalIdName} FROM {CloudTable}";

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                // قراءة البيانات
                                var id = reader[CloudLocalIdName]?.ToString(); // استخدام `?.ToString()` لتجنب NullReferenceException
                                var value = reader[CloudIdName]?.ToString(); // نفس الشيء هنا

                                // إضافة البيانات فقط إذا كانت قيمة ID غير فارغة
                                if (!string.IsNullOrWhiteSpace(id))
                                {
                                    dataDictionary[id] = value; // إضافة إلى القاموس
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء بشكل مناسب
                Console.WriteLine($"Error occurred: {ex.Message}");
            }

            return dataDictionary; // إرجاع القاموس الذي يحتوي على البيانات المسترجعة
        }

        private async Task<Dictionary<string, Dictionary<string, string>>> GetReferenceIds(List<TableReference> references, Module module, bool switchRef)
        {
            var result = new ConcurrentDictionary<string, Dictionary<string, string>>();  // Using ConcurrentDictionary for thread safety

            var tasks = references.Select(async reference =>
            {
                try
                {
                    var fetchedRefs = await FetchRefsAsync(reference.PrimaryName, reference.LocalName, reference.Alter, reference.TableFromName, module, switchRef);
                    if (fetchedRefs != null)
                    {
                        result.TryAdd(reference.Key.Trim(), fetchedRefs);  // Thread-safe addition to ConcurrentDictionary
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    Console.WriteLine($"Error fetching reference for {reference.Key}: {ex.Message}");
                }
            }).ToList();

            await Task.WhenAll(tasks);  // Wait for all tasks to complete
            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);  // Convert ConcurrentDictionary back to a regular Dictionary
        }


        private async Task<Dictionary<string, string>> FetchRefsAsync(string cloudPrimaryName, string cloudLocalIdName, string alter, string tableName, Module module,bool switchRef)
        {
            var SelectedTO = module.ToDb;
            if (SelectedTO == null)
            {
                Console.WriteLine("Database connection is null.");
                return null;
            }

            var dataDictionary = new Dictionary<string, string>();
            var query = $"SELECT {cloudPrimaryName} as Id, {cloudLocalIdName} as LocalId  FROM {tableName} WHERE {cloudLocalIdName} IS NOT NULL;";

            try
            {
                using (var connection = _dataBaseService.GetConnection(SelectedTO)) // استخدام الاتصال المعاد استخدامه من Connection Pooling
                {
                    await connection.OpenAsync(); // فتح الاتصال بشكل غير متزامن
                    Console.WriteLine("Connection opened successfully.");

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        // إضافة فحص لفلاتر إضافية في حال كان ذلك ضروريًا (مثل تحديد حد لعدد السجلات)
                        command.CommandTimeout = 30; // تعيين مهلة وقت التنفيذ للاستعلام (اختياري)

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var localId = reader["LocalId"]?.ToString();
                                var primaryId = reader["Id"]?.ToString();

                                    if (!switchRef)
                                    {
                                        if(!string.IsNullOrEmpty(localId))
                                            dataDictionary[localId] = primaryId;
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(primaryId))
                                            dataDictionary[primaryId] = localId;
                                    }
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // التعامل مع أخطاء SQL بشكل محدد
                Console.WriteLine($"SQL Exception: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                // التعامل مع أي استثناءات أخرى
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return dataDictionary; // إرجاع القاموس الذي يحتوي على البيانات المسترجعة
        }

        #endregion

        #region SyncOperations
        public async Task<ApiResponse<int>> SyncOperation(int moduleId)
        {
            try
            {
                var module = await _appDbContext.modules
                    .Include(c => c.Operations)
                    .Include(c => c.FromDb)
                    .Include(c => c.ToDb)
                    .FirstOrDefaultAsync(c => c.Id == moduleId);

                if (module is null)
                    return new ApiResponse<int>(false, "Module Not Found");

                if(module.Operations.Count==0)
                    return new ApiResponse<int>(false, "No Operation To Sync");
                var rowsAffected = await SyncProductOperation(module);
                if (!rowsAffected.Success)
                    return new ApiResponse<int>(false, rowsAffected.Message);


                var IdsAndLocalIdsOnProduct = await GetIdsFromProductCloud(module);
                var Result = await MapToIdWithCloudId(IdsAndLocalIdsOnProduct, module);
                var RowsEfectedFOrCustomer = await SyncCustomerOperation(module);
                ////
                ///
                var IdsOnCustomerPrices = await GetIdsGeneral(module,OperationType.Customer);
                var Result2 = await MapIdsGeneral(IdsOnCustomerPrices, module,OperationType.Customer);

                ///

                var StoreResult = await SyncStoreOperation(module);
                var StoreSellerIds = await GetIdsGeneral(module,OperationType.Store);
                var Result3 = await MapIdsGeneral(StoreSellerIds, module,OperationType.Store);

                var Res = rowsAffected.
                    Data + RowsEfectedFOrCustomer.Data + StoreResult.Data;

                if (Res == 0)
                    return new ApiResponse<int>(false, "No data available for synchronization.", Res);
                else
                    return new ApiResponse<int>(true, "Sync Success", Res);
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>(false, $"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region ProductOperationHelper

        private async Task<ApiResponse<int>> SyncProductOperation(Module Module)
        {
            var rowsAffected = 0;
            var ProductOperation = Module.Operations.FirstOrDefault(c => c.operationType == OperationType.Product);
            if (ProductOperation != null)
            {
                // Get the StringBuilder containing SQL update queries
                var updateQueries = await GetOperationProductQuery(ProductOperation, Module);

                if (updateQueries is null)
                    return new ApiResponse<int>(false,"No data available for synchronization.");

                var UpdatedQueryAsString = updateQueries.ToString();
                using (var connection = _dataBaseService.GetConnection(Module.ToDb))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = UpdatedQueryAsString;
                                rowsAffected = await command.ExecuteNonQueryAsync();
                            }

                            await transaction.CommitAsync();
                        }
                        catch(Exception ex) 
                        {
                            await transaction.RollbackAsync();
                            return new ApiResponse<int>(false,ex.Message);
                        }
                    }
                }

            }

            return new ApiResponse<int>(true,"Sync Success", rowsAffected);

        }
        private async Task<StringBuilder> GetOperationProductQuery(Operation operation, Module module)
        {
            var selectedData = new Dictionary<int, ItemDto>();
            var UpdateQuery = new StringBuilder();

            try
            {
                var query = $"SELECT {operation.ItemId}, {operation.PriceFrom},{operation.fromPriceInsertDate} FROM {operation.TableFrom} {operation.Condition}";

                var selectedFrom = module.FromDb;

                if (selectedFrom is null)
                    throw new Exception("No FromDataBase Selected");

                using (var connection = _dataBaseService.GetConnection(selectedFrom))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var itemId = reader.GetInt32(reader.GetOrdinal(operation.ItemId));
                                var itemPrice = reader.GetDecimal(reader.GetOrdinal(operation.PriceFrom));
                                var InsertedDate = reader.GetDateTime(reader.GetOrdinal(operation.fromPriceInsertDate));


                                if (InsertedDate > DateTime.Now)
                                    continue;

                                if (selectedData.TryGetValue(itemId, out var data))
                                {
                                    data.ItemPrice = itemPrice;
                                }
                                else
                                {
                                    var itemDto = new ItemDto
                                    {
                                        ItemId = itemId,
                                        ItemPrice = itemPrice
                                    };

                                    selectedData.Add(itemId, itemDto);
                                }
                            }
                        }
                    }
                }

                if (selectedData.Count == 0)
                    return null;
                foreach (var item in selectedData.Values)
                {
                    UpdateQuery.Append($"UPDATE {operation.TableTo} SET {operation.PriceTo} = {item.ItemPrice} WHERE {operation.LocalId} = {item.ItemId}; ");
                }

                return UpdateQuery;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
        private async Task<Dictionary<int, int>> GetIdsFromProductCloud(Module module)
        {
            var operation = module.Operations.FirstOrDefault(c => c.operationType == OperationType.Product);
            if (operation is null)
                return new Dictionary<int, int>();


            var Query = $"Select {operation.LocalId}, {operation.OPToItemPrimary} from {operation.TableTo}";
            var Result = new Dictionary<int, int>();
            var DataBase = module.ToDb;
            using (var connection = _dataBaseService.GetConnection(DataBase))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var PrimaryId = reader.GetInt32(reader.GetOrdinal(operation.OPToItemPrimary));
                            var LocalId = reader.GetInt32(reader.GetOrdinal(operation.LocalId));
                            Result.TryAdd(LocalId, PrimaryId);
                        }
                    }
                }
            }
            return Result;
        }
        private async Task<int> MapToIdWithCloudId(Dictionary<int, int> IdsWithLocalIds, Module module)
        {
            if (IdsWithLocalIds is null)
                return 0;

            var operation = module.Operations.FirstOrDefault(c => c.operationType == OperationType.Product);
            if (operation is null)
                return 0;

            var rowsAffected = 0;
            var Query = new StringBuilder();

            foreach (var item in IdsWithLocalIds)
            {
                var DateTimeNow = DateTime.Now;
                Query.Append($"update {operation.TableFrom} set {operation.CloudId}={item.Value},{operation.fromInsertFlag}=0,{operation.fromUpdateFlag}=0 where {operation.ItemId}={item.Key} AND  '{DateTimeNow}' > {operation.fromPriceInsertDate} And {operation.customerId}=0 And {operation.storeId}=0 ; ");
            }
            using (var connection = _dataBaseService.GetConnection(module.FromDb))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = Query.ToString();
                            rowsAffected = await command.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            return rowsAffected;
        }
        public class ItemDto
        {
            public int ItemId { get; set; }
            public decimal ItemPrice { get; set; }
        }
        #endregion

        #region CustomerOperationHelper
        private async Task<ApiResponse<int>> SyncCustomerOperation(Module Module)
        {
            var rowsAffected = 0;
            var CustomerOperation = Module.Operations.FirstOrDefault(c => c.operationType == OperationType.Customer);

            if (CustomerOperation != null)
            {
                // Get the StringBuilder containing SQL update queries
                var updateQueries = await GetOperationCustomerQuery(CustomerOperation, Module);

                if (updateQueries is null ||updateQueries.Length==0)
                    return new ApiResponse<int>(false, "No data available for synchronization.");

                var UpdatedQueryAsString = updateQueries.ToString();
                using (var connection = _dataBaseService.GetConnection(Module.ToDb))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = UpdatedQueryAsString;
                                rowsAffected = await command.ExecuteNonQueryAsync();
                            }

                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {

                            await transaction.RollbackAsync();
                            return new ApiResponse<int>(false,ex.Message);

                        }
                    }
                }

            }

            return new ApiResponse<int>(true,"Sync Success",rowsAffected);

        }
        private async Task<StringBuilder> GetOperationCustomerQuery(Operation operation, Module module)
        {
            var InsertedQuery = new StringBuilder();
            var UpdatedQuery = new StringBuilder();
            var ResultQuery = new StringBuilder();

            var Reference = await _appDbContext.References.Where(c => c.ModuleId == module.Id).ToListAsync();
            var ReferenceIds = await GetReferenceIds(Reference, module, false);
            var IdsTO = await GetCloudIds(operation, module,operation.OpToCustomerId,operation.OpToProductId,operation.TableTo);
            var DuplicatedData = new HashSet<(string CloudCustomerId, string CloudProductId)>();

            try
            {
                var query = $"SELECT {operation.OpFromUpdateDate},{operation.ItemId},{operation.customerId},{operation.OpFromPrimary},{operation.OpFromInsertDate},{operation.OpFromUpdateDate}, {operation.PriceFrom},{operation.fromPriceInsertDate}  FROM {operation.TableFrom} {operation.Condition}";

                var selectedFrom = module.FromDb;
                if (selectedFrom is null)
                    throw new Exception("No FromDataBase Selected");

                using (var connection = _dataBaseService.GetConnection(selectedFrom))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var LocalProductId = reader.GetInt32(reader.GetOrdinal(operation.ItemId));
                                var LocalCustomerId = reader.GetInt32(reader.GetOrdinal(operation.customerId));
                                var itemPrice = reader.GetDecimal(reader.GetOrdinal(operation.PriceFrom));
                                var insertedDate = reader.GetDateTime(reader.GetOrdinal(operation.fromPriceInsertDate));
                                var UpdateDate = reader.GetDateTime(reader.GetOrdinal(operation.OpFromUpdateDate));
                                var FromPrimary = reader.GetInt32(reader.GetOrdinal(operation.OpFromPrimary));

                                if (insertedDate > DateTime.Now)
                                    continue;

                                if (!ReferenceIds[operation.OpCustomerReference]
                                        .TryGetValue(LocalCustomerId.ToString(), out var CloudCustomerId) ||
                                    !ReferenceIds[operation.OPProductReference]
                                        .TryGetValue(LocalProductId.ToString(), out var CloudProductId))
                                {
                                    throw new InvalidDataException($"The provided reference value for ({operation.TableFrom}) with values ({LocalProductId},{LocalCustomerId}) does not exist in the corresponding table.");
                                }

                                var isDuplicate = DuplicatedData.Contains((CloudCustomerId, CloudProductId));

                                if (IdsTO.TryGetValue(Convert.ToInt32(CloudCustomerId), out var idValue) || isDuplicate)
                                {
                                    UpdatedQuery.Append(
                                        $"UPDATE {operation.TableTo} " +
                                        $"SET {operation.PriceTo} = {itemPrice} ," +
                                        $"{operation.LocalId} = {FromPrimary} " +
                                        $"WHERE {operation.OpToCustomerId} = '{CloudCustomerId}' AND {operation.OpToProductId} = '{CloudProductId}';");
                                }
                                else
                                {
                                    InsertedQuery.Append(
                                        $"INSERT INTO {operation.TableTo} " +
                                        $"({operation.LocalId},{operation.OpToCustomerId}, {operation.OpToProductId}, {operation.PriceTo}) " +
                                        $"VALUES ('{FromPrimary}','{CloudCustomerId}', '{CloudProductId}', {itemPrice});");

                                    // Track inserted pair to avoid re-insertion
                                    DuplicatedData.Add((CloudCustomerId, CloudProductId));
                                }
                            }
                        }
                    }
                }

                ResultQuery.Append(InsertedQuery);
                ResultQuery.Append(UpdatedQuery);
                var res = ResultQuery.ToString();
                return ResultQuery;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region StoreOperationHelper

        private async Task<ApiResponse<int>> SyncStoreOperation(Module Module)
        {
            var rowsAffected = 0;
            var StoreOperation = Module.Operations.FirstOrDefault(c => c.operationType == OperationType.Store);

            if (StoreOperation != null)
            {
                // Get the StringBuilder containing SQL update queries
                var updateQueries = await GetOperationStoreQuery(StoreOperation, Module);

                if (updateQueries is null||updateQueries.Length==0)
                    return new ApiResponse<int>(false, "No data available for synchronization.");


                var UpdatedQueryAsString = updateQueries.ToString();
                using (var connection = _dataBaseService.GetConnection(Module.ToDb))
                {
                    await connection.OpenAsync();

                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = UpdatedQueryAsString;
                                rowsAffected = await command.ExecuteNonQueryAsync();
                            }

                            await transaction.CommitAsync();
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new ApiResponse<int>(false, ex.Message);

                        }
                    }
                }

            }

            return new ApiResponse<int>(true,"Sync Success",rowsAffected);

        }
        private async Task<StringBuilder> GetOperationStoreQuery(Operation operation, Module module)
        {
            var InsertedQuery = new StringBuilder();
            var UpdatedQuery = new StringBuilder();
            var ResultQuery = new StringBuilder();

            var Reference = await _appDbContext.References.Where(c => c.ModuleId == module.Id).ToListAsync();
            var ReferenceIds = await GetReferenceIds(Reference, module, false);
            //var IdsTO = await GetToIdsFromSellerPrices(operation, module);
            var IdsTO = await GetCloudIds(operation, module,operation.OPTOSellerPrimary,operation.OpToProductId,operation.TableTo);


            var DuplicatedData = new HashSet<(string CloudSellerId, string CloudProductId)>();


            try
            {
                var query = $"SELECT {operation.OpFromUpdateDate},{operation.storeId},{operation.ItemId},{operation.customerId},{operation.OpFromPrimary},{operation.OpFromInsertDate},{operation.OpFromUpdateDate}, {operation.PriceFrom},{operation.fromPriceInsertDate}  FROM {operation.TableFrom} {operation.Condition}";

                var selectedFrom = module.FromDb;
                if (selectedFrom is null)
                    throw new Exception("No FromDataBase Selected");

                using (var connection = _dataBaseService.GetConnection(selectedFrom))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var LocalProductId = reader.GetInt32(reader.GetOrdinal(operation.ItemId));
                                var OpFromStoreId = reader.GetInt32(reader.GetOrdinal(operation.storeId));
                               
                                var itemPrice = reader.GetDecimal(reader.GetOrdinal(operation.PriceFrom));
                                var insertedDate = reader.GetDateTime(reader.GetOrdinal(operation.fromPriceInsertDate));
                                var UpdateDate = reader.GetDateTime(reader.GetOrdinal(operation.OpFromUpdateDate));
                                var FromPrimary = reader.GetInt32(reader.GetOrdinal(operation.OpFromPrimary));

                                if (insertedDate > DateTime.Now)
                                    continue;

                                if (!ReferenceIds[operation.OpSellerReference]
                                        .TryGetValue(OpFromStoreId.ToString(), out var CloudSellerId) ||
                                    !ReferenceIds[operation.OPProductReference]
                                        .TryGetValue(LocalProductId.ToString(), out var CloudProductId))
                                {
                                    throw new InvalidDataException($"The provided reference value for ({operation.TableFrom}) with values ({LocalProductId},{CloudSellerId}) does not exist in the corresponding table.");
                                }

                                var isDuplicate = DuplicatedData.Contains((CloudSellerId, CloudProductId));

                                if (IdsTO.TryGetValue(Convert.ToInt32(CloudSellerId), out var idValue) || isDuplicate)
                                {
                                    UpdatedQuery.Append(
                                        $"UPDATE {operation.TableTo} " +
                                        $"SET {operation.PriceTo} = {itemPrice} ," +
                                        $"{operation.LocalId} = {FromPrimary} " +
                                        $"WHERE {operation.OPTOSellerPrimary} = '{CloudSellerId}' AND {operation.OpToProductId} = '{CloudProductId}';");
                                }
                                else
                                {
                                    InsertedQuery.Append(
                                        $"INSERT INTO {operation.TableTo} " +
                                        $"({operation.LocalId},{operation.OPTOSellerPrimary}, {operation.OpToProductId}, {operation.PriceTo}) " +
                                        $"VALUES ('{FromPrimary}','{CloudSellerId}', '{CloudProductId}', {itemPrice});");

                                    // Track inserted pair to avoid re-insertion
                                    DuplicatedData.Add((CloudSellerId, CloudProductId));
                                }
                            }
                        }
                    }
                }

                ResultQuery.Append(InsertedQuery);
                ResultQuery.Append(UpdatedQuery);
                var res = ResultQuery.ToString();
                return ResultQuery;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }
        #endregion

        #region GetIdsAndMapIdsHelpers
        private async Task<int> MapIdsGeneral(Dictionary<int, int> IdsWithLocalIds, Module module, OperationType operationType)
        {
            if (IdsWithLocalIds is null)
                return 0;

            var operation = module.Operations.FirstOrDefault(c => c.operationType == operationType);
            if (operation is null)
                return 0;

            var Compare = operationType == OperationType.Store ? operation.storeId : operation.customerId;
            var rowsAffected = 0;
            var Query = new StringBuilder();

            foreach (var item in IdsWithLocalIds)
            {
                var DateTimeNow = DateTime.Now;
                Query.Append($"update {operation.TableFrom} set {operation.CloudId}={item.Value},{operation.fromInsertFlag}=0,{operation.fromUpdateFlag}=0 where {operation.OpFromPrimary}={item.Key} AND  '{DateTimeNow}' > {operation.fromPriceInsertDate} And {Compare}!=0 ; ");
            }
            using (var connection = _dataBaseService.GetConnection(module.FromDb))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = Query.ToString();
                            rowsAffected = await command.ExecuteNonQueryAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            return rowsAffected;
        }

        private async Task<Dictionary<int, int>> GetIdsGeneral(Module module, OperationType operationType)
        {
            var operation = module.Operations.FirstOrDefault(c => c.operationType == operationType);
            if (operation is null)
                return new Dictionary<int, int>();


            var Query = $"Select {operation.LocalId}, {operation.OPToItemPrimary} from {operation.TableTo}";
            var Result = new Dictionary<int, int>();
            var DataBase = module.ToDb;
            using (var connection = _dataBaseService.GetConnection(DataBase))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = Query;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var PrimaryId = reader.GetInt32(reader.GetOrdinal(operation.OPToItemPrimary));
                            var LocalId = reader.GetInt32(reader.GetOrdinal(operation.LocalId));
                            Result.TryAdd(LocalId, PrimaryId);
                        }
                    }
                }
            }
            return Result;
        }

        private async Task<Dictionary<int, int>> GetCloudIds(Operation operation, Module module,string Id1,string Id2,string tableTO)
        {
            try
            {
                var IdsToReturn = new Dictionary<int, int>();
                var query = $"SELECT {Id1},{Id2} FROM {tableTO} ";
                var Db = module.ToDb;
                if (Db is null)
                    throw new Exception("No Db Selected");


                using (var connection = _dataBaseService.GetConnection(Db))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var _id1 = reader.GetInt32(reader.GetOrdinal(Id1));
                                var _id2 = reader.GetInt32(reader.GetOrdinal(Id2));


                                if (_id1 != null)
                                    IdsToReturn.TryAdd(_id1, _id2);

                            }
                        }
                    }
                }

                return IdsToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}");
            }
        }



        #endregion
    }
}


public class Items
{
    public int Id { get; set; }
    public int LocalId { get; set; }
}