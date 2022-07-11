namespace STrain
{
    /// <summary>
    /// Responsible for performing <see cref="Query{T}"/>.
    /// </summary>
    /// <typeparam name="TQuery">Type of the <see cref="Query{T}"/>.</typeparam>
    /// <typeparam name="T">Return type of the <see cref="Query{T}"/>.</typeparam>
    public interface IQueryPerformer<TQuery, T>
        where TQuery: Query<T>
    {
        /// <summary>
        /// Performs <see cref="Query{T}"/>.
        /// </summary>
        /// <param name="query"><see cref="Query{T}"/> object.</param>
        /// <returns>Return value of the <see cref="Query{T}"/>.</returns>
        Task<T> PerformAsync(TQuery query, CancellationToken cancellationToken);
    }
}
