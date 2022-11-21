using Microsoft.AspNetCore.Builder;
using STrain.Tracing.NetCore.Middlewares;

namespace STrain.Tracing.NetCore
{
    public static class WebApplicationExtensions
    {
        public static void UseTracing(this WebApplication application)
        {
            application.UseMiddleware<TracingMiddleware>();
        }
    }
}
