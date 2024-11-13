using Integration.business.DTOs.FromDTOs;
using Integration.business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DataBaseController : ControllerBase
{
    private readonly IDataBaseService _fromDatabaseService;

    public DataBaseController(IDataBaseService fromDatabaseService)
    {
        _fromDatabaseService = fromDatabaseService;
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddFromDataBase([FromBody] DbToAddDTO fromDbToAddDTO)
    {
        var result = await _fromDatabaseService.AddDataBase(fromDbToAddDTO);
        if (result)
            return Ok();
        return BadRequest("Error adding to database");
    }

    [HttpPut("edit")]
    public async Task<IActionResult> EditFromDataBase([FromBody] DbToEditDTO fromDbToEditDTO)
    {
        var result = await _fromDatabaseService.EditFromDataBase(fromDbToEditDTO);
        if (result)
            return Ok();
        return NotFound("Database entry not found");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFromById(int id)
    {
        var result = await _fromDatabaseService.GetFromById(id);
        if (result != null)
            return Ok(result);
        return NotFound();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetFromList()
    {
        var result = await _fromDatabaseService.GetFromList();
        return Ok(result);
    }
}
