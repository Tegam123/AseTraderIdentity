using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AseTrader.Controllers.API
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class JWTController : Controller
    {

        readonly ApplicationDbContext _context;
        readonly IMapper _mapper;
        readonly IConfiguration _configuration;


        public JWTController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpGet]
        public List<string> GetCustomer()
        {
            return new List<string>(){ "Test","Test1"};

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
            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256)),
            new JwtPayload(claims));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}