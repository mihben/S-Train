namespace STrain.CQS.Http.RequestSending.Binders
{
    /// <summary>
    /// Responsible for determine the route of the sent <see cref="IRequest"/>.
    /// </summary>
    public interface IRouteBinder
    {
        /// <summary>
        /// Determines the route based on <see cref="IRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">Implementation type of <see cref="IRequest"/>.</typeparam>
        /// <param name="request">Instance of the <see cref="IRequest"/>.</param>
        /// <returns>Determined route.</returns>
        Task<string> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
