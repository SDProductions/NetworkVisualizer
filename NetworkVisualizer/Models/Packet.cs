using System;
using System.ComponentModel.DataAnnotations;

namespace NetworkVisualizer.Models
{
    public class Packet
    {
        // Handled by NetworkVisualizer, can recieve any value
        public int Id { get; set; }

        // Must be present and sent by packet sniffer
        [DataType(DataType.Time)]
        public DateTime DateTime { get; set; }
        public string PacketType { get; set; }
        public string DestinationHostname { get; set; }
        public string OriginHostname { get; set; }
    }
}
