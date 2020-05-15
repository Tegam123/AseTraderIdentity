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
            //IAlpacaClient alpacaClient = new AlpacaClient();
            //var positions = await alpacaClient.AlpacaClientTrading().ListPositionsAsync();

            var _user = await userManager.GetUserAsync(User);

            var client = new RestClient("https://paper-api.alpaca.markets/v2/positions");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", $"Bearer {_user.secret_accesstoken}");
            IRestResponse response = client.Execute(request);
            //Console.WriteLine(response.Content);

            var JsonObj_PortfolioData = JsonConvert.DeserializeObject(response.Content);

            PortfolioMapper mapper = new PortfolioMapper();

            mapper.TradingInfo = JsonObj_PortfolioData;

            //return Content(mapper.TradingInfo);

           return View(mapper);
        }
    }
}