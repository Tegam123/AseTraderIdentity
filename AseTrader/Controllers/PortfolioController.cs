// ***********************************************************************
// Assembly         : AseTrader
// Author           : Mikkel
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Models.Alpaca_dependency;
using AseTrader.Models.Portfolio;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// The Controllers namespace.
/// </summary>
namespace AseTrader.Controllers
{

    /// <summary>
    /// Class PortfolioController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    public class PortfolioController : Controller
    {


        /// <summary>
        /// Indexes the specified user manager.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            var _user = await userManager.GetUserAsync(User);

            Portfolio p = new Portfolio(_user.secret_accesstoken);

            PortfolioMapper mapper = p.SeePortfolio();

           return View(mapper);
        }
    }
}
