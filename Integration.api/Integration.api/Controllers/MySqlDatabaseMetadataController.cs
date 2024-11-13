using Integration.business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class MySqlDatabaseMetadataController : ControllerBase
{
    private readonly IDatabaseMySqlService _databaseMySqlMetadataService;

    public MySqlDatabaseMetadataController(IDatabaseMySqlService databaseMySqlMetadataService)
    {
        _databaseMySqlMetadataService = databaseMySqlMetadataService;
    }

    // Check if connected to the MySQL database
    [HttpGet("check-connection")]
    public async Task<IActionResult> CheckConnection([FromQuery] string connectionString)
    {
        bool canConnect = await _databaseMySqlMetadataService.CanConnectAsync(connectionString);

        if (canConnect)
        {
            return Ok(new { Message = "Connection successful!" });
        }
        else
        {
            return StatusCode(500, new { Message = "Failed to connect to the database." });
        }
    }

    // Example endpoint for getting all tables from MySQL
    [HttpGet("tables")]
    public async Task<IActionResult> GetAllTables([FromQuery] string connectionString)
    {
        var tables = await _databaseMySqlMetadataService.GetAllTablesAsync(connectionString);
        return Ok(tables);
    }

    // Example endpoint for getting all columns from a table in MySQL
    [HttpGet("columns")]
    public async Task<IActionResult> GetAllColumns([FromQuery] string connectionString, [FromQuery] string tableName)
    {
        var columns = await _databaseMySqlMetadataService.GetAllColumnsAsync(connectionString, tableName);
        return Ok(columns);
    }
}
