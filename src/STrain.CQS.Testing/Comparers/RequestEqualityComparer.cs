using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.Testing.Comparers
{
    public class RequestEqualityComparer<T> : IEqualityComparer<T>
        where T : IRequest
    {
        public bool Equals(T? x, T? y)
        {
            return x?.Equals(y) ?? false;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.GetHashCode();
        }
    }
}
