using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NetworkVisualizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Generate a new config file from default
            Config.config = Config.defaultConfig;
            Config.stats = new Config.PersistentStats
            {
                VisitCount = 0,
                InitializeTime = DateTime.UtcNow
            };

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
