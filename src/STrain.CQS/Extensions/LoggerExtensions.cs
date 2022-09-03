using Microsoft.Extensions.Logging;

namespace STrain.CQS.Extensions
{
    internal static class LoggerExtensions
    {
        public static StopwatchLogger LogStopwatch(this ILogger logger, string message) => logger.LogStopwatch(LogLevel.Debug, message);
        public static StopwatchLogger LogStopwatch(this ILogger logger, LogLevel level, string message) => new StopwatchLogger(logger, level, message);
    }
}
