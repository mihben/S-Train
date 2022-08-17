using Microsoft.Extensions.Logging;

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

        public async Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var key = _requestSenderKeyProvider(request);
            if (key is null) throw new InvalidOperationException($"Request sender key cannot be determined for {request}");

            var sender = _requestSenderFactory(key);
            if (sender is null) throw new InvalidOperationException($"Request sender was not found with {key}");

            return await sender.SendAsync<TRequest, T>(request, cancellationToken);
        }

        public async Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T> => await SendAsync<TQuery, T>(query, cancellationToken);

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command => await SendAsync<TCommand, object>(command, cancellationToken);
    }
}
