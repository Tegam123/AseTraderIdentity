using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AseTrader
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }  // skal den være public??(den er private hos Kudvenkat)

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddDefaultIdentity<User>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            
            services.AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                }).
                AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;

                options.SignIn.RequireConfirmedEmail = true;

            });

            services.ConfigureApplicationCookie(options => { options.LoginPath = new PathString("/account/login"); });

            services.AddAuthentication()
                 .AddGoogle(options =>
                 {
                     options.ClientId = "5319443857-5riuvt4qjp7ghon0hgv34i3ll5r43hpa.apps.googleusercontent.com";
                     options.ClientSecret = "O_U_5acXrEHX5Np_xSmfi7Y5";
                 })
                 .AddFacebook(options =>
                 {
                     options.AppId = "560845281478718";
                     options.AppSecret = "50f311381de1eb4988f44fc4a054414d";
                 })
                 .AddMicrosoftAccount(options =>
                 {
                     options.ClientId = "c2080273-e878-4efc-a6ad-2fff61a43c9b";
                     options.ClientSecret = "bOdCGq]/OTtt4JF:4q7H5m-BL1]iIK.m";
                 });

               


            services.AddControllersWithViews();
            services.AddRazorPages();

       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //InitializeRoles(roleManager);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
        //private string[] roles = new[] { "User", "Manager", "Administrator" };
        //private async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        //{
        //    foreach (var role in roles)
        //    {
        //        if (!await roleManager.RoleExistsAsync(role))
        //        {
        //            var newRole = new IdentityRole(role);
        //            await roleManager.CreateAsync(newRole);
        //            // In the real world, there might be claims associated with roles
        //            // _roleManager.AddClaimAsync(newRole, new )
        //        }
        //    }
        //}
    }
}
