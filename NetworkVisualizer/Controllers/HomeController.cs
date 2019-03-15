using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetworkVisualizer.Models;
using Newtonsoft.Json;
using Google.DataTable.Net.Wrapper;
using System;

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

            for (int t = 0; t < 24; t++)
            {
                Row r = dt.NewRow();
                List<int> domainSearches = TopDomainSearches(topDomains, t);

                r.AddCell(new Cell($"{t}:00"));
                foreach (int s in domainSearches)
                {
                    r.AddCell(new Cell(s));
                }

                dt.AddRow(r);
            }

            return dt.GetJson();
        }

        private List<int> TopDomainSearches(List<string> domains, int hour)
        {
            List<int> searches = new List<int>();
            int total = 0;

            foreach (string domain in domains)
            {
                int numberSearched = (from packet in _context.Packet
                                      where packet.DestinationHostname == domain && packet.DateTime.Hour == hour
                                      select packet).Count();
                total += numberSearched;
                searches.Add(numberSearched);
            }

            searches.Add(0);

            return searches;
        }

        // POST Index: Add list of packets to DB
        [HttpPost]
        public async Task Index(string destAddress)
        {
            Packet newPacket = new Packet
            {
                DateTime = DateTime.Now,
                PacketType = null,
                DestinationHostname = destAddress,
                OriginHostname = null
            };

            if (ModelState.IsValid)
                _context.Packet.Add(newPacket);

            await _context.SaveChangesAsync();
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
