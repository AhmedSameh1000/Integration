using Integration.data.Data;
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



    public class SeedStandardStatic
    {
        private readonly AppDbContext _appDbContext;
        public SeedStandardStatic(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }
        public async Task SeedStandards()
        {
           if (await _appDbContext.staticStandards.AnyAsync())
                return;

            var standards = new List<StaticStandard>()
            {
                new StaticStandard { Control = "toInsertFlagName", standard = "insertflag" },
                new StaticStandard { Control = "toInsertFlagName", standard = "insertedflag" },
                new StaticStandard { Control = "toInsertFlagName", standard = "insert_flag" },
                new StaticStandard { Control = "toInsertFlagName", standard = "inserted_flag" },

                new StaticStandard { Control = "fromInsertFlagName", standard = "insertflag" },
                new StaticStandard { Control = "fromInsertFlagName", standard = "insertedflag" },
                new StaticStandard { Control = "fromInsertFlagName", standard = "insert_flag" },
                new StaticStandard { Control = "fromInsertFlagName", standard = "inserted_flag" },

                new StaticStandard { Control = "fromUpdateFlagName", standard = "updateflag" },
                new StaticStandard { Control = "fromUpdateFlagName", standard = "updatedflag" },
                new StaticStandard { Control = "fromUpdateFlagName", standard = "updated_flag" },
                new StaticStandard { Control = "fromUpdateFlagName", standard = "update_flag" },

                new StaticStandard { Control = "toUpdateFlagName", standard = "updateflag" },
                new StaticStandard { Control = "toUpdateFlagName", standard = "updatedflag" },
                new StaticStandard { Control = "toUpdateFlagName", standard = "updated_flag" },
                new StaticStandard { Control = "toUpdateFlagName", standard = "update_flag" },

                new StaticStandard { Control = "ToDeleteFlagName", standard = "deleteFlag" },
                new StaticStandard { Control = "ToDeleteFlagName", standard = "deleteflag" },
                new StaticStandard { Control = "ToDeleteFlagName", standard = "deleted_flag" },
                new StaticStandard { Control = "ToDeleteFlagName", standard = "delete_flag" },

                new StaticStandard { Control = "fromDeleteFlagName", standard = "deleteFlag" },
                new StaticStandard { Control = "fromDeleteFlagName", standard = "deleteflag" },
                new StaticStandard { Control = "fromDeleteFlagName", standard = "deleted_flag" },
                new StaticStandard { Control = "fromDeleteFlagName", standard = "delete_flag" },

                new StaticStandard { Control = "localIdName", standard = "local_id" },
                new StaticStandard { Control = "localIdName", standard = "localid" },

                new StaticStandard { Control = "cloudIdName", standard = "cloud_id" },
                new StaticStandard { Control = "cloudIdName", standard = "cloudid" }
            };

            // Save to the database if needed (example)
            await _appDbContext.staticStandards.AddRangeAsync(standards);
            await _appDbContext.SaveChangesAsync();
        }
    }



}
