using Google.Protobuf.WellKnownTypes;
using Hangfire;
using Integration.business.DTOs.ModuleDTOs;
using Integration.business.Services.Implementation;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Integration.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;
        private readonly AppDbContext _appDbContext;

        public ModuleController(IModuleService moduleService,AppDbContext appDbContext)
        {
            _moduleService = moduleService;
            this._appDbContext = appDbContext;
        
        }

        [HttpPost("CreateModule")]
        public async Task<IActionResult> CreateModule(ModuleForCreateDTO moduleForCreateDTO)
        {
            try
            {
                var result = await _moduleService.CreateModule(moduleForCreateDTO);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
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
        [HttpPost("EditModule")]
        public async Task<IActionResult> EditModule(ModuleForEditDTO moduleForEditDTO)
        {
            try
            {
                var Result = await _moduleService.EditModule(moduleForEditDTO);
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
       
        [HttpGet("GetModule/{id}")]
        public async Task<IActionResult> GetModule(int id)
        {
            var Result=await _moduleService.GetModuleById(id);

            if(!Result.Success)
                return NotFound(Result);

            return Ok(Result);
        }  
        [HttpDelete("DeleteModule/{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            var Result=await _moduleService.DeleteModule(id);

            if(!Result.Success)
                return BadRequest(Result);

            return Ok(Result);
        }

        [HttpGet("DisableModule")]
        public async Task<IActionResult> DisableModule(int id)
        {
            var Result=await _moduleService.DisableModule(id);

            if(!Result.Success)
                return BadRequest(Result);

            return Ok(Result);
        } 

        [HttpGet("EnableModule")]
        public async Task<IActionResult> EnableModule(int id)
        {
            var Result=await _moduleService.EnableModule(id);

            if(!Result.Success)
                return BadRequest(Result);

            return Ok(Result);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task AutoSync()
        {
          

           var Modules = await _appDbContext.modules
               .Where(c => !c.isDisabled)
               .Select(c => new { c.Id, c.SyncType })
               .ToListAsync();
                foreach (var Module in Modules)
                {
                    try
                    {
                        await this._moduleService.Sync(Module.Id, Module.SyncType);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);  
                    }
                }
        }

        [HttpGet("AutoSync")]
        public async Task<IActionResult> AutoSync(bool value)
        {
            var isUpdated = await _moduleService.UpdateAutoValueAsync(value);

            if (!isUpdated)
            {
                var errorMessage = value ? "Failed to start the auto sync. Please try again." : "Failed to stop the auto sync. Please try again.";
                return BadRequest(new ApiResponse<bool>(false, errorMessage));
            }

            if (value)
            {
                // Start the auto sync
                RecurringJob.AddOrUpdate("AutoSyncModules", () => AutoSync(), "*/1 * * * *");
                return Ok(new ApiResponse<bool>(true, "Auto sync has been successfully started.",true));
            }
            else
            {
                // Stop the auto sync
                RecurringJob.RemoveIfExists("AutoSyncModules");
                return Ok(new ApiResponse<bool>(true, "Auto sync has been successfully stopped.",false));
            }
        }

        [HttpGet("GetAutoValue")]
        public async Task<IActionResult> GetAutoValue()
        {
            var Value=await _moduleService.GetAutoValueAsync();
            if (Value is null)
                return BadRequest(new ApiResponse<bool>(false, "The auto sync value is not available. Please try again later."));

            return Ok(new ApiResponse<bool>(true, "The auto sync value was successfully retrieved.", Value.Value));
        }

    }
}