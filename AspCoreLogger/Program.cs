using ElasticLogger.Extensions;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLogger(builder.Configuration, "APIElasticsearch");

    Log.Information("Starting API");

    builder.Services.AddApiConfiguration();


    builder.Services.AddSwagger(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseElasticSearch();

    app.UseApiConfiguration(app.Environment);

    app.UseSwaggerDoc();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}