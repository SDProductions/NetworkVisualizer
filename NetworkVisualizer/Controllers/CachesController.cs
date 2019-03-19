using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;

namespace NetworkVisualizer.Controllers
{
    public class CachesController : Controller
    {
        private readonly NetworkVisualizerContext _context;
        public CachesController(NetworkVisualizerContext context)
        {
            _context = context;
        }

        private bool LoggedIn()
        {
            if (Request.Cookies["isLoggedIn"] == "true")
                return true;

            return false;
        }

        public string GetLatestGraph(int? id)
        {
            if (id == null)
                return "";

            Cache graphCache = (from cache in _context.Cache
                               where cache.Key == $"Graph{id}"
                               orderby cache.ExpireTime descending
                               select cache).FirstOrDefault();
            if (graphCache == null)
                return "";

            return graphCache.Value;
        }

        // GET: Caches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cache.ToListAsync());
        }

        // GET: Caches/Create
        public IActionResult Create()
        {
            if (!LoggedIn())
                return Redirect("~/login");

            return View();
        }

        // POST: Caches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExpireTime,Key,Value")] Cache cache)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (ModelState.IsValid)
            {
                _context.Add(cache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cache);
        }

        // GET: Caches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            if (id == null)
                return NotFound();

            var cache = await _context.Cache
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cache == null)
                return NotFound();

            return View(cache);
        }

        // POST: Caches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!LoggedIn())
                return Redirect("~/login");

            var cache = await _context.Cache.FindAsync(id);
            _context.Cache.Remove(cache);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CacheExists(int id)
        {
            return _context.Cache.Any(e => e.Id == id);
        }
    }
}
