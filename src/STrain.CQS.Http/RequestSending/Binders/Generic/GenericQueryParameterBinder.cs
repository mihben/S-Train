using Microsoft.Extensions.Logging;
using STrain.CQS.Api;

namespace STrain.CQS.Http.RequestSending.Binders.Generic
{
    public class GenericQueryParameterBinder : IQueryParameterBinder
    {
        private readonly ILogger<GenericQueryParameterBinder> _logger;

        public GenericQueryParameterBinder(ILogger<GenericQueryParameterBinder> logger)
        {
            _logger = logger;
        }

        public Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (typeof(TRequest).IsAssignableTo(typeof(ICommand))) return Task.FromResult<string?>(null);

            return Task.FromResult(request.AsQueryString());
        }
    }
}
