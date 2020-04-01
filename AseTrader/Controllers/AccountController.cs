using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.dto;
using AseTrader.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AseTrader.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; set; }

        public AccountController(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
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
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Home", "Users");
                }

                foreach (var error in userCreationResult.Errors)
                {
                    ModelState.AddModelError(string.Empty,error.Description);
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


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,
                    model.Password, true, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "Users");
                }


                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }
            return View(model);
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



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }




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