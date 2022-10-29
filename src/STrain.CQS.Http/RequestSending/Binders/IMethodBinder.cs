namespace STrain.CQS.Http.RequestSending.Binders
{
    /// <summary>
    /// Responsible for determine the <see cref="HttpMethod"/> of the sent <see cref="IRequest"/>.
    /// </summary>
    public interface IMethodBinder
    {
        /// <summary>
        /// Determines the <see cref="HttpMethod"/> based on the <see cref="IRequest"/>.
        /// </summary>
        /// <typeparam name="TRequest">>Implementation type of <see cref="IRequest"/>.</typeparam>
        /// <param name="request">Instance of the <see cref="IRequest"/>.</param>
        /// <returns>Determined <see cref="HttpMethod"/>.</returns>
        Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
