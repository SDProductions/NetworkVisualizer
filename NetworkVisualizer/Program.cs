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
            if (!File.Exists("config.json"))
                File.WriteAllText("config.json", JsonConvert.SerializeObject(
                    new Config.AppConfig { HttpPostPassword = "HitlerDidNothingWrong.bmp" }, Formatting.Indented));

            string json = File.ReadAllText("config.json");
            Config.config = JsonConvert.DeserializeObject<Config.AppConfig>(json);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
