using Microsoft.Extensions.Logging;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using System.Reflection;

namespace STrain.CQS.Http.RequestSending.Providers.Attributive
{
    public class AttributiveHeaderParameterProvider : IParameterProvider
    {
        private readonly ILogger<AttributiveHeaderParameterProvider> _logger;

        public AttributiveHeaderParameterProvider(ILogger<AttributiveHeaderParameterProvider> logger)
        {
            _logger = logger;
        }

        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Setting HTTP header parameter(s)");
            var type = typeof(TRequest);
            if (type.IsGenericRequest())
            {
                message.Headers.Add("request-type", $"{request.GetType().FullName}, {request.GetType().Assembly.GetName().Name}");
                return Task.CompletedTask;
            }
            if (type.GetCustomAttribute<HeaderParameterAttribute>() is not null)
            {
                foreach (var property in type.GetRelevantProperties())
                {
                    message.Headers.Add(property.Name, property.GetValue(request)?.ToString() ?? string.Empty);
                }
                return Task.CompletedTask;
            }

            foreach (var property in type.GetRelevantProperties())
            {
                var attribute = property.GetCustomAttribute<HeaderParameterAttribute>();
                if (attribute is not null)
                    message.Headers.Add(attribute.Name ?? property.Name, property.GetValue(request)?.ToString() ?? string.Empty);
            }

            _logger.LogDebug("Done setting HTTP header parameter(s)");
            return Task.CompletedTask;
        }
    }
}
