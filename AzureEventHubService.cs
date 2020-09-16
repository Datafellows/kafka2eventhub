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
            client = new EventHubProducerClient(connectionString, eventHubName);
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