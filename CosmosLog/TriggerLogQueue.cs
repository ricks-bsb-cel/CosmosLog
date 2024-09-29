using Azure.Messaging.ServiceBus;
using CosmosLog.EntityFramework;
using CosmosLog.EntityFramework.Model;
using CosmosLog.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace CosmosLog
{
    public class TriggerLogQueue
    {
        private readonly ILogger<TriggerLogQueue> _logger;
        private readonly CosmosDbContext _ctx;

        public TriggerLogQueue(
            ILogger<TriggerLogQueue> logger,
            CosmosDbContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        [Function(nameof(TriggerLogQueue))]
        public async Task Run(
            [ServiceBusTrigger("logQueue", Connection = "CosmosLogServiceBusConnectionListenner")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            JObject obj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(message.Body.ToString());

            CosmosLogModel logData = new CosmosLogModel()
            {
                Source = obj["Source"].ToString()! ?? "Default",
                Category = obj["Category"].ToString()! ?? "Default",
                SubCategory = obj["SubCategory"].ToString()! ?? "Default",
                Level = obj["Level"].ToString()! ?? "Info",
                logPayload = (JObject)obj["logPayload"],
                Ttl = CosmosHelper.TtlDias(15)
            };

            _ctx.CosmosLog.Add(logData);
            int success = await _ctx.SaveChangesAsync();

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
