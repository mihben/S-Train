using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider provider, ILogger<CommandDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            _logger.LogDebug("Attempting to dispatch command", command.LogEntry());
            var performer = _provider.GetService<ICommandPerformer<TCommand>>();

            if (performer is null) throw new NotImplementedException($"Performer was not found for {command.LogEntry()}");

            await performer.PerformAsync(command, cancellationToken);
            _logger.LogDebug("Done attempting to dispatch command");
        }
    }
}
