using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkVisualizer.Models
{
    public class Packet
    {
        public int Id { get; set; }
        [DataType(DataType.Time)]
        public DateTime DateTime { get; set; }

        public string PacketType { get; set; }
        public string DestinationHostname { get; set; }
        public string OriginHostname { get; set; }
    }
}
