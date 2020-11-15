using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DataFellows.KafkaConsumer
{
    public class AzureEventHubService
    {
        private readonly EventHubProducerClient client;

        public AzureEventHubService(EventHubConfiguration eventHubConfiguration, string eventHubName)
        {
            // Workaround for 5.0 until new package is released
            // https://github.com/Azure/azure-sdk-for-net/issues/13899
            EventHubProducerClientOptions ehpco = new EventHubProducerClientOptions();
            ehpco.ConnectionOptions.TransportType = EventHubsTransportType.AmqpWebSockets;

            if (!string.IsNullOrWhiteSpace(eventHubConfiguration.ConnectionString))
            {
                client = new EventHubProducerClient(eventHubConfiguration.ConnectionString, eventHubName, ehpco);
            }
            else if (!string.IsNullOrWhiteSpace(eventHubConfiguration.EventHubNamespace))
            {
                client = new EventHubProducerClient(eventHubConfiguration.EventHubNamespace, eventHubName, new DefaultAzureCredential(), ehpco);
            }

            if (client == null)
                throw new ArgumentException("Invalid configuration for Eventhub service.");
        }

        public async Task SendToEventHubAsync(byte[] data)
        {
            EventData eventData = new EventData(data);
            using EventDataBatch eventBatch = await client.CreateBatchAsync();
            eventBatch.TryAdd(eventData);
            await client.SendAsync(eventBatch);
        }
    }
}