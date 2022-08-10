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
    public record Query<T> : IRequest
    {
        
    }
}
