﻿using Microsoft.Extensions.Logging;

namespace STrain.CQS.Performers
{
    public class CommandPerformerLogger<TCommand> : ICommandPerformer<TCommand>
        where TCommand : Command
    {
        private readonly ICommandPerformer<TCommand> _commandPerformer;
        private readonly ILogger<CommandPerformerLogger<TCommand>> _logger;

        public CommandPerformerLogger(ICommandPerformer<TCommand> commandPerformer, ILogger<CommandPerformerLogger<TCommand>> logger)
        {
            _commandPerformer = commandPerformer;
            _logger = logger;
        }

        public async Task PerformAsync(TCommand command, CancellationToken cancellationToken)
        {
#pragma warning disable CA2017 // Parameter count mismatch
            using var stopwatch = _logger.LogStopwatch("Performed command in {ElapsedTime} ms");
#pragma warning restore CA2017 // Parameter count mismatch
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["Performer"] = GetType() });
            _logger.LogDebug("Performing command");
            await _commandPerformer.PerformAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }

    public class QueryPerformerLogger<TQuery, TResponse> : IQueryPerformer<TQuery, TResponse>
        where TQuery : Query<TResponse>
    {
        private readonly IQueryPerformer<TQuery, TResponse> _queryPerformer;
        private readonly ILogger<QueryPerformerLogger<TQuery, TResponse>> _logger;

        public QueryPerformerLogger(IQueryPerformer<TQuery, TResponse> queryPerformer, ILogger<QueryPerformerLogger<TQuery, TResponse>> logger)
        {
            _queryPerformer = queryPerformer;
            _logger = logger;
        }

        public async Task<TResponse> PerformAsync(TQuery query, CancellationToken cancellationToken)
        {
#pragma warning disable CA2017 // Parameter count mismatch
            using var stopwatch = _logger.LogStopwatch("Performed query in {ElapsedTime} ms");
#pragma warning restore CA2017 // Parameter count mismatch
            using var scope = _logger.BeginScope(new Dictionary<string, object> { ["Performer"] = GetType() });
            var result = await _queryPerformer.PerformAsync(query, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
