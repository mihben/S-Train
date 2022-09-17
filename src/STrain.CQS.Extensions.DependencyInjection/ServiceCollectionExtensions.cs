using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using STrain;
using STrain.CQS;
using STrain.CQS.Dispatchers;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Senders;
using STrain.CQS.Validations;
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

        public static void AddRequestValidator<TValidator>(this IServiceCollection services)
            where TValidator : class, IRequestValidator
        {
            services.AddTransient<IRequestValidator, TValidator>();
        }

        public static void AddHttpRequestSender(this IServiceCollection services, string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            services.AddOptions<HttpRequestSenderOptions>(key)
                .Configure(configure)
                .Validate(options => options.BaseAddress is not null, "Base address is required")
                .PostConfigure(options =>
                {
                    if (options.BaseAddress is not null
                        && !options.BaseAddress.AbsoluteUri.EndsWith('/')) options.BaseAddress = new Uri(options.BaseAddress, "/");
                })
                .ValidateOnStart();

            services.AddHttpClient(key, (provider, client) =>
            {
                var options = provider.GetRequiredService<IOptionsSnapshot<HttpRequestSenderOptions>>();
                client.BaseAddress = options.Get(key).BaseAddress;
            });
        }
    }
}