using Microsoft.Extensions.Options;
using STrain.CQS.NetCore.RequestSending.Providers;
using System.Reflection;
using System.Text.RegularExpressions;

namespace STrain.CQS.NetCore.RequestSending.Attributive
{
    public class AttributivePathProvider : IPathProvider
    {
        private readonly IOptions<HttpRequestSenderOptions> _options;

        public AttributivePathProvider(IOptions<HttpRequestSenderOptions> options)
        {
            _options = options;
        }

        public string GetPath<TRequest>(TRequest request) where TRequest : IRequest
        {
            var type = typeof(TRequest);
            var attribute = type.GetRouteAttribute();
            if (attribute is null) return _options.Value.Path;
            return attribute.Path.ReplaceParameters(request);
        }
    }

    internal static class AttributivePathProviderExtensions
    {
        public static string ReplaceParameters<TRequest>(this string path, TRequest request)
        {
            var result = path;
            var type = typeof(TRequest);
            foreach (Match match in new Regex("{[a-zA-Z0-9_]*}").Matches(path).Cast<Match>())
            {
                var property = type.GetProperty(match.Value.Trim('{', '}'), Constants.HttpRequestSender.PropertyBindings | BindingFlags.IgnoreCase);
                if (property is null) throw new InvalidOperationException($"{match.Value} not found in {type}");
                result = result.Replace(match.Value, property.GetValue(request)?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
