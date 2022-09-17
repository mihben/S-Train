using LightInject;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using STrain.CQS.Blazor.Builders;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.Http.RequestSending.Providers;
using STrain.CQS.Http.RequestSending.Providers.Attributive;
using STrain.CQS.Http.RequestSending.Readers;
using STrain.CQS.Senders;
using System.Net.Mime;

namespace STrain.CQS.Blazor.Lightinject
{
    public static class HttpRequestSenderBuilderExtensions
    {
        public static HttpRequestSenderBuilder UseAttributePathProvider(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(registry => registry.RegisterTransient<IPathProvider>(factory => new AttributivePathProvider(factory.GetInstance<IOptionsSnapshot<HttpRequestSenderOptions>>().Get(builder.Key).Path, factory.GetInstance<ILogger<AttributivePathProvider>>()), builder.Key));
            return builder;
        }

        public static HttpRequestSenderBuilder UseMethodProvider<TMethodProvider>(this HttpRequestSenderBuilder builder)
            where TMethodProvider : class, IMethodProvider
        {
            builder.Builder.ConfigureContainer(container => container.AddMethodProvider<TMethodProvider>(builder.Key));
            return builder;
        }
        public static HttpRequestSenderBuilder UseAttributiveMethodProvider(this HttpRequestSenderBuilder builder)
            => builder.UseMethodProvider<AttributiveMethodProvider>();

        public static HttpRequestSenderBuilder UseAttributeParameterProvider(this HttpRequestSenderBuilder builder)
        {
            builder.Builder.ConfigureContainer(container =>
            {
                container.AddParameterProvider<AttributiveHeaderParameterProvider>($"{builder.Key}.header");
                container.AddParameterProvider<AttributiveQueryParameterProvider>($"{builder.Key}.query");
                container.AddParameterProvider<AttributiveBodyParameterProvider>($"{builder.Key}.body");
            });
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
