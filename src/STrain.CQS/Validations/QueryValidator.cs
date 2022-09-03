using Microsoft.Extensions.Logging;

namespace STrain.CQS.Validations
{
    public class QueryValidator : IQueryDispatcher
    {
        private readonly IRequestValidator _validator;
        private readonly IQueryDispatcher _dispatcher;
        private readonly ILogger<QueryValidator> _logger;

        public QueryValidator(IRequestValidator validator, IQueryDispatcher dispatcher, ILogger<QueryValidator> logger)
        {
            _validator = validator;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task<T> DispatchAsync<T>(Query<T> query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Validating query");
            await _validator.ValidateAsync(query, cancellationToken);
            return await _dispatcher.DispatchAsync(query, cancellationToken);
        }
    }
}
