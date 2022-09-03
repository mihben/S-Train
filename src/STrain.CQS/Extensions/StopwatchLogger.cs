using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace STrain.CQS.Extensions
{
    public class StopwatchLogger : IDisposable
    {
        private readonly ILogger _logger;
        private readonly LogLevel _level;
        private readonly string _message;
        private readonly Stopwatch _stopwatch;
        private bool _disposedValue;

        public StopwatchLogger(ILogger logger, LogLevel level, string message)
        {
            _logger = logger;
            _level = level;
            _message = message;
            _stopwatch = Stopwatch.StartNew();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stopwatch.Stop();
                    _logger.Log(_level, _message, _stopwatch.ElapsedMilliseconds);
                }

                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
