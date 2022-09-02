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
            using var scope = _logger.BeginScope<Dictionary<string, object>>(new Dictionary<string, object> { ["Query"] = query.GetType() });
            _logger.LogTrace("Object: {@QueryObject}", query);
            var type = typeof(IQueryPerformer<,>).MakeGenericType(query.GetType(), typeof(T));
            var performer = _provider.GetService(type);

            if (performer is null) throw new NotImplementedException($"Performer was not found for {query.LogEntry()}");

            _logger.LogDebug("Performing {Performer}", performer);
            return await ((dynamic)performer).PerformAsync((dynamic)query, cancellationToken);
        }
    }
}
