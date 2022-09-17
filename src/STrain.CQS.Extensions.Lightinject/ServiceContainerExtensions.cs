using Microsoft.Extensions.Logging;
using STrain;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.CQS.Http.RequestSending.Providers;
using STrain.CQS.Senders;

namespace LightInject
{
    public static class ServiceContainerExtensions
    {
        public static void AddPathBinder<TRouteBinder>(this IServiceContainer container, string key)
            where TRouteBinder : class, IRouteBinder
        {
            container.RegisterTransient<IRouteBinder, TRouteBinder>(key);
        }

        public static void AddMethodBinder<TMethodBinder>(this IServiceContainer container, string key)
            where TMethodBinder : class, IMethodBinder
        {
            container.RegisterTransient<IMethodBinder, TMethodBinder>(key);
        }

        public static void AddQueryParameterBinder<TQueryParameterBinder>(this IServiceContainer container, string key)
            where TQueryParameterBinder : IQueryParameterBinder
        {
            container.RegisterTransient<IQueryParameterBinder, TQueryParameterBinder>(key);
        }

        public static void AddHeaderParameterBinder<THeaderParameterBinder>(this IServiceContainer container, string key)
            where THeaderParameterBinder : IHeaderParameterBinder
        {
            container.RegisterTransient<IHeaderParameterBinder, THeaderParameterBinder>(key);
        }

        public static void AddHttpSender(this IServiceContainer container, string key)
        {
            container.RegisterTransient<IRequestSender>(factory =>
            {
                var clientFactory = factory.GetInstance<IHttpClientFactory>();

                return new HttpRequestSender(clientFactory.CreateClient(key),
                                                factory.GetInstance<IServiceProvider>(),
                                                factory.GetInstance<IRouteBinder>(key),
                                                factory.GetInstance<IMethodBinder>(key),
                                                factory.GetInstance<IQueryParameterBinder>(key),
                                                factory.GetInstance<IHeaderParameterBinder>(key),
                                                factory.GetInstance<IResponseReaderProvider>(key),
                                                factory.GetInstance<IRequestErrorHandler>(key),
                                                factory.GetInstance<ILogger<HttpRequestSender>>());
            }, key);
        }
    }
}