using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using STrain.CQS.Blazor.Builders;
using LightInject;

namespace STrain
{
    public static class RequestRouterBuilderExtensions
    {
        public static RequestRouterBuilder AddHttpSender(this RequestRouterBuilder builder, string key)
        {
            builder.Builder.ConfigureContainer(container => container.AddHttpSender(key));
            return builder;
        }
    }
}
