using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AseTrader.Controllers
{
    public class ConfirmEmailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ConfirmEmailController> _logger;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public ConfirmEmailController(ApplicationDbContext context, UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ConfirmEmailController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            Configuration = configuration;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.Errormessage = $"The User ID {userId} is invalid";
                return View("../Account/NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View("../Account/ConfirmEmail");
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("../Account/ErrorHandlingView");
        }

    }
}