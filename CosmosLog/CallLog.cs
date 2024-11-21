using CosmosLogCall;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log")]
            HttpRequest req,
            string ttl
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody)) return new BadRequestResult();

            object requestObj = JsonSerializer.Deserialize<object>(requestBody)!;
            if (requestObj == null) return new BadRequestResult();

            if (!string.IsNullOrEmpty(ttl))
            {
                long _ttl;

                if (long.TryParse(ttl, out _ttl))
                {
                    log.SetTtl(_ttl);
                }
            }

            LogResult sendResult = await log.send(requestObj);

            return new OkObjectResult(sendResult);
        }


        [Function("CallLogLevel")]
        public async Task<IActionResult> RunLevel(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "log/source/{source}")]
            HttpRequest req,
            string source
        )
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody)) return new BadRequestResult();

            object requestObj = JsonSerializer.Deserialize<object>(requestBody)!;
            if (requestObj == null) return new BadRequestResult();

            log.SetSource(source);

            LogResult sendResult = await log.send(requestObj);

            return new OkObjectResult(sendResult);
        }
    }
}
