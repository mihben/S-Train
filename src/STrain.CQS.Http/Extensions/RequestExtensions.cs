using STrain.CQS;
using System.Reflection;
using System.Web;

namespace STrain
{
    internal static class RequestExtensions
    {
        public static string? AsQueryString<TRequest>(this TRequest request)
            where TRequest : IRequest
        {
            var collection = HttpUtility.ParseQueryString("");
            foreach (var property in request.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = property.GetValue(request);
                if (value is not null) collection.Add(property.Name, value.ToString());
            }
            return collection.ToString();
        }
    }
}
