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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AseTrader.Controllers
{
    
    public class TradingController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

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
        public bool Buy_SI_Stocks_dispatcher(StockBuilder builder)
        {
            
            Buy_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price);

            return builder != null;

        }

        private async Task<User> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public async Task Buy_SI_Stocks(string sym, int quantity, decimal price)
        {
            IBuyStock buyStock = new BuyStock();
            IAlpacaClient alpacaClient = new AlpacaClient();

            var currentUser = await GetCurrentUser();

            buyStock.BuyStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }

        [HttpPost]
        public bool Sell_SI_Stocks_dispatcher(SellStockBuilder builder)
        {
          
            Sell_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price);

            return builder != null;

        }

        public async Task Sell_SI_Stocks(string sym, int quantity, decimal price)
        {
            ISellStock sellStock = new SellStock();

            var currentUser = await GetCurrentUser();

            sellStock.SellStocks_http(sym, quantity, price, currentUser.secret_accesstoken);
        }


        //Test under here
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
