using Core.TableDb;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Salony.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationDbUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            foreach (Enums.AllEnums.Roles role in (Enums.AllEnums.Roles[])Enum.GetValues(typeof(Enums.AllEnums.Roles)))
            {
               var ss =  await roleManager.CreateAsync(new IdentityRole(role.ToString()));
            }
        }
    }
}
