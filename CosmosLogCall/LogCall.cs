using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace CosmosLogCall
{
    public class LogCall : IDisposable
    {
        // Substitua pelos seus valores
        private readonly string CosmosLogServiceBusConnection;
        private readonly string queueName = "logqueue";
        private readonly string Source;

        private readonly IConfiguration _configuration;
        private readonly ServiceBusClient sbClient;
        private readonly ServiceBusSender sbSender;


        public LogCall(IConfiguration configuration)
        {
            _configuration = configuration;
            CosmosLogServiceBusConnection = _configuration.GetConnectionString("CosmosLogServiceBusConnection")!;
            Source = _configuration.GetConnectionString("CosmosLogSource")!;

            if (string.IsNullOrEmpty(CosmosLogServiceBusConnection)) throw new ArgumentException("CosmosLogServiceBusConnection não configurado");
            if (string.IsNullOrEmpty(Source)) throw new ArgumentException("Source (nome do sistema) não configurado");

            sbClient = new ServiceBusClient(CosmosLogServiceBusConnection);

            sbSender = sbClient.CreateSender(queueName);
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
