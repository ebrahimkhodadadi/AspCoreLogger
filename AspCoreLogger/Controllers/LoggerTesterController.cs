using AspCoreLogger.CustomLog;
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
        
        [HttpGet("LoggerCustom")]
        public void LoggerCustom(string name)
        {
            try
            {
                throw new NotImplementedException($"cannot connect to database {name}");
            }
            catch (Exception ex)
            {
                _logger.DatabaseConnectionFailed(ex);
            }
        }
        
        [HttpGet("LoggerInformation")]
        public IActionResult LoggerInformation()
        {
            _logger.LogInformation("LoggerInformation called at {time}", DateTime.Now);

            return Ok();
        }

        [HttpGet("LoggerError")]
        public IActionResult LoggerError()
        {
            try
            {
                throw new NotImplementedException("Log Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogError called");
            }

            return Ok();
        }

        [HttpGet("LoggerCritical")]
        public IActionResult LoggerCritical()
        {
            _logger.LogCritical("LogCritical called");

            return BadRequest();
        }

        [HttpGet("LoggerDebug")]
        public IActionResult LoggerDebug()
        {
            _logger.LogDebug("LogDebug called");

            return NoContent();
        }

        [HttpGet("LoggerTrace")]
        public IActionResult LoggerTrace()
        {
            _logger.LogTrace("LogTrace called");

            return Ok();
        }

        [HttpGet("LoggerWarning")]
        public IActionResult LoggerWarning()
        {
            _logger.LogWarning("LogWarning called");

            return NotFound();
        }
    }
}