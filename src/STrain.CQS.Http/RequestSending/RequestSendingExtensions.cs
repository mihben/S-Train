using System.Reflection;

namespace STrain.CQS.Http.RequestSending
{
    internal static class RequestSendingExtensions
    {
        public static RouteAttribute? GetRouteAttribute(this Type type)
        {
            return type.GetCustomAttribute<RouteAttribute>();
        }

        public static bool IsGenericRequest(this Type type)
        {
            return type.GetRouteAttribute() is null;
        }

        public static IEnumerable<PropertyInfo> GetRelevantProperties(this Type type)
        {
            return type.GetProperties(Constants.HttpRequestSender.PropertyBindings);
        }
    }
}
