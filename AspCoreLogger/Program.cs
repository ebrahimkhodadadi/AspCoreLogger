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

            builder.Host.AddLoggerService(builder.Configuration, builder.Environment, builder.Logging);

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
    }
}