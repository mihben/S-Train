namespace STrain.CQS.Http.RequestSending.Binders
{
    public interface IRouteBinder
    {
        Task<string?> BindAsync<TRequest>(TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
