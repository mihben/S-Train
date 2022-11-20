using Microsoft.Extensions.Logging;
using STrain.CQS;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.Tracing.Core.Contexts;
using System.Net.Http.Headers;

namespace STrain.Tracing.Http.Binders
{
    public class TracingHeaderParameterBinder : IHeaderParameterBinder
    {
        private readonly TracingContext _context;
        private readonly ILogger<TracingHeaderParameterBinder> _logger;

        public TracingHeaderParameterBinder(TracingContext context, ILogger<TracingHeaderParameterBinder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task BindAsync<TRequest>(TRequest request, HttpRequestHeaders headers, CancellationToken cancellationToken) where TRequest : IRequest
        {
            var requestId = Guid.NewGuid();
            _logger.LogDebug("Set {header} header to {requestId}", Constants.Headers.REQUEST_ID, requestId);
            headers.Add(Constants.Headers.REQUEST_ID, requestId.ToString());

            _context.CorrelationId ??= Guid.NewGuid();
            _logger.LogDebug("Set {header} header to {correlationId}", Constants.Headers.CORRELATION_ID, _context.CorrelationId);
            headers.Add(Constants.Headers.CORRELATION_ID, _context.CorrelationId.ToString());

            _logger.LogDebug("Set {header} header to {correlationId}", Constants.Headers.CAUSATION_ID, _context.RequestId);
            headers.Add(Constants.Headers.CAUSATION_ID, _context.RequestId.ToString());

            return Task.CompletedTask;
        }
    }
}
