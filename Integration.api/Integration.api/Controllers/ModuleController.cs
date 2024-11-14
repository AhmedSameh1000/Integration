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
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }


        [HttpGet("Sync")]
        public async Task<IActionResult> Sync(int ModuleId,SyncType syncType)
        {
            try
            {
                var Result = await _moduleService.Sync(ModuleId, syncType);
                if (!Result.Success)
                    return BadRequest(Result);

                return Ok(Result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet("Modules")]
        public async Task<IActionResult> GetModules()
        {
            return Ok(await _moduleService.GetModules());
        }

    }
}