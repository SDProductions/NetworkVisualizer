using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;

namespace NetworkVisualizer.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly NetworkVisualizerContext _context;
        public AdminController(NetworkVisualizerContext context)
        {
            _context = context;
        }

        // GET: Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: Config
        public IActionResult Config()
        {
            AddAudit("Access - Config");
            return View();
        }

        // GET: Audit
        public IActionResult Audit()
        {
            return View(_context.Audit.ToList());
        }

        // GET: Account
        public IActionResult Account()
        {
            return View();
        }

        // GET: Help
        public IActionResult Help()
        {
            return View();
        }

        // GET: Packets
        public async Task<IActionResult> Packets()
        {
            AddAudit("Access - Packet");
            return View(await _context.Packet.ToListAsync());
        }

        // GET: PacketDelete
        public async Task<IActionResult> PacketDelete(int? id)
        {
            if (id == null)
                return NotFound();

            var packet = await _context.Packet.FindAsync(id);
            _context.Packet.Remove(packet);
            await _context.SaveChangesAsync();

            AddAudit("Delete - Packet");
            return Redirect("~/packets");
        }

        // Delete all packets from specified time (within minute)
        public IActionResult PacketDeleteFromTime(long? id)
        {
            if (!id.HasValue)
                return NotFound();

            DateTime time = new DateTime(id.Value);

            var packets = from packet in _context.Packet
                          where packet.DateTime.Day == time.Day &&
                                packet.DateTime.Hour == time.Hour &&
                                packet.DateTime.Minute == time.Minute
                          select packet;
            _context.Packet.RemoveRange(packets);
            _context.SaveChanges();

            AddAudit($"Delete - {packets.ToList().Count} packets from time {time.AddHours(NetworkVisualizer.Config.config.UTCHoursOffset)}");
            return Redirect("~/packets");
        }

        private void AddAudit(string Action)
        {
            var identity = User.Identity as ClaimsIdentity; // Azure AD V2 endpoint specific

            _context.Audit.Add(new Audit
            {
                DateTime = DateTime.UtcNow,
                Username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value,
                Action = Action
            });

            _context.SaveChanges();
        }
    }
}