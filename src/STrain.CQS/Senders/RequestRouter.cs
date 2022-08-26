﻿using Microsoft.Extensions.Logging;

namespace STrain.CQS.Senders
{
    public class RequestRouter : IRequestSender
    {
        private readonly Func<string, IRequestSender?> _requestSenderFactory;
        private readonly Func<IRequest, string?> _requestSenderKeyProvider;
        private readonly ILogger<RequestRouter> _logger;

        public RequestRouter(Func<string, IRequestSender?> requestSenderFactory, Func<IRequest, string?> requestSenderKeyProvider, ILogger<RequestRouter> logger)
        {
            _requestSenderFactory = requestSenderFactory;
            _requestSenderKeyProvider = requestSenderKeyProvider;
            _logger = logger;
        }

        public Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            return SendInternalAsync<TRequest, T>(request, cancellationToken);
        }

        private async Task<T?> SendInternalAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            var key = _requestSenderKeyProvider(request);
            if (key is null) throw new InvalidOperationException($"Request sender key cannot be determined for {request}");

            var sender = _requestSenderFactory(key);
            if (sender is null) throw new InvalidOperationException($"Request sender was not found with {key}");

            _logger.LogDebug("Sending {request} via {sender} sender", request.GetType(), key);
            return await sender.SendAsync<TRequest, T>(request, cancellationToken);
        }
    }
}
