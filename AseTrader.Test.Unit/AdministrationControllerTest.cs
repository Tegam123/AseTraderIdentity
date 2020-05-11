using System;
using System.Threading.Tasks;
using AseTrader.Controllers;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace AseTrader.Test.Unit
{
    [TestFixture]
    public class AdministrationControllerTest
    {

        private AdministrationController uut;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context; 

        [SetUp]
        public void SetUp()
        {
            var UserStoreFake = Substitute.For<IUserStore<User>>();
            _userManager =
                Substitute.For<UserManager<User>>(UserStoreFake, null, null, null, null, null, null, null, null);
            var rolestorefake = Substitute.For<IRoleStore<User>>();
            _roleManager = Substitute.For<RoleManager<IdentityRole>>(rolestorefake, null, null, null, null);
            

            uut = new AdministrationController(_roleManager,_userManager);

        }


        [Test]
        public void CreateRole_ReturnsAView_ForCreatingARole()
        {

            // Act
            var result =  uut.CreateRole() as ViewResult;

            // Assert 
            Assert.AreEqual("CreateRole", result.ViewName);
        }
    }
}
