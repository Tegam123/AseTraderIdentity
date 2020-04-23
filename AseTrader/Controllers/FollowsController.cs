using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AseTrader.Data;
using AseTrader.Models.EntityModels;

namespace AseTrader.Controllers
{
    public class FollowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FollowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Follows
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Follow.Include(f => f.Followers).Include(f => f.Following);
            return View(await applicationDbContext.ToListAsync());
        }
    }
}
