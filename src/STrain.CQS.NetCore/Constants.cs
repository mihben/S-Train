using System.Reflection;

namespace STrain.CQS.NetCore
{
    internal static class Constants
    {
        public static class HttpRequestSender
        {
            public const BindingFlags PropertyBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
        }
    }
}
