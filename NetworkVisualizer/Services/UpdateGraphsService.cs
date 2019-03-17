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

                _context.Cache.Add(new Cache
                {
                    ExpireTime = DateTime.Now.AddDays(1),
                    Key = "Graph1",
                    Value = GenerateDatatableJson()
                });

                _context.SaveChanges();
            }
        }

        public string GenerateDatatableJson()
        {
            DataTable dt = new DataTable();

            // Get most searched domains in database
            List<string> topDomains = new List<string>();
            using (var scope = _scopeFactory.CreateScope())
            {
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

            // Create datapoints for every hour
            for (int t = 0; t <= 24; t++)
            {
                Row r = dt.NewRow();
                DateTime targetDate = DateTime.UtcNow.AddHours(t - 7).AddDays(-1);
                List<int> domainSearches = TopDomainSearches(topDomains, targetDate);

                r.AddCell(new Cell($"{targetDate.Hour}:00"));
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
