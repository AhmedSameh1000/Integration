using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Integration.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DataBaseMetaDataController : ControllerBase
    {
        private readonly IDataBaseMetaDataService _dataBaseMetaDataService;
        private readonly AppDbContext _appDbContext;

        public DataBaseMetaDataController(IDataBaseMetaDataService dataBaseMetaDataService,AppDbContext appDbContext)
        {
            _dataBaseMetaDataService = dataBaseMetaDataService;
            this._appDbContext = appDbContext;
        }

        // Check if connected to the database
        [HttpGet("check-connection")]
        public async Task<IActionResult> CheckConnection(int DataBaseId)
        {
            var Result=await _dataBaseMetaDataService.CanConnectAsync(DataBaseId);

            if(!Result) 
                return NotFound();

            return Ok(Result);
        }

        // Example endpoint for getting all tables
        [HttpGet("tables")]
        public async Task<IActionResult> GetAllTables(int DataBaseId)
        {
            var tables = await _dataBaseMetaDataService.GetAllTablesAsync(DataBaseId);
            return Ok(tables);
        }


        // Example endpoint for getting all columns from a table
        [HttpGet("columns")]
        public async Task<IActionResult> GetAllColumns([FromQuery] int DataBaseId, [FromQuery] string tableName)
        {
            var tables = await _dataBaseMetaDataService.GetAllColumnsAsync(DataBaseId,tableName);
            return Ok(tables);
        }


        [HttpGet("GetStatics")]

        public async Task<IActionResult> GetAllStandards()
        {
            var standards = await _appDbContext.staticStandards
            .GroupBy(c => c.Control)
            .Select(g => new
            {
                Control = g.Key,
                Standards = g.Select(s => s.standard).ToList()
            })
        .ToListAsync();

            return Ok(standards);
        }

    }
}
