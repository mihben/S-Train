using STrain;
using STrain.CQS.Dispatchers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCQS(this IServiceCollection services)
        {
            services.AddScoped<IRequestDispatcher, RequestDispatcher>();
        }

        public static void AddPerformer<TPerformer, TImplementation>(this IServiceCollection services)
            where TPerformer : class
            where TImplementation : class, TPerformer
        {
            services.AddScoped<TPerformer, TImplementation>();
        }
    }
}