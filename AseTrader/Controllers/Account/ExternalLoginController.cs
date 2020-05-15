using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
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
    public class ExternalLoginController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<ExternalLoginController> _logger;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public ExternalLoginController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<ExternalLoginController> logger,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            Configuration = configuration;
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "ExternalLogin",
                new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties); //Implements IactionResult and redirects the user to the providers loginpage
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("../Account/Login", loginViewModel);
            }

            //Henter logininfo
            var info = await _signInManager.GetExternalLoginInfoAsync();
            //Info indeholder: Providerkey: to uniqly identify this user, Loginprovider"Google", Principal:System.security.Claims:claimsprincipal
            //Principals claim indeholder bl.a. brugernavn, fornavn, efternavn og emailadresse
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");

                return View("../Account/Login", loginViewModel);
            }

            // added (30/04) 
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            User user = null;

            if (email != null) //checking to see if we have recieved email from external provider
            {
                user = await _userManager.FindByEmailAsync(email); // then check if user have a local useraccount

                if (user != null && !user.EmailConfirmed) // by now user is already authenticated by external provider
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("../Account/Login", loginViewModel);
                }
            }

            //tjekker om vi kan signe bruger ind med oplysninger fra external provider - om bruger findes i dbo.AspNetUserLogins i vores database
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await _userManager.CreateAsync(user);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        var confirmationLink = Url.Action("ConfirmEmail", "Register",
                            new { userId = user.Id, token = token }, Request.Scheme);

                        _logger.Log(LogLevel.Warning, confirmationLink);

                        var mailMessage = new MailMessage("asetrader2@gmail.com", user.Email, "Confirmation email ASE Trader", confirmationLink);

                        SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); //(465)config for free email - not viable for many emails 

                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new NetworkCredential("asetrader2@gmail.com", "PassWord1!");

                        smtpClient.Send(mailMessage);

                        ViewBag.ErrorTitle = "Registration succesful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                               "email, by clicking on the confirmation link we have emailed you";
                        return View("../Account/ErrorHandlingView");
                    }

                    await _userManager.AddLoginAsync(user, info); //tilføjer brugerens oplysninger til dbo.AspNetUserLogins med foreign key til asp.net.user table
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not recieved from: {info.LoginProvider}";
                ViewBag.Errormassage = $"Please contact support on kildahl@support.dk";

                return View("../Account/ErrorHandlingView");
            }
        }

    }
}