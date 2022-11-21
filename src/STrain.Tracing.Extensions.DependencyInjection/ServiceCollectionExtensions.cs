using STrain.Tracing;
using STrain.Tracing.Core.Contexts;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTracing(this IServiceCollection services)
        {
            services.AddScoped<TracingContext>();
            services.AddScoped<ITracingContext<Guid?>>(provider => provider.GetRequiredService<TracingContext>());
        }
    }
}
