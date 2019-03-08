using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NetworkVisualizer.Models
{
    public class NetworkVisualizerContext : DbContext
    {
        public NetworkVisualizerContext (DbContextOptions<NetworkVisualizerContext> options)
            : base(options)
        {
        }

        public DbSet<NetworkVisualizer.Models.Packet> Packet { get; set; }
    }
}
