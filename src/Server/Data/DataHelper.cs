using System.Diagnostics;
using Blazor.HelloGalaxy.Server.Models;
using Blazor.HelloGalaxy.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blazor.HelloGalaxy.Server.Data;

public static class DataHelper
{
    [DebuggerStepThrough]
    public static DbContextOptionsBuilder Set(this DbContextOptionsBuilder builder, IConfiguration configuration, string connectionName = "identity") =>
        builder.UseSqlite(configuration.GetConnectionString(connectionName));


    public static ApplicationDbContext GetDbContext(this IConfiguration configuration,
        string connectionName = "identity")
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        dbContextOptionsBuilder.Set(configuration, connectionName);
        var options = dbContextOptionsBuilder.Options;
        return new ApplicationDbContext(options);
    }

    public static async Task InitializeAsync(this WebApplication app, string connectionName = "identity", CancellationToken cancellationToken = default)
    {
        await using var dbContext = app.Configuration.GetDbContext(connectionName);
        await dbContext.Database.MigrateAsync(cancellationToken);

        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(); 
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        await SeedUserAndRoleAsync(roleManager, userManager, "admin@admin.com", "admin");
        await SeedUserAndRoleAsync(roleManager, userManager, "user@user.com", "user");
    }

    public static async Task SeedUserAndRoleAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, string user, string role)
    {
        await CreateRoleAsync(roleManager, role); 
        await AssignUserRoleAsync(userManager, user, role); 
    } 
      
    public static async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string roleName) 
    { 
        var roleExists = await roleManager.RoleExistsAsync(roleName); 
        if (!roleExists) 
        { 
            var role = new IdentityRole 
            { 
                Name = roleName 
            }; 
            await roleManager.CreateAsync(role); 
        } 
    }

    public static async Task AssignUserRoleAsync(this UserManager<IdentityUser> userManager, string utente, string ruolo, string password = "MyPassword1!")
    {
        var userExists = await userManager.FindByEmailAsync(utente) is not null;
        if (!userExists)
        {
            var newUser = new IdentityUser
            {
                UserName = utente,
                Email = utente
            };

            var result = await userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, ruolo);
            }
        }
    }
} 

