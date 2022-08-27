using STrain.CQS.Dispatchers;

namespace STrain.CQS.Validations
{
    public class CommandValidator : ICommandDispatcher
    {
        private readonly IRequestValidator _validator;
        private readonly ICommandDispatcher _dispatcher;

        public CommandValidator(IRequestValidator validator, ICommandDispatcher dispatcher)
        {
            _validator = validator;
            _dispatcher = dispatcher;
        }

        public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            await _validator.ValidateAsync(command, cancellationToken);
            await _dispatcher.DispatchAsync(command, cancellationToken);
        }
    }
}
