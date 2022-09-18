using Microsoft.Extensions.Logging;
using STrain.CQS.Api;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericMethodBinder : IMethodBinder
    {
        private readonly ILogger<GenericMethodBinder> _logger;

        public GenericMethodBinder(ILogger<GenericMethodBinder> logger)
        {
            _logger = logger;
        }

        public Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Binding method");
            if (typeof(TRequest).IsAssignableTo(typeof(ICommand))) return Task.FromResult(HttpMethod.Post);
            if (typeof(TRequest).IsAssignableTo(typeof(IQuery))) return Task.FromResult(HttpMethod.Get);

            throw new NotSupportedException("Unsupported request type");
        }
    }
}
