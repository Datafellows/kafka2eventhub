using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Threading.Tasks;

namespace DataFellows.KafkaConsumer
{
    public class AzureEventHubService
    {
        private readonly EventHubProducerClient client;
        public AzureEventHubService(string connectionString, string eventHubName)
        {
            // Workaround for 5.0 until new package is released
            // https://github.com/Azure/azure-sdk-for-net/issues/13899
            EventHubProducerClientOptions ehpco = new EventHubProducerClientOptions();
            ehpco.ConnectionOptions.TransportType = EventHubsTransportType.AmqpWebSockets;

            client = new EventHubProducerClient(connectionString, eventHubName, ehpco);
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