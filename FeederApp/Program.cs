using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            Random rnd = new Random();

            // Generate random packets and serialize
            List<Tuple<string, string, string>> packets = new List<Tuple<string, string, string>>();
            for (int i = 0; i < 5; i++)
            {
                // Create a new random packet and add to list
                Tuple<string, string, string> packet = new Tuple<string, string, string>(
                    types[rnd.Next(0, types.Count)], domains[rnd.Next(0, domains.Count)], "FeederApp");
                packets.Add(packet);
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
            Console.ReadLine();
        }
    }
}
