using Microsoft.Extensions.Logging;
using STrain.CQS.Api;
using System.Net.Http.Json;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericBodyParameterBinder : IBodyParameterBinder
    {
        private readonly ILogger<GenericBodyParameterBinder> _logger;

        public GenericBodyParameterBinder(ILogger<GenericBodyParameterBinder> logger)
        {
            _logger = logger;
        }

        public Task<HttpContent?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            _logger.LogDebug("Binding body parameter");
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (typeof(TRequest).IsAssignableTo(typeof(IQuery)))
            {
                _logger.LogDebug("Skipped binding body parameter (Request is IQuery)");
                return Task.FromResult<HttpContent?>(null);
            }

            var result = JsonContent.Create(request);
            _logger.LogTrace("Body parameter: {@bodyParameter}", result);
            return Task.FromResult((HttpContent?)result);
        }
    }
}
