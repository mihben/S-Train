using STrain.CQS.Attributes.RequestSending.Http.Parameters;
using System.Reflection;

namespace STrain.CQS.NetCore.RequestSending.Providers.Attributive
{
    public class AttributiveHeaderParameterProvider : IParameterProvider
    {
        public Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
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
                {
                    message.Headers.Add(attribute.Name ?? property.Name, property.GetValue(request)?.ToString() ?? string.Empty);
                }
            }

            return Task.CompletedTask;
        }
    }
}
