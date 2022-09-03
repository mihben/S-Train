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
            return await _requestReceiver.ReceiveCommandAsync(new AuthorizedCommand(), cancellationToken);
        }

        [HttpGet("unauthorized")]
        public async Task<IActionResult> UnauthorizedAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveCommandAsync(new UnathorizedCommand(), cancellationToken);
        }

        [HttpGet("forbidden")]
        public async Task<IActionResult> ForbiddenAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveCommandAsync(new ForbiddenCommand(), cancellationToken);
        }

        [HttpGet("allow-anonymus")]
        public async Task<IActionResult> AllowAnonymusAsync(CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveCommandAsync(new AllowAnonymusCommand(), cancellationToken);
        }
    }
}
