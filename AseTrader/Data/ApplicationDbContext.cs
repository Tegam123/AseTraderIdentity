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
        //public DbSet<Follow> Followers { get; set; }
        //public DbSet<User> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER MODEL
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            //-> Many To Many - Follow
            //modelBuilder.Entity<Follow>().HasAlternateKey(f => new { f.followersId, f.followingId }).HasName("U_follower");
            //modelBuilder.Entity<Follow>().HasKey(f => new { f.followersId, f.followingId });
            //modelBuilder.Entity<Follow>().HasIndex(f => new { f.followersId, f.followingId }).IsUnique();
            modelBuilder.Entity<User>()
                .HasMany(u => u.Followers)
                .WithOne(f => f.Followers)
                .HasForeignKey(f => f.followersId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Following)
                .WithMany(u => u.Following)
                .HasForeignKey(f => f.followingId).OnDelete(DeleteBehavior.NoAction);
        }
        //public DbSet<Follow> Followers { get; set; }
        //public DbSet<User> Users { get; set; }



        public DbSet<AseTrader.Models.EntityModels.Follow> Follow { get; set; }
    }
}
