using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AseTrader.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public RegisterController(ApplicationDbContext context, UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterController> logger,
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

                    var confirmationLink = Url.Action("ConfirmEmail", "ConfirmEmail",
                                            new { userId = newUser.Id, token = token }, Request.Scheme); //added (30/04) building confirmation link/url

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
                    return View("../Account/ErrorHandlingView");


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
            return View("../Account/Register");
        }
    }
}