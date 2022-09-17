namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IQueryParameterBinder
    {
        Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
