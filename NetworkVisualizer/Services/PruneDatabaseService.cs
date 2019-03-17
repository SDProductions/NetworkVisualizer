using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetworkVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkVisualizer
{
    internal class PruneDatabaseService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private Timer _timer;
        
        public PruneDatabaseService(ILogger<PruneDatabaseService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // Every 5 minutes run DoWork
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PruneDatabaseService is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        // Prune database
        private void DoWork(object state)
        {
            _logger.LogInformation("PruneDatabaseService is working.");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<NetworkVisualizerContext>();

                // Get packets older than 24 hours and expired caches
                List<Packet> oldPackets = (from packet in _context.Packet
                                           where DateTime.Now.Subtract(packet.DateTime) >= TimeSpan.FromHours(24)
                                           select packet).ToList();
                List<Cache> oldCache = (from cache in _context.Cache
                                        where DateTime.Now > cache.ExpireTime
                                        select cache).ToList();

                // Remove these and save changes
                _context.Packet.RemoveRange(oldPackets);
                _context.Cache.RemoveRange(oldCache);
                _context.SaveChanges();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PruneDatabaseService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
