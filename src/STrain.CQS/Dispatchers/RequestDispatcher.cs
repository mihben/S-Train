using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.Dispatchers
{
    public class RequestDispatcher : IRequestDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RequestDispatcher> _logger;

        public RequestDispatcher(IServiceProvider provider, ILogger<RequestDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            _logger.LogDebug("Dispatching {command}", command.LogEntry());
            var performers = _provider.GetService<IEnumerable<ICommandPerformer<TCommand>>>();

            if (performers is null || !performers.Any()) throw new NotImplementedException($"Performer was not found for {command.LogEntry()}");
            if (performers.Count() > 1) throw new InvalidOperationException($"More than one performers are registered for {command.LogEntry()}");

            await performers.First().PerformAsync(command, cancellationToken);
        }

        public async Task<T> DispatchAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken) where TQuery : Query<T>
        {
            _logger.LogDebug("Dispatching {query}", query.LogEntry());
            var performers = _provider.GetService<IEnumerable<IQueryPerformer<TQuery, T>>>();

            if (performers is null || !performers.Any()) throw new NotImplementedException($"Performer was not found for {query.LogEntry()}");
            if (performers.Count() > 1) throw new InvalidOperationException($"More than one performers are registered for {query.LogEntry()}");

            return await performers.First().PerformAsync(query, cancellationToken);
        }
    }
}
