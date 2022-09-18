using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Web;

namespace STrain.CQS.Http.RequestSending.Providers.Attributive
{
    public class AttributiveQueryParameterProvider : IParameterProvider
    {
        private readonly ILogger<AttributiveQueryParameterProvider> _logger;

        public AttributiveQueryParameterProvider(ILogger<AttributiveQueryParameterProvider> logger)
        {
            _logger = logger;
        }

        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (message.RequestUri is null) throw new ArgumentException("Uri is required");

            _logger.LogDebug("Setting HTTP query parameter(s)");
            var type = typeof(TRequest);
            if (!type.IsGenericRequest())
            {
                if (type.GetCustomAttribute<QueryParameterAttribute>() is not null) message.RequestUri = message.RequestUri.SetQuery(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty), request);
                else message.RequestUri = message.RequestUri.SetQuery(type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).Where(p => p.GetCustomAttribute<QueryParameterAttribute>() is not null), request);
            }

            _logger.LogDebug("Done setting HTTP query parameter(s)");
            return Task.CompletedTask;
        }
    }

    internal static class AttributiveQueryParameterProviderExtensions
    {
        public static Uri SetQuery<TRequest>(this Uri uri, IEnumerable<PropertyInfo> properties, TRequest request)
            where TRequest : IRequest
        {
            var uriBuilder = new UriBuilder(uri);
            var queryCollection = HttpUtility.ParseQueryString("");
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
                queryCollection.Add(attribute?.Name?.ToLower() ?? property.Name.ToLower(), property.GetValue(request)?.ToString() ?? string.Empty);
            }
            uriBuilder.Query = queryCollection.ToString();
            return uriBuilder.Uri;
        }
    }
}
