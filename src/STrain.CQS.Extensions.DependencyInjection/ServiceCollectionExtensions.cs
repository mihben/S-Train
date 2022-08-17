using STrain;
using STrain.CQS;
using STrain.CQS.Dispatchers;
using STrain.CQS.Senders;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddCQS(this IServiceCollection services)
        {
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        }

        public static void AddPerformer<TPerformer, TImplementation>(this IServiceCollection services)
            where TPerformer : class
            where TImplementation : class, TPerformer
        {
            services.AddScoped<TPerformer, TImplementation>();
        }

        public static void AddRequestRouter(this IServiceCollection services, Func<IRequest, string> requestKeyProvider)
        {
            services.AddSingleton(requestKeyProvider);
            services.AddScoped<IRequestSender, RequestRouter>();
        }
    }
}