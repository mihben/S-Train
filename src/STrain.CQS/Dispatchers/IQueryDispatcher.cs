using STrain.CQS.Api;

namespace STrain
{
    /// <summary>
    /// Dispatch <see cref="Command"/> or <see cref="Query{T}"/> to <see cref="ICommandPerformer{TCommand}"/> or <see cref="IQueryPerformer{TQuery, T}"/>.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatch <see cref="Query{T}"/> to <see cref="IQueryPerformer{TQuery, T}"/>.
        /// </summary>
        /// <typeparam name="TQuery">Type of the <see cref="Query{T}"/>.</typeparam>
        /// <typeparam name="T">Type of  the return value.</typeparam>
        /// <param name="query">Query object.</param>
        /// <returns>Return value of the <see cref="Query{T}"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown, if any <see cref="IQueryPerformer{TQuery, T}"/> was found.</exception>
        /// <exception cref="InvalidOperationException">Thrown, if multiple <see cref="IQueryPerformer{TQuery, T}"/> were found.</exception>
        Task<T> DispatchAsync<T>(Query<T> query, CancellationToken cancellationToken);
    }
}
