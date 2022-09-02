using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> ReceiveAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : Command
        {
            await _commandDispatcher.DispatchAsync(command, cancellationToken);
            return new AcceptedResult();
        }

        public async Task<IActionResult> ReceiveAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken) where TQuery : Query<T>
        {
            return new OkObjectResult(await _queryDispatcher.DispatchAsync(query, cancellationToken));
        }
    }
}
