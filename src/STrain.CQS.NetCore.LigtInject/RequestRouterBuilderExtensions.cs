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
        public static HttpRequestSenderBuilder AddHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            builder.Builder.Services.AddHttpRequestSender(key, configure);
            builder.Builder.Host.ConfigureContainer<IServiceContainer>((_, container) => container.AddHttpSender(key));
            return new HttpRequestSenderBuilder(key, builder.Builder);
        }
    }
}
