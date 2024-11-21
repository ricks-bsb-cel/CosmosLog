using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace CosmosLogCall
{
    public class LogCall : IDisposable
    {
        // Substitua pelos seus valores
        private readonly string CosmosLogServiceBusConnection;
        private readonly string queueName = "logqueue";
        private string Source;

        private readonly ServiceBusClient sbClient;
        private readonly ServiceBusSender sbSender;

        private long? Ttl = null;

        public LogCall(IConfiguration configuration)
        {
            CosmosLogServiceBusConnection = configuration["CosmosLogServiceBusConnectionSender"]! ?? configuration["Values:CosmosLogServiceBusConnectionSender"]!;
            Source = configuration["CosmosLogSource"]!;

            if (string.IsNullOrEmpty(CosmosLogServiceBusConnection)) throw new ArgumentException("CosmosLogServiceBusConnectionSender não configurado");
            if (string.IsNullOrEmpty(Source)) throw new ArgumentException("Source (nome do sistema) não configurado");

            sbClient = new ServiceBusClient(CosmosLogServiceBusConnection);

            sbSender = sbClient.CreateSender(queueName);
        }

        public void SetTtl(long ttlSeconds)
        {
            this.Ttl = ttlSeconds;
        }

        public void SetSource(string source)
        {
            Source = source;
        }

        public async Task<LogResult> send(
            object log,
            string Level = "Info",
            string Category = "Default",
            string SubCategory = "Default"
        )
        {
            LogModel logMessage = new LogModel()
            {
                Source = Source,
                Level = Level,
                Category = Category,
                SubCategory = SubCategory,
                logPayload = log
            };

            if (this.Ttl != null) logMessage.Ttl = Ttl.Value;

            string jsonMessage = JsonSerializer.Serialize(logMessage);

            ServiceBusMessage message = new ServiceBusMessage(jsonMessage);

            try
            {
                await sbSender.SendMessageAsync(message);

                return new LogResult();
            }
            catch (Exception ex)
            {
                return new LogResult()
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        void IDisposable.Dispose()
        {
            if (sbSender != null) sbSender.DisposeAsync();
            if (sbClient != null) sbClient.DisposeAsync();
        }
    }
}
