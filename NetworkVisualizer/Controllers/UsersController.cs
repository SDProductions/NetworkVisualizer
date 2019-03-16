using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;

namespace NetworkVisualizer.Controllers
{
    public class UsersController : Controller
    {
        private readonly NetworkVisualizerContext _context;

        public UsersController(NetworkVisualizerContext context)
        {
            _context = context;
        }

        private bool LoggedIn()
        {
            if (Request.Cookies["isLoggedIn"] == "true")
                return true;

            return false;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View(await _context.User.ToListAsync());
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            if (!LoggedIn())
                return Redirect("../login");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password")] User user)
        {
            if (!LoggedIn())
                return Redirect("../login");

            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!LoggedIn())
                return Redirect("../login");

            if (id == null)
                return NotFound();

            var user = await _context.User.FindAsync(id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password")] User user)
        {
            if (!LoggedIn())
                return Redirect("../login");

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
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!LoggedIn())
                return Redirect("../login");

            if (id == null)
                return NotFound();

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!LoggedIn())
                return Redirect("../login");

            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
