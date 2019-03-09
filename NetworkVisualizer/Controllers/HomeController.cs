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
        private readonly NetworkVisualizerContext _context;
        public HomeController(NetworkVisualizerContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            return View();
        }

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
