﻿using Microsoft.Extensions.Logging;
using STrain.CQS.Api;
using System.Diagnostics;

namespace STrain.CQS.Receivers
{
    public class RequestLogger<TResponse> : IRequestReceiver<TResponse>
    {
        private readonly IRequestReceiver<TResponse> _requestReceiver;
        private readonly ILogger<RequestLogger<TResponse>> _logger;

        public RequestLogger(IRequestReceiver<TResponse> requestReceiver, ILogger<RequestLogger<TResponse>> logger)
        {
            _requestReceiver = requestReceiver;
            _logger = logger;
        }

        public async Task<TResponse> ReceiveCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
        {
#pragma warning disable CA2017 // Parameter count mismatch
            using var stopwatch = _logger.LogStopwatch("Received command in {ElapsedTime} ms");
#pragma warning restore CA2017 // Parameter count mismatch
            var response = await _requestReceiver.ReceiveCommandAsync(command, cancellationToken).ConfigureAwait(false);
            _logger.LogTrace("Response object: {@ResponseObject}", response);
            return response;
        }

        public async Task<TResponse> ReceiveQueryAsync<TQuery>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using var _ = _logger.BeginScope(new Dictionary<string, object> { ["Query"] = query.GetType() });
                _logger.LogDebug("Receiving query");

                var response = await _requestReceiver.ReceiveQueryAsync(query, cancellationToken);
                _logger.LogTrace("Response object: {@ResponseObject}", response);
                return response;
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogDebug("Received query in {ElapsedTime} ms", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
