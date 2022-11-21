using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.Builders;

namespace STrain.Tracing.NetCore
{
    public static class CQSBuilderExtensions
    {
        public static CQSBuilder AddTracing(this CQSBuilder builder)
        {
            builder.Builder.Services.AddTracing();

            return builder;
        }
    }
}
