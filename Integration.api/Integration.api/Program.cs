using AutoRepairPro.Business.Service.Implementation;
using AutoRepairPro.Data.Repositories.Interfaces;
using Hangfire;
using Integration.business.Helpers;
using Integration.business.Services.Implementation;
using Integration.business.Services.Interfaces;
using Integration.data.Data;
using Integration.data.Models;
using Integration.data.Repositories.Implementation;
using Integration.data.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySql.EntityFrameworkCore.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(c => c.UseSqlServerStorage(builder.Configuration.GetConnectionString("IntegrationDb")));
builder.Services.AddHangfireServer();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("IntegrationDb")));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IDataBaseService, DataBaseService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IReferenceService, ReferenceService>();
builder.Services.AddScoped<IDataBaseMetaDataService , DataBaseMetaDataService>();

builder.Services.AddScoped<IDatabaseMySqlService, MySqlService>();
builder.Services.AddScoped<IDatabaseSqlService, SqlServerService>();






builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 5;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = true;
})
           .AddEntityFrameworkStores<AppDbContext>()
           .AddDefaultTokenProviders();


var _JWT = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JWT>>().Value;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = _JWT.Issuer,
        ValidAudience = _JWT.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWT.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();
using (var Scope = app.Services.CreateScope())
{
    var UserManger = Scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var RoleManger = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var AppDbContext = Scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await new SeedInitialData(RoleManger, UserManger).SeedRoles();
    await new SeedAdminData(RoleManger, UserManger).SeedAdmin();
    await new SeedStandardStatic(AppDbContext).SeedStandards();
}
app.UseHangfireDashboard("/dashboard");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
