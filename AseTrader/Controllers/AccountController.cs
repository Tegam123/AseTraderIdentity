using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.dto;
using AseTrader.Models.EntityModels;
using AseTrader.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace AseTrader.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public AccountController(ApplicationDbContext context, UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    UserName = user.Email,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,

                };

                var userCreationResult = await _userManager.CreateAsync(newUser, user.Password);

                if (userCreationResult.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser); //added (30/04) generating confirmation token for user upon creation; next we build confirmation link/url

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new {userId = newUser.Id, token = token}, Request.Scheme); //added (30/04) building confirmation link/url
                    
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    //if (user.Email.ToLower() == "davidbaekhoj@hotmail.com")
                    //{
                    //    //var adminClaim = new Claim("Admin",);
                    //}

                    var mailMessage = new MailMessage("asetrader2@gmail.com", newUser.Email, "Confirmation email ASE Trader", confirmationLink); // actual message; inhereting from/Implements IDisposaple (https://docs.microsoft.com/en-us/dotnet/api/system.net.mail.mailmessage?view=netcore-3.1)

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587); //(465)config for free email - not viable for many emails 
                    
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential("asetrader2@gmail.com", "PassWord1!");

                    smtpClient.Send(mailMessage);

                    ViewBag.ErrorTitle = "Registration succesful";
                    ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                           "email, by clicking on the confirmation link we have emailed you";
                    return View("EmailConfirmation");


                    //commenting out following 2 lines (30/04) to make sure user cannot login before registration confirmation
                    //adding above 4 lines -- needed for registration confirmation
                    //await _signInManager.SignInAsync(newUser, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                }

                foreach (var error in userCreationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return BadRequest(ModelState);
            }

            return View(user);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


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
                return View("NotFound");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("EmailConfirmation");

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

            return View(model);
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
                                    (await  _userManager.CheckPasswordAsync(user, model.Password))) // last condition ensures provided username and password is correct (avoiding brute force attacks and account enumerations)
                {
                    ModelState.AddModelError(string.Empty,"Email not yet confirmed");
                    return View(model);
                }
                //--- 

                var result = await _signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false); // changed (30/04) to accomodate RememberMe

                if (result.RequiresTwoFactor)
                {
                    
                }
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))  // BS: added inner if statement (30/04)
                    {
                        return Redirect(returnUrl);
                    }
                    else
                        return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
           
            return new ChallengeResult(provider, properties);
        }


        [AllowAnonymous]
        public async Task<ActionResult> 
            ExternalLoginCallback(string returnUrl = null, string remoteError = null)
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

                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");

                return View("Login", loginViewModel);
            }

            // added (30/04) 
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            User user = null;

            if (email != null) //checking to see if we have recieved email from external provider
            {
                user = await _userManager.FindByEmailAsync(email); // then find user

                if (user != null && !user.EmailConfirmed) // by now user is already authenticated by external provider
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginViewModel);
                }
            }

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

                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new {userId = user.Id, token = token}, Request.Scheme);

                        _logger.Log(LogLevel.Warning, confirmationLink);

                        var mailMessage = new MailMessage("brianstjernholm@hotmail.com", user.Email, confirmationLink, null);

                        ViewBag.ErrorTitle = "Registration succesful";
                        ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                                               "email, by clicking on the confirmation link we have emailed you";
                        return View("EmailConfirmation");
                    }

                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                ViewBag.ErrorTitle = $"Email claim not recieved from: {info.LoginProvider}";
                ViewBag.Errormassage = $"Please contact support on kildahl@support.dk";

                return View("Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


        //[HttpPost("Jwtlogin")]
        //public async Task<IActionResult> JWTlogin([FromBody] LoginViewModel loginUser)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginUser.Email);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError(string.Empty, "Invalid login");
        //        return BadRequest(ModelState);
        //    }

        //    var passwordSignInResult = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
        //    if (passwordSignInResult.Succeeded)
        //    {
        //        return new ObjectResult(GenerateToken(loginUser.Email));
        //    }

        //    return BadRequest("Invalid login");
        //}




        //private string GenerateToken(string username)
        //{
        //    var claims = new Claim[]
        //    {
        //        new Claim(ClaimTypes.Name, username),
        //        new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
        //    };

        //    var token = new JwtSecurityToken(
        //        new JwtHeader(new SigningCredentials(
        //            new SymmetricSecurityKey(Encoding.UTF8.GetBytes("the secret that needs to be at least 16 characeters long for HmacSha256")),
        //            SecurityAlgorithms.HmacSha256)),
        //        new JwtPayload(claims));

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}