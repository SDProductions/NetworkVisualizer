using NetworkVisualizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkVisualizer
{
    public class Tests
    {
        public async Task AddPacketsToDB()
        {
            HttpClient client = new HttpClient();
            List<Packet> packets = new List<Packet>
            {
                new Packet
                {
                    DateTime = DateTime.Now,
                    PacketType = "meme",
                    DestinationHostname = "urmom",
                    OriginHostname = "gottem"
                },
                new Packet
                {
                    DateTime = DateTime.Now,
                    PacketType = "meme2",
                    DestinationHostname = "urmom",
                    OriginHostname = "gottem"
                },
                new Packet
                {
                    DateTime = DateTime.Now,
                    PacketType = "meme3",
                    DestinationHostname = "urmom",
                    OriginHostname = "gottem"
                }
            };

            var values = new Dictionary<string, string>
            {
                { "password", Config.config.HttpPostPassword },
                { "json", JsonConvert.SerializeObject(packets) }
            };

            var content = new FormUrlEncodedContent(values);

            await client.PostAsync("https://localhost:44365", content);
        }
    }
}
