
namespace AspCoreLogger.CustomLog;

public static partial class Log
{
    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Error,
        Message = "Failed to connect to the database"
    )]
    public static partial void DatabaseConnectionFailed(this ILogger logger, Exception ex);
}