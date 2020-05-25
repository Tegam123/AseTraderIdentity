// ***********************************************************************
// Assembly         : AseTrader
// Author           : Mikkel & Lena
// Created          : 04-24-2020
//
// Last Modified By : Mikkel
// Last Modified On : 05-12-2020
// ***********************************************************************
// <copyright file="PortfolioController.cs" company="AseTrader">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AseTrader.Models;
using AseTrader.Models.Portfolio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AseTrader.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            var _user = await userManager.GetUserAsync(User);

            IPortfolio p = new Portfolio(_user.secret_accesstoken);

            PortfolioMapper mapper = p.SeePortfolio();

            return View(mapper);
        }
    }
}