using AutoRepairPro.Business.DTO.AuthDTOs;
using Integration.data.Models;

namespace Integration.business.Services.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthModel> LoginAsync(LogInDTo model);
        Task<string> GenerateToken(AppUser user);
    }


}