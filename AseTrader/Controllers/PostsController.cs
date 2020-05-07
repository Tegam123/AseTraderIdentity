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

        // GET: Posts - Laver en liste af posts
        public async Task<IActionResult> Index([FromServices]UserManager<User> userManager)
        {
            //Returnerer brugeren af systemet.
            var user = await userManager.GetUserAsync(User);

            //Finder brugere brugeren følger.
            var userFollow = _context.Users.Where(user => user.Id == user.Id).Include(user => user.Following).First();

            //Finder mine posts.
            var myPosts = _context.Posts.Where(mp => mp.ApplicationUserId == user.Id).ToList();

            //Laver ny liste af posts da man ikke vil have alle posts i systemet, bare de posts fra dem brugeren følger.
            IEnumerable<Post> posts = new List<Post>();

            //Smider mine posts i den nye liste.
            posts = posts.Concat(myPosts);

            //Smider followers posts i listen.
            foreach (var followersPosts in userFollow.Following)
            {
                posts = posts.Concat(_context.Posts.Where(p => p.ApplicationUserId == followersPosts.followersId).Include(p => p.ApplicationUser).ToList());
            }

            //Sørger for at bruge PostsViewModel, så man både kan se og oprette posts på samme vindue.
            var pvm = new PostsViewModel();

            //Sorterer posts i dato rækkefølge.
            pvm.Posts = posts.OrderByDescending(p => p.Date);

            //Returnerer mine og follwers posts.
            return View(pvm);
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostsViewModel post, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                //Opretter et ny post.
                var p = new Post();

                //Sætter den indskrevne tekst = comment parameteren.
                p.Comment = post.CurrentPost.Comment;

                //Sætter dagens dato og tid = date parameteren.
                p.Date = DateTime.Now;

                //Finder brugeren der er logget ind.
                p.ApplicationUser = await userManager.GetUserAsync(User);

                //Tilføjer postet til databasen.
                _context.Add(p);

                //Gemmer ændringerne i databasen.
                await _context.SaveChangesAsync();

                //Returnerer os til action(Index).
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", post.CurrentPost.ApplicationUserId); //Tjek om det bliver brugt.

            //Finder brugerens posts.
            var userPosts = _context.Posts.Include(p => p.ApplicationUser);

            //Sørger for at bruge PostsViewModel.
            var pvm = new PostsViewModel();

            //Lægger posts over i pvm.
            pvm.Posts = await userPosts.ToListAsync();

            //Returnerer til index view.
            return View("Index", pvm);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Finder den specifikke post ud fra postId'et og sletter den.
            var post = await _context.Posts.Include(p => p.ApplicationUser).FirstOrDefaultAsync(m => m.PostId == id);

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
            //Finder post i databasen.
            var post = await _context.Posts.FindAsync(id);

            //Sletter fra databasen.
            _context.Posts.Remove(post);

            //Gemmer ændringen.
            await _context.SaveChangesAsync();

            //Returnerer os til action(Index).
            return RedirectToAction(nameof(Index));
        }
    }
}
