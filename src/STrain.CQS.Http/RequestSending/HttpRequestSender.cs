using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.CQS.Http.RequestSending.Readers;
using STrain.CQS.Senders;

namespace STrain.CQS.Http.RequestSending
{
    public class HttpRequestSender : IRequestSender
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRouteBinder _routeBinder;
        private readonly IMethodBinder _methodBinder;
        private readonly IQueryParameterBinder _queryParameterBinder;
        private readonly IHeaderParameterBinder _headerParameterBinder;
        private readonly IBodyParameterBinder _bodyParameterBinder;
        private readonly IResponseReaderProvider _responseReaderProvider;
        private readonly IRequestErrorHandler _requestErrorHandler;
        private readonly ILogger<HttpRequestSender> _logger;

        public HttpRequestSender(HttpClient httpClient,
                                 IServiceProvider serviceProvider,
                                 IRouteBinder routeBinder,
                                 IMethodBinder methodBinder,
                                 IQueryParameterBinder queryParameterBinder,
                                 IHeaderParameterBinder headerParameterBinder,
                                 IBodyParameterBinder bodyParameterBinder,
                                 IResponseReaderProvider responseReaderProvider,
                                 IRequestErrorHandler requestErrorHandler,
                                 ILogger<HttpRequestSender> logger)
        {
            _httpClient = httpClient;
            _serviceProvider = serviceProvider;
            _routeBinder = routeBinder;
            _methodBinder = methodBinder;
            _queryParameterBinder = queryParameterBinder;
            _headerParameterBinder = headerParameterBinder;
            _bodyParameterBinder = bodyParameterBinder;
            _responseReaderProvider = responseReaderProvider;
            _requestErrorHandler = requestErrorHandler;
            _logger = logger;
        }

        public async Task<T?> GetAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T> => await SendAsync<TQuery, T>(query, cancellationToken);

        public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command => await SendAsync<TCommand, object>(command, cancellationToken).ConfigureAwait(false);

        public Task<T?> SendAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (_httpClient.BaseAddress is null) throw new InvalidOperationException("Http client has not been initialized.");

            return SendInternalAsync<TRequest, T>(request, cancellationToken);
        }

        private async Task<T?> SendInternalAsync<TRequest, T>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest
        {
            _logger.LogDebug("Creating HTTP request");
            using (_logger.LogStopwatch("Sent HTTP request in {ElapsedTime} ms"))
            {

                var uriBuilder = new UriBuilder(_httpClient.BaseAddress!)
                {
                    Path = await _routeBinder.BindAsync(request, cancellationToken),
                    Query = await _queryParameterBinder.BindAsync(request, cancellationToken)
                };

                var message = new HttpRequestMessage
                {
                    RequestUri = uriBuilder.Uri,
                    Method = await _methodBinder.BindAsync(request, cancellationToken),
                    Content = await _bodyParameterBinder.BindAsync(request, cancellationToken)
                };
                await _headerParameterBinder.BindAsync(request, message.Headers, cancellationToken);

                _logger.LogDebug("Sending HTTP request to {RequestUri}", message.RequestUri);
                _logger.LogTrace("Request message: {@Message}", message);
                var response = await _httpClient.SendAsync(message, cancellationToken);
                _logger.LogTrace("Response message: {@Message}", response);
                if (!response.IsSuccessStatusCode) await _requestErrorHandler.HandleAsync(response, cancellationToken);

                if (response.Content.Headers.ContentLength == 0) return default;
                return (T?)await ((IResponseReader)_serviceProvider.GetRequiredService(_responseReaderProvider[response.Content.Headers.ContentType?.MediaType])).ReadAsync<T>(response, cancellationToken);
            }
        }
    }
}
