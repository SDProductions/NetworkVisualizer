using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FeederApp
{
    class Program
    {
        public static List<string> domains = new List<string>
        {
            "microsoft.com",
            "azure.com",
            "github.com",
            "github.io",
            "google.com",
            "youtube.com"
        };
        public static List<string> types = new List<string>
        {
            "HTTPS",
            "HTTP",
            "UDP",
            "TCP",
            "DNS"
        };

        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            Program p = new Program();
            p.SendPackets(15);

            Console.ReadLine();
        }

        private void SendPackets(int amount)
        {
            Random rnd = new Random();

            // Generate random packets and serialize
            List<Tuple<string, string, string>> packets = new List<Tuple<string, string, string>>();
            for (int i = 0; i < amount; i++)
            {
                // Create a new random packet and add to list
                Tuple<string, string, string> packet = new Tuple<string, string, string>(
                    types[rnd.Next(0, types.Count)], domains[rnd.Next(0, domains.Count)], "FeederApp");
                packets.Add(packet);
                Console.WriteLine($"Sending: {packet}");
            }
            string json = JsonConvert.SerializeObject(packets);

            // Encode json with password
            var values = new Dictionary<string, string>
                {
                    { "password", "HitlerDidNothingWrong.bmp" },
                    { "json", json }
                };
            var content = new FormUrlEncodedContent(values);

            // Post to site
            var response = client.PostAsync("https://networkvisualizer.azurewebsites.net", content);
            Console.WriteLine($"Sent {amount} packets.");
        }

        private void ReturnDateTimes()
        {
            Console.WriteLine("UpdateGraphs DateTime");
            for (int t = 0; t < 24; t++)
            {
                DateTime targetDate = DateTime.UtcNow.AddHours(t - 6).AddDays(-1);
                Console.WriteLine(targetDate + " - " + targetDate.Hour);
            }
            Console.WriteLine("PacketRecieve DateTime");
            Console.WriteLine(DateTime.UtcNow.Subtract(TimeSpan.FromHours(7)));
        }
    }
}
