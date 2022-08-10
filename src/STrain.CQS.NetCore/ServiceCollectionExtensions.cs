using Microsoft.Extensions.Configuration;
using STrain.CQS.MVC.GenericRequestHandling;
using STrain.CQS.MVC.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGenericRequestHandler(this IServiceCollection services, Action<GenericRequestHandlerOptions, IConfiguration> configure)
        {
            services.AddOptions<GenericRequestHandlerOptions>()
                .Configure(configure)
                .Validate(options => !string.IsNullOrWhiteSpace(options.Path), "Path is required for generic request handler")
                .Validate(options => !options.Path.StartsWith('/'), "Generic request handler path cannot be started with '/'");

            services.AddControllers()
                .AddApplicationPart(typeof(GenericRequestController).Assembly);

            services.AddControllers(options => options.ModelBinderProviders.Insert(0, new RequestModelBinderProvider()));
        }
    }
}
