using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.TableDb;


namespace Salony.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationDbUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationDbUser
            {
                UserName = "aait@aait.sa",
                Email = "aait@aait.sa",
                fullName = "aait@aait.sa",
                showPassword = "123456",
                typeUser = (int)Enums.AllEnums.TypeUser.admin,
                activeCode = true,
                registerDate = Helper.Helper.GetCurrentDate(),
                isActive = true,
                img = "Images/Users/Default.Png",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "123456");
                await userManager.AddToRoleAsync(defaultUser, Enums.AllEnums.Roles.SuperAdmin.ToString());
                await userManager.AddToRoleAsync(defaultUser, Enums.AllEnums.Roles.Admin.ToString());
            }
            else
            {
                if (!await userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.SuperAdmin.ToString()))
                {
                    await userManager.AddToRoleAsync(user, Enums.AllEnums.Roles.SuperAdmin.ToString());
                }
                if (!await userManager.IsInRoleAsync(user, Enums.AllEnums.Roles.Admin.ToString()))
                {
                    await userManager.AddToRoleAsync(user, Enums.AllEnums.Roles.Admin.ToString());
                }
            }


        }
    }
}