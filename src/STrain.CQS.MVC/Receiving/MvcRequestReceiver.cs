using Microsoft.AspNetCore.Mvc;
using STrain.CQS.Api;
using STrain.CQS.Dispatchers;
using STrain.CQS.Receivers;

namespace STrain
{
    public interface IMvcRequestReceiver : IRequestReceiver<IActionResult> { }

    public class MvcRequestReceiver : IMvcRequestReceiver
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public MvcRequestReceiver(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public async Task<IActionResult> ReceiveCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
            await _commandDispatcher.DispatchAsync((dynamic)command, cancellationToken);
            return new AcceptedResult();
        }

        public async Task<IActionResult> ReceiveQueryAsync<TQuery>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery
        {
            return new OkObjectResult(await _queryDispatcher.DispatchAsync((dynamic)query, cancellationToken));
        }
    }
}
