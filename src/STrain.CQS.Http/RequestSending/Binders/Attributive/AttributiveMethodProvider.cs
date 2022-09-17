using Microsoft.Extensions.Logging;
using STrain.CQS.Api;
using STrain.CQS.Attributes.RequestSending.Http;
using System.Reflection;

namespace STrain.CQS.Http.RequestSending.Providers.Attributive
{
    public class AttributiveMethodProvider : IMethodProvider
    {
        private readonly ILogger<AttributiveMethodProvider> _logger;

        public AttributiveMethodProvider(ILogger<AttributiveMethodProvider> logger)
        {
            _logger = logger;
        }

        public HttpMethod GetMethod<TRequest>()
            where TRequest : IRequest
        {
            _logger.LogDebug("Attempting to determine HTTP request method");
            var type = typeof(TRequest);
            HttpMethod? result = null;
            if (type.IsGenericRequest())
            {
                if (type.IsAssignableTo(typeof(ICommand))) result = HttpMethod.Post;
                if (type.IsAssignableTo(typeof(IQuery))) result = HttpMethod.Get;
            }

            var attribute = type.GetCustomAttribute<MethodAttribute>();
            if (attribute is not null) result = attribute.Method;

            if (result is null)
            {
                _logger.LogDebug("Fail attempting to determing HTTP request method");
                throw new InvalidOperationException($"Method of {type} cannot be determined");
            }

            _logger.LogDebug("Done attempting to determine HTTP request method");
            return result;
        }
    }
}
