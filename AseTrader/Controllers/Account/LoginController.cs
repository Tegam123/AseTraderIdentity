using System.Linq;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AseTrader.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public LoginController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            Configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View("../Account/Login", model);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) //BS: added second param (30/04)
        {
            //added to avoid error in if(ModelState.IsValid)
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                //added to ensure correct error message if user has not confirmed registration---
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && !user.EmailConfirmed &&
                    (await _userManager.CheckPasswordAsync(user, model.Password))) // last condition ensures provided username and password is correct (avoiding brute force attacks and account enumerations)
                {
                    ModelState.AddModelError(string.Empty, "Email not yet confirmed");
                    return View("../Account/Login", model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false); // changed (30/04) to accomodate RememberMe


                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))  // BS: added inner if statement (30/04)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("/Areas/Identity/Pages/Account/LoginWith2fa", new { ReturnUrl = returnUrl });
                    //return RedirectToPage("/Areas/Identity/Pages/Account/LoginWith2fa", new { ReturnUrl = returnUrl });
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View("../Account/Login", model);
        }
    }
}