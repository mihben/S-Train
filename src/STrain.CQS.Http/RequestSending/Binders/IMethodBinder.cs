namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IMethodBinder
    {
        Task<HttpMethod> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
