using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Providers;
using STrain.CQS.Http.RequestSending.Readers;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.MVC.Options;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.NetCore
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

            services.AddControllers(options => options.ModelBinderProviders.Insert(0, new RequestModelBinderProvider()))
                .AddApplicationPart(typeof(GenericRequestController).Assembly);
        }

        public static void AddPathProvider<TPathProvider>(this IServiceCollection services)
            where TPathProvider : class, IPathBinder
        {
            services.AddTransient<IPathBinder, TPathProvider>();
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
