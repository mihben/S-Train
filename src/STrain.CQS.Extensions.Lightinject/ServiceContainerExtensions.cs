using Microsoft.Extensions.Logging;
using STrain;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Providers;
using STrain.CQS.Senders;

namespace LightInject
{
    public static class ServiceContainerExtensions
    {
        public static void AddPathProvider<TPathProvider>(this IServiceContainer container, string key)
            where TPathProvider : class, IPathProvider
        {
            container.RegisterTransient<IPathProvider, TPathProvider>(key);
        }

        public static void AddMethodProvider<TMethodProvider>(this IServiceContainer container, string key)
            where TMethodProvider : class, IMethodProvider
        {
            container.RegisterTransient<IMethodProvider, TMethodProvider>(key);
        }

        public static void AddParameterProvider<TParameterProvider>(this IServiceContainer container, string key)
            where TParameterProvider : class, IParameterProvider
        {
            container.RegisterTransient<IParameterProvider, TParameterProvider>(key);
        }

        public static void AddHttpSender(this IServiceContainer container, string key)
        {
            container.RegisterTransient<IRequestSender>(factory =>
            {
                var clientFactory = factory.GetInstance<IHttpClientFactory>();

                var pathProvider = factory.GetInstance<IPathProvider>(key);
                var methodProvider = factory.GetInstance<IMethodProvider>(key);
                var parameterProviders = new List<IParameterProvider>
                {
                    factory.GetInstance<IParameterProvider>($"{key}.header"),
                    factory.GetInstance<IParameterProvider>($"{key}.query"),
                    factory.GetInstance<IParameterProvider>($"{key}.body")
                };
                var responseReaderProvider = factory.GetInstance<IResponseReaderProvider>(key);
                var requestErrrorHandler = factory.GetInstance<IRequestErrorHandler>(key);

                return new HttpRequestSender(clientFactory.CreateClient(key),
                                                factory.GetInstance<IServiceProvider>(),
                                                pathProvider,
                                                methodProvider,
                                                parameterProviders,
                                                responseReaderProvider,
                                                requestErrrorHandler,
                                                factory.GetInstance<ILogger<HttpRequestSender>>());
            }, key);
        }
    }
}