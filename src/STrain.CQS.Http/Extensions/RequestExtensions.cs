using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using STrain.CQS;
using System.Collections;
using System.Reflection;

namespace STrain
{
    public static class RequestExtensions
    {
        public static IDictionary<string, StringValues> AsQueryString<TRequest>(this TRequest request)
            where TRequest : IRequest
        {
            var result = QueryHelpers.ParseQuery(string.Empty);
            foreach (var property in request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(request);
                if (value is not null && value is not string && value is IEnumerable values) result.Add(property.Name, values.AsStringValues());
                else if (value is not null) result.Add(property.Name, value.ToString());
            }
            return result;
        }

        public static StringValues AsStringValues(this IEnumerable values)
        {
            var stringValues = new List<string>();
            foreach (var value in values)
            {
                stringValues.Add(value?.ToString() ?? string.Empty);
            }
            return new StringValues(stringValues.ToArray());
        }
    }
}
