using STrain.CQS.Extensions;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static StopwatchLogger LogStopwatch(this ILogger logger, string message, LogLevel level = LogLevel.Debug) => new(logger, level, message);
    }
}
