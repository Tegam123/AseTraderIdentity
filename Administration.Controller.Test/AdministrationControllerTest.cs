using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AseTrader.Controllers;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;


namespace Administration.Controller.Test
{
    [TestFixture]
    public class Administrationcontrollertest
    {


        private SqliteConnection sqliteConnection;
        private ApplicationDbContext applicationDbContext;
        //private IOptions<TokenValidation> tokenValidation;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;


        public void SetUp()
        {
            // Benytter iservice til at kunne lave Usermanager og Rolemanager
            IServiceCollection serviceCollection = new ServiceCollection();

            // Laver en "ny" database i hukommelse til tests. 
            sqliteConnection = new SqliteConnection("DataSource=:memory:");
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(sqliteConnection));


            applicationDbContext = serviceCollection.BuildServiceProvider().GetService<ApplicationDbContext>();
            applicationDbContext.Database.OpenConnection();
            applicationDbContext.Database.EnsureCreated();

            // Gør at identity bruger database fra hukommelse til at lave user og role manager.
            serviceCollection.
                AddIdentity<User, IdentityRole>()
                .AddUserManager<UserManager<User>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Henter de to managers 
            _userManager = serviceCollection.BuildServiceProvider().GetService<UserManager<User>>();
            _roleManager = serviceCollection.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();




            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            //var tokenValidationSection = configuration.GetSection("TokenValidation");
            //serviceCollection.Configure<TokenValidation>(tokenValidationSection);

            //tokenValidationSettings = serviceCollection.BuildServiceProvider().GetService<IOptions<TokenValidation>>().Value;
            ////serviceCollection.Configure<TokenValidation>()

            //Koden for reelle Managers findes her
            // https://github.com/tgolla/ASP.NETCoreIdentityUnitTestingDemo/blob/master/WebApplication.UnitTest/AuthenticationControllerUnitTest.cs 

            // Mock eksempel kan findes her
            // https://github.com/alastairchristian/TestingSamples/blob/master/SweetShop.Tests/SweetServiceTests.cs 
        }

        [TearDown]
        public void TearDown()
        {
            applicationDbContext.Database.EnsureDeleted();
            applicationDbContext.Dispose();
            sqliteConnection.Close();
        }

        [Test]
        public void CreateRole_returnsView_()
        {
            // Arrange
            var uut = new AdministrationController(_roleManager, _userManager);

            // Act
            var result = uut.CreateRole() as ViewResult;

            //Assert
            Assert.AreEqual("CreateRole", result.ViewName);
        }

        [Test]
        public void Test_ListRole_returnsView()
        {
            // Arrange
            var RoleStore = new Mock<IRoleStore<IdentityRole>>();
           
            var uut = new AdministrationController(_roleManager, _userManager);

            // Act
            var result = uut.CreateRole() as ViewResult;
            var model = result.ViewData.Model;
            var roles = _roleManager.Roles;

            //Assert
            Assert.AreEqual("ListRoles", result.ViewName);
        }

    }
}
