using Integration.business.DTOs.ModuleDTOs;
using Integration.business.DTOs.ReferenceDTos;
using Integration.business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace Integration.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceController : ControllerBase
    {
        private readonly IReferenceService _referenceService;

        public ReferenceController(IReferenceService referenceService)
        {
            this._referenceService = referenceService;
        }

        [HttpPost("AddReference")]
        public async Task<IActionResult> AddReference(ReferenceForCreateDTO referenceForCreateDTO)
        {
            var Result=await _referenceService.AddReference(referenceForCreateDTO);

            if(!Result.Success)
                return BadRequest(Result);

            return Ok(Result);
        }
    }
}
