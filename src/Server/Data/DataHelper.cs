using Microsoft.AspNetCore.Identity;

namespace Blazor.HelloGalaxy.Server.Data;

public static class DataHelper 
{ 
    public static async Task SeedUserAndRoleAsync( 
        RoleManager<IdentityRole> roleManager, 
        UserManager<IdentityUser> userManager, 
        string user, 
        string role) 
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

    public static async Task AssignUserRoleAsync(this UserManager<IdentityUser> userManager, string utente,
        string ruolo, string password = "MyPassword1!")
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

