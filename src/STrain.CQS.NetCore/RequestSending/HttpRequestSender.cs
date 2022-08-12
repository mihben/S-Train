using Microsoft.Extensions.Logging;
using STrain.CQS.Senders;

namespace STrain.CQS.NetCore.RequestSending
{
    public class HttpRequestSender : IRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IPathProvider _pathProvider;
        private readonly IMethodProvider _methodProvider;
        private readonly ILogger<HttpRequestSender> _logger;

        public HttpRequestSender(HttpClient httpClient, IPathProvider pathProvider, IMethodProvider methodProvider, ILogger<HttpRequestSender> logger)
        {
            _httpClient = httpClient;
            _pathProvider = pathProvider;
            _methodProvider = methodProvider;
            _logger = logger;
        }

        public Task GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command
        {
            var message = new HttpRequestMessage(_methodProvider.GetMethod<TCommand>(), _pathProvider.GetPath(command));

            _logger.LogDebug("Sending request to {uri}", message.RequestUri);
            _logger.LogTrace("Request message: {@message}", message);
            await _httpClient.SendAsync(message, cancellationToken);
        }
    }
}
