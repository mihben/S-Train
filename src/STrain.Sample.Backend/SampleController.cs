using Microsoft.AspNetCore.Mvc;
using STrain.Sample.Api;

namespace STrain.Sample.Backend
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IRequestDispatcher _dispatcher;

        public SampleController(IRequestDispatcher dispatcher)
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
