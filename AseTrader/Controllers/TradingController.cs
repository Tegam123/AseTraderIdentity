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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AseTrader.Controllers
{
    
    public class TradingController : Controller
    {

        private readonly ApplicationDbContext _context;

        public TradingController(ApplicationDbContext context)
        {
            _context = context;
        }
        //private readonly ILogger<TradingController> _logger;

        //public TradingController(ILogger<TradingController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private static string _accesstokens = "";
        public IActionResult TradingRecieverCode([FromQuery] string code)
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
            request.AddParameter("redirect_uri", "https://tradingplatform20200325081838.azurewebsites.net/Home/TradingRecieverCode");
            IRestResponse response = client.Execute(request);


            //Accesstoken contains the token which shall be used to buy stocks for a specific user. 
            //This should therefore be protected from hackers.
            //var accesstoken = JsonConvert.DeserializeObject(response.Content);

            var parsed_accesstoken = JObject.Parse(response.Content);
            var accesstoken = parsed_accesstoken["access_token"].ToString();
            _accesstokens = accesstoken;

            var u = new User();
            u.secret_accesstoken = accesstoken;
            _context.Add(u);

            //The final acecesstoken is placed accesstoken, need to be placed in database.
            return Content(accesstoken);
            return View(code);
        }



        public IActionResult Trading()
        {
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

        public static async Task Buy_SI_Stocks(string sym, int quantity, decimal price)
        {
            IBuyStock buyStock = new BuyStock();
            IAlpacaClient alpacaClient = new AlpacaClient();

            buyStock.BuyStocks_http(sym, quantity, price, _accesstokens);
        }

        [HttpPost]
        public bool Sell_SI_Stocks_dispatcher(SellStockBuilder builder)
        {
          
            Sell_SI_Stocks(builder.stock_symbol, builder.quantity, builder.price);

            return builder != null;

        }

        public static async Task Sell_SI_Stocks(string sym, int quantity, decimal price)
        {
            ISellStock sellStock = new SellStock();
            

            sellStock.SellStocks_http(sym, quantity, price, _accesstokens);
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
