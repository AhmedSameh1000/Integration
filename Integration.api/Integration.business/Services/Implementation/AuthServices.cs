using AutoRepairPro.Business.DTO.AuthDTOs;

using Integration.business.Helpers;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace AutoRepairPro.Business.Service.Implementation
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _appDbContext;
        private readonly JWT _jwt;

        public AuthServices(UserManager<AppUser> userManager,
            IOptions<JWT> jwt,RoleManager<IdentityRole> roleManager,
            AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
            _jwt = jwt.Value;
        }


        public async Task<string> GenerateToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email),
                new Claim("Name", user.FullName),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.ToLocalTime().AddMonths(1),
                signingCredentials: signingCredentials);

            var Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Token;
        }

        public async Task<AuthModel> LoginAsync(LogInDTo model)
        {
            var authModel = new AuthModel();

            var user = model.Email.Contains("@")
                ? await _userManager.FindByEmailAsync(model.Email)
                : await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            return await GetAuthModel(user, GenerateUserToken: true);
        }

        private async Task<AuthModel> GetAuthModel(AppUser user, bool GenerateUserToken)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var AuthModel = new AuthModel()
            {
                Email = user.Email,
                Username = user.UserName,
                Roles = roles.ToList(),
                IsAuthenticated = true
            };

            if (GenerateUserToken)
            {
                AuthModel.Token = await GenerateToken(user);
            }

            return AuthModel;

        }


    }
}