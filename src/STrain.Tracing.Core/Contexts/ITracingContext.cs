namespace STrain.Tracing
{
    public interface ITracingContext<T>
    {
        T RequestId { get; }
        T CorrelationId { get; }
        T CausationId { get; }
    }
}
