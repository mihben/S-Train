using Microsoft.Extensions.Logging;
using STrain;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.CQS.Senders;

namespace LightInject
{
    public static class ServiceContainerExtensions
    {
        /// <summary>
        /// Register <see cref="IRouteBinder"/> implementation.
        /// </summary>
        /// <typeparam name="TRouteBinder">Implementation type of the route binder. Must implement <see cref="IRouteBinder"/> interface.</typeparam>
        /// <param name="key">Unique key of the route binder.</param>
        public static void AddRouteBinder<TRouteBinder>(this IServiceContainer container, string key)
            where TRouteBinder : class, IRouteBinder => container.RegisterTransient<IRouteBinder, TRouteBinder>(key);

        /// <summary>
        /// Register <see cref="IMethodBinder"/> implementation.
        /// </summary>
        /// <typeparam name="TMethodBinder">Implementation type of method binder. Must implement <see cref="IMethodBinder"/> interface.</typeparam>
        /// <param name="key">Unique key of the route binder.</param>
        public static void AddMethodBinder<TMethodBinder>(this IServiceContainer container, string key)
            where TMethodBinder : class, IMethodBinder => container.RegisterTransient<IMethodBinder, TMethodBinder>(key);

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

        public static void AddBodyParameterBinder<TBodyParameterBinder>(this IServiceContainer container, string key)
            where TBodyParameterBinder : IBodyParameterBinder
        {
            container.RegisterTransient<IBodyParameterBinder, TBodyParameterBinder>(key);
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
                                                factory.GetInstance<IBodyParameterBinder>(key),
                                                factory.GetInstance<IResponseReaderProvider>(key),
                                                factory.GetInstance<IRequestErrorHandler>(key),
                                                factory.GetInstance<ILogger<HttpRequestSender>>());
            }, key);
        }
    }
}