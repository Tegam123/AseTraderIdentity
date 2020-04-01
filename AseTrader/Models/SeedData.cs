using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AseTrader.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any movies.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                context.Users.AddRange(new User
                    {

                        FirstName = "David",
                        LastName = "Tegam",
                        Email = "dt@gmail.com",
                        
                    },

                    new User
                    {
                        FirstName = "Victor",
                        LastName = "Kildahl",
                        Email = "vk@gmail.com",
                        
                    },

                    new User
                    {
                        FirstName = "Lasse",
                        LastName = "Mosel",
                        Email = "lm@gmail.com",
                        
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
