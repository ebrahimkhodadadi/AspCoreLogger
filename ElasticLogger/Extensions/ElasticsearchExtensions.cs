using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Nest;
using Serilog;

namespace ElasticLogger.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        #region httpLogging
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add(HeaderNames.Accept);
            logging.RequestHeaders.Add(HeaderNames.ContentType);
            logging.RequestHeaders.Add(HeaderNames.ContentDisposition);
            logging.RequestHeaders.Add(HeaderNames.ContentEncoding);
            logging.RequestHeaders.Add(HeaderNames.ContentLength);

            logging.MediaTypeOptions.AddText("application/json");
            logging.MediaTypeOptions.AddText("multipart/form-data");
        });
        #endregion

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

    public static void UseElasticSearch(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder
            .UseHttpLogging()
            .UseSerilogRequestLogging();
    }
}
