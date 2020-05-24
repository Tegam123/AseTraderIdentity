using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AseTrader.Controllers;
using AseTrader.Models;
using AseTrader.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Xunit;

namespace Administration.Controller.Test
{


    public class AdminstrationController_test_Mocks
    {
        //private IOptions<TokenValidation> tokenValidation;
        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private AdministrationController uut; 

        [SetUp]
        public void SetUp()
        {

            var mockUserStore = new Mock<IUserStore<User>>();
            var userManager = new UserManager<User>(mockUserStore.Object,null,null,null,null,null,null,null,null);

            var IRolStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _roleManager = new RoleManager<IdentityRole>(IRolStoreMock.Object,null,null,null,null);


            IRolStoreMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(IdentityResult.Success);

            uut = new AdministrationController(_roleManager,_userManager);
        }

        [Test]
        public void CreateRole_returnsView_()
        {
            // Arrange
            var uut = new AdministrationController(_roleManager, _userManager);

            // Act
            var result = uut.CreateRole() as ViewResult;

            //Assert
            NUnit.Framework.Assert.AreEqual("CreateRole", result.ViewName);
        }

        [Test]
        public async Task Test_ListRole_returnsView()
        {
            // Arrange
          

            // Act
            var model = new CreateRoleViewModel
            {
                RoleName = "Admin"
            };

            var result = await uut.CreateRole(model) ;

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        }

    }
}
