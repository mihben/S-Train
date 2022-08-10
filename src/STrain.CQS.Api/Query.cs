using System.Diagnostics.CodeAnalysis;

namespace STrain
{
    /// <summary>
    /// Represents a query.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the result.
    /// </typeparam>
    [ExcludeFromCodeCoverage]
    public class Query<T> : IRequest, IEqualityComparer<Query<T>>
    {
        /// <summary>
        /// Unique identifier of the <see cref="Query{T}"/>.
        /// </summary>
        public object RequestId { get; }

        /// <summary>
        /// Create new <see cref="Command"/> instance.
        /// </summary>
        /// <param name="requestId">Unique idenftifier of the <see cref="Query{T}"/>. Will be generated if it is <see cref="null"/></param>
        public Query(Guid? requestId = null)
        {
            RequestId = requestId ?? Guid.NewGuid();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(this, obj as Query<T>);
        /// <inheritdoc/>
        public override int GetHashCode() => GetHashCode(this);

        /// <inheritdoc/>
        public bool Equals(Query<T>? x, Query<T>? y)
        {
            return (x is null && y is null)
                 || (x?.RequestId.Equals(y?.RequestId) ?? false);
        }
        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] Query<T> obj)
        {
            return obj.RequestId.GetHashCode();
        }
    }
}
