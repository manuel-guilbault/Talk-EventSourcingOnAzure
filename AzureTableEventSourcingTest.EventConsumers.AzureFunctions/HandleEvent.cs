using AzureTableEventSourcingTest.Infrastructure;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureTableEventSourcingTest.EventConsumers.AzureFunctions
{
    public static class HandleEvent
    {
        [FunctionName("HandleEvent")]
        public static void Run([ServiceBusTrigger("events", Connection = "ServiceBus:ConnectionString")] Message message, ILogger logger)
        {
            var @event = EventSerializer.FromBytes(message.Body);
            logger.LogInformation(@event.ToString());
        }
    }
}
