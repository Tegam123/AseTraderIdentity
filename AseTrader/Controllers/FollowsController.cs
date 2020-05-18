using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Identity;

namespace AseTrader.Controllers
{
    public class FollowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Follows - Laver en liste af followers
        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            var user = await userManager.GetUserAsync(User);

            var following = _context.Follow.Where(mp => mp.followingId == user.Id).Include(f => f.Followers).ToList();

            return View(following);
        }


    }
}
