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

namespace AseTrader.Controllers
{
    public class PortfolioController : Controller
    {

        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            var _user = await userManager.GetUserAsync(User);

            Portfolio p = new Portfolio(_user.secret_accesstoken);

            PortfolioMapper mapper = p.SeePortfolio();

           return View(mapper);
        }
    }
}
