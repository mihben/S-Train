using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.RegularExpressions;

namespace STrain.CQS.Http.RequestSending.Providers.Attributive
{
    public class AttributivePathProvider : IPathProvider
    {
        private readonly string? _path;
        private readonly ILogger<AttributivePathProvider> _logger;

        public AttributivePathProvider(string? path, ILogger<AttributivePathProvider> logger)
        {
            _path = path;
            _logger = logger;
        }

        public string GetPath<TRequest>(TRequest request) where TRequest : IRequest
        {
            _logger.LogDebug("Attempting to determine HTTP request path");
            var type = typeof(TRequest);
            var attribute = type.GetRouteAttribute();

            var result = attribute?.Path.ReplaceParameters(request) ?? _path;
            if (result is null)
            {
                _logger.LogDebug("Fail attempting to determine HTTP request path");
                throw new InvalidOperationException($"Path of the {request} request cannot be determined. It must be configured via request attribute or in the configuration");
            }

            _logger.LogDebug("Done attempting to determine HTTP request path");
            return result;
        }
    }

    internal static class AttributivePathProviderExtensions
    {
        public static string ReplaceParameters<TRequest>(this string path, TRequest request)
        {
            var result = path;
            var type = typeof(TRequest);
            foreach (var parameter in new Regex("{[a-zA-Z0-9_]*}").Matches(path).Select(m => m.Value))
            {
                var property = type.GetProperty(parameter.Trim('{', '}'), Constants.HttpRequestSender.PropertyBindings | BindingFlags.IgnoreCase);
                if (property is null) throw new InvalidOperationException($"{parameter} not found in {type}");
                result = result.Replace(parameter, property.GetValue(request)?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
