using Microsoft.Extensions.Logging;
using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using System.Net.Http.Headers;
using System.Reflection;

namespace STrain.CQS.Http.RequestSending.Binders.Attributive
{
    public class AttributiveHeaderParameterBinder : IHeaderParameterBinder
    {
        private readonly ILogger<AttributiveHeaderParameterBinder> _logger;

        public AttributiveHeaderParameterBinder(ILogger<AttributiveHeaderParameterBinder> logger)
        {
            _logger = logger;
        }

        public Task BindAsync<TRequest>(TRequest request, HttpRequestHeaders headers, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Binding header parameters");

            var type = typeof(TRequest);
            if (type.GetCustomAttribute<HeaderParameterAttribute>() is not null) request.SerializeToHeaders(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), headers);
            else request.SerializeToHeaders(type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        .Where(p => p.GetCustomAttribute<HeaderParameterAttribute>() is not null), headers);

            return Task.CompletedTask;
        }
    }

    internal static class AttributiveHeaderParameterBinderExtensions
    {
        public static void SerializeToHeaders<TRequest>(this TRequest request, IEnumerable<PropertyInfo> properties, HttpRequestHeaders headers)
        {
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<HeaderParameterAttribute>();
                var value = property.GetValue(request);
                if (value is not null) headers.Add(attribute?.Name ?? property.Name, value.ToString());
            }
        }
    }
}
