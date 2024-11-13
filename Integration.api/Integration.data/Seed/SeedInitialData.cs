using Integration.data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Integration.data.Seed
{
    public class SeedInitialData
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public SeedInitialData(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task SeedRoles()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = Constants.Admin,
                    NormalizedName = Constants.Admin.ToUpper(),
                }
            };

            foreach (var role in roles)
            {
                if (await _roleManager.RoleExistsAsync(role.Name))
                {
                    continue;
                }
                await _roleManager.CreateAsync(role);
            }



        }
    }

    public class SeedAdminData
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public SeedAdminData(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task SeedAdmin()
        {
            var userToSeed = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "Admin@gmail.com",
                UserName = "Admin@gmail.com",
                FullName = "Admin",
                EmailConfirmed = true,
            };

            if (_userManager.Users.Any(c => c.Email == "Admin@gmail.com"))
                return;
            var result = await _userManager.CreateAsync(userToSeed, "admin1490");

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(userToSeed, new[] { Constants.Admin });
            }
        }


        public async Task SeedAllRolesToAdmin()
        {
            var adminUser = await _userManager.FindByEmailAsync("Admin@gmail.com");
            if (adminUser == null)
            {
                return;
            }

            var allRoles = await _roleManager.Roles.ToListAsync();

            foreach (var role in allRoles)
            {
                if (await _userManager.IsInRoleAsync(adminUser, role.Name))
                {
                    continue;
                }

                await _userManager.AddToRoleAsync(adminUser, role.Name);
            }
        }
    }
}
