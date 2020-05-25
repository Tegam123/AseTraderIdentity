// ***********************************************************************
// Assembly         : AseTrader
// Author           : Mikkel & Lena
// Created          : 04-24-2020
//
// Last Modified By : Mikkel
// Last Modified On : 04-28-2020
// ***********************************************************************
// <copyright file="TradingController.cs" company="AseTrader">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.BuyStock;
using AseTrader.Models.SellStock;
using AseTrader.Models.Stock_info_builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AseTrader.Controllers
{
    [Authorize]
    public class TradingController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<User> _userManager;

        public TradingController(ApplicationDbContext context, [FromServices]UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> TradingRecieverCode([FromQuery] string code)
        {
            object? model = code;

            var client = new RestClient("https://api.alpaca.markets/oauth/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", model);
            request.AddParameter("client_id", "594f6429f720875517565db7db39a584");
            request.AddParameter("client_secret", "d126b090b64c5310aa35b8e3cb7ce29661a4d8fa");
            request.AddParameter("redirect_uri", "https://asetrader20200424150538.azurewebsites.net/Trading/TradingRecieverCode");

            IRestResponse response = client.Execute(request);

            //Accesstoken contains the token which shall be used to buy stocks for a specific user.
            var parsed_accesstoken = JObject.Parse(response.Content);
            var accesstoken = parsed_accesstoken["access_token"].ToString();

            var user = await _userManager.GetUserAsync(User);

            user.secret_accesstoken = accesstoken;

            await _userManager.UpdateAsync(user);

            _context.Update(user);

            _context.SaveChanges();

            return View("Trading");
        }

        public async Task<IActionResult> TradingAsync([FromServices]UserManager<User> userManager)
        {
            var user = await _userManager.GetUserAsync(User);
            return View();
        }

        public IActionResult Tester()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult Buy_SI_Stocks_dispatcher(StockBuilder builder)
        {
            var containsInt = builder.stock_symbol.Any(char.IsDigit);

            if (builder.stock_symbol is string && builder.stock_symbol == builder.stock_symbol.ToUpper() && builder.quantity is int && builder.price is decimal && containsInt == false && builder.quantity != 0 && builder.price != 0)
            {
                Buy_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price, builder.email);
                return Ok();
            }

            return BadRequest();
        }

        private async Task<User> GetCurrentUser(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task Buy_SI_Stocks(string sym, int quantity, decimal price, string email)
        {
            IBuyStock buyStock = new BuyStock();

            var currentUser = await GetCurrentUser(email);

            buyStock.BuyStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }

        [HttpPost]
        public ActionResult Sell_SI_Stocks_dispatcher(SellStockBuilder builder)
        {
            var containsInt = builder.stock_symbol.Any(char.IsDigit);

            if (builder.stock_symbol is string && builder.stock_symbol == builder.stock_symbol.ToUpper() && builder.quantity is int && builder.price is decimal && containsInt == false && builder.quantity != 0 && builder.price != 0)
            {
                Sell_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price, builder.email);
                return Ok();
            }

            return BadRequest();
        }

        public async Task Sell_SI_Stocks(string sym, int quantity, decimal price, string email)
        {
            ISellStock sellStock = new SellStock();

            var currentUser = await GetCurrentUser(email);

            sellStock.SellStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }

        [HttpPost]
        public IActionResult SetTheme(string data)
        {
            CookieOptions cookies = new CookieOptions();
            cookies.Expires = DateTime.Now.AddDays(1);

            Response.Cookies.Append("theme", data, cookies);
            return Ok();
        }
    }
}