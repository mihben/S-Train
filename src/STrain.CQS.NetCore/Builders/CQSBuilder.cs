using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.NetCore.Builders
{
    [ExcludeFromCodeCoverage]
    public class CQSBuilder
    {
        public WebApplicationBuilder Builder { get; }

        public CQSBuilder(WebApplicationBuilder builder)
        {
            Builder = builder;
        }

        public CQSBuilder AddPerformer<TPerformer, TImplementation>()
            where TPerformer : class
            where TImplementation : class, TPerformer
        {
            Builder.Services.AddPerformer<TPerformer, TImplementation>();
            return this;
        }
    }
}
