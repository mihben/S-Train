using STrain.CQS;

namespace STrain
{
    public class Query<T> : IRequest, IEquatable<Query<T>>
    {
        public Guid RequestId { get; }

        public Query(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        public bool Equals(Query<T>? other) => RequestId.Equals(other?.RequestId);
        public override bool Equals(object? obj) => Equals(obj as Query<T>);
        public override int GetHashCode() => RequestId.GetHashCode();
    }
}
