using System;

namespace DataFellows.KafkaConsumer
{
    public class KafkaConfiguration
    {
        public string GroupId { get; set; } = "kafka-consumer";
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string[] Topics { get; set; }
        public string SslCertificateLocation { get; set; }
        public string SslKeyLocation { get; set; }
    }
}