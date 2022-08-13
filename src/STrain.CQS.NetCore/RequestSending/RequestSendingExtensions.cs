using System.Reflection;

namespace STrain.CQS.NetCore.RequestSending
{
    internal static class RequestSendingExtensions
    {
        public static bool IsGenericRequest(this Type type)
        {
            return type.GetCustomAttribute<RouteAttribute>() is null;
        }

        public static IEnumerable<PropertyInfo> GetRelevantProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        }
    }
}
