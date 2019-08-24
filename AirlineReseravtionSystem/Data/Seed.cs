using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AirlineReseravtionSystem.Data
{
    public class Seed
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var SignInManager = serviceProvider.GetRequiredService<SignInManager<IdentityUser>>();
            string[] roleNames = {"Admin","User"};
            IdentityResult roleResult;

            foreach(var roleName in roleNames)
            {
                //creating the roles and seeding them to the database
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if(!roleExist)
                {
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //creating a super user who could maintain the web app
            var powerUser = new IdentityUser
            {
                UserName = configuration.GetSection("AppSettings")["UserEmail"],
                Email = configuration.GetSection("AppSettings")["UserEmail"]
            };

            string userPassword = configuration.GetSection("AppSettings")["UserPassword"];
            var user = await UserManager.FindByEmailAsync(configuration.GetSection("AppSettings")["UserEmail"]);

            if(user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(powerUser, userPassword);
                if(createPowerUser.Succeeded)
                {
                    await UserManager.AddToRoleAsync(powerUser, "Admin");
                }
            }
        }
    }
}
