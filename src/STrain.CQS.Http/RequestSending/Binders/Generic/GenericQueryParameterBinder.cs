using Microsoft.AspNetCore.Http;
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
            _logger.LogDebug("Binding query parameters");
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (typeof(TRequest).IsAssignableTo(typeof(ICommand)))
            {
                _logger.LogDebug("Binding query parameters skipped (Request is ICommand)");
                return Task.FromResult<string?>(null);
            }

            var result = request.AsQueryString();
            _logger.LogTrace("Query parameter: {queryParameter}", result);
            return Task.FromResult<string?>(QueryString.Create(result).ToString());
        }
    }
}
