using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;

namespace NetworkVisualizer.Controllers
{
    public class AdminController : Controller
    {
        private readonly NetworkVisualizerContext _context;
        public AdminController(NetworkVisualizerContext context)
        {
            _context = context;
        }

        // Check if the login cookie exists
        private bool LoggedIn()
        {
            if (Request.Cookies["isLoggedIn"] == "true")
                return true;
            return false;
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login with user info, if in db, add a login cookie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Id,Username,Password")] User user)
        {
            if (ModelState.IsValid && _context.User.FirstOrDefault(
                acc => acc.Username == user.Username && acc.Password == user.Password) != null)
            {
                Response.Cookies.Append("isLoggedIn", "true", new CookieOptions
                {
                    Path = "/",
                    IsEssential = true,
                    Expires = DateTime.Now.AddMinutes(5)
                });
                return Redirect("../admin");
            }

            return View();
        }

        // GET: Index
        public IActionResult Index()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View();
        }

        // GET: PacketList
        public async Task<IActionResult> PacketList()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View(await _context.Packet.ToListAsync());
        }
    }
}