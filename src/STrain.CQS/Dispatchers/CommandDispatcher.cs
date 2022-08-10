using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace STrain.CQS.Dispatchers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _provider;
        private ILogger<CommandDispatcher> _logger;

        public CommandDispatcher(IServiceProvider provider, ILogger<CommandDispatcher> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            _logger.LogDebug("Dispatching {command}", command.LogEntry());
            var performer = _provider.GetService<ICommandPerformer<TCommand>>();

            if (performer is null) throw new NotImplementedException($"Performer was not found for {command.LogEntry()}");

            await performer.PerformAsync(command, cancellationToken);
        }
    }
}
