using Microsoft.Extensions.Logging;

namespace STrain.CQS.Senders
{
    public class RequestRouter : IRequestSender
    {
        private readonly ILogger<RequestRouter> _logger;

        public RequestRouter(ILogger<RequestRouter> logger)
        {
            _logger = logger;
        }

        public Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken) where TQuery : Query<T>
        {
            throw new NotImplementedException();
        }

        public Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : Command
        {
            throw new NotImplementedException();
        }
    }
}
