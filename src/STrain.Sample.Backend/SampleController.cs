using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STrain.Core.Exceptions;
using STrain.CQS.Dispatchers;
using STrain.Sample.Api;

namespace STrain.Sample.Backend
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly IRequestSender _sender;
        private readonly ICommandDispatcher _dispatcher;

        public SampleController(IRequestSender sender, ICommandDispatcher dispatcher)
        {
            _sender = sender;
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SampleCommand command, CancellationToken cancellationToken)
        {
            await _sender.SendAsync(command, cancellationToken);
            return Accepted();
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            return Ok(await _sender.GetAsync<SampleQuery, SampleQuery.Result>(new SampleQuery("Test-Value"), cancellationToken));
        }

        [HttpGet("external")]
        public async Task<ActionResult<string>> GetAsync([FromQuery] string criteria, CancellationToken cancellationToken)
        {
            return Ok(await _sender.SendAsync<ExternalSampleRequest, string>(new ExternalSampleRequest(criteria), cancellationToken));
        }

        [Authorize(AuthenticationSchemes = "Unathorized")]
        [HttpGet("authorized-endpoint")]
        public Task<IActionResult> AuthorizeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IActionResult)Ok());
        }

        [HttpGet("internal-server-error")]
        public Task<IActionResult> InternalServerErrorAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException();
        }

        [HttpGet("verification-error")]
        public Task<IActionResult> VerificationErrorAsync(CancellationToken cancellationToken)
        {
            throw new VerificationException("/errors/custom-verification-error", "Verification error.", "Custom verification error. Can be used for business logic related errors.");
        }

        [HttpGet("Forbidden")]
        [Authorize(AuthenticationSchemes = "Forbidden", Policy = "forbidden-policy")]
        public Task<IActionResult> ForbiddenAsync(CancellationToken cancellationToken)
        {
            throw new VerificationException("/errors/custom-verification-error", "Verification error.", "Custom verification error. Can be used for business logic related errors.");
        }

        [HttpGet("invalid-request")]
        public async Task<IActionResult> InvalidRequestAsyncAsync(CancellationToken cancellationToken)
        {
            await _dispatcher.DispatchAsync(new SampleCommand(string.Empty), cancellationToken);
            return Ok();
        }
    }
}
