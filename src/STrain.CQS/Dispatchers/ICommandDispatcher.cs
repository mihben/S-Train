namespace STrain.CQS.Dispatchers
{
    public interface ICommandDispatcher
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
    }
}
