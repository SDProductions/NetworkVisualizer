namespace NetworkVisualizer
{
    public class Config
    {
        public static AppConfig config = new AppConfig();

        public struct AppConfig
        {
            public string HttpPostPassword { get; set; }

            public bool DataGenerationEnabled { get; set; }
        }
    }
}
