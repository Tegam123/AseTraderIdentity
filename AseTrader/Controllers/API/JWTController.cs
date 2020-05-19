using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AseTrader.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Account")]
    [Authorize(AuthenticationSchemes = "Identity.Application" + "," +JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class JWTController : Controller
    {

        readonly ApplicationDbContext _context;
        //readonly IMapper _mapper;
        readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public JWTController(ApplicationDbContext context, 
             
            IConfiguration configuration,
            UserManager<User> userManager,
            SignInManager<User> signInManager
            )
        {
            _context = context;
           // _mapper = mapper;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        

        [HttpGet("Test")]
        public List<string> GetCustomer()
        {
            return new List<string>(){ "Test","Test1"};

        }

        


        [AllowAnonymous]
        [HttpPost("jwtLogin")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel viewuser)
        {
            var user = await _userManager.FindByEmailAsync(viewuser.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login");
                return BadRequest(ModelState);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, viewuser.Password, false);
            if (result.Succeeded)
            {
                return  new ObjectResult(GenerateToken(user));
            }

            return BadRequest("Invalid login");
        }

        private string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddHours(3)).ToUnixTimeSeconds().ToString()),
            };

            var secret = _configuration["JwtSecretKey"];
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(new JwtHeader(credentials), new JwtPayload(claims));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}