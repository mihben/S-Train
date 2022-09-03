using STrain.CQS.Extensions;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static StopwatchLogger LogStopwatch(this ILogger logger, string message) => logger.LogStopwatch(message, LogLevel.Debug);
        public static StopwatchLogger LogStopwatch(this ILogger logger, string message, LogLevel level) => new(logger, level, message);
    }
}
