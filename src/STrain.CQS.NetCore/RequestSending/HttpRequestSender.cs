using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.NetCore.RequestSending.Parsers;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.Senders;
using System.Net.Http.Json;

namespace STrain.CQS.NetCore.RequestSending
{
    public class HttpRequestSender : IRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPathProvider _pathProvider;
        private readonly IMethodProvider _methodProvider;
        private readonly IEnumerable<IParameterProvider> _parameterProviders;
        private readonly IResponseReaderProvider _responseReaderProvider;
        private readonly ILogger<HttpRequestSender> _logger;

        public HttpRequestSender(HttpClient httpClient, IServiceProvider serviceProvider, IPathProvider pathProvider, IMethodProvider methodProvider, IEnumerable<IParameterProvider> parameterProviders, IResponseReaderProvider responseReaderProvider, ILogger<HttpRequestSender> logger)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
            _pathProvider = pathProvider;
            _methodProvider = methodProvider;
            _parameterProviders = parameterProviders;
            _responseReaderProvider = responseReaderProvider;
            _logger = logger;
        }

        public async Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T> => await SendAsync<TQuery, T>(query, cancellationToken).ConfigureAwait(false);

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command => await SendAsync<TCommand, object>(command, cancellationToken).ConfigureAwait(false);

        public async Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            var message = new HttpRequestMessage(_methodProvider.GetMethod<TRequest>(), $"{_httpClient.BaseAddress}{_pathProvider.GetPath(request)}");
            foreach (var parameterProvider in _parameterProviders)
            {
                await parameterProvider.SetParametersAsync(message, request, cancellationToken);
            }

            _logger.LogDebug("Sending request to {uri}", message.RequestUri);
            _logger.LogTrace("Request message: {message}", message);
            var response = await _httpClient.SendAsync(message, cancellationToken);
            if (response.Content.Headers.ContentLength == 0) return default;
            response.EnsureSuccessStatusCode();
            return (T?)(await ((IResponseReader)_serviceProvider.GetRequiredService(_responseReaderProvider[response.Content.Headers.ContentType.MediaType])).ReadAsync<T>(response, cancellationToken));
        }
    }
}
