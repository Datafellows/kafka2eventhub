using System;

namespace DataFellows.KafkaConsumer
{
    public class EventHubConfiguration
    {
        public string ConnectionString { get; set; }
        public string EventHubNamespace { get; set; }
    }
}