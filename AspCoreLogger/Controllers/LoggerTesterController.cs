using Microsoft.AspNetCore.Mvc;

namespace AspCoreLogger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggerTesterController : ControllerBase
    {
        private readonly ILogger<LoggerTesterController> _logger;

        public LoggerTesterController(ILogger<LoggerTesterController> logger)
        {
            _logger = logger;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        [HttpGet(Name = "LoggerInformation")]
        public IEnumerable<WeatherForecast> LoggerInformation()
        {
            _logger.LogInformation("GetWeatherForecast called at {time}", DateTime.Now);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet(Name = "LoggerError")]
        public void LoggerError()
        {
            try
            {
                throw new NotImplementedException("Log Error");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "LogError called");
            }
        }

        [HttpGet("AttributeLogger")]
        [LoggerMessage(0, LogLevel.Information, "Doing something at {Time}")]
        public void AttributeLogger(ILogger logger)
        {
            logger.LogInformation("Doing something at {Time}", DateTimeOffset.UtcNow);
        }

        [HttpGet(Name = "LoggerCritical")]
        public void LoggerCritical()
        {
            _logger.LogCritical("LogCritical called");
        }

        [HttpGet(Name = "LoggerDebug")]
        public void LoggerDebug()
        {
            _logger.LogDebug("LogDebug called");
        }

        [HttpGet(Name = "LoggerTrace")]
        public void LoggerTrace()
        {
            _logger.LogTrace("LogTrace called");
        }

        [HttpGet(Name = "LoggerWarning")]
        public void LoggerWarning()
        {
            _logger.LogWarning("LogWarning called");
        }
    }
}