using Microsoft.AspNetCore.Builder;

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
