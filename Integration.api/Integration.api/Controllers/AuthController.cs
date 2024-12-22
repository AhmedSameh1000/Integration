using AutoRepairPro.Business.DTO.AuthDTOs;
using Integration.business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }
        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(LogInDTo logInDTo)
        {
            var Result = await _authServices.LoginAsync(logInDTo);

            if(!Result.IsAuthenticated)
                return BadRequest(Result);
            return Ok(Result);
        } 
    }
}
