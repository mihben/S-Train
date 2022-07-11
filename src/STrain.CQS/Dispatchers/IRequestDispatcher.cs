namespace STrain
{
    /// <summary>
    /// Dispatch <see cref="Command"/> or <see cref="Query{T}"/> to <see cref="ICommandPerformer{TCommand}"/> or <see cref="IQueryPerformer{TQuery, T}"/>.
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatch <see cref="Command"/> to <see cref="ICommandPerformer{TCommand}"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of the <see cref="Command"/>.</typeparam>
        /// <param name="command"><typeparamref name="TCommand"/> object.</param>
        /// <exception cref="NotImplementedException">Thrown, if any <see cref="ICommandPerformer{TCommand}"/> was found.</exception>
        /// <exception cref="InvalidOperationException">Thrown, if multiple <see cref="ICommandPerformer{TCommand}"/> were found.</exception>
        Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
            where TCommand : Command;
        /// <summary>
        /// Dispatch <see cref="Query{T}"/> to <see cref="IQueryPerformer{TQuery, T}"/>.
        /// </summary>
        /// <typeparam name="TQuery">Type of the <see cref="Query{T}"/>.</typeparam>
        /// <typeparam name="T">Type of  the return value.</typeparam>
        /// <param name="query">Query object.</param>
        /// <returns>Return value of the <see cref="Query{T}"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown, if any <see cref="IQueryPerformer{TQuery, T}"/> was found.</exception>
        /// <exception cref="InvalidOperationException">Thrown, if multiple <see cref="IQueryPerformer{TQuery, T}"/> were found.</exception>
        Task<T> DispatchAsync<TQuery, T>(TQuery query, CancellationToken cancellationToken)
            where TQuery : Query<T>;
    }
}
