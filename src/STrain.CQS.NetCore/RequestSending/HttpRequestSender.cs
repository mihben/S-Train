using Microsoft.Extensions.Logging;
using STrain.CQS.Senders;

namespace STrain.CQS.NetCore.RequestSending
{
    public class HttpRequestSender : IRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IPathProvider _pathProvider;
        //private readonly IMethodProvider _methodProvider;
        private readonly ILogger<HttpRequestSender> _logger;

        public HttpRequestSender(HttpClient httpClient, IPathProvider pathProvider, ILogger<HttpRequestSender> logger)
        {
            _httpClient = httpClient;
            _pathProvider = pathProvider;
            _logger = logger;
        }

        public Task GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        {
            //var type = typeof(TCommand);
            ////var message = new HttpRequestMessage(_methodProvider[type], _pathProvider[type]);

            //_logger.LogDebug("Sending request to {uri}", message.RequestUri);
            //_logger.LogTrace("Request message: {@message}", message);
            //await _httpClient.SendAsync(message, cancellationToken);
        }
    }
}
