using System;
using System.IO;
using System.Text.Json;

namespace DataFellows.KafkaConsumer
{
    public class Configuration
    {
        private static Configuration _configuration;
        public KafkaConfiguration Kafka { get; set; }
        public EventHubConfiguration EventHub { get; set; }

        public static Configuration GetConfiguration(string[] args)
        {
            if (_configuration == null)
            {
                _configuration = new Configuration();
                _configuration.Kafka = new KafkaConfiguration();
                _configuration.EventHub = new EventHubConfiguration();
            }


            string config = "";
            if (args.Length > 0 && args[0].ToUpperInvariant().EndsWith(".JSON"))
                config = args[0];
            else
            if (Environment.GetEnvironmentVariable("DF_CONFIG") != null)
                config = Environment.GetEnvironmentVariable("DF_CONFIG");

            Console.WriteLine("{0} {1}", Directory.GetCurrentDirectory(), "config.json");
            if (Directory.GetFiles(Directory.GetCurrentDirectory(), "config.json", new EnumerationOptions() {MatchCasing = MatchCasing.PlatformDefault}).Length > 0)
                config = Path.Join(Directory.GetCurrentDirectory(), "config.json");

            if (!string.IsNullOrEmpty(config))
                DeserializeConfigFile(config);

            ParseEnvironmentVariables();
            return _configuration;
        }

        private static void DeserializeConfigFile(string filename)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            _configuration = JsonSerializer.Deserialize<Configuration>(File.ReadAllText(filename), options);
        }

        private static void ParseEnvironmentVariables()
        {
            if (Environment.GetEnvironmentVariable("DF_BOOTSTRAPSERVERS") != null)
                _configuration.Kafka.BootstrapServers = Environment.GetEnvironmentVariable("DF_BOOTSTRAPSERVERS");

            if (Environment.GetEnvironmentVariable("DF_GROUPID") != null)
                _configuration.Kafka.GroupId = Environment.GetEnvironmentVariable("DF_GROUPID");

            if (Environment.GetEnvironmentVariable("DF_TOPICS") != null)
                _configuration.Kafka.Topics = Environment.GetEnvironmentVariable("DF_TOPICS").Split(',');

            if (Environment.GetEnvironmentVariable("DF_EVENTHUB") != null)
                _configuration.EventHub.ConnectionString = Environment.GetEnvironmentVariable("DF_EVENTHUB");                
        }

        public override string ToString()
        {
            return $"{_configuration.EventHub.ConnectionString}";
        }
    }
}