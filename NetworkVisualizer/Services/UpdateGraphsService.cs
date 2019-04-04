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

                _context.SaveChanges();
            }
        }

        public string GenerateMainDatatableJson()
        {
            DataTable dt = new DataTable();

            // Get most searched domains in database
            List<string> topDomains = new List<string>();
            using (var scope = _scopeFactory.CreateScope())
            {
                // Get context (database)
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();
                topDomains = _context.Packet
                             .GroupBy(q => q.DestinationHostname)
                             .OrderByDescending(gp => gp.Count())
                             .Take(4)
                             .Select(g => g.Key).ToList();
            }

            // Add most searched domains as columns in datatable, with Time and Other category
            dt.AddColumn(new Column(ColumnType.String, "Time", "Time"));
            foreach (string domain in topDomains)
            {
                dt.AddColumn(new Column(ColumnType.Number, domain, domain));
            }
            dt.AddColumn(new Column(ColumnType.Number, "other sites", "other sites"));

            // Create datapoints for every hour, starting with 23 hours ago up to now
            for (int t = 1; t <= 24; t++)
            {
                Row r = dt.NewRow();
                
                DateTime targetDateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)).AddHours(t);

                // Add rows, each with top search result numbers & a time with offset applied
                List<int> domainSearches = TopDomainSearches(topDomains, targetDateTime);
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

        private List<int> TopDomainSearches(List<string> domains, DateTime date)
        {
            List<int> searches = new List<int>();
            int total = 0;

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();

                // Add the number of searches for each domain to list
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

                // Add number of non-top domain searches to list
                int otherSearched = (from packet in _context.Packet
                                     where !domains.Contains(packet.DestinationHostname)
                                     && packet.DateTime.Hour == date.Hour
                                     && packet.DateTime.Day == date.Day
                                     select packet).Count();
                searches.Add(otherSearched);
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
