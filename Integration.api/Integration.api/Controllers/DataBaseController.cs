using Integration.business.DTOs.FromDTOs;
using Integration.business.Services.Interfaces;
using Integration.data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DataBaseController : ControllerBase
{
    private readonly IDataBaseService _DatabaseService;

    public DataBaseController(IDataBaseService DatabaseService)
    {
        _DatabaseService = DatabaseService;
    }


    [HttpPost("addDataBase")]
    public async Task<IActionResult> addDataBase([FromBody] DbToAddDTO DbToAddDTO)
    {
        var result = await _DatabaseService.AddDataBase(DbToAddDTO);
        if (result)
            return Ok();
        return BadRequest("Error adding to database");
    }


    [HttpPut("editDataBase")]
    public async Task<IActionResult> editDataBase([FromBody] DbToEditDTO DbToEditDTO)
    {
        var result = await _DatabaseService.EditDataBase(DbToEditDTO);
        if (result)
            return Ok();
        return NotFound("Database entry not found");
    }

    [HttpGet("GetDataBase/{id}")]
    public async Task<IActionResult> GetDataBase(int id)
    {
        var result = await _DatabaseService.GetById(id);
        if (result != null)
            return Ok(result);
        return NotFound();
    }

    [HttpGet("DataBases")]
    public async Task<IActionResult> GetDataBases()
    {
        var result = await _DatabaseService.GetList();
        return Ok(result);
    }
    [HttpGet("GetDataBaseTypes")]
    public IActionResult GetSyncTypes()
    {
        var DataBaseTypes = Enum.GetNames(typeof(DataBaseType));
        return Ok(DataBaseTypes);
    } 
    [HttpDelete("DeleteDataBase/{dbId}")]
    public async Task<IActionResult> GetSyncTypes(int dbId)
    {
        var Result=await _DatabaseService.DeleteDataBase(dbId);
        if(!Result.Success)
            return BadRequest(Result);

        return Ok(Result);
    }
    [HttpPost("AddColumn")]
    public async Task<IActionResult> AddColumn(ColumnToAdd columnToAdd)
    {
        var Result = await _DatabaseService.AddColumn(columnToAdd);
        if (!Result.Success)
            return BadRequest(Result);

        return Ok(Result);
    }
}
