﻿using System;
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
            //var users = from u in _context.User select u;
            var users = from u in _context.Users select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.FirstName.Contains(searchString));
            }

            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var user = await _context.User
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FirstName,LastName,Email,PwHash,AccountType")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        //// GET: Users/Edit/5
        //public async Task<IActionResult> Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //var user = await _context.User.FindAsync(id);
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("UserId,FirstName,LastName,Email,PwHash,AccountType")] User user)
        //{
        //    if (id != user.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(user);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UserExists(user.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(user);
        //}

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var user = await _context.User
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            //var user = await _context.User.FindAsync(id);
            //_context.User.Remove(user);
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            //return _context.User.Any(e => e.UserId == id);
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe(string? id, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                var friend = await userManager.FindByIdAsync(id);
                var user = await userManager.GetUserAsync(User);

                if (await _context.Follow.Where(m => m.followersId == friend.Id).Where(m => m.followingId == user.Id)
                    .SingleOrDefaultAsync() == null)
                {
                    var follower = new Follow() { Followers = friend, Following = user };
                    _context.Add(follower);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index", "Users");
            }
            return RedirectToAction("Index", "Users");
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(string? id, [FromServices]UserManager<User> userManager)
        {
            if (ModelState.IsValid)
            {
                var friend = await userManager.FindByIdAsync(id);
                var user = await userManager.GetUserAsync(User);

                if (await _context.Follow.Where(m => m.followersId == friend.Id).Where(m => m.followingId == user.Id)
                    .SingleOrDefaultAsync() == null)
                {
                    return RedirectToAction("Index", "Users");
                }

                var follower = await _context.Follow.Where(m => m.followersId == friend.Id).Where(m => m.followingId == user.Id).SingleOrDefaultAsync();
                //var test = await _context.Users.FindAsync(follower);
                _context.Remove(follower);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Users");
            }
            return RedirectToAction("Index", "Users");
        }
    }
}
