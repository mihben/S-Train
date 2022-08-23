﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            return Ok(await _sender.GetAsync<SampleQuery, SampleQuery.Result>(new SampleQuery("Test-Value"), cancellationToken));
        }

        [HttpGet("external")]
        public async Task<ActionResult<string>> GetAsync([FromQuery]string criteria, CancellationToken cancellationToken)
        {
            return Ok(await _sender.SendAsync<ExternalSampleRequest, string>(new ExternalSampleRequest(criteria), cancellationToken));
        }
    }
}
