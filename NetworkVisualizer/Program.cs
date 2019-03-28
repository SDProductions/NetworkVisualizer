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
            File.WriteAllText("config.json", JsonConvert.SerializeObject(
                new Config.AppConfig
                {
                    HttpPostPassword = "HitlerDidNothingWrong.bmp",
                    DataGenerationEnabled = true
                }, 
                Formatting.Indented));

            // Load the config file
            string json = File.ReadAllText("config.json");
            Config.config = JsonConvert.DeserializeObject<Config.AppConfig>(json);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
