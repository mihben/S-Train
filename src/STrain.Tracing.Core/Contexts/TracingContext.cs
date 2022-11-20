namespace STrain.Tracing.Core.Contexts
{
    public class TracingContext : ITracingContext<Guid?>
    {
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? CausationId { get; set; }
    }
}
