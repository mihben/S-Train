using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace STrain.CQS.Test.Unit.Support
{
    public static class TestLoggerFactory
    {
        private static List<ILogger> _loggers = new();

        public static ILogger<TContext> CreateLogger<TContext>(ITestOutputHelper outputHelper)
        {
            var logger = _loggers.FirstOrDefault(l => l is ILogger<TContext>) as ILogger<TContext>;
            if (logger is null)
            {
                logger = LoggerFactory.Create(builder => builder.AddXUnit(outputHelper)).CreateLogger<TContext>();
                _loggers.Add(logger);
            }
            return logger;
        }
    }
}
