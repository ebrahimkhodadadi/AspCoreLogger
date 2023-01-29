using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ElasticLogger.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultIndex = configuration["ElasticsearchSettings:defaultIndex"];//$"{Assembly.GetExecutingAssembly().GetName().Name}-{DateTime.UtcNow:yyyy-MM}";
        var basicAuthUser = configuration["ElasticsearchSettings:username"];
        var basicAuthPassword = configuration["ElasticsearchSettings:password"];

        var settings = new ConnectionSettings(new Uri(configuration["ElasticsearchSettings:uri"]));

        if (!string.IsNullOrEmpty(defaultIndex))
            settings = settings.DefaultIndex(defaultIndex);

        if (!string.IsNullOrEmpty(basicAuthUser) && !string.IsNullOrEmpty(basicAuthPassword))
            settings = settings.BasicAuthentication(basicAuthUser, basicAuthPassword);

        settings.EnableApiVersioningHeader();

        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);
    }
}
