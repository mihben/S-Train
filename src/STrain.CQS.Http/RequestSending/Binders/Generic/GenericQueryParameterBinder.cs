using Microsoft.Extensions.Logging;

namespace STrain.CQS.Http.RequestSending.Providers.Generic
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

            return Task.FromResult(request.AsQueryString());
        }
    }
}
