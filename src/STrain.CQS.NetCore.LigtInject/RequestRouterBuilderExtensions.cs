using LightInject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.Http.RequestSending;
using STrain.CQS.NetCore.Builders;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.NetCore.LigtInject
{
    [ExcludeFromCodeCoverage]
    public static class RequestRouterBuilderExtensions
    {
        public static RequestRouterBuilder AddHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure, Action<HttpRequestSenderBuilder> build)
        {
            builder.Builder.Services.AddHttpRequestSender(key, configure);
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddHttpSender(key));
            build(new HttpRequestSenderBuilder(key, builder.Builder));
            return builder;
        }

        public static RequestRouterBuilder AddGenericHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
            => builder.AddHttpSender(key, configure, builder => builder.UseGenericMethodBinder()
                                                                    .UseGenericBodyParameterBinder()
                                                                    .UseGenericHeaderParameterBinder()
                                                                    .UseGenericQueryParameterBinder()
                                                                    .UseGenericRouteBinder()
                                                                    .UseGenericErrorHandler()
                                                                    .UseResponseReaders(registry => registry.UseDefaultTextResponseReader().UseDefaultJsonResponseReader()));

        public static RequestRouterBuilder AddAttributiveHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
            => builder.AddHttpSender(key, configure, builder => builder.UseAttributiveQueryParameterBinder()
                                                                            .UseAttributiveBodyParameterBinder()
                                                                            .UseAttributiveHeaderParameterBinder()
                                                                            .UseAttributiveMethodBinder()
                                                                            .UseAttributiveRouteBinder()
                                                                            .UseDefaultErrorHandler()
                                                                            .UseResponseReaders(registry => registry.UseDefaultTextResponseReader().UseDefaultJsonResponseReader()));
    }
}
