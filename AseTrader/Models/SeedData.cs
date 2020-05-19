using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AseTrader.Models
{
    public static class SeedData
    {

        public static async Task Initialize(ApplicationDbContext context,
            UserManager<User> usermanager,
            RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminRole = "Admin";

            string password = "KodeOrd123";

            if (await roleManager.FindByIdAsync(adminRole) == null)
            {
                await roleManager.CreateAsync(new IdentityRole
                {
                    Name = adminRole,
                });
            }

            if (await usermanager.FindByEmailAsync("davidbaekhoj@hotmail.com") == null)
            {
                var user = new User
                {
                    FirstName = "David",
                    LastName = "Tegam",
                    Email = "dt@gmail.com",
                    UserName = "dt@gmail.com",
                    EmailConfirmed = true,
                };

                var userCreationResult = await usermanager.CreateAsync(user,password);
                if (userCreationResult.Succeeded)
                {
                    var result = await usermanager.AddToRoleAsync(user, adminRole);
                }
            }

            if (await usermanager.FindByEmailAsync("vk@gmail.com") == null)
            {
                var user = new User
                {
                    FirstName = "Victor",
                    LastName = "Kildahl",
                    Email = "vk@gmail.com",
                    EmailConfirmed = true,
                    UserName = "vk@gmail.com"
                };

                await usermanager.CreateAsync(user, password);

            }

            if (await usermanager.FindByEmailAsync("lm@gmail.com") == null)
            {
                var user = new User
                {
                    FirstName = "Lasse",
                    LastName = "Mosel",
                    Email = "lm@gmail.com",
                    PasswordHash = "123456",
                    UserName = "lm@gmail.com",
                };
                await usermanager.CreateAsync(user, password);
            }
        }

        //public static void Initialize(IServiceProvider serviceProvider)
        //{

        //    using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        //    {
        //        // Look for any movies.
        //        if (context.Users.Any())
        //        {
        //            return;   // DB has been seeded
        //        }

        //        context.Users.AddRange(new User
        //            {

        //                FirstName = "David",
        //                LastName = "Tegam",
        //                Email = "dt@gmail.com",
        //                PasswordHash = "123456",
        //                UserName = "dt@gmail.com"

        //        },

        //            new User
        //            {
        //                FirstName = "Victor",
        //                LastName = "Kildahl",
        //                Email = "vk@gmail.com",
        //                PasswordHash = "123456",
        //                UserName = "vk@gmail.com"
        //            },

        //            new User
        //            {
        //                FirstName = "Lasse",
        //                LastName = "Mosel",
        //                Email = "lm@gmail.com",
        //                PasswordHash = "123456",
        //                UserName = "lm@gmail.com", 

        //            }
        //        );
        //        context.SaveChanges();
        //    }
        //}
    }
}
