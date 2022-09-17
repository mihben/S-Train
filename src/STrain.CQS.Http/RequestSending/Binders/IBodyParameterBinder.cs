namespace STrain.CQS.Http.RequestSending.Binders
{
    public interface IBodyParameterBinder
    {
        Task<HttpContent?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
