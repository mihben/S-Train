using Microsoft.Extensions.Logging;
using STrain.CQS.Attributes.RequestSending.Http;
using System.Reflection;

namespace STrain.CQS.Http.RequestSending.Binders.Attributive
{
    public class AttributiveMethodBinder : IMethodBinder
    {
        private readonly ILogger<AttributiveMethodBinder> _logger;

        public AttributiveMethodBinder(ILogger<AttributiveMethodBinder> logger)
        {
            _logger = logger;
        }

        public Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("Binding method");
            var attribute = typeof(TRequest).GetCustomAttribute<MethodAttribute>();
            if (attribute is null) throw new InvalidOperationException("Method attribute is not found.");

            _logger.LogTrace("Method: {method}", attribute.Method);
            return Task.FromResult(attribute.Method);
        }
    }
}
