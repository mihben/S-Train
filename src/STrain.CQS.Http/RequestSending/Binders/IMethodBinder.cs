namespace STrain.CQS.Http.RequestSending.Binders
{
    public interface IMethodBinder
    {
        Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
