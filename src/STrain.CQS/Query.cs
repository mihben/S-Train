using System.Diagnostics.CodeAnalysis;

namespace STrain
{
    public class Query<T> : IRequest, IEqualityComparer<Query<T>>
    {
        public Guid RequestId { get; }

        public Query(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        public override bool Equals(object? obj) => Equals(this, obj as Query<T>);
        public override int GetHashCode() => GetHashCode(this);

        public bool Equals(Query<T>? x, Query<T>? y)
        {
            return (x is null && y is null)
                 || (x?.RequestId.Equals(y?.RequestId) ?? false);
        }

        public int GetHashCode([DisallowNull] Query<T> obj)
        {
            return obj.RequestId.GetHashCode();
        }
    }
}
