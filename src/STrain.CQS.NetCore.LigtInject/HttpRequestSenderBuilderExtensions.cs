using LightInject;
using Microsoft.Extensions.Options;
using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.RequestSending;
using STrain.CQS.NetCore.RequestSending.Attributive;
using STrain.CQS.NetCore.RequestSending.Providers;
using STrain.CQS.NetCore.RequestSending.Providers.Attributive;
using STrain.CQS.Senders;

namespace STrain.CQS.NetCore.LigtInject
{
    public static class HttpRequestSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseDefaults(this HttpRequestSenderBuilder builder)
        {
            builder.UseAttributivePathProvider();
            builder.UseAttributeMethodProvider();
            builder.UseAttributiveParameterProviders();
            builder.UseResponseReaders();

            builder.UseGenericErrorHandler();

            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributivePathProvider(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterTransient<IPathProvider>(factory => new AttributivePathProvider(factory.GetInstance<IOptionsSnapshot<HttpRequestSenderOptions>>().Get(builder.Key).Path), builder.Key));
            return builder;
        }
        public static HttpRequestSenderBuilder UsePathProvider<TPathProvider>(this HttpRequestSenderBuilder builder)
            where TPathProvider : class, IPathProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterTransient<IPathProvider, TPathProvider>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributeMethodProvider(this HttpRequestSenderBuilder builder) => builder.UseMethodProvider<AttributiveMethodProvider>();
        public static HttpRequestSenderBuilder UseMethodProvider<TMethodProvider>(this HttpRequestSenderBuilder builder)
            where TMethodProvider : class, IMethodProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterTransient<IMethodProvider, TMethodProvider>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveParameterProviders(this HttpRequestSenderBuilder builder) => builder.UseParameterProviders<AttributiveHeaderParameterProvider, AttributiveQueryParameterProvider, AttributiveBodyParameterProvider>();
        public static HttpRequestSenderBuilder UseParameterProviders<THeaderParameterProvider, TQueryParameterProvider, TBodyParameterProvider>(this HttpRequestSenderBuilder builder)
            where THeaderParameterProvider : class, IParameterProvider
            where TQueryParameterProvider : class, IParameterProvider
            where TBodyParameterProvider : class, IParameterProvider
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterTransient<IParameterProvider, THeaderParameterProvider>($"{builder.Key}.header");
                registry.RegisterTransient<IParameterProvider, TQueryParameterProvider>($"{builder.Key}.query");
                registry.RegisterTransient<IParameterProvider, TBodyParameterProvider>($"{builder.Key}.body");
            });
            return builder;
        }

        public static HttpRequestSenderBuilder UseResponseReaders(this HttpRequestSenderBuilder builder) => builder.UseResponseReaders(register => register.UseDefaults());
        public static HttpRequestSenderBuilder UseResponseReaders(this HttpRequestSenderBuilder builder, Action<ResponseReadersRegister> registrate)
        {
            var responseReaderRegistry = new ResponseReaderRegistry();
            registrate(new ResponseReadersRegister(responseReaderRegistry, builder.Builder));
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) =>
            {
                registry.RegisterInstance(responseReaderRegistry, builder.Key);
                registry.RegisterTransient<IResponseReaderProvider>(factory => factory.GetInstance<ResponseReaderRegistry>(builder.Key), builder.Key);
            });
            return builder;
        }

        public static HttpRequestSenderBuilder UseErrorHandler<TErrorHandler>(this HttpRequestSenderBuilder builder)
            where TErrorHandler : IRequestErrorHandler
        {
            builder.Builder.Host.ConfigureContainer<IServiceRegistry>((_, registry) => registry.RegisterTransient<IRequestErrorHandler, TErrorHandler>(builder.Key));
            return builder;
        }
        public static HttpRequestSenderBuilder UseGenericErrorHandler(this HttpRequestSenderBuilder builder) => builder.UseErrorHandler<GenericRequestErrorHandler>();
    }
}
