using Microsoft.AspNetCore.Mvc;
using STrain.CQS.Dispatchers;
using STrain.CQS.Senders;
using STrain.Sample.Api;

namespace STrain.Sample.Backend
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IRequestSender _sender;

        public SampleController(IRequestSender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(command, cancellationToken);
            return Accepted();
        }
    }
}
