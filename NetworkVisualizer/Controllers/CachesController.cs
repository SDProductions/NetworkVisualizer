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
    public class CachesController : Controller
    {
        private readonly NetworkVisualizerContext _context;
        public CachesController(NetworkVisualizerContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
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
            AddAudit("Access - Caches");
            return View(await _context.Cache.ToListAsync());
        }

        // GET: Caches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Caches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExpireTime,Key,Value")] Cache cache)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            AddAudit("Create - Cache");
            return View(cache);
        }

        // GET: Caches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
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
            var cache = await _context.Cache.FindAsync(id);
            _context.Cache.Remove(cache);
            await _context.SaveChangesAsync();

            AddAudit("Delete - Cache");
            return RedirectToAction(nameof(Index));
        }

        private bool CacheExists(int id)
        {
            return _context.Cache.Any(e => e.Id == id);
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
