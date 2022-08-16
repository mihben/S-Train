using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.MVC.Options;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Parsers;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.Senders;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void AddGenericRequestHandler(this IServiceCollection services, Action<GenericRequestHandlerOptions, IConfiguration> configure)
        {
            services.AddOptions<GenericRequestHandlerOptions>()
                .Configure(configure)
                .Validate(options => !string.IsNullOrWhiteSpace(options.Path), "Path is required for generic request handler")
                .PostConfigure(options =>
                {
                    if (options.Path is not null) options.Path = options.Path.TrimStart('/');
                })
                .ValidateOnStart();

            services.AddControllers()
                .AddApplicationPart(typeof(GenericRequestController).Assembly);

            services.AddControllers(options => options.ModelBinderProviders.Insert(0, new RequestModelBinderProvider()));
        }

        public static void AddHttpRequestSender(this IServiceCollection services, Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            services.AddOptions<HttpRequestSenderOptions>()
                .Configure(configure)
                .Validate(options => options.BaseAddress is not null, "Base address is required")
                .PostConfigure(options =>
                {
                    if (options.BaseAddress is not null
                        && !options.BaseAddress.AbsoluteUri.EndsWith('/')) options.BaseAddress = new Uri(options.BaseAddress, "/");
                })
                .ValidateOnStart();

            services.AddHttpClient<IRequestSender, HttpRequestSender>((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<HttpRequestSenderOptions>>();
                client.BaseAddress = options.Value.BaseAddress;
            });
        }

        public static void AddPathProvider<TPathProvider>(this IServiceCollection services)
            where TPathProvider : class, IPathProvider
        {
            services.AddTransient<IPathProvider, TPathProvider>();
        }

        public static void AddMethodProvider<TMethodProvider>(this IServiceCollection services)
            where TMethodProvider : class, IMethodProvider
        {
            services.AddTransient<IMethodProvider, TMethodProvider>();
        }

        public static void AddParameterProvider<TParameterProvider>(this IServiceCollection services)
            where TParameterProvider : class, IParameterProvider
        {
            services.AddTransient<IParameterProvider, TParameterProvider>();
        }

        public static void AddResponseReader<TResponseReader>(this IServiceCollection services)
            where TResponseReader : class, IResponseReader
        {
            services.AddTransient<TResponseReader>();
        }

        public static void AddResponseReaderRegistry(this IServiceCollection services, ResponseReaderRegistry registry)
        {
            services.AddSingleton(registry);
            services.AddTransient<IResponseReaderProvider>(provider => provider.GetRequiredService<ResponseReaderRegistry>());
        }
    }
}
