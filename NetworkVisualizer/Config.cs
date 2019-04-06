using System;

namespace NetworkVisualizer
{
    public static class Config
    {
        public static AppConfig config = new AppConfig();
        public static PersistentStats stats = new PersistentStats();

        public struct AppConfig
        {
            public string HttpPostPassword { get; set; }
            public bool DataGenerationEnabled { get; set; }
            public bool DataEqualizationEnabled { get; set; }
            public int UTCHoursOffset { get; set; }
        }

        public static AppConfig defaultConfig = new AppConfig
        {
            HttpPostPassword = "UberMegaStrongPassword123!!",
            DataGenerationEnabled = true,
            DataEqualizationEnabled = true,
            UTCHoursOffset = -7
        };

        public struct PersistentStats
        {
            public int VisitCount { get; set; }
            public DateTime InitializeTime { get; set; }
        }
    }
}
