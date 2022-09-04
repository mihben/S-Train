using Microsoft.AspNetCore.Mvc;

namespace STrain.Sample.Backend.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class ExternalController : ControllerBase
    {
        private readonly IRequestSender _sender;

        public ExternalController(IRequestSender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] int size, CancellationToken cancellationToken)
        {
            return Ok(await _sender.SendAsync<Api.Sample.ExternalRequest, IEnumerable<Api.Sample.ExternalRequest.Result>>(new Api.Sample.ExternalRequest(size), cancellationToken));
        }
    }
}
