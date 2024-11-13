using Integration.business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SqlDatabaseMetadataController : ControllerBase
{
    private readonly IDatabaseSqlService _databaseSqlMetadataService;

    public SqlDatabaseMetadataController(IDatabaseSqlService databaseSqlMetadataService)
    {
        _databaseSqlMetadataService = databaseSqlMetadataService;
    }

    // Check if connected to the database
    [HttpGet("check-connection")]
    public async Task<IActionResult> CheckConnection([FromQuery] string connectionString)
    {
        bool canConnect = await _databaseSqlMetadataService.CanConnectAsync(connectionString);

        if (canConnect)
        {
            return Ok(new { Message = "Connection successful!" });
        }
        else
        {
            return StatusCode(500, new { Message = "Failed to connect to the database." });
        }
    }

    // Example endpoint for getting all tables
    [HttpGet("tables")]
    public async Task<IActionResult> GetAllTables([FromQuery] string connectionString)
    {
        var tables = await _databaseSqlMetadataService.GetAllTablesAsync(connectionString);
        return Ok(tables);
    }

    // Example endpoint for getting all columns from a table
    [HttpGet("columns")]
    public async Task<IActionResult> GetAllColumns([FromQuery] string connectionString, [FromQuery] string tableName)
    {
        var columns = await _databaseSqlMetadataService.GetAllColumnsAsync(connectionString, tableName);
        return Ok(columns);
    }
}
