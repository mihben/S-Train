using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.Dispatchers
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<QueryDispatcher> _logger;

        public QueryDispatcher(IServiceProvider provider, ILogger<QueryDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task<T> DispatchAsync<T>(Query<T> query, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Dispatching {query}", query.LogEntry());
            var type = typeof(IQueryPerformer<,>).MakeGenericType(query.GetType(), typeof(T));
            var performer = _provider.GetService(type);

            if (performer is null) throw new NotImplementedException($"Performer was not found for {query.LogEntry()}");

            return await ((dynamic)performer).PerformAsync((dynamic)query, cancellationToken);
        }
    }
}
