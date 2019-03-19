using System;
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
                return Redirect("~/admin");
            }

            return View();
        }

        // GET: Index
        public IActionResult Index()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // GET: Config
        public IActionResult Config()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // GET: Audit
        public IActionResult Audit()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // GET: PacketList
        public async Task<IActionResult> PacketList()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View(await _context.Packet.ToListAsync());
        }

        // GET: PacketCreate
        public IActionResult PacketCreate()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // POST: PacketCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PacketCreate([Bind("Id,DateTime,PacketType,DestinationHostname,OriginHostname")] Packet packet)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (ModelState.IsValid)
            {
                _context.Add(packet);
                await _context.SaveChangesAsync();
                return Redirect("~/packets");
            }
            return View(packet);
        }

        // GET: PacketDelete
        public async Task<IActionResult> PacketDelete(int? id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (id == null)
                return NotFound();

            var packet = await _context.Packet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packet == null)
                return NotFound();

            return View(packet);
        }

        // POST: PacketDelete
        [HttpPost, ActionName("PacketDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PacketDeleteConfirmed(int id)
        {
            if (!LoggedIn())
                Redirect("~/login");

            var packet = await _context.Packet.FindAsync(id);
            _context.Packet.Remove(packet);
            await _context.SaveChangesAsync();
            return Redirect("~/packets");
        }

        private bool PacketExists(int id)
        {
            return _context.Packet.Any(e => e.Id == id);
        }

        // GET: UserList
        public async Task<IActionResult> UserList()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View(await _context.User.ToListAsync());
        }

        // GET: UserCreate
        public IActionResult UserCreate()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // POST: UserCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate([Bind("Id,Username,Password")] User user)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Redirect("~/users");
            }
            return View(user);
        }

        // GET: UserDelete
        public async Task<IActionResult> UserDelete(int? id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (id == null)
                return NotFound();

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: UserDelete
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserDeleteConfirmed(int id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return Redirect("~/users");
        }

        // GET: UserEdit
        public async Task<IActionResult> UserEdit(int? id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (id == null)
                return NotFound();

            var user = await _context.User.FindAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        // POST: UserEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(int id, [Bind("Id,Username,Password")] User user)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Redirect("~/users");
            }
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}