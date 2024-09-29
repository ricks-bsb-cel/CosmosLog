using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using CosmosLogCall;

namespace CosmosLog
{
    public class CallLog
    {
        private readonly ILogger<CallLog> _logger;
        private readonly IConfiguration _configuration;

        private readonly LogCall log;

        public CallLog(
            ILogger<CallLog> logger,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _configuration = configuration;

            log = new LogCall(_configuration);
        }

        [Function("CallLog")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            HttpRequest req
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody)) return new BadRequestResult();

            JObject requestObj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(requestBody);
            LogResult sendResult = await log.send(requestObj);

            return new OkObjectResult(sendResult);
        }
    }
}
