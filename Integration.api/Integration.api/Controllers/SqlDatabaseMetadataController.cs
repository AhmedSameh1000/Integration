using Integration.business.Services.Interfaces;
using Integration.data.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SqlDatabaseMetadataController : ControllerBase
{
    private readonly IDatabaseSqlService _databaseSqlMetadataService;
    private readonly IDatabaseMySqlService _databaseMySqlService;

    public SqlDatabaseMetadataController(IDatabaseSqlService databaseSqlMetadataService,IDatabaseMySqlService databaseMySqlService)
    {
        _databaseSqlMetadataService = databaseSqlMetadataService;
        _databaseMySqlService = databaseMySqlService;
    }

    // Check if connected to the database
    [HttpGet("check-connection")]
    public async Task<IActionResult> CheckConnection([FromQuery] string connectionString,[FromQuery]DataBaseType dataBaseType)
    {

        bool canConnect = false;
        if (dataBaseType == DataBaseType.SqlServer)
        {
             canConnect = await _databaseSqlMetadataService.CanConnectAsync(connectionString);
        }
        else
        {
            canConnect = await _databaseMySqlService.CanConnectAsync(connectionString);
        }

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
