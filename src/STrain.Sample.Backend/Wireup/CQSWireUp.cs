using STrain.CQS.NetCore.Builders;
using STrain.Sample.Backend.Performers;
using System.Diagnostics.CodeAnalysis;

[assembly:ExcludeFromCodeCoverage]
namespace STrain.Sample.Backend.Wireup
{
    public static class CQSWireUp
    {
        public static void Build(CQSBuilder builder)
        {
            builder.AddPerformerFrom<SampleCommandPerformer>();
        }
    }
}
