namespace STrain.CQS.NetCore.RequestSending.Providers
{
    public interface IMethodProvider
    {
        HttpMethod GetMethod<TRequest>()
            where TRequest : IRequest;
    }
}
