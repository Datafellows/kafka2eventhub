using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

namespace DataFellows.KafkaConsumer
{
    class Program
    {
        private static readonly Dictionary<string, AzureEventHubService> eventHubs = new Dictionary<string, AzureEventHubService>();
        static async Task Main(string[] args)
        {
            Configuration configuration = Configuration.GetConfiguration(args);
            Console.WriteLine(configuration.ToString());

            foreach (string topic in configuration.Kafka.Topics)
            {
                eventHubs.Add(topic, new AzureEventHubService(configuration.EventHub.ConnectionString, topic));
            }

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            AppDomain.CurrentDomain.ProcessExit += (_, e) =>
            {
                cancellationTokenSource.Cancel();
            };
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            await ProcessStreamAsync(configuration, cancellationTokenSource);
           LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Consumer closed.");
        }

        static async Task ProcessStreamAsync(Configuration configuration, CancellationTokenSource cancellationTokenSource)
        {
            using (var consumer = new ConsumerBuilder<Ignore, byte[]>(getConsumerConfiguration(configuration.Kafka))
                        .SetErrorHandler((_, e) => LogError($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Error: {e.Reason}"))
                        .Build())
            {
                consumer.Subscribe(configuration.Kafka.Topics);
                LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Subscribing to topics: {String.Join(',', configuration.Kafka.Topics)} on {configuration.Kafka.BootstrapServers}.");
                LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Consumer ready. Waiting for events");

                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(cancellationTokenSource.Token);
                        string meta = "timestamp:" + consumeResult.Message.Timestamp.UnixTimestampMs.ToString() + " topic:" + consumeResult.Topic + " partition:" + consumeResult.Partition.Value.ToString() + " offset:" + consumeResult.Offset.Value;
                        LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Received: {meta}");

                        try
                        {
                            await eventHubs[consumeResult.Topic].SendToEventHubAsync(consumeResult.Message.Value);
                            LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Processed: {meta}");
                        }
                        catch (Exception exception)
                        {
                            LogError($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} There was an error storing the Kafka message. {consumeResult.TopicPartitionOffset.ToString()}\r\n{exception.ToString()}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    LogMessage($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} Closing consumer");
                    consumer.Close();
                }
            }
        }

        static ConsumerConfig getConsumerConfiguration(KafkaConfiguration kafkaConfiguration)
        {
            ConsumerConfig config = new ConsumerConfig
            {
                BootstrapServers = kafkaConfiguration.BootstrapServers,
                GroupId = kafkaConfiguration.GroupId,
                ClientId = Environment.MachineName,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableSslCertificateVerification = false
            };

            if (!string.IsNullOrEmpty(kafkaConfiguration.SslCertificateLocation) && !string.IsNullOrEmpty(kafkaConfiguration.SslKeyLocation))
            {
                config.SecurityProtocol = SecurityProtocol.Ssl;
                config.SslCertificateLocation = kafkaConfiguration.SslCertificateLocation;
                config.SslKeyLocation = kafkaConfiguration.SslKeyLocation;
            }

            return config;
        }

        static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} {message}");
        }

        static void LogError(string message)
        {
            Console.Error.WriteLine($"{DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")} {message}");
        }
    }
}
