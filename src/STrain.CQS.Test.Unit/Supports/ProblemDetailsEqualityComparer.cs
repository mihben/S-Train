using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace STrain.CQS.Test.Unit.Supports
{
    internal class ProblemDetailsEqualityComparer : IEqualityComparer<ProblemDetails?>
    {
        public bool Equals(ProblemDetails? x, ProblemDetails? y)
        {
            return x?.Status == y?.Status &&
                x?.Title == y?.Title &&
                x?.Detail == y?.Detail &&
                x?.Instance == y?.Instance &&
                x?.Type == y?.Type;
        }

        public int GetHashCode([DisallowNull] ProblemDetails obj)
        {
            return HashCode.Combine(obj.Status.GetHashCode(), obj.Title?.GetHashCode(), obj.Detail?.GetHashCode(), obj.Instance?.GetHashCode(), obj.Type?.GetHashCode(), obj.Extensions.GetHashCode());
        }
    }
}
