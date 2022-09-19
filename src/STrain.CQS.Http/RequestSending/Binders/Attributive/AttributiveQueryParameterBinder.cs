using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Web;

namespace STrain.CQS.Http.RequestSending.Binders.Attributive
{
    public class AttributiveQueryParameterBinder : IQueryParameterBinder
    {
        private readonly ILogger<AttributiveQueryParameterBinder> _logger;

        public AttributiveQueryParameterBinder(ILogger<AttributiveQueryParameterBinder> logger)
        {
            _logger = logger;
        }

        public Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            _logger.LogDebug("Binding query parameters");

            var type = typeof(TRequest);
            string? result;
            if (type.GetCustomAttribute<QueryParameterAttribute>() is not null) result = request.SerializeToQueryString(type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
            else result = request.SerializeToQueryString(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                        .Where(p => p.GetCustomAttribute<QueryParameterAttribute>() is not null));

            _logger.LogTrace("Query parameter: {query}", result);
            return Task.FromResult(result);
        }
    }

    internal static class AttributiveQueryParameterBinderExtensions
    {
        public static string? SerializeToQueryString<TRequest>(this TRequest request, IEnumerable<PropertyInfo> properties)
        {
            var result = HttpUtility.ParseQueryString(string.Empty);
            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttribute<QueryParameterAttribute>();
                var value = property.GetValue(request);
                if (value is not null) result.Add(attribute?.Name ?? property.Name.ToLower(), value.ToString());
            }
            return result.ToString();
        }
    }
}
