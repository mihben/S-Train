namespace STrain.CQS.Http.RequestSending.Binders
{
    public interface IQueryParameterBinder
    {
        Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
