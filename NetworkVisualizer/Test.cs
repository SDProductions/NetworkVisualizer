using NetworkVisualizer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NetworkVisualizer
{
    public class Test
    {
        // Gets context so DB is accessible within this file
        private readonly NetworkVisualizerContext _context;
        public Test(NetworkVisualizerContext context)
        {
            _context = context;
        }

        // Test: Send [chunk] # of POST requests (3 packets each) to DB
        // Returns true if all packets added successfully
        public async Task<bool> AddPacketsToDB(int chunks)
        {
            HttpClient client = new HttpClient();
            for (int i = 0; i < chunks; i++)
            {
                List<Packet> packets = new List<Packet>
                {
                    new Packet
                    {
                        DateTime = DateTime.Now,
                        PacketType = $"TEST {1+i}",
                        DestinationHostname = "127.0.0.1",
                        OriginHostname = "127.0.0.1"
                    },
                    new Packet
                    {
                        DateTime = DateTime.Now,
                        PacketType = $"TEST {2+i}",
                        DestinationHostname = "127.0.0.1",
                        OriginHostname = "127.0.0.1"
                    },
                    new Packet
                    {
                        DateTime = DateTime.Now,
                        PacketType = $"TEST {3+i}",
                        DestinationHostname = "127.0.0.1",
                        OriginHostname = "127.0.0.1"
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
            
            // Check if each test packet exists, if not return false
            for (int i = 0; i < chunks; i++)
                if (_context.Packet.Any(e => e.PacketType != $"TEST {1 + i}"))
                    return false;
                
            return true;
        }
    }
}
