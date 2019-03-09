using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetworkVisualizer.Models;
using Newtonsoft.Json;

namespace NetworkVisualizer.Controllers
{
    public class HomeController : Controller
    {
        // Gets context so DB is accessible within this file
        private readonly NetworkVisualizerContext _context;
        public HomeController(NetworkVisualizerContext context)
        {
            _context = context; 
        }

        // GET Index: Display main page
        public IActionResult Index()
        {
            return View();
        }

        // POST Index: Add list of packets to DB
        [HttpPost]
        public async Task Index(string password, string json)
        {
            if (password != Config.config.HttpPostPassword)
                return;

            List<Packet> packets = JsonConvert.DeserializeObject<List<Packet>>(json);

            if (ModelState.IsValid)
            {
                _context.AddRange(packets);
                await _context.SaveChangesAsync();
            }
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
