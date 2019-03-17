using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetworkVisualizer.Models;
using Google.DataTable.Net.Wrapper;
using System;
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

        public string GenerateDatatableJson()
        {
            DataTable dt = new DataTable();

            List<string> topDomains = _context.Packet
                                      .GroupBy(q => q.DestinationHostname)
                                      .OrderByDescending(gp => gp.Count())
                                      .Take(4)
                                      .Select(g => g.Key).ToList();

            dt.AddColumn(new Column(ColumnType.String, "Time", "Time"));
            foreach (string domain in topDomains)
            {
                dt.AddColumn(new Column(ColumnType.Number, domain, domain));
            }
            dt.AddColumn(new Column(ColumnType.Number, "other sites", "other sites"));

            // Create datapoints for every hour
            for (int t = 0; t <= 24; t++)
            {
                Row r = dt.NewRow();
                DateTime targetDate = DateTime.UtcNow.AddHours(t-7).AddDays(-1);
                List<int> domainSearches = TopDomainSearches(topDomains, targetDate);

                r.AddCell(new Cell($"{targetDate.Hour}:00"));
                foreach (int s in domainSearches)
                {
                    r.AddCell(new Cell(s));
                }

                dt.AddRow(r);
            }

            return dt.GetJson();
        }

        private List<int> TopDomainSearches(List<string> domains, DateTime date)
        {
            List<int> searches = new List<int>();
            int total = 0;

            foreach (string domain in domains)
            {
                int numberSearched = (from packet in _context.Packet
                                      where packet.DestinationHostname == domain 
                                      && packet.DateTime.Hour == date.Hour
                                      && packet.DateTime.Day == date.Day
                                      select packet).Count();
                total += numberSearched;
                searches.Add(numberSearched);
            }

            int otherSearched = (from packet in _context.Packet
                                 where !domains.Contains(packet.DestinationHostname)
                                 && packet.DateTime.Hour == date.Hour
                                 && packet.DateTime.Day == date.Day
                                 select packet).Count();
            searches.Add(otherSearched);

            return searches;
        }

        // POST Index: Add list of packets to DB
        [HttpPost]
        public string Index(string json)
        {
            // Check password and reject if mismatch
            /*if (password != Config.config.HttpPostPassword)
                return "Mismatched password.";*/

            // Get list of pseudopacket-objects from json
            List<Tuple<string, string, string>> packets = 
                JsonConvert.DeserializeObject<List<Tuple<string, string, string>>>(json);

            // Convert to packet object with PST time, add to db
            foreach (Tuple<string, string, string> packet in packets)
            {
                Packet newPacket = new Packet
                {
                    DateTime = DateTime.UtcNow.AddHours(-7),
                    PacketType = packet.Item1,
                    DestinationHostname = packet.Item2,
                    OriginHostname = packet.Item3
                };

                if (ModelState.IsValid)
                    _context.Packet.Add(newPacket);
            }
            
            _context.SaveChanges();
            return "Operation successful.";
        }
        
        public IActionResult About()
        {
            return View();
        }

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
