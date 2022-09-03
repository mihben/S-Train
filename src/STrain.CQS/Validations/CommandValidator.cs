using Microsoft.Extensions.Logging;
using STrain.CQS.Dispatchers;

namespace STrain.CQS.Validations
{
    public class CommandValidator : ICommandDispatcher
    {
        private readonly IRequestValidator _validator;
        private readonly ICommandDispatcher _dispatcher;
        private readonly ILogger<CommandValidator> _logger;

        public CommandValidator(IRequestValidator validator, ICommandDispatcher dispatcher, ILogger<CommandValidator> logger)
        {
            _validator = validator;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            _logger.LogDebug("Validating command");
            await _validator.ValidateAsync(command, cancellationToken);
            await _dispatcher.DispatchAsync(command, cancellationToken);
        }
    }
}
