using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STrain.Core.Exceptions;
using STrain.Sample.Api;

namespace STrain.Sample.Backend.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly IMvcRequestReceiver _receiver;

        public ErrorController(IMvcRequestReceiver receiver)
        {
            _receiver = receiver;
        }

        [HttpGet("not-found")]
        public Task<IActionResult> NotFoundAsync(CancellationToken cancellationToken)
        {
            throw new NotFoundException("NotFoundedResource");
        }

        [HttpGet("internal-server-error")]
        public Task<IActionResult> InternalServerErrorAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException();
        }

        [HttpGet("verification-error")]
        public Task<IActionResult> VerificiationErrorAsync(CancellationToken cancellationToken)
        {
            throw new VerificationException("/errors/custom-verification-error", "Verification error.", "Custom verification error. Can be used for business logic related errors.");
        }

        [HttpGet("validation-error")]
        public Task<IActionResult> ValidationErrorAsync(CancellationToken cancellationToken)
        {
            return _receiver.ReceiveCommandAsync(new Error.ValidatedCommand(string.Empty), cancellationToken);
        }

        [Authorize(AuthenticationSchemes = "Unathorized")]
        [HttpGet("unathorized")]
        public Task<IActionResult> UnathorizedAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IActionResult)Ok());
        }

        [Authorize(AuthenticationSchemes = "Forbidden", Policy = "Forbidden")]
        [HttpGet("forbidden")]
        public Task<IActionResult> ForbiddenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IActionResult)Ok());
        }
    }
}
