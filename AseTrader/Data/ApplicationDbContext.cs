using System;
using System.Collections.Generic;
using System.Text;
using AseTrader.Models;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AseTrader.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        //public DbSet<User> Users { get; set; }
    }
}
