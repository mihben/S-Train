using LightInject;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.CQS.Http.RequestSending.Binders.Attributive;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.NetCore.Builders;
using STrain.CQS.NetCore.ErrorHandling;
using STrain.CQS.Senders;

namespace STrain.CQS.NetCore.LigtInject
{
    public static class HttpRequestSenderBuilderExtensions
    {
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
        public static HttpRequestSenderBuilder UseDefaultErrorHandler(this HttpRequestSenderBuilder builder) => builder.UseErrorHandler<DefaultErrorHandler>();
    }

    public static class GenericHttpRequestSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseGenericRouteBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.RegisterTransient<IRouteBinder>(factory => new GenericRouteBinder(factory.GetInstance<IOptionsSnapshot<HttpRequestSenderOptions>>().Get(builder.Key).Path, factory.GetInstance<ILogger<GenericRouteBinder>>()), builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericMethodBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddMethodBinder<GenericMethodBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericHeaderParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddHeaderParameterBinder<GenericHeaderParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericBodyParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddBodyParameterBinder<GenericBodyParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericQueryParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddQueryParameterBinder<GenericQueryParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericErrorHandler(this HttpRequestSenderBuilder builder) => builder.UseErrorHandler<GenericRequestErrorHandler>();
    }

    public static class AttributiveHttpRequestSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseAttributiveRouteBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddRouteBinder<AttributiveRouteBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveMethodBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddMethodBinder<AttributiveMethodBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveQueryParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddQueryParameterBinder<AttributiveQueryParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveHeaderParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddHeaderParameterBinder<AttributiveHeaderParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseAttributiveBodyParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddBodyParameterBinder<AttributiveBodyParameterBinder>(builder.Key));
            return builder;
        }
    }
}
