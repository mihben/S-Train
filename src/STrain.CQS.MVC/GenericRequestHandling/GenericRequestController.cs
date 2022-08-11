using Microsoft.AspNetCore.Mvc;
using STrain.CQS.Api;
using STrain.CQS.Dispatchers;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    public class GenericRequestController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public GenericRequestController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(ICommand command, CancellationToken cancellationToken)
        {
            await _commandDispatcher.DispatchAsync((dynamic)command, cancellationToken);
            return Accepted();
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(IQuery query, CancellationToken cancellationToken)
        {
            return Ok(await _queryDispatcher.DispatchAsync((dynamic)query, cancellationToken));
        }
    }
}
