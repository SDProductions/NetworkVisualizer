using Google.DataTable.Net.Wrapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkVisualizer.Services
{
    internal class UpdateGraphsService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private Timer _timer;

        public UpdateGraphsService(ILogger<PruneDatabaseService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateGraphsService is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("UpdateGraphsService is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();

                // Add generated graphs for each graph
                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1),
                    Key = "Graph1",
                    Value = GenerateMainDatatableJson()
                });
                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1),
                    Key = "Graph2",
                    Value = GenerateDomainDatatableJson("google", true)
                });
                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1),
                    Key = "Graph3",
                    Value = GenerateDomainDatatableJson("google", false)
                });
                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1),
                    Key = "Graph4",
                    Value = GenerateStatisticsDatatableJson()
                });
                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.UtcNow.AddDays(1),
                    Key = "Graph5",
                    Value = GenerateGamesDatatableJson()
                });

                _context.SaveChanges();
            }
        }

        public string GenerateMainDatatableJson()
        {
            DataTable dt = new DataTable();

            // Get most searched domains in database, add non-top for graph
            List<string> topDomains;
            using (var scope = _scopeFactory.CreateScope())
            {
                // Get context (database)
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();
                topDomains = _context.Packet
                             .GroupBy(q => q.DestinationHostname)
                             .OrderByDescending(gp => gp.Count())
                             .Take(4)
                             .Select(g => g.Key).ToList();
                topDomains.Add("other sites");
            }

            // Add most searched domains as columns in datatable, with Time and Other category
            dt.AddColumn(new Column(ColumnType.String, "Time", "Time"));
            foreach (string domain in topDomains)
            {
                dt.AddColumn(new Column(ColumnType.Number, domain, domain));
            }

            // Create datapoints for every hour, starting with 23 hours ago up to now
            for (int t = 0; t <= 24; t++)
            {
                Row r = dt.NewRow();
                
                DateTime targetDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).AddHours(t);

                // Add rows, each with top search result numbers & a time with offset applied
                List<int> domainSearches = DomainSearches(topDomains, targetDateTime);
                r.AddCell(new Cell($"{targetDateTime.AddHours(Config.config.UTCHoursOffset).Hour}:00"));
                foreach (int s in domainSearches)
                {
                    r.AddCell(new Cell(s));
                }

                dt.AddRow(r);
            }

            // Return datatable as json
            return dt.GetJson();
        }

        private string GenerateDomainDatatableJson(string provider, bool inclusive)
        {
            DataTable dt = new DataTable();

            // Get most searched domains with Google (eg. drive.google.com)
            List<string> topDomains = new List<string>();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();
                topDomains = _context.Packet
                             .Where(p => p.DestinationHostname.Contains(provider) == inclusive)
                             .GroupBy(q => q.DestinationHostname)
                             .OrderByDescending(gp => gp.Count())
                             .Take(4)
                             .Select(g => g.Key).ToList();
            }

            // Add most searched domains as columns in datatable with Time
            dt.AddColumn(new Column(ColumnType.String, "Time", "Time"));
            foreach (string domain in topDomains)
            {
                dt.AddColumn(new Column(ColumnType.Number, domain, domain));
            }

            // Create datapoints for every hour, starting with 12 hours ago up to now
            for (int t = 0; t <= 12; t++)
            {
                Row r = dt.NewRow();

                DateTime targetDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(12)).AddHours(t);

                // Add rows, each with top search result numbers & a time with offset applied
                List<int> domainSearches = DomainSearches(topDomains, targetDateTime);
                r.AddCell(new Cell($"{targetDateTime.AddHours(Config.config.UTCHoursOffset).Hour}:00"));
                foreach (int s in domainSearches)
                {
                    r.AddCell(new Cell(s));
                }

                dt.AddRow(r);
            }

            return dt.GetJson();
        }

        public string GenerateStatisticsDatatableJson()
        {
            DataTable dt = new DataTable();

            dt.AddColumn(new Column(ColumnType.String, "Stat", "Stat"));
            dt.AddColumn(new Column(ColumnType.String, "Value", "Value"));
            
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();

                int uniqueUsers = _context.Packet
                                 .Where(p => p.Id / 1 == p.Id)
                                 .GroupBy(q => q.OriginHostname)
                                 .Select(g => g.Key).ToList().Count;

                dt.AddRow(makeRow(dt, "Packets Today", _context.Packet.ToList().Count.ToString()));
                //dt.AddRow(makeRow(dt, "Total Packets", _context.Packet.Last().Id.ToString()));
                dt.AddRow(makeRow(dt, "Unique Users", uniqueUsers.ToString()));
                dt.AddRow(makeRow(dt, "Total Visits", Config.stats.VisitCount.ToString()));
                dt.AddRow(makeRow(dt, "Site Uptime", (DateTime.UtcNow - Config.stats.InitializeTime).ToString().Substring(0, 8)));
            }
            
            return dt.GetJson();
        }

        public string GenerateGamesDatatableJson()
        {
            DataTable dt = new DataTable();

            dt.AddColumn(new Column(ColumnType.String, "Stat", "Stat"));
            dt.AddColumn(new Column(ColumnType.String, "Value", "Value"));

            List<string> games = new List<string>
            {
                "krunker.io",
                "surviv.io",
                "2048.io",
                "tetris.io",
                "roblox.com",
                "fortnite.com"
            };

            List<string> topDomains = new List<string>();
            List<int> searches = new List<int>();
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();
                topDomains = _context.Packet
                             .Where(p => games.Contains(p.DestinationHostname))
                             .GroupBy(q => q.DestinationHostname)
                             .OrderByDescending(gp => gp.Count())
                             .Take(10)
                             .Select(g => g.Key).ToList();

                int numberSearched;
                foreach (string domain in topDomains)
                {
                    numberSearched = (from packet in _context.Packet
                                        where packet.DestinationHostname == domain
                                        && DateTime.UtcNow - packet.DateTime <= TimeSpan.FromHours(12)
                                        select packet).Count();
                    
                    searches.Add(numberSearched);
                }
            }

            foreach (string domain in topDomains)
            {
                Row r = dt.NewRow();

                r.AddCell(new Cell(domain));
                r.AddCell(new Cell(searches[topDomains.IndexOf(domain)]));

                dt.AddRow(r);
            }

            return dt.GetJson();
        }

        private Row makeRow(DataTable dt, string name, string value)
        {
            Row r = dt.NewRow();

            r.AddCell(new Cell(name));
            r.AddCell(new Cell(value));

            return r;
        }

        private List<int> DomainSearches(List<string> domains, DateTime date)
        {
            List<int> searches = new List<int>();

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();

                // Add the number of searches for each domain to list
                int numberSearched;
                foreach (string domain in domains)
                {
                    if (domain == "other sites")
                    {
                        numberSearched = (from packet in _context.Packet
                                          where !domains.Contains(packet.DestinationHostname)
                                          && packet.DateTime.Hour == date.Hour
                                          && packet.DateTime.Day == date.Day
                                          select packet).Count();
                    }
                    else
                    {
                        numberSearched = (from packet in _context.Packet
                                              where packet.DestinationHostname == domain
                                              && packet.DateTime.Hour == date.Hour
                                              && packet.DateTime.Day == date.Day
                                              select packet).Count();
                    }

                    searches.Add(numberSearched);
                }
            }

            return searches;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdateGraphsService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
