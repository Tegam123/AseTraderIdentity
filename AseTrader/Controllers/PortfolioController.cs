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

namespace AseTrader.Controllers
{
    public class PortfolioController : Controller
    {

        public async Task<IActionResult> Index()
        {
            //IAlpacaClient alpacaClient = new AlpacaClient();
            //var positions = await alpacaClient.AlpacaClientTrading().ListPositionsAsync();

            var client = new RestClient("https://paper-api.alpaca.markets/v2/positions");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer 34bb3413-9fa3-407a-9087-19999d1e8e66");
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