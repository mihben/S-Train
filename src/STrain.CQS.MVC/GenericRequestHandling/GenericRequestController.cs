using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using STrain.CQS.Api;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.MVC.GenericRequestHandling
{
    [ExcludeFromCodeCoverage]
    public class GenericRequestController : ControllerBase
    {
        private readonly IMvcRequestReceiver _requestReceiver;
        private readonly ILogger<GenericRequestController> _logger;

        public GenericRequestController(IMvcRequestReceiver requestReceiver, ILogger<GenericRequestController> logger)
        {
            _requestReceiver = requestReceiver;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(ICommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Receiving ");
            return await _requestReceiver.ReceiveCommandAsync((dynamic)command, cancellationToken).ConfigureAwait(false);
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(IQuery query, CancellationToken cancellationToken)
        {
            return await _requestReceiver.ReceiveQueryAsync((dynamic)query, cancellationToken);
        }
    }
}
