using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Models.Alpaca_dependency;
using AseTrader.Models.Portfolio;
using Microsoft.AspNetCore.Mvc;

namespace AseTrader.Controllers
{
    public class PortfolioController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            IAlpacaClient alpacaClient = new AlpacaClient();
            var positions = await alpacaClient.AlpacaClientTrading().ListPositionsAsync();

            return View(new Portfolio(positions));
        }
    }
}