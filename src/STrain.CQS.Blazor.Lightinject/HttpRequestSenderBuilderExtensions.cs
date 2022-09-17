using LightInject;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using STrain.CQS.Blazor.Builders;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Binders;
using STrain.CQS.Http.RequestSending.Binders.Generic;
using STrain.CQS.Http.RequestSending.Readers;
using STrain.CQS.Senders;
using System.Net.Mime;

namespace STrain.CQS.Blazor.Lightinject
{
    public static class HttpRequestSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseGenericPathBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(registry => registry.RegisterTransient<IRouteBinder>(factory => new GenericRouteBinder(factory.GetInstance<IOptionsSnapshot<HttpRequestSenderOptions>>().Get(builder.Key).Path), builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericMethodBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container => container.AddMethodBinder<GenericMethodBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericQueryParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container => container.AddQueryParameterBinder<GenericQueryParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericHeaderParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container => container.AddHeaderParameterBinder<GenericHeaderParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseGenericBodyParameterBinder(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container => container.AddBodyParameterBinder<GenericBodyParameterBinder>(builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseDefaultResponseReader(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container =>
            {
                var responseReaderRegistry = new ResponseReaderRegistry();
                responseReaderRegistry.Registrate<JsonResponseReader>(MediaTypeNames.Application.Json);
                responseReaderRegistry.Registrate<StringResponseReader>(MediaTypeNames.Text.Plain);
                container.RegisterInstance<IResponseReaderProvider>(responseReaderRegistry, builder.Key);
                container.RegisterTransient<JsonResponseReader>();
                container.RegisterTransient<StringResponseReader>();
            });

            return builder;
        }

        public static HttpRequestSenderBuilder UseRequestErrorHandler(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container => container.RegisterTransient<IRequestErrorHandler, DefaultErrorHandler>(builder.Key));
            return builder;
        }
    }
}
