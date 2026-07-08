using Microsoft.AspNetCore.Identity;
using WarehouseDAL.Entities.Identity;
using WarehouseDAL.Models;

namespace WarehouseDAL.Data.SeedData
{
    public static class DefalutUsers
    {
        public static async Task SeedSuperAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            User? user = await userManager.FindByNameAsync("Admin");

            if (user == null)
            {
                User superAdmin = new User
                {
                    Name = "Admin",
                    UserName = "Admin",
                    Email = "Admin@Warehouse.com",
                    CreateDate = DateTime.Now,
                    LastAction = LastAction.Insert
                };

                IdentityResult result = await userManager.CreateAsync(superAdmin, "Admin@123");

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine,
                        result.Errors.Select(e => e.Description)));
                }

                if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
                {
                    throw new Exception($"Role '{AppRoles.Admin}' does not exist.");
                }

                IdentityResult addRoleResult = await userManager.AddToRoleAsync(superAdmin, AppRoles.Admin);

                if (!addRoleResult.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine,
                        addRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}