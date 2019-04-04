using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetworkVisualizer.Models
{
    public class Audit
    {
        public int Id { get; set; }
        
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime DateTime { get; set; }
    }
}
