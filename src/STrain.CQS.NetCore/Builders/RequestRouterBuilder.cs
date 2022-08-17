using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.RequestSending;

namespace STrain.CQS.NetCore.Builders
{
    public class RequestRouterBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public RequestRouterBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public HttpRequestSenderBuilder AddHttpSender(string key, Action<HttpRequestSenderOptions, IConfiguration> configure)
        {
            Builder.Services.AddHttpRequestSender(key, configure);
            return new HttpRequestSenderBuilder(key, Builder).UseDefaults();
        }
    }
}
