using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Login/login");
            });

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = "940235020309-h57ssqflbbvs5t83ai0jav5gk5n3l624.apps.googleusercontent.com"; //Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = "c-7kLG7unXQs30PetvdBglib"; //Configuration["Authentication:Google:ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    options.AppId = "560845281478718"; //Configuration["Authentication:Facebook:AppId"];
                    options.AppSecret = "50f311381de1eb4988f44fc4a054414d"; //Configuration["Authentication:Facebook:AppSecret"];
                })
                .AddTwitter(options =>
                {
                    options.ConsumerKey = "560845281478718"; //Configuration["Authentication:Twitter:ClientId"];
                    options.ConsumerSecret = "50f311381de1eb4988f44fc4a054414d"; //Configuration["Authentication:Twitter:ClientSecret"];
                })
                .AddJwtBearer("Jwt", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSecretKey"])),
                        ValidateLifetime = true, 
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                });
                //.AddMicrosoftAccount(options =>
                //{
                //    options.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                //    options.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
                /*})*/;

                var _userManager = services.BuildServiceProvider().GetService<UserManager<User>>();
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("IsAdmin",
                        policy => policy.RequireClaim("Admin"));
                });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, 
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
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

            //SeedData.Initialize(context, userManager, roleManager).Wait();
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
