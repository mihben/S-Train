using STrain.CQS.NetCore.Builders;
using STrain.Sample.Backend.Performers;

namespace STrain.Sample.Backend.Wireup
{
    public static class CQSWireUp
    {
        public static void Build(CQSBuilder builder)
        {
            builder.AddPerformerFrom<SampleCommandPerformer>();

            builder.AddGenericRequestHandler();
        }
    }
}
