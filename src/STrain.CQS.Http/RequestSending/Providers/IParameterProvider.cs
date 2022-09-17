namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IParameterProvider
    {
        Task SetParametersAsync<TRequest>(HttpRequestMessage message, TRequest request, CancellationToken cancellationToken)
            where TRequest : IRequest;
    }
}
