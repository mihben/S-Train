using Microsoft.AspNetCore.Mvc;
using STrain.CQS.Dispatchers;
using STrain.Sample.Api;

namespace STrain.Sample.Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;

        public SampleController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            await _dispatcher.DispatchAsync(command, cancellationToken);
            return Accepted();
        }
    }
}
