using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Text;

namespace Integration.business.Services.Implementation
{

    public class SyncMySqlToSqlService : ISyncMySqlToSqlService
    {
        private readonly AppDbContext _appDbContext;

        public SyncMySqlToSqlService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ApiResponse<int>> SyncFromMySqlToSql(int moduleId)
        {
            var module = await _appDbContext.modules
                .Include(c=>c.ToDb)
                .Include(c=>c.FromDb)
                .Include(c=>c.columnFroms)
                .FirstOrDefaultAsync(c => c.Id == moduleId);
            if (module is null)
                return new ApiResponse<int>(false, "Module Not Found");
            var references = await _appDbContext.References
                  .Where(c => c.ModuleId == moduleId)
                  .ToListAsync();

            var ReferencesIds = await GetReferenceASync(references,module);



            var columnFrom = module.columnFroms;
            //var queryFrom = $"SELECT {string.Join(',', columnFrom.Select(c => c.Name))} FROM {module.TableFrom?.Name} WHERE {string.Join(" OR ", module.conditionFroms?.Select(c => c.Operation) ?? new List<string>())}";
            var queryFrom = $"SELECT {string.Join(',', columnFrom.Select(c => c.ColumnFromName))} FROM {module.TableFromName}";
            var updateQueries = new StringBuilder();
            var allValues = new List<Dictionary<string, string>>(); 
            var AllIdsAndLocalIdsOnCloud = await FetchIdsAsync(module.ToPrimaryKeyName, module.LocalIdName, module.TableToName,module);

            var SelectedFrom = module.FromDb;
            if (SelectedFrom is null)
                return new ApiResponse<int>(false, "No FromDataBase Selected");
            using (MySqlConnection connection = new MySqlConnection(SelectedFrom.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = queryFrom;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var data = "";
                            var id = -1;
                            var rowValues = new Dictionary<string, string>(); // لتخزين القيم في كل صف

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var n = reader.GetName(i);
                                var d = columnFrom.FirstOrDefault(c => c.ColumnFromName == n);
                                var Name = reader.GetName(i);

                                // معالجة القيمة بحسب نوعها
                                dynamic value;
                                if (reader.IsDBNull(i))
                                {
                                    value = "NULL";
                                }
                                else
                                {
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

                                    if (d != null)
                                    {
                                        if (d.isReference)
                                        {
                                            if (ReferencesIds.ContainsKey(d.TableReferenceName))
                                            {
                                                if (ReferencesIds[d.TableReferenceName].TryGetValue(value, out object newid))
                                                {
                                                    var NewValued = newid as dynamic;
                                                    if (NewValued != null)
                                                    {
                                                        if (NewValued.Id is int idValue)
                                                        {
                                                            value = idValue.ToString();
                                                        }
                                                        else
                                                        {
                                                            value = NewValued.Id.ToString();
                                                        }
                                                    }
                                                }

                                            }
                                        }


                                    }

                                }




                                rowValues[Name] = value;

                                if (Name == module.fromPrimaryKeyName)
                                {
                                    int key = Convert.ToInt32(reader.GetValue(i).ToString());

                                    if (AllIdsAndLocalIdsOnCloud.TryGetValue(key.ToString(), out var newId))
                                    {
                                        id = Convert.ToInt32((newId as Dictionary<dynamic, object>)[module.ToPrimaryKeyName]);
                                    }
                                    else
                                    {
                                        id = -1;
                                    }
                                }

                                if (d != null && !string.IsNullOrEmpty(d.ColumnToName) && id != -1)
                                {
                                    data += $"{d.ColumnToName} = '{value}',";
                                }
                            }

                            allValues.Add(rowValues);

                            if (id == -1)
                            {
                                var insertValues = string.Join(", ", columnFrom
                                    .Where(c => !string.IsNullOrEmpty(c.ColumnToName))
                                    .Select(c => $"'{rowValues[c.ColumnFromName]}'"));

                                var insertQuery = $"INSERT INTO {module.TableToName} ({string.Join(",", columnFrom.Where(c => !string.IsNullOrEmpty(c.ColumnToName)).Select(c => c.ColumnToName))}) VALUES ({insertValues})";

                                insertQuery = insertQuery.Replace("AM", string.Empty);
                                insertQuery = insertQuery.Replace("PM", string.Empty);
                                updateQueries.Append(insertQuery);
                                updateQueries.Append(";");
                            }

                            // تصحيح موضع الـ else block
                            if (!string.IsNullOrEmpty(data))
                            {
                                var updateQuery = $"UPDATE {module.TableToName} SET {data.TrimEnd(',')} WHERE {module.ToPrimaryKeyName}={id}";
                                updateQueries.Append(updateQuery);
                                updateQueries.Append(";");
                            }
                        }

                    }
                }
            }


            var RowsEfected = await UpdateRowsAsync(updateQueries,module);


            return new ApiResponse<int>(true, "Sync Successfully", RowsEfected);
        }
        private async Task<int> UpdateRowsAsync(StringBuilder updateQueries,Module module)
        {
            var DataBaseSelected = module.ToDb;
            if (DataBaseSelected is null)
                return 0;

            var Connectionstr = DataBaseSelected.ConnectionString;
            int rowsAffected = 0;

            // Use SqlConnection for SQL Server
            using (var connection = new SqlConnection(Connectionstr))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        // Use SqlCommand for executing the combined query string
                        using (var command = new SqlCommand(updateQueries.ToString(), connection, (SqlTransaction)transaction))
                        {
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
        private async Task<Dictionary<dynamic, object>> FetchIdsAsync(string CloudIdName, string CloudLocalIdName, string CloudTable,Module module)
        {
            var SelectedTO = module.ToDb;
            if (SelectedTO is null)
                return null;

            var dataDictionary = new Dictionary<dynamic, object>();

            using (var connection = new SqlConnection(SelectedTO.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand($"SELECT {CloudIdName}, {CloudLocalIdName} FROM {CloudTable}", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var id = reader[CloudLocalIdName].ToString(); // Assuming local_id is a string
                            var value = new Dictionary<dynamic, object>();

                            // Storing each column's value with its column name as the key
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                value[reader.GetName(i)] = reader.GetValue(i);
                            }

                            // Using local_id as the key in the data dictionary
                            dataDictionary[id] = value;
                        }
                    }
                }
            }

            return dataDictionary; // Return the dictionary with local_id as the key
        }
     
        
        private async Task<Dictionary<string, Dictionary<dynamic, object>>> GetReferenceASync(List<TableReference> references,Module module)
        {
            var result = new Dictionary<string, Dictionary<dynamic, object>>();

            foreach (var reference in references)
            {
                result.Add(reference.TableFromName, await FetchRefsAsync(reference.PrimaryName, reference.LocalName, reference.TableFromName, module));
            }
            return result;
        }
     
        
        private async Task<Dictionary<dynamic, object>> FetchRefsAsync(string cloudPrimaryName, string cloudLocalIdName, string tableName,Module module)
        {
            var SelectedTO = module.ToDb;
            if (SelectedTO is null)
                return null;

            var dataDictionary = new Dictionary<dynamic, object>();

            try
            {
                using (var connection = new SqlConnection(SelectedTO.ConnectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand($"SELECT {cloudPrimaryName} as Id, {cloudLocalIdName} as LocalId FROM {tableName};", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var localId = reader[cloudLocalIdName].ToString();

                                var obj = new
                                {
                                    Id = reader[cloudPrimaryName],
                                    LocalId = reader[cloudLocalIdName]
                                };

                                if (localId != null)
                                {
                                    dataDictionary[localId] = obj;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your preferred logging framework)
                Console.WriteLine(ex.Message);
            }

            return dataDictionary; // Return the dictionary with local_id as the key
        }
  
    

    }
}
