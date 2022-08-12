namespace STrain.CQS.NetCore.RequestSending
{
    public interface IMethodProvider
    {
        HttpMethod GetMethod<TRequest>()
            where TRequest : IRequest;
    }
}
