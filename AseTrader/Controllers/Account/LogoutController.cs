using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AseTrader.Controllers
{
    public class LogoutController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        public IConfiguration Configuration { get; set; }

        public LogoutController(SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            Configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
    }
}