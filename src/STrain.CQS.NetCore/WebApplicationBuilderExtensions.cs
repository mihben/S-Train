using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using STrain.CQS.NetCore.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AspNetCore.Builder
{
    [ExcludeFromCodeCoverage]
    public static class WebApplicationBuilderExtensions
    {
        public static void AddCQS(this WebApplicationBuilder builder, Action<CQSBuilder> build)
        {
            builder.Services.AddCQS();
            build(new CQSBuilder(builder));
        }
    }
}