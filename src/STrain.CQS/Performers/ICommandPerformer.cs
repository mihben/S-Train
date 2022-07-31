namespace STrain
{
    /// <summary>
    /// Responsible for performing <see cref="Command"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the <see cref="Command"/>.</typeparam>
    public interface ICommandPerformer<in TCommand>
        where TCommand : Command
    {
        /// <summary>
        /// Performs <see cref="Command"/>.
        /// </summary>
        /// <param name="command"><see cref="Command"/> object.</param>
        Task PerformAsync(TCommand command, CancellationToken cancellationToken);
    }
}
