namespace STrain.CQS.Http.RequestSending.Providers
{
    public interface IMethodProvider
    {
        HttpMethod GetMethod<TRequest>()
            where TRequest : IRequest;
    }
}
