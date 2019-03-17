using System;

namespace NetworkVisualizer.Models
{
    public class Cache
    {
        public int Id { get; set; }

        public DateTime ExpireTime { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
