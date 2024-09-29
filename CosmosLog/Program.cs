using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CosmosLog.EntityFramework;

var host = new HostBuilder().ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile(
        "local.settings.json",
        optional: true,
        reloadOnChange: true
    ).AddEnvironmentVariables();

})
.ConfigureFunctionsWebApplication()
.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;
    services.AddSingleton(configuration);

    string CosmosEndPoint = configuration.GetValue<string>("CosmosEndPoint")! ?? configuration.GetValue<string>("Values:CosmosEndPoint")!;
    string CosmosAccountKey = configuration.GetValue<string>("CosmosAccountKey")! ?? configuration.GetValue<string>("Values:CosmosAccountKey")!;
    string CosmosDatabaseName = configuration.GetValue<string>("CosmosDatabaseName")! ?? configuration.GetValue<string>("Values:CosmosDatabaseName")!;

    // https://learn.microsoft.com/en-us/ef/core/providers/cosmos/?tabs=dotnet-core-cli
    services.AddDbContext<CosmosDbContext>(options =>
       options.UseCosmos(
           accountEndpoint: CosmosEndPoint,
           accountKey: CosmosAccountKey,
           databaseName: CosmosDatabaseName
       )
    );

    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();
    // Adicione outros serviços aqui
})
.Build();

host.Run();

/*
 Se surgir: System.InvalidOperationException: The gRPC channel URI 'http://0' could not be parsed
* Atualize todas as bibliotecas lá no Nuget (aba atualizações)
*/