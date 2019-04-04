using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetworkVisualizer.Models;
using System;
using Newtonsoft.Json;

namespace NetworkVisualizer.Controllers
{
    [AllowAnonymous]
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
        public string Index(string password, string json)
        {
            // Check password and reject if mismatch
            if (password != Config.config.HttpPostPassword)
                return "Mismatched password.";

            // Get list of pseudopacket-objects from json
            List<Tuple<string, string, string>> packets = 
                JsonConvert.DeserializeObject<List<Tuple<string, string, string>>>(json);

            // Convert to Packet, add to DB
            foreach (Tuple<string, string, string> packet in packets)
            {
                Packet newPacket = new Packet
                {
                    DateTime = DateTime.UtcNow,
                    PacketType = packet.Item1,
                    DestinationHostname = packet.Item2,
                    OriginHostname = packet.Item3
                };

                if (ModelState.IsValid)
                    _context.Packet.Add(newPacket);
            }

            // Add an audit
            _context.Audit.Add(new Audit
            {
                DateTime = DateTime.UtcNow,
                Username = "POST",
                Action = "Recieved packets"
            });
            _context.SaveChanges();
            return "Operation successful.";
        }
        
        // GET About: Display about
        public IActionResult About()
        {
            return View();
        }

        // GET Terms: Display terms
        public IActionResult Terms()
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
