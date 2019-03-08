using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Packets
        public async Task<IActionResult> Index()
        {
            return View(await _context.Packet.ToListAsync());
        }

        // GET: Packets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packet = await _context.Packet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packet == null)
            {
                return NotFound();
            }

            return View(packet);
        }

        // GET: Packets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Packets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DateTime,PacketType,DestinationHostname,OriginHostname")] Packet packet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(packet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(packet);
        }

        // GET: Packets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packet = await _context.Packet.FindAsync(id);
            if (packet == null)
            {
                return NotFound();
            }
            return View(packet);
        }

        // POST: Packets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DateTime,PacketType,DestinationHostname,OriginHostname")] Packet packet)
        {
            if (id != packet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(packet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PacketExists(packet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(packet);
        }

        // GET: Packets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var packet = await _context.Packet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (packet == null)
            {
                return NotFound();
            }

            return View(packet);
        }

        // POST: Packets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
