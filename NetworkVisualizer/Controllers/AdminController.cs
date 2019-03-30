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
            return View();
        }

        // GET: Audit
        public IActionResult Audit()
        {
            return View();
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

        // GET: PacketList
        public async Task<IActionResult> Packets()
        {
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
            return Redirect("~/packets");
        }
    }
}