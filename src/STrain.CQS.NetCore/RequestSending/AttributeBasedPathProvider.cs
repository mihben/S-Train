using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.RegularExpressions;

namespace STrain.CQS.NetCore.RequestSending
{
    public class AttributeBasedPathProvider : IPathProvider
    {
        private readonly IOptions<HttpRequestSenderOptions> _options;

        public AttributeBasedPathProvider(IOptions<HttpRequestSenderOptions> options)
        {
            _options = options;
        }

        public string GetPath<TRequest>(TRequest request) where TRequest : IRequest
        {
            var type = typeof(TRequest);
            var attribute = type.GetCustomAttribute<PathAttribute>();

            if (attribute is null) return _options.Value.Path;
            return attribute.Path.ReplaceParameters(request);
        }
    }

    internal static class AttributeBasedPathProviderExtensions
    {
        public static string ReplaceParameters<TRequest>(this string path, TRequest request)
        {
            var result = path;
            var type = typeof(TRequest);
            foreach (Match match in new Regex("{[a-zA-Z0-9_]*}").Matches(path))
            {
                var property = type.GetProperty(match.Value.Trim('{', '}'), BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                if (property is null) throw new InvalidOperationException($"{match.Value} not found in {type}");
                result = result.Replace(match.Value, property.GetValue(request)?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
