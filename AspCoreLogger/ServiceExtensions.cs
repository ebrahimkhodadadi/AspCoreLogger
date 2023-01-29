
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;
using Serilog;

namespace AspCoreLogger;

public static class ServiceExtensions
{
    /// <summary>
    /// ILogger in ElasticSearch Database
    /// </summary>
    /// <param name="host"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <param name="logging"></param>
    public static void AddLoggerService(this IHostBuilder host,ConfigurationManager configuration, IWebHostEnvironment environment, ILoggingBuilder logging)
    {
        host.UseContentRoot(Directory.GetCurrentDirectory());
        host.UseSerilog((hostingContext, config) => config
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .WriteTo.Console()
            .WriteTo.Elasticsearch(ConfigureElasticSearchSink())
            .ReadFrom.Configuration(hostingContext.Configuration));
        
        configuration
                    .SetBasePath(environment.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();

        logging.ClearProviders();
        logging.AddConsole();
        logging.AddSerilog();
    }

    private static ElasticsearchSinkOptions ConfigureElasticSearchSink()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();
        var elasticUri = new Uri(configuration["ElasticConfiguration:Uri"]);
        return new ElasticsearchSinkOptions(elasticUri)
        {
            AutoRegisterTemplate = true,
            IndexFormat = $"logging-microservice-architecture-demo-{DateTime.UtcNow:yyyy-MM}"
        };
    }
}
