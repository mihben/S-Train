using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.Receivers;

namespace STrain.CQS.NetCore.Builders
{
    public class MvcRequestReceiverBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public MvcRequestReceiverBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }
    }
}
