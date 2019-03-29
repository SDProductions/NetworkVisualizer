using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace NetworkVisualizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Generate a new config file from default
            Config.config =
                new Config.AppConfig
                {
                    HttpPostPassword = "HitlerDidNothingWrong.bmp",
                    DataGenerationEnabled = true,
                    UTCHoursOffset = -7
                };

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
