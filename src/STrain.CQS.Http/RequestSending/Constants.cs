using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace STrain.CQS.Http.RequestSending
{
    [ExcludeFromCodeCoverage]
    internal static class Constants
    {
        public static class HttpRequestSender
        {
            public const BindingFlags PropertyBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
        }
    }
}
