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
    }
}
