using Microsoft.AspNetCore.Mvc;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    public class AuthorizationController : ControllerBase
    {
        private readonly IMvcRequestReceiver _requestReceiver;

        public AuthorizationController(IMvcRequestReceiver requestReceiver)
        {
            _requestReceiver = requestReceiver;
        }

        [HttpGet("authorized")]
        public async Task<IActionResult> AuthorizedAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveAsync(new AuthorizedCommand(), cancellationToken);
        }

        [HttpGet("unauthorized")]
        public async Task<IActionResult> UnauthorizedAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveAsync(new UnathorizedCommand(), cancellationToken);
        }

        [HttpGet("forbidden")]
        public async Task<IActionResult> ForbiddenAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveAsync(new ForbiddenCommand(), cancellationToken);
        }

        [HttpGet("allow-anonymus")]
        public async Task<IActionResult> AllowAnonymusAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveAsync(new AllowAnonymusCommand(), cancellationToken);
        }
    }
}
