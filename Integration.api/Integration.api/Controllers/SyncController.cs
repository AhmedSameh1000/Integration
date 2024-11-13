using Integration.business.Services.Implementation;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace Integration.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ILocalService _localService;

        public SyncController(ILocalService localService)
        {
            _localService = localService;
        }


        [HttpGet("Sync")]
        public async Task<IActionResult> SyncMySqlToSqlServer(int ModuleId)
        {
            try
            {
                var Result = await _localService.SyncSqlToSql(ModuleId);
                if (!Result.Success)
                    return BadRequest(Result);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }




        [HttpGet("Getdata")]
        public async Task<IActionResult> Data()
        {
            var dataList = new List<Dictionary<string, object>>();

            try
            {
                using (var connection = new MySqlConnection("Server=srv1373.hstgr.io;Database=u530506966_testpos;Uid=u530506966_testpos;Pwd=p4I=IQ$P;"))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand("SELECT name FROM categories where local_id=1005", connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                dataList.Add(row);
                            }
                        }
                    }
                }

                // Returning the data as JSON
                return Ok(dataList);
            }
            catch (Exception ex)
            {
                // Handle exception (you can log it if necessary)
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
  
    }
}