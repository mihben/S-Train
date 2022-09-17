using LightInject;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.Blazor.Builders;
using STrain.CQS.Http.RequestSending;

namespace STrain
{
    public static class RequestRouterBuilderExtensions
    {
        public static RequestRouterBuilder AddHttpSender(this RequestRouterBuilder builder, string key, Action<HttpRequestSenderOptions, IConfiguration> configure, Action<HttpRequestSenderBuilder> build)
        {
            builder.Builder.Services.AddHttpRequestSender(key, configure);
            builder.Builder.ConfigureContainer(container => container.AddHttpSender(key));
            build(new HttpRequestSenderBuilder(builder.Builder, key));
            return builder;
        }
    }
}
