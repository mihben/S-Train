using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using STrain.Tracing.Core.Contexts;
using STrain.Tracing.Http;

namespace STrain.Tracing.NetCore.Middlewares
{
    public class TracingMiddleware
    {
        private readonly TracingContext _context;
        private readonly RequestDelegate _next;
        private readonly ILogger<TracingMiddleware> _logger;

        public TracingMiddleware(TracingContext context, RequestDelegate next, ILogger<TracingMiddleware> logger)
        {
            _context = context;
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _context.RequestId = context.Request.Headers.Get(Constants.Headers.REQUEST_ID, () => Guid.NewGuid());
            _logger.LogSetValue(nameof(_context.RequestId), _context.RequestId);

            _context.CorrelationId = context.Request.Headers.Get(Constants.Headers.CORRELATION_ID, () => Guid.NewGuid());
            _logger.LogSetValue(nameof(_context.CorrelationId), _context.CorrelationId);

            _context.CausationId = context.Request.Headers.Get(Constants.Headers.CAUSATION_ID, () => null);
            _logger.LogSetValue(nameof(_context.CausationId), _context.CausationId);

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [nameof(_context.RequestId)] = _context.RequestId!,
                [nameof(_context.CorrelationId)] = _context.CorrelationId!,
                [nameof(_context.CausationId)] = _context.CausationId.HasValue ? _context.CausationId : string.Empty
            }))
            {
                await _next.Invoke(context);
            }
        }

    }

    internal static class TracingMiddlewareExtensions
    {
        public static void LogSetValue(this ILogger<TracingMiddleware> logger, string header, Guid? value) => logger.LogDebug("Set {header} to {value}", header, value);

        public static Guid? Get(this IHeaderDictionary headers, string key, Func<Guid?> generator)
        {
            var header = headers[key];
            if (header.Count > 0) return Guid.Parse(header.First());
            return generator();
        }
    }
}
