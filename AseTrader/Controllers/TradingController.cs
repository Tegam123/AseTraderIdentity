// ***********************************************************************
// Assembly         : AseTrader
// Author           : Mikkel
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
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using AseTrader.Models;
using AseTrader.Models.Stock_info_builder;
using AseTrader.Models.BuyStock;
using AseTrader.Models.SellStock;
using AseTrader.Models.Alpaca_dependency;
using Microsoft.AspNetCore.Authorization;
using AseTrader.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AseTrader.Controllers
{
    [Authorize]
    /// <summary>
    /// Class TradingController.
    /// Implements the <see cref="Microsoft.AspNetCore.Mvc.Controller" />
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    public class TradingController : Controller
    {

        /// <summary>
        /// The context
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// The user manager
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradingController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userManager">The user manager.</param>
        public TradingController(ApplicationDbContext context, [FromServices]UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //private readonly ILogger<TradingController> _logger;

        //public TradingController(ILogger<TradingController> logger)
        //{
        //    _logger = logger;
        //}

        //public IActionResult Index()
        //{

        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}



        //private static string _accesstokens = "34bb3413-9fa3-407a-9087-19999d1e8e66";
        /// <summary>
        /// Tradings the reciever code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> TradingRecieverCode([FromQuery] string code)
        {
            object? model = code;

            //return content is used to see if the code was right.
            //return Content(model);

            var client = new RestClient("https://api.alpaca.markets/oauth/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", model);
            request.AddParameter("client_id", "594f6429f720875517565db7db39a584");
            request.AddParameter("client_secret", "d126b090b64c5310aa35b8e3cb7ce29661a4d8fa");
            request.AddParameter("redirect_uri", "https://asetrader20200424150538.azurewebsites.net/Trading/TradingRecieverCode");
            //request.AddParameter("redirect_uri", "https://localhost:44361/Trading/TradingRecieverCode");
            IRestResponse response = client.Execute(request);
            
        
            //Accesstoken contains the token which shall be used to buy stocks for a specific user. 
            //This should therefore be protected from hackers.
            //var accesstoken = JsonConvert.DeserializeObject(response.Content);

            var parsed_accesstoken = JObject.Parse(response.Content);
            var accesstoken = parsed_accesstoken["access_token"].ToString();
            //_accesstokens = accesstoken;





            //var userid = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            user.secret_accesstoken = accesstoken;

            await _userManager.UpdateAsync(user);

            _context.Update(user);

            _context.SaveChanges();

            

            //The final acecesstoken is placed accesstoken, need to be placed in database.
            //return Content(accesstoken);
            return View("Trading");
        }

        /// <summary>
        /// trading as an asynchronous operation.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <returns>IActionResult.</returns>
        public async Task<IActionResult> TradingAsync([FromServices]UserManager<User> userManager)
        {
            var user = await _userManager.GetUserAsync(User);
            return View();
        }

        /// <summary>
        /// Testers this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        public IActionResult Tester()
        {
            return View();
        }

        /// <summary>
        /// Errors this instance.
        /// </summary>
        /// <returns>IActionResult.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Buys the si stocks dispatcher.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [HttpPost]
        public ActionResult Buy_SI_Stocks_dispatcher(StockBuilder builder)
        {
            var containsInt = builder.stock_symbol.Any(char.IsDigit);

            if (builder.stock_symbol is string && builder.quantity is int && builder.price is decimal && containsInt == false && builder.quantity != 0 && builder.price != 0)
            {
                Buy_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price, builder.email);
                return Ok();
            }

            return BadRequest();

        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        /// <returns>User.</returns>
        private async Task<User> GetCurrentUser(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Buys the si stocks.
        /// </summary>
        /// <param name="sym">The sym.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="price">The price.</param>
        public async Task Buy_SI_Stocks(string sym, int quantity, decimal price, string email)
        {
            IBuyStock buyStock = new BuyStock();
            IAlpacaClient alpacaClient = new AlpacaClient();

            var currentUser = await GetCurrentUser(email);

            buyStock.BuyStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }

        /// <summary>
        /// Sells the si stocks dispatcher.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [HttpPost]
        public ActionResult Sell_SI_Stocks_dispatcher(SellStockBuilder builder)
        {

            var containsInt = builder.stock_symbol.Any(char.IsDigit);

            if (builder.stock_symbol is string && builder.quantity is int && builder.price is decimal && containsInt == false && builder.quantity != 0 && builder.price != 0)
            {
                Sell_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price, builder.email);
                return Ok();
            }

            return BadRequest();

        }

        /// <summary>
        /// Sells the si stocks.
        /// </summary>
        /// <param name="sym">The sym.</param>
        /// <param name="quantity">The quantity.</param>
        /// <param name="price">The price.</param>
        public async Task Sell_SI_Stocks(string sym, int quantity, decimal price, string email)
        {
            ISellStock sellStock = new SellStock();

            var currentUser = await GetCurrentUser(email);

            sellStock.SellStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }


        //Test under here
        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IActionResult.</returns>
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
