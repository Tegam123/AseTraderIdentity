using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AseTrader.Data;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Authorization;
using AseTrader.Models;
using Microsoft.AspNetCore.Identity;
using AseTrader.Models.ViewModels;

namespace AseTrader.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            var user = await userManager.GetUserAsync(User);
            var test = _context.Users.Where(t => t.Id == user.Id).Include(p => p.Following).First();

            IEnumerable<Post> posts = new List<Post>();


            var mineposts = _context.Posts.Where(p => p.ApplicationUserId == user.Id).ToList();

            posts = posts.Concat(mineposts);
            
           
            foreach(var followersPosts in test.Following)
            {
                posts = posts.Concat(_context.Posts.Where(p => p.ApplicationUserId == followersPosts.followersId).Include(p => p.ApplicationUser).ToList());
            }


            var vm = new PostsViewModel();
            vm.Posts = posts.OrderByDescending(x => x.Date);
            return View(vm);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostsViewModel post, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                var p = new Post();
                p.Comment = post.CurrentPost.Comment;
                p.Date = DateTime.Now;
                p.ApplicationUser = await userManager.GetUserAsync(User);
                _context.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", post.CurrentPost.ApplicationUserId);

            var query = _context.Posts.Include(p => p.ApplicationUser);
            var vm = new PostsViewModel();
            vm.Posts = await query.ToListAsync();

            return View("Index", vm);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(long id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
