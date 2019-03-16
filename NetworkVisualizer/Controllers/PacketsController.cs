using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;

namespace NetworkVisualizer.Controllers
{
    public class PacketsController : Controller
    {
        private readonly NetworkVisualizerContext _context;

        public PacketsController(NetworkVisualizerContext context)
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
            if (ModelState.IsValid && _context.User.Contains(_context.User.FirstOrDefault(
                acc => acc.Username == user.Username && acc.Password == user.Password)))
            {
                Response.Cookies.Append("isLoggedIn", "true", new CookieOptions
                {
                    Path = "/",
                    Expires = DateTime.Now.AddMinutes(5)
                });
                return Redirect("../packets");
            }

            return View();
        }

        // GET: Packets
        public async Task<IActionResult> Index()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View(await _context.Packet.ToListAsync());
        }

        // GET: Packets/Create
        public IActionResult Create()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View();
        }

        // POST: Packets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,PacketType,DestinationHostname,OriginHostname")] Packet packet)
        {
            if (!LoggedIn())
                return Redirect("../login");

            if (ModelState.IsValid)
            {
                _context.Add(packet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(packet);
        }

        // GET: Packets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!LoggedIn())
                return Redirect("../login");

            if (id == null)
                return NotFound();

            var packet = await _context.Packet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packet == null)
                return NotFound();

            return View(packet);
        }

        // POST: Packets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!LoggedIn())
                Redirect("../login");

            var packet = await _context.Packet.FindAsync(id);
            _context.Packet.Remove(packet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PacketExists(int id)
        {
            return _context.Packet.Any(e => e.Id == id);
        }
    }
}
