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


            foreach (var f in test.Following)
            {
                var query = _context.Posts.Where(p => p.ApplicationUserId == f.followersId).ToList();
                posts = posts.Concat(query);
            }


            //var test2 = _context.Posts.Include(p => p.ApplicationUser);
            var vm = new PostsViewModel();
            vm.Posts = posts;
            return View(vm);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostsViewModel post, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                var p = new Post();
                p.Comment = post.CurrentPost.Comment;
                p.ApplicationUser = await userManager.GetUserAsync(User);
                _context.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", post.CurrentPost.ApplicationUserId);

            //var user = await userManager.GetUserAsync(User);
            //var test = _context.Users.Find(user.Id);
            //    //.Include(p => p.Following).ToList();


            

            var query = _context.Posts.Include(p => p.ApplicationUser);
            var vm = new PostsViewModel();
            vm.Posts = await query.ToListAsync();


            return View("Index", vm);
        }


        //var friend = await userManager.FindByIdAsync(id);
        //var user = await userManager.GetUserAsync(User);

        //    if (await _context.Follow.Where(m => m.followersId == friend.Id).Where(m => m.followingId == user.Id)
        //.SingleOrDefaultAsync() == null)






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
