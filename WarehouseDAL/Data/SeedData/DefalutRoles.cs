using Microsoft.AspNetCore.Identity;
using WarehouseDAL.Entities.Identity;
using WarehouseDAL.Models;

namespace WarehouseDAL.Data.SeedData;

public static class DefalutRoles
{
    public static async Task SeedAsync(RoleManager<Role> roleManager)
    {
        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new Role { Name = AppRoles.Admin, LastAction = LastAction.Insert });
            await roleManager.CreateAsync(new Role { Name = AppRoles.Cacher, LastAction = LastAction.Insert });
        }
    }
}

