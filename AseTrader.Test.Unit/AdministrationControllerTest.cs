using System;
using System.Threading.Tasks;
using AseTrader.Controllers;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using NUnit.Framework;
using Xunit;

namespace AseTrader.Test.Unit
{
    [TestFixture]
    public class AdministrationControllerTest
    {

        private AdministrationController uut;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;

        public static Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(
                roleStore.Object, null, null, null, null);

        }

        public class FakeUserManager : UserManager<User>
        {
            public FakeUserManager() : base(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object)
            { }
        }


        public class FakeRoleManager : RoleManager<IdentityRole>
        {
            public FakeRoleManager() : base(new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object)
            { }
        }
        [SetUp]
        public void SetUp()
        {
            //Set up 

           
            _roleManager = new FakeRoleManager();
            _userManager = new FakeUserManager();
             
            uut = new AdministrationController(_roleManager,_userManager);

        }


        [Test]
        public void CreateRole_ReturnsAView_ForCreatingARole()
        {
            
            // Act
            var result =  uut.CreateRole() as ViewResult;

            // Assert 
            Xunit.Assert.IsType<ViewResult>(result);
        }
    }
}
