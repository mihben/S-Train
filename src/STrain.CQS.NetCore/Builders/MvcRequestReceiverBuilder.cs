using Microsoft.AspNetCore.Builder;

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
