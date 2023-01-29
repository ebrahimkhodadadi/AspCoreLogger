using AspCoreLogger.Middleware;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection.PortableExecutable;

namespace AspCoreLogger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddCors();
            builder.Services.AddHttpContextAccessor();
            
            builder.Services.AddEndpointsApiExplorer();
            
            builder.Services.AddSwaggerGen();

            #region serilog
            builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
            builder.Host
                .UseSerilog((hostingContext, config) => config
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationId()
                .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSearchSink())
                .ReadFrom.Configuration(hostingContext.Configuration));
            
            builder.Configuration
                        .SetBasePath(builder.Environment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddSerilog();
            #endregion

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
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
}