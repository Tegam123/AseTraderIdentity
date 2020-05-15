using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AseTrader.Data;
using AseTrader.Models;
using AseTrader.Models.EntityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AseTrader.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
            
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchString)
        {
            //Hvis der er skrevet i søgebaren, så søger den databasen igennem for dette.
            if (!String.IsNullOrEmpty(searchString))
            {
                var users = _context.Users.Where(u => u.FirstName.Contains(searchString));   
            }

            //Returnerer brugerer.
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Finder brugeren ud fra id'et.
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        //// GET: Users/Delete/5
        //public async Task<IActionResult> Delete(string? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //Finder brugeren ud fra id'et.
        //    var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(long id)
        //{
        //    //Finder brugeren
        //    var user = await _context.Users.FindAsync(id);

        //    //Fjerner brugeren.
        //    _context.Users.Remove(user);

        //    //Gemmer ændringer.
        //    await _context.SaveChangesAsync();

        //    //Returnerer til action(Index).
        //    return RedirectToAction(nameof(Index));
        //}


        [HttpGet]
        public async Task<IActionResult> Subscribe(string? id, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                //Finder brugeren man ønsker at følge.
                var userIWantToFollow = await userManager.FindByIdAsync(id);

                //Finder brugeren der er logget ind.
                var user = await userManager.GetUserAsync(User);

                //Tjekker at brugeren ikke er i ens follow list.
                if (await _context.Follow.Where(m => m.followersId == userIWantToFollow.Id).Where(m => m.followingId == user.Id).SingleOrDefaultAsync() == null)
                {
                    //Opretter ny følger.
                    var follower = new Follow() { Followers = userIWantToFollow, Following = user };

                    //Tilføjer brugeren til databasen.
                    _context.Add(follower);

                    //Gemmer ændringen i databasen.
                    await _context.SaveChangesAsync();
                }

                //Returnerer til action(Index) i users.
                return RedirectToAction("Index", "Users");
            }
            // Returnerer til action(Index) i users.
            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(string? id, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                //Finder brugeren man ønsker at følge.
                var userIWantToUnfollow = await userManager.FindByIdAsync(id);

                //Finder brugeren der er logget ind.
                var user = await userManager.GetUserAsync(User);

                //Tjekker om brugeren er i listen i forvejen.
                if (await _context.Follow.Where(m => m.followersId == userIWantToUnfollow.Id).Where(m => m.followingId == user.Id).SingleOrDefaultAsync() == null)
                {
                    return RedirectToAction("Index", "Users");
                }

                //Finder brugeren man ikke vil følge længere ud fra id'et.
                var stopFollowing = await _context.Follow.Where(m => m.followersId == userIWantToUnfollow.Id).Where(m => m.followingId == user.Id).SingleOrDefaultAsync();

                //Fjerner brugeren fra databasen.
                _context.Remove(stopFollowing);

                //Gemmer ændringer i databasen.
                await _context.SaveChangesAsync();

                // Returnerer til action(Index) i users.
                return RedirectToAction("Index", "Users");
            }
            // Returnerer til action(Index) i users.
            return RedirectToAction("Index", "Users");
        }
    }
}
