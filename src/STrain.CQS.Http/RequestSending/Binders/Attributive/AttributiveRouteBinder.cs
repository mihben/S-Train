using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace STrain.CQS.Http.RequestSending.Binders.Attributive
{
    public class AttributiveRouteBinder : IRouteBinder
    {
        private readonly ILogger<AttributiveRouteBinder> _logger;

        private const string _pattern = "\\{[a-zA-Z\\-_]*\\}";

        public AttributiveRouteBinder(ILogger<AttributiveRouteBinder> logger)
        {
            _logger = logger;
        }

        public Task<string> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            _logger.LogDebug("Binding route");

            var type = typeof(TRequest);
            var attribute = type.GetCustomAttribute<RouteAttribute>();
            if (attribute is null) throw new InvalidOperationException("Route attribute is not found.");

            var result = attribute.Path;
            foreach (var match in new Regex(_pattern).Matches(result).Select(m => m.Value))
            {
                var name = match.Trim('{').Trim('}');
                var parameter = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (parameter is null) throw new InvalidOperationException($"Property '{name}' is not found.");

                var value = parameter.GetValue(request);
                if (value is null) throw new InvalidOperationException("Route property cannot be null.");
                result = result.Replace(match, value.ToString());
            }

            _logger.LogTrace("Route: {route}", result);
            return Task.FromResult(result);
        }
    }
}
