using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.Builders;

namespace Microsoft.AspNetCore.Builder
{
    public static class WebApplicationBuilderExtensions
    {
        public static void AddCQS(this WebApplicationBuilder builder, Action<CQSBuilder> build)
        {
            builder.Services.AddCQS();
            build(new CQSBuilder(builder));
        }
    }
}